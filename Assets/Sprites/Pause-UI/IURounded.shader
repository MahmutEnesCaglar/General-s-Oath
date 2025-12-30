Shader "UI/Rounded"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Radius ("Radius", Range(0, 100)) = 20
    }
    
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float2 uv : TEXCOORD0; float4 vertex : SV_POSITION; };
            
            sampler2D _MainTex;
            float4 _Color;
            float _Radius;
            
            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target {
                float2 uv = i.uv * 2 - 1;
                float dist = length(max(abs(uv) - 1 + _Radius/100, 0));
                float alpha = 1 - smoothstep(_Radius/100 - 0.01, _Radius/100, dist);
                
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                col.a *= alpha;
                return col;
            }
            ENDCG
        }
    }
}
