Shader "Custom/Water"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        _NoiseTex ("Perlin Noise Texture", 2D) = "white" {}
        _ScrollSpeed ("Scroll Sped (X,Y)", Vector) = (0.1, 0.1, 0, 0)
        _ShallowColour ("Shallow Colour", Color) = (1,1,1,1)
        _DeepColour ("Deep Colour", Color) = (1,1,1,1)
        _FoamColour ("Foam Colour", Color) = (1, 1, 1, 1)
        _SurfaceNoiseCutoff ("Foam Cutoff", Range(0, 1)) = 0.5
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

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);


            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
                float4 _NoiseTex_ST;
                float4 _ScrollSpeed;
                half4 _ShallowColour;
                half4 _DeepColour;
                half4 _FoamColour;
                float _SurfaceNoiseCutoff;
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
                float2 noiseUV : TEXCOORD1;
            };



            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);

                float2 noiseUV = TRANSFORM_TEX(IN.uv, _NoiseTex);
                OUT.noiseUV = noiseUV + (_ScrollSpeed.xy * _Time.y);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float depthDifference = 1.0;
                float waterDepthDifference = saturate(depthDifference / 1);
                float4 waterColour = lerp(_ShallowColour, _DeepColour, waterDepthDifference);
 
                float surfaceNoiseSample = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, IN.noiseUV).r;
 
                float surfaceNoiseCutoff = depthDifference * _SurfaceNoiseCutoff;
 
                float aaWidth = 0.02;
                float surfaceNoise = smoothstep(surfaceNoiseCutoff - aaWidth, surfaceNoiseCutoff + aaWidth, surfaceNoiseSample);
                half4 surfaceNoiseColour = _FoamColour * surfaceNoise;
 
                return waterColour + surfaceNoiseColour;
            }
            ENDHLSL
        }
    }
}
