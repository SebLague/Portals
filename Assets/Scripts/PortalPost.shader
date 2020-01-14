Shader "Hidden/PortalPost"
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

            sampler2D _CameraDepthTexture;
            sampler2D _MainTex;
            sampler2D portalTexture;
            sampler2D portalDepthTexture;
            float4 tint;


            fixed4 frag (v2f i) : SV_Target
            {
                // Create ray
                float3 rayPos = _WorldSpaceCameraPos;
                float viewLength = length(i.viewVector);
                float3 rayDir = i.viewVector / viewLength;

                fixed4 sceneCol = tex2D(_MainTex, i.uv);
                fixed4 portalCol = tex2D(portalTexture, i.uv);

                float portalDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(portalDepthTexture, i.uv)) * viewLength;
                float sceneDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv)) * viewLength;
                
                //return portalCol;
                //return sceneCol;
                return (portalDepth < sceneDepth+.1) ? portalCol : sceneCol;
            }
            ENDCG
        }
    }
}
