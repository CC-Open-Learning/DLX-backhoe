using UnityEngine;

namespace RemoteEducation.Helpers.Unity
{
	public class Rotate : MonoBehaviour
	{
        [Tooltip("Speed of rotation on each axis in degrees per second.")]
        [SerializeField] private Vector3 angularSpeed;

        private Vector3 angularSpeedRadians;

        private void Awake()
        {
            angularSpeedRadians = angularSpeed * Mathf.Deg2Rad;
        }

        private void Update()
		{
            transform.Rotate(angularSpeedRadians);
		}
	}
}