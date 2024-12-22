Shader "Unlit/PanelGlow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlowColor ("Glow Color", Color) = (1,1,1,1)
        _GlowIntensity ("Glow Intensity", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _GlowColor;
            float _GlowIntensity;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
              fixed4 texColor = tex2D(_MainTex,i .uv);
              return texColor + _GlowColor * _GlowIntensity * texColor.a;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
