using UnityEngine;

namespace LNE.ProstheticVision
{
	/// <summary>
	/// Easy-access device shader property IDs
	/// </summary>
	public static class ShaderProperties
	{
		public static int	electrodeTexture	= Shader.PropertyToID("_electrode_tex"),
							electrodeRadius		= Shader.PropertyToID("_electrode_radius"),
							sizeVariance		= Shader.PropertyToID("_size_variance"),
							intensityVariance	= Shader.PropertyToID("_intensity_variance"),
							brokenChance		= Shader.PropertyToID("_broken_chance"),
							polyretinaRadius	= Shader.PropertyToID("_polyretina_radius"),
							axonTexture			= Shader.PropertyToID("_axon_tex"),
							decayConst			= Shader.PropertyToID("_decay_const"),
							seed0				= Shader.PropertyToID("_seed0"),
							seed1				= Shader.PropertyToID("_seed1"),
							seed2				= Shader.PropertyToID("_seed2"),
							eyeGaze				= Shader.PropertyToID("_eye_gaze"),
							pulse				= Shader.PropertyToID("_pulse"),
							headsetDiameter		= Shader.PropertyToID("_headset_diameter"),
							outlineDevice		= Shader.PropertyToID("_outline_device"),
							brightness			= Shader.PropertyToID("_brightness"),
							edgeContrast		= Shader.PropertyToID("_contrast"),
							edgeBrightness		= Shader.PropertyToID("_brightness"),
							edgeSaturation		= Shader.PropertyToID("_saturation"),
							edgeThickness		= Shader.PropertyToID("_thickness"),
							edgeSensitivity		= Shader.PropertyToID("_blur_tap"),
							luminanceLevels		= Shader.PropertyToID("_luminance_levels"),
							edgeThreshold		= Shader.PropertyToID("_threshold"),
							fadeTexture			= Shader.PropertyToID("_fade_tex"),
							eyeGazeDelta		= Shader.PropertyToID("_eye_gaze_delta"),
							luminanceBoost		= Shader.PropertyToID("_luminance_boost"),
							fastDecayTime		= Shader.PropertyToID("_fast_decay_time"),
							slowDecayTime		= Shader.PropertyToID("_slow_decay_time"),
							decayThreshold		= Shader.PropertyToID("_decay_threshold"),
							fastDecayRate		= Shader.PropertyToID("_fast_decay_rate"),
							slowDecayRate		= Shader.PropertyToID("_slow_decay_rate"),
							recoveryDelay		= Shader.PropertyToID("_recovery_delay"),
							recoveryTime		= Shader.PropertyToID("_recovery_time"),
							recoveryExponent	= Shader.PropertyToID("_recovery_exponent");
	}
}
