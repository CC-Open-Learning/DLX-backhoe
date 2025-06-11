/*
 *  FILE          :	RaycastTarget.cs
 *  PROJECT       :	JWCProject
 *  PROGRAMMER    :	Andy Sgro
 *  FIRST VERSION :	Jan 7, 2020
 *  DESCRIPTION   : Allows programmers to define what happens when rays
 *					are shot at objects, without needing to create an entierly
 *					new Raycast class just for that single use-case.
 */

using UnityEngine;

namespace RemoteEducation.Helpers.Unity
{
	/**
	* NAME	  : RaycastTarget
	* PURPOSE : 
	*	- The purpose of this class is to allow programmers to define what
	*	  happens what happens when raycasts hit this object.
	*	- This classes HitByRay() method is called by the Raycaster
	*	  class (or a child of it) when it has shot a ray this object.
	*	- This class prevents programmers from needing to duplicate Raycast logic
	*	  when everytime they need to implement it, because it's already in the Raycaster
	*	  class.
	*/
	public abstract class RaycastTarget : MonoBehaviour
	{
		//*******************//
		// abstract function //
		//*******************//

		/**
		 * \brief	Override this class to specify what happens when a compatible
		 *			Raycaster component has shot a ray at this GameObject. You can
		 *			tell if a Raycaster is compatible with this GameObject because
		 *			it shoots rays at the collision layer that this GameObject is on.
		 *			
		 * \param	RaycastHit hitInfo  : Contains information about the raycast collision.
		 * \param	Raycaster raycaster : The Raycaster that hit this GameObject.
		 */
		public abstract void HitByRay(RaycastHit hitInfo, Raycaster raycaster);
	}
}