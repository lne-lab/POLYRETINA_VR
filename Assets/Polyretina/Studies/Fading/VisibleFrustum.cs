using UnityEngine;

namespace LNE.Studies.FadingV1
{
	[RequireComponent(typeof(Camera))]
	public class VisibleFrustum : MonoBehaviour
	{
		private void OnDrawGizmos()
		{
			var cam = GetComponent<Camera>();

			Gizmos.matrix = cam.transform.localToWorldMatrix;

			// frustum
			Gizmos.DrawFrustum(
				Vector3.zero,
				cam.fieldOfView,
				cam.farClipPlane,
				cam.nearClipPlane,
				1
			);

			// line
			Gizmos.DrawFrustum(
				Vector3.zero,
				.00001f,
				cam.farClipPlane,
				cam.nearClipPlane,
				1
			);
		}
	}
}
