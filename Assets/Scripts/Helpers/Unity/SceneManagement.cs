/*
FILENAME		: SceneManagement.cs
PROJECT			: JWC Project
PROGRAMMERS		: Andy Sgro
FIRST VERSION	: Nov 8, 2019
DESCRIPTION		: This helps us start on the first scene, regardless of which scene
				  we're debugging on.
*/

using UnityEngine;

namespace RemoteEducation.Helpers.Unity
{
	/**
	* NAME	  : SceneManagement
	* PURPOSE : 
	*	- This helps us start on the first scene, regardless of which scene
		  we're debugging on.
*/
	public class SceneManagement : MonoBehaviour
	{
		//****************//
		// enums & fields //
		//****************//

		private enum SceneType
		{
			Start,
			Menu,
			Play
		}

		private static bool wasOnStartScene = false;
		//private static bool wasOnOtherScene = false;
		[SerializeField] private SceneType sceneType = SceneType.Play;

		//***********************//
		// monobehaviour methods //
		//***********************//

		/**
		 * \brief	When the scene loads up, if it's not the Start scene,
		 *			and the Start scene hasn't been loaded up yet,
		 *			exit this scene and load up the Start scene.
		 *			Otherwise, stay on this scene.
		 */
		private void Awake()
		{
			if (sceneType == SceneType.Start)
			{
				wasOnStartScene = true;
			}
			else
			{
				//wasOnOtherScene = true;
				if (!wasOnStartScene)
				{
					UnityEngine.SceneManagement.SceneManager.LoadScene(0);
				}
			}
		}
	}
}
