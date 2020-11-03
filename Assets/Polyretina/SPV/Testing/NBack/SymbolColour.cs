using UnityEngine;

namespace LNE.Testing.NBack
{
	public class SymbolColour : MonoBehaviour
	{
		public Color colour
		{
			get
			{
				return default;
			}

			set
			{
				foreach (var renderer in GetComponentsInChildren<Renderer>())
				{
					renderer.material.color = value;
				}
			}
		}
	}
}
