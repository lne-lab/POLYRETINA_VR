Shader "LNE/Saccade Simulation"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}

	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#define FIRST_PASS

			#include "UnityCG.cginc"
			#include "Vert.cginc"
			#include "Coordinates.cginc"

			sampler2D _MainTex;

			float2 _headset_diameter;
			float _electrode_pitch;

			uint _saccade_type;
			float _saccade_interval;

			float _interrupt_on_time;
			float _interrupt_off_time;

			static const float2 hexagon[] = 
			{ 
				float2(0, 0),
				float2(+1, 0),
				float2(-1, 0),
				float2(+0.5, +0.86603),
				float2(-0.5, +0.86603),
				float2(+0.5, -0.86603),
				float2(-0.5, -0.86603)
			};

			float2 calc_offset(float2 uv, float2 direction)
			{
				float2 r_uv = pixel_to_retina(uv, _headset_diameter);
				return retina_to_pixel(r_uv + direction, _headset_diameter);
			}

			float interrupt()
			{
				return step(fmod(_Time.y, _interrupt_on_time + _interrupt_off_time), _interrupt_on_time);
			}

			float4 frag(float2 uv : TEXCOORD0) : SV_TARGET
			{
				uint frame = _Time.y / _saccade_interval;
				float2 saccade_uv = calc_offset(uv, hexagon[frame % _saccade_type] * _electrode_pitch);

				return tex2D(_MainTex, saccade_uv) * interrupt();
			}
			ENDCG
		}
	}
}
