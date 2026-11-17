Shader "Custom/SimpleWater"
{
    Properties
    {
        [Header(Depth)]
        _DeepColour("Deep Colour", Color) = (1, 1, 1, 1)
        _ShallowColour("Shallow Colour", Color) = (1, 1, 1, 1)
        _DepthFadeDistance("Depth Fade Distance", Float) = 1.0

        [Header(Horizon)]
        _HorizonColour("Horizon Colour", Color) = (0.6, 0.85, 0.95, 1.0)
        _FresnelPower("Fresnel Power", Float) = 5.0

        [Header(Normals)]
        [Normal] _NormalsTexture("Normals Texture", 2D) = "bump" {}
        _NormalsScale("Normals Scale", Float) = 1.0
        _NormalsSpeed("Normals Speed", Float) = 0.05
        _NormalsStrength("Normals Strength", Float) = 1.0

        [Header(Refraction)]
        _RefractionScale("Refraction Scale", Float) = 0.2
        _RefractionSpeed("Refraction Speed", Float) = 0.05
        _RefractionStrength("Refraction Strength", Float) = 0.02

        [Header(Surface Foam)]
        _SurfaceFoamTexture("Surface Foam Texture", 2D) = "black" {}
        _SurfaceFoamTiling("Surface Foam Tiling", Float) = 1.0
        _SurfaceFoamDirection("Surface Foam Direction", Range(0, 360)) = 0.0
        _SurfaceFoamSpeed("Surface Foam Speed", Float) = 0.05
        _SurfaceFoamDistortion("Surface Foam Distortion", Float) = 0.5
        _FoamColour("Foam Colour", Color) = (1,1,1,1)
        _FoamBlend("Foam Blend", Range(0,1)) = 0.5

        [Header(Intersection Foam)]
        _IntersectionFoamTexture("Intersection Foam Texture", 2D) = "white" {}
        _IntersectionFoamTiling("Intersection Foam Tiling", Float) = 1.0
        _IntersectionFoamDirection("Intersection Foam Direction", Range(0, 360)) = 0.0
        _IntersectionFoamSpeed("Intersection Foam Speed", Float) = 0.05
        _IntersectionFoamDepth("Intersection Foam Depth", Float) = 0.5
        _IntersectionFoamFade("Intersection Foam Fade", Range(0, 1)) = 0.5
        _IntersectionFoamCutoff("Intersection Foam Cutoff", Float) = 1.0
        _IntersectionFoamColour("Intersection Foam Colour", Color) = (1, 1, 1, 1)
        _IntersectionFoamBlend("Intersection Foam Blend", Range(0, 1)) = 0.5

        [Header(Lighting)]
        _LightingSmoothness("Lighting Smoothness", Range(0, 1)) = 0.8
        _LightingHardness("Lighting Hardness", Range(0, 1)) = 0.5
        _SpecularColor("Specular Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                float3 normalWS : TEXCOORD3;
                half4 tangentWS : TEXCOORD4;
            };

            
            TEXTURE2D(_NormalsTexture);
            SAMPLER(sampler_NormalsTexture);

            TEXTURE2D(_SurfaceFoamTexture);
            SAMPLER(sampler_SurfaceFoamTexture);

            TEXTURE2D(_IntersectionFoamTexture);
            SAMPLER(sampler_IntersectionFoamTexture);

            CBUFFER_START(UnityPerMaterial)
                half4 _ShallowColour;
                half4 _DeepColour;
                float _DepthFadeDistance;
                half4 _HorizonColour;
                float _FresnelPower;
                float _NormalsScale;
                float _NormalsSpeed;
                float _NormalsStrength;
                float _RefractionScale;
                float _RefractionSpeed;
                float _RefractionStrength;
                float _SurfaceFoamTiling;
                float _SurfaceFoamDirection;
                float _SurfaceFoamSpeed;
                float _SurfaceFoamDistortion;
                half4 _FoamColour;
                float _FoamBlend;
                float _IntersectionFoamTiling;
                float _IntersectionFoamDirection;
                float _IntersectionFoamSpeed;
                float _IntersectionFoamDepth;
                float _IntersectionFoamFade;
                float _IntersectionFoamCutoff;
                half4 _IntersectionFoamColour;
                float _IntersectionFoamBlend;
                float _LightingSmoothness;
                float _LightingHardness;
                half4 _SpecularColor;
            CBUFFER_END

            float2 Unity_GradientNoise_Dir(float2 p)
            {
                p = p % 289.0;
                float x = (34.0 * p.x + 1.0) * p.x % 289.0 + p.y;
                x = (34.0 * x + 1.0) * x % 289.0;
                x = frac(x/41.0) * 2.0 - 1.0;
                return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
            }

            float Unity_GradientNoise(float2 uv)
            {
                float2 ip = floor(uv);
                float2 fp = frac(uv);
                float d00 = dot(Unity_GradientNoise_Dir(ip), fp);
                float d01 = dot(Unity_GradientNoise_Dir(ip + float2(0.0, 1.0)), fp - float2(0.0, 1.0));
                float d10 = dot(Unity_GradientNoise_Dir(ip + float2(1.0, 0.0)), fp - float2(1.0, 0.0));
                float d11 = dot(Unity_GradientNoise_Dir(ip + float2(1.0, 1.0)), fp - float2(1.0, 1.0));
                fp = fp * fp * fp * (fp * (fp * 6.0 - 15.0) + 10.0);
                return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
            }

            void DistortUV_float(float2 UV, float Amount, out float2 Out)
            {
                float time = _Time.y;

                UV.y += Amount * 0.01 * (sin(UV.x * 3.5 + time * 0.35) + sin(UV.x * 4.8 + time * 1.05) + sin(UV.x * 7.3 + time * 0.45)) / 3.0;
                UV.x += Amount * 0.12 * (sin(UV.y * 4.0 + time * 0.50) + sin(UV.y * 6.8 + time * 0.75) + sin(UV.y * 11.3 + time * 0.2)) / 3.0;
                UV.y += Amount * 0.12 * (sin(UV.x * 4.2 + time * 0.64) + sin(UV.x * 6.3 + time * 1.65) + sin(UV.x * 8.2 + time * 0.45)) / 3.0;

                Out = UV;
            }

            float4 OverlayBlend(float4 Base, float4 Overlay, float Blend)
            {
                float opacity = saturate(Overlay.a);

                float4 overwriteOut = lerp(Base,Overlay,opacity);
                float4 linearDodgeOut = lerp(Base, Base + Overlay, opacity);

                return lerp(overwriteOut, linearDodgeOut, Blend);
            }

            float3 BlendedNormals(float2 uv, float speed, float scale)
            {
                float dirRad = radians(1.0);
                float2 panDir = float2(cos(dirRad), sin(dirRad));

                // Top branch: 0.5 * Scale, -0.5 * Speed
                float tilingA = rcp(max(0.5 * scale, 0.0001));
                float2 uvA = (uv * tilingA) + (panDir * (-0.5 * speed) * _Time.y);
                float3 normalA = UnpackNormal(SAMPLE_TEXTURE2D(_NormalsTexture, sampler_NormalsTexture, uvA));

                // Bottom branch: 1.0 * Scale, 1.0 * Speed
                float tilingB = rcp(max(scale, 0.0001));
                float2 uvB = (uv * tilingB) + (panDir * speed * _Time.y);
                float3 normalB = UnpackNormal(SAMPLE_TEXTURE2D(_NormalsTexture, sampler_NormalsTexture, uvB));

                // Normal Blend (Mode: Default)
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

            void AdditionalLighting_float(float3 normalWS, float3 positionWS, float3 viewWS, float smoothness, float hardness, out float3 specular)
            {
                specular = 0;

                #ifndef SHADERGRAPH_PREVIEW
                smoothness = exp2(10 * smoothness + 1);

                normalWS = normalize(normalWS);
                viewWS = SafeNormalize(viewWS);

                int pixelLightCount = GetAdditionalLightsCount();
                for (int i = 0; i < pixelLightCount; ++i)
                {
                    Light light = GetAdditionalLight(i, positionWS);
                    float3 attenuatedLight = light.color * light.distanceAttenuation * light.shadowAttenuation;

                    float specular_soft = LightingSpecular(light.direction, normalWS, viewWS, smoothness);
                    float specular_hard = smoothstep(0.005, 0.01, specular_soft);
                    float specular_term = lerp(specular_soft, specular_hard, hardness);

                    specular += specular_term * attenuatedLight;
                }
                #endif
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.uv = IN.uv;

                VertexPositionInputs posInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs normInputs = GetVertexNormalInputs(IN.normalOS, IN.tangentOS);

                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.positionHCS = TransformWorldToHClip(OUT.positionWS);

                real sign = IN.tangentOS.w * GetOddNegativeScale();
                OUT.tangentWS = half4(normInputs.tangentWS.xyz, sign);

                OUT.screenPos = ComputeScreenPos(OUT.positionHCS);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 screenUVs = IN.screenPos.xy / IN.screenPos.w;
                
                float3 blendedNormalTS = BlendedNormals(IN.uv, _NormalsSpeed, _NormalsScale);
                float3 strengthNormalsTS = float3(blendedNormalTS.xy * _NormalsStrength,
                    lerp(1.0, blendedNormalTS.z, saturate(_NormalsStrength))
                    );

                float sgn = IN.tangentWS.w;
                float3 bitangentWS = sgn * cross(IN.normalWS.xyz, IN.tangentWS.xyz);
                half3x3 tangentToWorld = half3x3(IN.tangentWS.xyz, bitangentWS.xyz, IN.normalWS.xyz);

                float3 normalWS = TransformTangentToWorld(strengthNormalsTS, tangentToWorld);
                normalWS = NormalizeNormalPerPixel(normalWS);

                float tiling = rcp(max(_RefractionScale, 0.0001));
                float2 baseNoiseUV = IN.positionWS.xz * tiling;

                float2 uvX = baseNoiseUV + float2(_Time.y * _RefractionSpeed, _Time.y * _RefractionSpeed * 0.7);
                float2 uvY = baseNoiseUV * 1.15 + float2(-_Time.y * _RefractionSpeed * 0.8, _Time.y * _RefractionSpeed) + 17.3;

                float2 noise2D = float2(Unity_GradientNoise(uvX), Unity_GradientNoise(uvY));
                float2 uvOffset = (noise2D * 2.0 - 1.0) * _RefractionStrength;

                float2 refractedScreenUVs = saturate(screenUVs + uvOffset);
                float refractedEyeDepth = LinearEyeDepth(SampleSceneDepth(refractedScreenUVs), _ZBufferParams);
                if(refractedEyeDepth < IN.screenPos.w)
                {
                    refractedScreenUVs = screenUVs;
                }

                float rawDepth = SampleSceneDepth(screenUVs);
                float sceneEyeDepth = LinearEyeDepth(rawDepth, _ZBufferParams);


                float3 viewVectorWS = _WorldSpaceCameraPos - IN.positionWS;
                float3 scenePositionWS = ((-viewVectorWS / IN.screenPos.w) * sceneEyeDepth) + _WorldSpaceCameraPos;

                float verticalDepth = (IN.positionWS - scenePositionWS).y;
                float depthMask = saturate(exp(-verticalDepth / _DepthFadeDistance));
                half4 depthColour = lerp(_DeepColour, _ShallowColour, depthMask);

                float3 viewDirWS = GetWorldSpaceNormalizeViewDir(IN.positionWS);
                float fresnel = pow(1.0 - saturate(dot(normalWS, viewDirWS)), _FresnelPower);

                half4 waterSurfaceColour = lerp(depthColour, _HorizonColour, fresnel);

                half3 sceneColour = SampleSceneColor(refractedScreenUVs);
                half3 underwaterColour = sceneColour * (1.0 - waterSurfaceColour.a);

                half3 waterBasedRGB = (waterSurfaceColour.rgb * waterSurfaceColour.a) + underwaterColour;

                float dirRad = radians(_SurfaceFoamDirection);
                float2 panDir = float2(cos(dirRad), sin(dirRad));
                float2 panningUV = (IN.uv * _SurfaceFoamTiling) + (panDir * _SurfaceFoamSpeed * _Time.y);

                float2 distortedFoamUV;
                DistortUV_float(panningUV, _SurfaceFoamDistortion, distortedFoamUV);

                half4 surfFoamTex = SAMPLE_TEXTURE2D(_SurfaceFoamTexture, sampler_SurfaceFoamTexture, distortedFoamUV);
                float surfFoamMask = step(0.5, surfFoamTex.r);
                float4 surfFoamOverlay = surfFoamMask * _FoamColour;


                float depthFade = saturate((sceneEyeDepth - IN.screenPos.w) / max(_IntersectionFoamDepth, 0.0001));
                
                float fadeEdge1 = min(1.0 - _IntersectionFoamFade, 0.9999);
                float intersectionFadeMask = 1.0 - smoothstep(fadeEdge1, 1.0, depthFade + 0.1);

                float interDirRad = radians(_IntersectionFoamDirection);
                float2 interPanDir = float2(cos(interDirRad), sin(interDirRad));
                float2 interPanningUV = (IN.uv * _IntersectionFoamTiling) + (interPanDir * _IntersectionFoamSpeed * _Time.y);

                half4 interFoamTex = SAMPLE_TEXTURE2D(_IntersectionFoamTexture, sampler_IntersectionFoamTexture, interPanningUV);
                float interFoamStep = step(depthFade * _IntersectionFoamCutoff, interFoamTex.r);

                float4 tintedInterFoam = interFoamStep * _IntersectionFoamColour;
                float4 interFoamOverlay = float4(tintedInterFoam.rgb, tintedInterFoam.a * intersectionFadeMask);
                
                float4 afterSurfaceFoam = OverlayBlend(float4(waterBasedRGB, 1.0), surfFoamOverlay, _FoamBlend);
                float4 afterIntersectionFoam = OverlayBlend(afterSurfaceFoam, interFoamOverlay, _IntersectionFoamBlend);

                float mainSpec;
                MainLighting_float(normalWS, IN.positionWS, viewDirWS, _LightingSmoothness, mainSpec);

                float3 additionalSpec;
                AdditionalLighting_float(normalWS, IN.positionWS, viewDirWS, _LightingSmoothness, _LightingHardness, additionalSpec);

                float mainSpecHard = step(0.5, mainSpec);
                float mainSpecBlended = lerp(mainSpec, mainSpecHard, _LightingHardness);
                float4 mainSpecCol = (mainSpecBlended * _SpecularColor) * _SpecularColor.a;

                float3 totalSpecular = mainSpecCol.rgb + additionalSpec;
                float3 finalRGB = totalSpecular + afterIntersectionFoam.rgb;

                return half4(finalRGB, 1.0);
            }
            ENDHLSL
        }
    }
}