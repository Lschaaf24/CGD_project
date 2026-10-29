Shader "Custom/URP-Skybox-Cubemap-MultiAxis"
{
    Properties
    {
        _Tex ("Cubemap (HDR)", Cube) = "_Skybox" {}
        [Gamma] _Exposure ("Exposure", Range(0, 8)) = 1.0
        _RotationX ("X Rotation Angle", Range(0, 360)) = 0
        _RotationY ("Y Rotation Angle", Range(0, 360)) = 0
        _RotationZ ("Z Rotation Angle", Range(0, 360)) = 0
        
        [KeywordEnum(Y_Axis, X_Axis, Z_Axis)] _RotationAxis("Rotation Axis", Float) = 0
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
            float _RotationX;
            float _RotationY;
            float _RotationZ;



             float3 RotateVector(float3 vertex, float degreesX, float degreesY, float degreesZ)
            {
                float alpha = -degreesX * PI / 180.0;
                float sina, cosa;
                sincos(alpha, sina, cosa);
                float2x2 m = float2x2(cosa, -sina, sina, cosa);

                float3 xAxis = float3(vertex.x, mul(m, vertex.yz));

                alpha = -degreesY * PI / 180.0;
                sincos(alpha, sina, cosa);
                m = float2x2(cosa, -sina, sina, cosa);
                float3 yAxis = float3(mul(m, vertex.xz), vertex.y).xzy;

                alpha = -degreesZ * PI / 180.0;
                sincos(alpha, sina, cosa);
                m = float2x2(cosa, -sina, sina, cosa);
                float3 zAxis = float3(mul(m, vertex.xy), vertex.z);
                
                return xAxis + yAxis + zAxis;
    
            }

            Varyings vert (Attributes input)
            {
                Varyings output;

                float3 rotatedPosition = RotateVector(input.positionOS.xyz, _RotationX, _RotationY, _RotationZ);
                
                output.positionCS = TransformObjectToHClip(rotatedPosition);
                output.texcoord = input.positionOS.xyz;
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
