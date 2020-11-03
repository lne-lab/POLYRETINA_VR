#ifndef PHOSPHERISATION_CGINC
#define PHOSPHERISATION_CGINC

	#include "UnityCG.cginc"
	#include "Coordinates.cginc"
	#include "Functions.cginc"

	/*
	 * Properties
	 */

#ifdef FIRST_PASS
	#define TEX _MainTex
#else
	#ifndef GRAB_PASS
	#define GRAB_PASS
	#endif
	#define TEX _GrabTexture
#endif

	sampler2D TEX;

	sampler2D _electrode_tex;
	float2 _eye_gaze;
	float2 _headset_diameter;
	float _electrode_radius;
	float _polyretina_radius;
	float _broken_chance;
	float _size_variance;
	float _intensity_variance;
	float _brightness;
	int _pulse;
	int _luminance_levels;
	float _luminance_boost;

	/*
	 * Functions
	 */

	float rand(float seed_x, float seed_y, float seed_z)
	{
		return frac(sin(dot(float3(seed_x, seed_y, seed_z), float3(12.9898, 78.233, 45.5432))) * 43758.5453);
	}

	void calc_luminance(float2 eye_uv, out bool is_electrode, out float2 electrode_pos, out bool electrode_is_on, out float luminance)
	{
		// electrode position
		float4 data = tex2D(_electrode_tex, eye_uv);
		float2 position_um = data.rg;
		float2 position_px = retina_to_pixel(position_um, _headset_diameter) + _eye_gaze;

		// electrode position (out param)
		electrode_pos = position_px;

		// input luminance
		float3 input = tex2D(TEX, position_px).rgb;

		// electrode size, intensity and if broken
		float broken = step(data.b, _broken_chance);
		float size = 1.0 + lerp(-_size_variance, _size_variance, data.a);
		float intensity = 1.0 - lerp(0.0, _intensity_variance, rand(data.b, data.a, _Time.y));

		// distances
		float distance_to_electrode = distance(pixel_to_retina(eye_uv, _headset_diameter), position_um);
		float distance_to_fovea = length(position_um);

		// pixel is an electrode if... (out param)
		is_electrode = step(distance_to_electrode, _electrode_radius * size) *	// inside an electrode
			step(distance_to_fovea, _polyretina_radius - _electrode_radius) *	// inside the polyretina
			(1 - broken);														// electrode is not broken

		// electrode luminance (out param)
		luminance = Luminance(input);														// base luminancy
		luminance = clamp(luminance + _luminance_boost, 0, 1);								// boost activation chance and clamp
		luminance = round(luminance * (_luminance_levels - 1)) / (_luminance_levels - 1);	// set to an interval of levels
		luminance *= is_electrode * _brightness * intensity;								// adjust luminance

		// electrode is on (out param)
		electrode_is_on = luminance > .001;

		// apply on/off frequency pulse
		luminance *= _pulse;
	}

	float calc_luminance(float2 eye_uv)
	{
		bool is_electrode;
		float2 electrode_pos;
		bool electrode_is_on;
		float luminance;

		calc_luminance(eye_uv, is_electrode, electrode_pos, electrode_is_on, luminance);
		return luminance;
	}

	/*
	 * Frag (without fading)
	 */

	float4 phospherisation(float2 uv : TEXCOORD0) : SV_TARGET
	{
#ifdef GRAB_PASS
			_eye_gaze.y = -_eye_gaze.y;
#endif

			float2 eye_uv = uv - _eye_gaze;
			float luminance = calc_luminance(eye_uv);
			float4 output = float4(luminance.xxx, 1.0);

#ifdef OUTLINE
			output += outline_polyretina(eye_uv, _headset_diameter, _polyretina_radius);
#endif

			return output;
	}

	/*
	 * Fading
	 */

	 /*
	  * Properties
	  */

	sampler2D _fade_tex;

	float2 _eye_gaze_delta;

	float _fast_decay_time;
	float _slow_decay_time;
	float _fast_decay_rate;
	float _slow_decay_rate;
	float _decay_threshold;

	float _recovery_delay;
	float _recovery_time;
	float _recovery_exponent;

	/*
	 * Functions
	 */

	float delta_time()
	{
		return unity_DeltaTime.r;
	}

	float decay(float brightness, float epoch_p1, float epoch_pn)
	{
		// fast decay
		brightness -= (epoch_p1 < _fast_decay_time) * _fast_decay_rate * delta_time();

		// slow decay
		brightness -= (epoch_p1 > _fast_decay_time) * (epoch_pn < _slow_decay_time) * _slow_decay_rate * delta_time();

		return max(brightness, 0);
	}

	float recover(float brightness, float epoch_pn)
	{
		// brightness -> time
		float time = (1 - pow(1 - brightness, 1 / _recovery_exponent)) * _recovery_time;

		float after_delay = 1 - (epoch_pn < _recovery_delay);

		// increase time
		time += after_delay * delta_time();
		time = min(time, _recovery_time);

		// time -> brightness
		return 1 - pow(1 - (time / _recovery_time), _recovery_exponent);
	}

	float4 update_fade(bool electrode_is_on, float4 fade_data)
	{
		float brightness = fade_data.r;	// current potential brightness of the phosphene
		float epoch_p1 = fade_data.g;	// time since the first pulse (of this sequence)
		float epoch_pn = fade_data.b;	// time since the last pulse 

		epoch_p1 += delta_time();
		epoch_pn += delta_time();

		epoch_p1 *= 1 - electrode_is_on * _pulse * (brightness > .999);
		epoch_pn *= 1 - electrode_is_on * _pulse;

		brightness = decay(brightness, epoch_p1, epoch_pn);
		brightness = recover(brightness, epoch_pn);

		return float4(brightness, epoch_p1, epoch_pn, 0);
	}

	/*
	 * Frag (with fading)
	 */

	struct MRT
	{
		float4 phos : SV_TARGET0;
		float4 fade : SV_TARGET1;
	};

	MRT phos_w_fade_mrt(float2 uv : TEXCOORD0)
	{
		// correct eye-gaze

#ifdef GRAB_PASS
		_eye_gaze.y = -_eye_gaze.y;
		_eye_gaze_delta.y = -_eye_gaze_delta.y;
#endif

		float2 eye_uv = uv - _eye_gaze;

		// retrieve needed variables
		bool is_electrode;
		float2 electrode_pos;
		bool electrode_is_on;
		float luminance;
		calc_luminance(eye_uv, is_electrode, electrode_pos, electrode_is_on, luminance);

		// retrieve last frames fade data
		float4 fade_data = tex2D(_fade_tex, electrode_pos + _eye_gaze_delta);

		// apply fade if requested
#ifdef USE_FADING
		luminance *= fade_data.r;
#endif

		// output multiple render targets
		MRT mrt;

		// output phosphene image
		mrt.phos = float4(luminance.xxx, 1.0);

		// with optional outline
#ifdef OUTLINE
		mrt.phos += outline_polyretina(eye_uv, _headset_diameter, _polyretina_radius);
#endif

		// ouput updated fade data
		mrt.fade = update_fade(electrode_is_on, fade_data) * is_electrode;

		return mrt;
	}
#endif
