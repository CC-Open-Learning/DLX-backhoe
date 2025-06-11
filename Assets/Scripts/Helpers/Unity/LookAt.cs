/*
FILENAME		: LookAt.cs
PROJECT			: JWC Project
PROGRAMMERS		: Andy Sgro
FIRST VERSION	: Nov 26, 2019
DESCRIPTION		: This class forces a Transform to look at another Transform.
*/

using UnityEngine;

namespace RemoteEducation.Helpers.Unity
{
	/**
	* NAME	  : LookAt
	* PURPOSE : 
	*	- This class forces a Transform to look at another Transform.
*/
	public class LookAt : MonoBehaviour
	{
		public Transform ToLookAt;

		/**
		* \brief	Makes the GameObject face the object specified
		*			by the 'ToLookAt' field.
		*/
		private void Update()
		{
			transform.eulerAngles = Physics3.GetAngle(transform.position, ToLookAt.position);
		}
	}
}