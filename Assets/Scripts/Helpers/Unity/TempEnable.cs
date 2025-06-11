/*
 *  FILE          :	TempEnable.cs
 *  PROJECT       :	JWCProject
 *  PROGRAMMER    :	Andy Sgro
 *  FIRST VERSION :	Jan 6, 2020
 *  DESCRIPTION   : Temporally activates an object. 
 */

using RemoteEducation;
using UnityEngine;

namespace RemoteEducation.Helpers.Unity
{
	/**
	* NAME	  : TempEnable
	* PURPOSE : 
	*	- Temporally activates an object. 
*/
	public class TempEnable : MonoBehaviour
	{
		//********//
		// fields //
		//********//

		[SerializeField] internal GameObject highlight = null;
		[SerializeField] internal float timeout = 0.25f;

		// keeps track when the lastest highlight request was made
		private float highlightRequest = Constants.UNDEFINED;


		//***************************//
		// non-monobehaviour methods //
		//***************************//

		/**
		*	\brief	Updates the highlightRequest field,
		*			which makes the outline visible for a brief period of time.
		*/
		public void Highlight()
		{
			highlightRequest = Time.timeSinceLevelLoad;
		}

		//***********************//
		// monobehaviour methods //
		//***********************//

		/**
			*	\brief	If the highlight request has expired (or no highlights were made in the first place),
			*			don't show the outline.
			*/
		private void LateUpdate()
		{
			if (highlightRequest != Constants.UNDEFINED)
				if (Time.timeSinceLevelLoad < (highlightRequest + timeout))
					highlight.SetActive(true);
				else
					highlight.SetActive(false);
		}

		/**
			*	\brief	Tells the developer if there are errors.
			*/
		private void Start()
		{
			if ((timeout <= 0) | (highlight == null))
				Debug.LogError(gameObject + "'s Highlighter component has invalid fields.");
		}
	}
}