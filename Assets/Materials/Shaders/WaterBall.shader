Shader "Custom/WaterBall"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        _NoiseTexture("Noise", 2D) = "white" {}
        _NoiseTiling("Noise Tiling", Float) = 1.0
        _NoiseSpeed("Noise Speed", Float) = 1.0
        _NoiseDirection("Noise Direction", Range(0, 360)) = 0.0
        

        [Header(Normals)]
        [Normal] _NormalsTexture("Normals Texture", 2D) = "bump" {}
        _NormalsScale("Normals Scale", Float) = 1.0
        _NormalsSpeed("Normals Speed", Float) = 0.05
        _NormalsStrength("Normals Strength", Float) = 1.0

        [Header(Lighting)]
        _LightingSmoothness("Lighting Smoothness", Range(0, 1)) = 0.8
        _LightingHardness("Lighting Hardness", Range(0, 1)) = 0.5
        _SpecularColor("Specular Color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
                float3 normalWS : TEXCOORD1;
                half4 tangentWS : TEXCOORD2;
                float3 positionWS : TEXCOORD3;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            TEXTURE2D(_NoiseTexture);
            SAMPLER(sampler_NoiseTexture);

            TEXTURE2D(_NormalsTexture);
            SAMPLER(sampler_NormalsTexture);


            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
                float4 _NoiseTexture_ST;
                float _NoiseDirection;
                float _NoiseTiling;
                float _NoiseSpeed;
                float _LightingSmoothness;
                float _LightingHardness;
                half4 _SpecularColor;
                float _NormalsScale;
                float _NormalsSpeed;
                float _NormalsStrength;
            CBUFFER_END

            float3 BlendedNormals(float2 uv, float speed, float scale)
            {
                float dirRad = radians(1.0);
                float2 panDir = float2(cos(dirRad), sin(dirRad));

                float tilingA = rcp(max(0.5 * scale, 0.0001));
                float2 uvA = (uv * tilingA) + (panDir * (-0.5 * speed) * _Time.y);
                float3 normalA = UnpackNormal(SAMPLE_TEXTURE2D(_NormalsTexture, sampler_NormalsTexture, uvA));

                float tilingB = rcp(max(scale, 0.0001));
                float2 uvB = (uv * tilingB) + (panDir * speed * _Time.y);
                float3 normalB = UnpackNormal(SAMPLE_TEXTURE2D(_NormalsTexture, sampler_NormalsTexture, uvB));

                return normalize(float3(normalA.xy + normalB.xy, normalA.z * normalB.z));
            }

            float LightingSpecular(float3 L, float3 N, float3 V, float smoothness)
            {
                float3 H = SafeNormalize(float3(L) + float3(V));
                float NdotH = saturate(dot(N, H));
                return pow(NdotH, smoothness);
            }

            void MainLighting_float(float3 normalWS, float3 positionWS, float3 viewWS, float smoothness, out float specular)
            {
                specular = 0.0;

                #ifndef SHADERGRAPH_PREVIEW
                smoothness = exp2(10 * smoothness + 1);

                normalWS = normalize(normalWS);
                viewWS = SafeNormalize(viewWS);

                Light mainLight = GetMainLight(TransformWorldToShadowCoord(positionWS));
                specular = LightingSpecular(mainLight.direction, normalWS, viewWS, smoothness);
                #endif
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float noiseDirRad = radians(_NoiseDirection);
                float2 noisePanDir = float2(cos(noiseDirRad), sin(noiseDirRad));
                float2 panningUV = (IN.uv * _NoiseTiling) + (noisePanDir * _NoiseSpeed * _Time.y);
                IN.positionOS.xyz =  normalize(IN.positionOS.xyz) * _NoiseTexture.SampleLevel(sampler_NoiseTexture, panningUV, 0).r * 2.8;

                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);

                VertexPositionInputs posInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs normInputs = GetVertexNormalInputs(IN.normalOS, IN.tangentOS);
                
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.positionHCS = TransformWorldToHClip(OUT.positionWS);

                real sign = IN.tangentOS.w * GetOddNegativeScale();
                OUT.tangentWS = half4(normInputs.tangentWS.xyz, sign);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float3 blendedNormalTS = BlendedNormals(IN.uv, _NormalsSpeed, _NormalsScale);
                float3 strengthNormalsTS = float3(blendedNormalTS.xy * _NormalsStrength,
                    lerp(1.0, blendedNormalTS.z, saturate(_NormalsStrength))
                    );

                float sgn = IN.tangentWS.w;
                float3 bitangentWS = sgn * cross(IN.normalWS.xyz, IN.tangentWS.xyz);
                half3x3 tangentToWorld = half3x3(IN.tangentWS.xyz, bitangentWS.xyz, IN.normalWS.xyz);

                float3 normalWS = TransformTangentToWorld(strengthNormalsTS, tangentToWorld);
                normalWS = NormalizeNormalPerPixel(normalWS);

                float3 viewDirWS = GetWorldSpaceNormalizeViewDir(IN.positionWS);

                float mainSpec;
                MainLighting_float(normalWS, IN.positionWS, viewDirWS, _LightingSmoothness, mainSpec);

                float mainSpecHard = step(0.5, mainSpec);
                float mainSpecBlended = lerp(mainSpec, mainSpecHard, _LightingHardness);
                float4 mainSpecCol = (mainSpecBlended * _SpecularColor) * _SpecularColor.a;

                
                half4 color = _BaseColor + mainSpecCol;
                return color;
            }
            ENDHLSL
        }
    }
}
