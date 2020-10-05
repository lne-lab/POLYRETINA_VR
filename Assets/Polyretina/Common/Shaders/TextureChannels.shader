Shader "LNE/TextureChannels" 
{
	Properties 
	{
		_SubTex ("Texture (RGB)", 2D) = "white" {}
		[Toggle] _R("R", Float) = 0
		[Toggle] _G("G", Float) = 0
		[Toggle] _B("B", Float) = 0
		[Toggle] _A("A", Float) = 0
	}

	SubShader 
	{
		Pass 
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"
			
			sampler2D _SubTex;
			float _R;
			float _G;
			float _B;
			float _A;

			float4 frag(v2f_img i) : COLOR
			{
				float4 input = tex2D(_SubTex, i.uv);
				float4 output = float4(0, 0, 0, 1);

				if (_R)
				{
					output.r = input.r;

					float gba = _G + _B + _A;

					if (gba == 0)
					{
						output.rgb = input.rrr;
					}
				}

				if (_G)
				{
					float r = _R;

					if (r == 0)
					{
						output.r = input.g;
					}
					else
					{
						output.g = input.g;
					}

					float rba = r + _B + _A;

					if (rba == 0)
					{
						output.rgb = input.ggg;
					}
				}

				if (_B)
				{
					float rg = _R + _G;

					if (rg == 0)
					{
						output.r = input.b;
					}
					else if (rg == 1)
					{
						output.g = input.b;
					}
					else
					{
						output.b = input.b;
					}

					float rga = rg + _A;

					if (rga == 0)
					{
						output.rgb = input.bbb;
					}
				}

				if (_A)
				{
					float rgb = _R + _G + _B;

					if (rgb == 0)
					{
						output.r = input.a;
					}
					else if (rgb == 1)
					{
						output.g = input.a;
					}
					else if (rgb == 2)
					{
						output.b = input.a;
					}
					else
					{
						output.a = input.a;
					}

					if (rgb == 0)
					{
						output.rgb = input.aaa;
					}
				}

				return output;
			}
			ENDCG
		}
	}
}
