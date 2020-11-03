using UnityEngine;

namespace LNE.Studies.FadingV1
{
	public class RotateAround : MonoBehaviour
	{
#pragma warning disable 649
		[SerializeField]
		private Transform target;
#pragma warning restore 649

		public void Rotate(float angle)
		{
			transform.RotateAround(target.position, Vector3.up, angle);
		}
	}
}
