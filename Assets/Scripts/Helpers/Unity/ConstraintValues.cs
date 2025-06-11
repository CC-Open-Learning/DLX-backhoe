/*
* FILE			: ConstraintValues.cs
* PROJECT		: BroseVRTraining
* PROGRAMMER	: Andy Sgro
* FIRST VERSION : July 29, 2019
* DESCRIPTION	: This script provides public variables for the
*				  ConstrainPosition and PullyUp scripts.
*/

using UnityEngine;

namespace RemoteEducation.Helpers.Unity
{
	/**
	* NAME	  : ConstraintValues
	* PURPOSE : 
	*	- This script provides public variables for the
	*	  ConstrainPosition and PullyUp scripts.
	*	- The reason why this script is separate from those
	*	  scripts is because it allows the Game Designers to
	*	  adjust the public variables in one place, rather
	*	  than having to adjust the public variables in many
	*	  places.
*/
	public class ConstraintValues : MonoBehaviour
	{
		//********//
		// fields //
		//********//

		[Tooltip("Defines the boundary that constrains the game objects.")]
		public BoxCollider boundary;
		[Tooltip("The transform that the game objects follow.")]
		public Transform grabbable;

		//************//
		// properties //
		//************//

		/**
		* \brief	Returns the lower dimensions of the
		*			BoxCollider's boundary.
		*/
		public Vector3 Min
		{
			get
			{
				return boundary.bounds.min;
			}
		}

		/**
		* \brief	Returns the upper dimensions of the
		*			BoxCollider's boundary.
		*/
		public Vector3 Max
		{
			get
			{
				return boundary.bounds.max;
			}
		}

		/**
		* \brief	Informs the derived class about whether this
		*			class has valid (non-null) values.
		*/
		public bool IsValid
		{
			get
			{
				if ((boundary != null) & (grabbable != null))
					return true;
				else
					return false;
			}
		}
	}
}