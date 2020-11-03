using UnityEngine;

namespace LNE.ProstheticVision
{
	using SP = ShaderProperties;

	[CreateAssetMenu(fileName = "Edge Detector", menuName = "LNE/External Processor/Edge Detector")]
	public class EdgeDetector : ExternalProcessor
	{
		/*
		 * Public fields
		 */

		[Space]
		public Shader shader = null;

		public Strength sensitivity = Strength.Low;

		[Range(.1f, 20)]
		public float contrast = 1;
		[Range(-.5f, .5f)]
		public float brightness = 0;
		[Range(-10, 10)]
		public float saturation = 2;

		[Range(0, 6)]
		public int thickness = 0;

		[Range(0, 1)]
		public float threshold = .1f;

		/*
		 * Private fields
		 */

		private Material material;

		/*
		 * ImageRenderer overrides
		 */

		public override void Start()
		{
			if (shader == null)
			{
				Debug.LogError($"{name} does not have a shader.");
				return;
			}

			material = new Material(shader);
		}

		public override void Update()
		{
			if (material == null)
			{
				Debug.LogError($"{name} does not have a material.");
				return;
			}

#if UNITY_EDITOR
			// update this every frame only while in the editor
			UpdateSensitivity();

			material.SetFloat(SP.edgeContrast, contrast);
			material.SetFloat(SP.edgeBrightness, brightness);
			material.SetFloat(SP.edgeSaturation, saturation);

			UpdateThickness();
			material.SetFloat(SP.edgeThreshold, threshold);
#endif
		}

		public override void GetDimensions(out int width, out int height)
		{
			throw new System.Exception("Edge Detector does not have dimensions.");
		}

		public override void OnRenderImage(Texture source, RenderTexture destination)
		{
			if (material == null)
			{
				Debug.LogError($"{name} does not have a material.");
				Graphics.Blit(source, destination);
				return;
			}

			if (on)
			{
				Graphics.Blit(source, destination, material);
			}
			else
			{
				Graphics.Blit(source, destination);
			}
		}

		/*
		 * Private methods
		 */

		private void UpdateThickness()
		{
			if (material.IsKeywordEnabled($"THICKNESS_{thickness}") == false)
			{
				SetThickness(thickness);
			}
		}

		private void UpdateSensitivity()
		{
			if (sensitivity == Strength.High && (material.IsKeywordEnabled("TAP_5") || material.IsKeywordEnabled("TAP_13")))
			{
				material.DisableKeyword("TAP_5");
				material.DisableKeyword("TAP_13");
			}
			else if (sensitivity == Strength.Medium && material.IsKeywordEnabled("TAP_5") == false)
			{
				material.EnableKeyword("TAP_5");
				material.DisableKeyword("TAP_13");
			}
			else if (sensitivity == Strength.Low && material.IsKeywordEnabled("TAP_13") == false)
			{
				material.DisableKeyword("TAP_5");
				material.EnableKeyword("TAP_13");
			}
		}

		private void SetThickness(int thickness)
		{
			material.EnableKeyword($"THICKNESS_{thickness}");
			for (int i = 0; i <= 6; i++)
			{
				if (i != thickness)
				{
					material.DisableKeyword($"THICKNESS_{i}");
				}
			}
		}
	}
}
