using UnityEngine;

namespace LNE.Studies.FadingV1
{
    public class Billboard : MonoBehaviour
    {
        void Update()
        {
            transform.forward = transform.position - Camera.main.transform.position;
        }
    }
}
