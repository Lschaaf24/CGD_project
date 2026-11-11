Shader "Custom/Water"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        _NoiseTex ("Perlin Noise Texture", 2D) = "white" {}
        _ScrollSpeed1 ("Scroll Sped (X,Y) 01", Vector) = (0.1, 0.1, 0, 0)
        _ScrollSpeed2 ("Scroll Sped (X,Y) 02", Vector) = (0.1, 0.1, 0, 0)
        _ShallowColour ("Shallow Colour", Color) = (1,1,1,1)
        _DeepColour ("Deep Colour", Color) = (1,1,1,1)
        _FoamColour ("Foam Colour", Color) = (1, 1, 1, 1)
        _SurfaceNoiseCutoff ("Foam Cutoff", Range(0, 1)) = 0.5
        _Steepness ("Steepness", Range(0, 1)) = 0.3
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #ifndef WATER_G
                #define WATER_G 9.81f
            #endif

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);


            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
                float4 _NoiseTex_ST;
                float4 _ScrollSpeed1;
                float4 _ScrollSpeed2;
                half4 _ShallowColour;
                half4 _DeepColour;
                half4 _FoamColour;
                float _SurfaceNoiseCutoff;
                float _Steepness;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 noiseUV1 : TEXCOORD1;
                float2 noiseUV2 : TEXCOORD2;
            };

            float3 calculateOffsets(float3 positionWS, float amplitude, float2 direction, float steepness, float waveLength, float t)
            {
                // Always normalize direction vector so magnitude doesn't stretch wavelength
                float2 d = normalize(direction);

                float k = (2.0f * PI) / max(0.0001f, waveLength);
                float angularFreq = sqrt(max(0.0f, WATER_G * k));

                // Spatial phase calculated from World Space XZ coordinates
                float phase = k * dot(d, positionWS.xz) - (angularFreq * t);

                float dx = steepness * amplitude * d.x * cos(phase);
                float dy = amplitude * sin(phase);
                float dz = steepness * amplitude * d.y * cos(phase);

                return float3(dx, dy, dz);
            
            }
            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 d = calculateOffsets(IN.positionOS, 0.5f, float2(1, 1), _Steepness, 10.0f, _Time.y);

                IN.positionOS.x += d.x;
                IN.positionOS.y = d.y;
                IN.positionOS.z += d.z;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);

                float2 baseNoiseUV = TRANSFORM_TEX(IN.uv, _NoiseTex);
                OUT.noiseUV1 = baseNoiseUV + (_ScrollSpeed1.xy * _Time.y);
                OUT.noiseUV2 = (baseNoiseUV * 1.2) + (_ScrollSpeed2.xy * _Time.y);

                return OUT;
               
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float depthDifference = 1.0;
                float waterDepthDifference = saturate(depthDifference / 1);
                float4 waterColour = lerp(_ShallowColour, _DeepColour, waterDepthDifference);
 
                float sample1 = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, IN.noiseUV1).r;
                float sample2 = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, IN.noiseUV2).r;

                float surfaceNoiseSample = (sample1 + sample2) * 0.5;
            
                float surfaceNoiseCutoff = depthDifference * _SurfaceNoiseCutoff;
 
                float aaWidth = 0.02;
                float surfaceNoise = smoothstep(surfaceNoiseCutoff - aaWidth, surfaceNoiseCutoff + aaWidth, surfaceNoiseSample);
                half4 surfaceNoiseColour = _FoamColour * surfaceNoise;
 


                return waterColour + surfaceNoiseColour;
                //return half4(surfaceNoiseSample, surfaceNoiseSample, surfaceNoiseSample, 1.0);

            }




            ENDHLSL
        }
    }
}
