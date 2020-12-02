Shader "LNE/Crosshair"
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

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            static const float SIZE = 20.0;    //def=5.0
            static const float GIRTH = SIZE / 10.0;

            sampler2D _MainTex;
            float2 _target_pixel;

            float4 frag(v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                float2 current_pixel = i.uv * _ScreenParams.xy;

                if (current_pixel.x < _target_pixel.x + GIRTH && current_pixel.x > _target_pixel.x - GIRTH)
                {
                    if (current_pixel.y < _target_pixel.y + SIZE && current_pixel.y > _target_pixel.y - SIZE)
                    {
                        col.xyz = float3(1, 0, 0);
                    }
                }

                if (current_pixel.y < _target_pixel.y + GIRTH && current_pixel.y > _target_pixel.y - GIRTH)
                {
                    if (current_pixel.x < _target_pixel.x + SIZE && current_pixel.x > _target_pixel.x - SIZE)
                    {
                        col.xyz = float3(1, 0, 0);
                    }
                }

                return col;
            }
            ENDCG
        }
    }
}
