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
            #pragma target 3.5
            #pragma require 2darray

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

            float4x4 screenMatrices[10];
            UNITY_DECLARE_TEX2DARRAY(screenTextures);
            int numScreens;

            // Returns (dstToBox, dstInsideBox). If ray misses box, dstInsideBox will be zero
            float2 rayBoxDst(float3 boundsMin, float3 boundsMax, float3 rayOrigin, float3 invRaydir) {
                // Adapted from: http://jcgt.org/published/0007/03/04/
                float3 t0 = (boundsMin - rayOrigin) * invRaydir;
                float3 t1 = (boundsMax - rayOrigin) * invRaydir;
                float3 tmin = min(t0, t1);
                float3 tmax = max(t0, t1);
                
                float dstA = max(max(tmin.x, tmin.y), tmin.z);
                float dstB = min(tmax.x, min(tmax.y, tmax.z));

                // CASE 1: ray intersects box from outside (0 <= dstA <= dstB)
                // dstA is dst to nearest intersection, dstB dst to far intersection

                // CASE 2: ray intersects box from inside (dstA < 0 < dstB)
                // dstA is the dst to intersection behind the ray, dstB is dst to forward intersection

                // CASE 3: ray misses box (dstA > dstB)

                float dstToBox = max(0, dstA);
                float dstInsideBox = max(0, dstB - dstToBox);
                return float2(dstToBox, dstInsideBox);
            }
            
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
            float4 tint;

            bool hit(float3 pos, float3 dir, float depth, float4x4 boxMatrix) {
                float2 res = rayBoxDst(-.5, 0.5,mul(boxMatrix, float4(pos,1)), 1/mul(boxMatrix, float4(dir,0)));
                return res.y > 0 && res.x < depth;

            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Create ray
                float3 rayPos = _WorldSpaceCameraPos;
                float viewLength = length(i.viewVector);
                float3 rayDir = i.viewVector / viewLength;

                float nonlin_depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                float depth = LinearEyeDepth(nonlin_depth) * viewLength;

                fixed4 col = tex2D(_MainTex, i.uv);
                return UNITY_SAMPLE_TEX2DARRAY(screenTextures, float3(i.uv.xy, 1));
                //fixed4 portalCol = tex2D(portalTexture, i.uv);
                
                for (int screenIndex = 0; screenIndex < numScreens; screenIndex ++) {
                    if (hit(rayPos, rayDir, depth, screenMatrices[screenIndex])) {//
                       // col = tex2D(portalTexture, i.uv);
                       col = UNITY_SAMPLE_TEX2DARRAY(screenTextures, float3(i.uv.xy, screenIndex));
                    }
                }
                
                return col;
            }
            ENDCG
        }
    }
}
