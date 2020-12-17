using UnityEngine;

namespace LNE.Studies.Augmented
{
	using ProstheticVision;

	public class FovChanger : MonoBehaviour
	{
		public void ChangeFieldOfView()
		{
			var implant = Prosthesis.Instance.Implant as EpiretinalImplant;

			implant.fieldOfView += 10;

			if (implant.fieldOfView > 45)
			{
				implant.fieldOfView = 15;
			}
		}
	}
}
