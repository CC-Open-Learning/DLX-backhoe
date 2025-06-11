/*
 *  FILE          :	RaycastOutliner.cs
 *  PROJECT       :	JWCProject
 *  PROGRAMMER    :	Andy Sgro
 *  FIRST VERSION :	Jan 7, 2020
 *  DESCRIPTION   : Outlines objects when they are pointed at.
 */

using UnityEngine;

namespace RemoteEducation.Helpers.Unity
{

	/**
	* NAME	  : RaycastOutliner
	* PURPOSE : 
	*	- This class inherits the Raycaster class, so it shoots
	*	  rays at objects.
	*	- When this class points to a GameObject with an Outliner
	*	  component, it calls its Highlight() method, which forms
	*	  an outline around that object.
	*/
	public class RaycastOutliner : Raycaster
	{
		//*******************//
		// overridden method //
		//*******************//

		/**
		 * \brief	When the parent Raycaster class shoots a ray that
		 *			collides with an object that not only is on a valid collision layer,
		 *			but also contains an Outliner component, then call the Highlight()
		 *			method on that component.
		 */
		protected override bool IntersectionAction(RaycastHit hitInfo)
		{
			// This class needs to be refactored to use the Outline class
			return false;
		}
	}
}