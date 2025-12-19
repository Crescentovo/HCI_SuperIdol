Shader "Custom/StencilMasked"
{
    //应用遮罩的对象
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        // ★ 关键：只有 stencil == 1 的区域才渲染 Sprite
        Stencil
        {
            Ref 1
            Comp Equal
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _Color;

            v2f vert (appdata_t IN)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(IN.vertex);
                o.texcoord = IN.texcoord;
                o.color = IN.color * _Color;
                return o;
            }

            fixed4 frag (v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                return c;
            }
            ENDCG
        }
    }

}
