using LNE.UI.Attributes;
using UnityEngine;

namespace LNE.ProstheticVision
{
	[CreateAssetMenu(fileName = "Saccade Simulator", menuName = "LNE/External Processor/Saccade Simulator")]
	public class SaccadeSimulator : EdgeDetector
	{
		public enum SaccadeType { None = 1, Left = 2, LeftAndRight = 3, Hexagonal = 7 }

		/*
		 * Public fields
		 */

		public bool useEdgeDetection;

		[Header("Saccade")]

		[CustomLabel(label = "Shader")]
		public Shader saccadeShader = null;

		[CustomLabel(label = "Type")]
		public SaccadeType saccadeType = SaccadeType.None;

		[CustomLabel(label = "Interval")]
		public float saccadeInterval = 1;

		[Header("Interruption")]

		public float onTime;
		public float offTime;

		/*
		 * Private fields
		 */

		private Material saccadeMaterial = null;

		/*
		 * Private properties
		 */

		private EpiretinalImplant Implant => Prosthesis.Instance.Implant as EpiretinalImplant;

		/*
		 * Edge Detector overrides
		 */

		public override void Start()
		{
			base.Start();

			if (saccadeShader == null)
			{
				Debug.LogError($"{name} does not have a shader.");
				return;
			}

			saccadeMaterial = new Material(saccadeShader);
		}

		public override void Update()
		{
			base.Update();

#if UNITY_EDITOR
			// update this every frame only while in the editor
			saccadeMaterial.SetInt("_saccade_type", (int)saccadeType);
			saccadeMaterial.SetFloat("_saccade_interval", saccadeInterval);
			saccadeMaterial.SetVector("_headset_diameter", Implant.headset.GetRetinalDiameter());
			saccadeMaterial.SetFloat("_electrode_pitch", Implant.layout.GetSpacing(LayoutUsage.Anatomical));
			saccadeMaterial.SetFloat("_interrupt_on_time", onTime);
			saccadeMaterial.SetFloat("_interrupt_off_time", offTime);
#endif
		}

		public override void OnRenderImage(Texture source, RenderTexture destination)
		{
			var tempRT = RenderTexture.GetTemporary(source.width, source.height);

			var cachedOn = on;
			on = useEdgeDetection;
			base.OnRenderImage(source, tempRT);
			on = cachedOn;

			if (saccadeMaterial == null)
			{
				Debug.LogError($"{name} does not have a material.");
				Graphics.Blit(tempRT, destination);
				return;
			}

			if (on)
			{
				Graphics.Blit(tempRT, destination, saccadeMaterial);
			}
			else
			{
				Graphics.Blit(tempRT, destination);
			}

			RenderTexture.ReleaseTemporary(tempRT);
		}
	}
}
