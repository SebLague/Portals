Shader "Unlit/RecTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _V("V", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            int mode;
            float _V;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
               
                //return float4(i.uv.xy,0,0);
                float2 uv = i.screenPos.xy / i.screenPos.w;
                float2 uvc = ((uv*2-1) * _V)*.5+.5;
                
                //return float4(uv.xy,0,0);
                if (mode == 1) {
                    return 0;
                } 
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                
   
                return col;
            }
            ENDCG
        }
    }
}
