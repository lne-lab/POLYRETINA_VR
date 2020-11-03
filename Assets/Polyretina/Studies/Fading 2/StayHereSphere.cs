using UnityEngine;
using UnityEngine.Events;

namespace LNE.Studies.FadingV2
{
	public class StayHereSphere : MonoBehaviour
	{
#pragma warning disable 649
		[SerializeField]
		private Transform _target;

		[SerializeField]
		private float _radius;

		[SerializeField]
		private UnityEvent _onLeaveSphere;

		[SerializeField]
		private UnityEvent _onEnterSphere;
#pragma warning restore 649

		private bool inside;

		void Update()
		{
			if (Vector3.Distance(_target.position, transform.position) > _radius)
			{
				if (inside)
				{
					_onLeaveSphere.Invoke();
					inside = false;
				}
			}
			else
			{
				if (inside == false)
				{
					_onEnterSphere.Invoke();
					inside = true;
				}
			}
		}

		public void ThisToTarget()
		{
			transform.position = _target.position;
		}

		public void TargetToThis()
		{
			_target.position = transform.position;
		}
	}
}
