Shader "Hidden/Fog"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            // vertex input: position, UV
            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 viewVector : TEXCOORD1;
            };
            
            v2f vert (appdata v) {
                v2f output;
                output.pos = UnityObjectToClipPos(v.vertex);
                output.uv = v.uv;
                // Camera space matches OpenGL convention where cam forward is -z. In unity forward is positive z.
                // (https://docs.unity3d.com/ScriptReference/Camera-cameraToWorldMatrix.html)
                float3 viewVector = mul(unity_CameraInvProjection, float4(v.uv * 2 - 1, 0, -1));
                output.viewVector = mul(unity_CameraToWorld, float4(viewVector,0));
                return output;
            }

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            float4 params;

            float4 colA;
            float4 colB;
            float4 colC;
            float4 colD;

            float remap(float v, float minOld, float maxOld, float minNew, float maxNew) {
                return minNew + (v-minOld) * (maxNew - minNew) / (maxOld-minOld);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                 // Create ray
                float3 rayPos = _WorldSpaceCameraPos;
                float viewLength = length(i.viewVector);
                float3 rayDir = i.viewVector / viewLength;
                
                // Depth and cloud container intersection info:
                float nonlin_depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                float depth = LinearEyeDepth(nonlin_depth) * viewLength;
                float t = saturate(exp(-(depth + params.y) * params.x*.01));

                //return t;
                float4 skyCol = lerp(colB, colC, saturate(i.uv.x + params.w));
                //return skyCol;
                float4 fogCol = lerp(colA, skyCol, rayDir.y * .5 + .5);
               
                fixed4 col = tex2D(_MainTex, i.uv);
                return col * t + fogCol * (1-t);
            }
            ENDCG
        }
    }
}
