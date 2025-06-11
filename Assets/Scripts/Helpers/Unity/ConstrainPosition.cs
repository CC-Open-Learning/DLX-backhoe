/*
* FILE			: ConstrainPosition.cs
* PROJECT		: BroseVRTraining
* PROGRAMMER	: Andy Sgro
* FIRST VERSION : July 29, 2019
* DESCRIPTION	: This script constrains the position of the GameObject
*				  that this script is attached to, so that it follows
*				  an object, but also stays within boundaries.
*/

using UnityEngine;

namespace RemoteEducation.Helpers.Unity
{
	/**
	* NAME	  : ConstrainPosition
	* PURPOSE : 
	*	- This script constrains the position of the GameObject
	*	  that this script is attached to, so that it follows
	*	  an object, but also stays within boundaries.
	*	- This script acquires tracking and constraint/boundary
	*	  information from the ConstraintValues script, which
	*	  acts as a set of public variables for this script.
	*	- You can choose which of the 3 axis are updated.
*/
	public class ConstrainPosition : MonoBehaviour
	{
		//********//
		// fields //
		//********//

		[Tooltip("Shared public variables. Defines the object that this game object follows, and the boundary that constrains this object.")]
		public ConstraintValues constraintValues;
		[Tooltip("Adjust the size of the boundary (in metres).")]
		public Vector3 boundarySizeOffset;
		public bool trackX;
		public bool trackY;
		public bool trackZ;


		//***********************//
		// monobehaviour methods //
		//***********************//

		/**
		 * \brief	Update position.
		 */
		private void Awake()
		{
			SetPosition();
		}

		/**
		 * \brief	Update position.
		 */
		private void Update()
		{
			SetPosition();
		}

		//***************************//
		// non-monobehaviour methods //
		//***************************//

		/**
		* \brief	In each Update, adjust this GameObject's position
		*			so that it's as close to the target as possible,
		*			while still being within the boundary.
		* 
		* \detail	The x, y, and z positions are only adjusted if this script's public
		*			boolean variables were set to true.
		*/
		private void SetPosition()
		{
			if (constraintValues.IsValid)
			{
				Vector3 parentPos = constraintValues.grabbable.position;
				Vector3 min = constraintValues.Min - boundarySizeOffset;
				Vector3 max = constraintValues.Max + boundarySizeOffset;
				var pos = transform.position;

				if (trackX)
				{
					transform.position = new Vector3(Mathf.Clamp(parentPos.x, min.x, max.x), pos.y, pos.z);
				}
				if (trackY)
				{
					transform.position = new Vector3(pos.x, Mathf.Clamp(parentPos.y, min.y, max.y), pos.z);
				}
				if (trackZ)
				{
					transform.position = new Vector3(pos.x, pos.y, Mathf.Clamp(parentPos.z, min.z, max.z));
				}
			}
		}

	}
}