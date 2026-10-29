Shader "Custom/URP-Skybox-Cubemap-Locked"
{
    Properties
    {
        _Tex ("Cubemap (HDR)", Cube) = "_Skybox" {}
        [Gamma] _Exposure ("Exposure", Range(0, 8)) = 1.0
        
        // Allows you to pick which side of the cubemap you are staring at
        _OffsetRotationY ("Manual Y Offset", Range(0, 360)) = 0 
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" "RenderPipeline"="UniversalPipeline" }
        Cull Off ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float3 texcoord     : TEXCOORD0;
            };

            TextureCube _Tex;
            SamplerState sampler_Tex;

            float _Exposure;
            float _OffsetRotationY;
            
            // The matrix we pass from the C# script
            float4x4 _CamCounterRotation;

            Varyings vert (Attributes input)
            {
                Varyings output;

                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                
                // 1. Counter-rotate the sampling vector by the camera's inverse matrix
                float3 lockedCoord = mul((float3x3)_CamCounterRotation, input.positionOS.xyz);
                
                // 2. Apply the manual Y offset so you can choose what part of the skybox to look at
                float radY = -_OffsetRotationY * PI / 180.0;
                float sina, cosa;
                sincos(radY, sina, cosa);
                float2x2 mY = float2x2(cosa, -sina, sina, cosa);
                lockedCoord.xz = mul(mY, lockedCoord.xz);

                output.texcoord = lockedCoord;
                
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                half4 c = _Tex.Sample(sampler_Tex, input.texcoord);
                c.rgb *= _Exposure;
                return half4(c.rgb, 1.0);
            }
            ENDHLSL
        }
    }
}