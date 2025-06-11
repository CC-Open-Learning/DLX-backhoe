/*
 *	FILE		: TeleportOnFall.cs
 *	PROJECT		:
 */

using UnityEngine;

namespace RemoteEducation.Helpers.Unity
{
	public class TeleportOnFall : MonoBehaviour
	{
		[SerializeField] private Transform defaultPostion = null;
		[SerializeField] private Collider teleportCollider = null;

		private Vector3 defaultPostionVector = Vector3.zero;

		private void Awake()
		{
			defaultPostionVector = defaultPostion.position;
		}


		private void OnCollisionEnter(Collision collision)
		{
			if (collision.collider == teleportCollider)
			{
				transform.position = defaultPostionVector;
			}
		}
	}
}