using UnityEngine;

namespace RemoteEducation.Helpers.Unity
{
	public class LookAtCamera : MonoBehaviour
	{
        private Transform mainCamera;

        private void Start()
        {
            if (Camera.main == null)
            {
                Destroy(this);
                return;
            }

            mainCamera = Camera.main.transform;
        }
		
		private void Update()
		{           
            transform.eulerAngles = Physics3.GetAngle(transform.position, mainCamera.position);
		}
	}
}