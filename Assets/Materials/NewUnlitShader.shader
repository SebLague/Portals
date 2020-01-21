Shader "Unlit/RecTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _T("Truth amount", Range(0,1)) = 0
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
                float4 uv : TEXCOORD0;
                
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            int mode;
            float _T;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
               
                //return float4(i.uv.xy,0,0);
                float2 uv = i.screenPos.xy / i.screenPos.w;
     
                //return float4(uv.xy,0,0);
                if (mode == 1) {
                    return 0;
                } 
                // sample the texture
                float2 myUV = (i.uv.xy / i.uv.w) * .5 + .5;
                float4 truth = float4(uv, 0, 0);
                float4 test = float4(myUV,0,0);

                //return truth * _T + test * (1-_T);

                fixed4 col = tex2D(_MainTex, myUV);
                
   
                return col;
            }
            ENDCG
        }
    }
}
