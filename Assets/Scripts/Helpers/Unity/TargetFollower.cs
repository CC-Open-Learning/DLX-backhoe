/*
* FILE			: ReferenceFollower.cs
* PROJECT		: Brose VR Training Project
* PROGRAMMER	: Jerry
* FIRST VERSION : August 16, 2019
* DESCRIPTION	: This script makes the gameObject travel
*				  towards another gameObject.
*/

using UnityEngine;

namespace RemoteEducation.Helpers.Unity
{
	/**
	* NAME	  : ReferenceFollower
	* PURPOSE :
	*	- This script makes the gameObject follow another gameObject.
*/
	public class TargetFollower : MonoBehaviour
	{
		[SerializeField] public GameObject target = null;

		[SerializeField] private float moveSpeed = 5;
		[SerializeField] private Vector3 offset = Vector3.zero;


		/**
		* \brief	Make this gameObject follow another gameObject.
		*/
		void Update()
		{
			Vector3 offsetPosition = target.transform.position + offset;
			float maxDistDelta = Vector3.Distance(gameObject.transform.position, offsetPosition) * moveSpeed * Time.deltaTime;

			if ((target != null) & (gameObject.transform.position != offsetPosition))
			{
				gameObject.transform.position = Vector3.MoveTowards(
					gameObject.transform.position,
					offsetPosition,
					maxDistDelta);
			}
		}
	}
}