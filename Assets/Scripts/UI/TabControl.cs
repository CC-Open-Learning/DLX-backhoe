/*
 *  FILE          : TabControl.cs
 *  PROJECT       :	JWCProject
 *  PROGRAMMER    :	Michael Hilts
 *  FIRST VERSION :	2019-11-01
 *  DESCRIPTION   : This file contains the TabControl class which is responsible for
 *					allowing the user to tab through ui elements in its selectables array.
 */

#region Using Statements

using UnityEngine;
using UnityEngine.UI;

#endregion

/*
 * CLASS:       TabControl
 * DESCRIPTION: This class is responsible for allowing the user to tab through ui elements
 *				in its selectables array
 */
namespace RemoteEducation.UI
{
	public class TabControl : MonoBehaviour
	{
		[Tooltip("All selectable ui elements that will be tabbed through.")]
		[SerializeField]
		private Selectable[] selectables = null;

		private int selected;   // currently selected index

		private float update;   // used in update() to limit calls to the tab keyup listener

		#region MonoBehaviour Callbacks

		/*
		 * FUNCTION    : Start()
		 * DESCRIPTION : Start is called once before the first frame update. It will set
		 *				 the selected index to -1 to signify nothing.
		 * PARAMETERS  :
		 *		VOID
		 * RETURNS     :
		 *		VOID
		 */

		void Start()
		{
			if (selectables != null)
			{
				selected = -1;
			}
		}

		/*
		 * FUNCTION    : Update()
		 * DESCRIPTION : Update is called once per frame. It will call CheckForTab().
		 * PARAMETERS  :
		 *		VOID
		 * RETURNS     :
		 *		VOID
		 */

		void Update()
		{
			//Use timer to limit calls to CheckForTab() listener to each second
			//Without this timer multiple calls to CheckForTab can happen as the user presses the tab key
			//this is a problem as allowing such an action leads to an irregular selection process with tab.
			update += Time.deltaTime;
			if (update > 1.0f)
			{
				update = 0.0f;
				CheckForTab();
			}
		}

		/*
		 * FUNCTION    : OnDisable()
		 * DESCRIPTION : This method is called when the object is disabled. It
		 *				 will set the selected index to nothing.
		 * PARAMETERS  :
		 *		VOID
		 * RETURNS     :
		 *		VOID
		 */

		private void OnDisable()
		{
			selected = -1;
		}

		#endregion

		#region Private Methods

		/*
		 * FUNCTION    : SelectFirst()
		 * DESCRIPTION : This method will set the selected index to 0 and select it.
		 * PARAMETERS  :
		 *		VOID
		 * RETURNS     :
		 *		VOID
		 */

		private void SelectFirst()
		{
			selected = 0;
			selectables[selected].Select();
		}

		/*
		 * FUNCTION    : CheckForTab()
		 * DESCRIPTION : If the tab key is pressed this method will incrememnt the selected
		 *				 index and select that ui element.
		 * PARAMETERS  :
		 *		VOID
		 * RETURNS     :
		 *		VOID
		 */

		private void CheckForTab()
		{
			// if tab is pressed
			if (Input.GetKeyUp(KeyCode.Tab))
			{
				// if selected is nothing, select first
				if (selected == -1)
				{
					SelectFirst();
				}

				// is ui element is already selected
				else
				{
					// increment selected and if not past the last select the element
					if (++selected <= selectables.Length - 1)
					{
						selectables[selected].Select();
					}

					// if selected was past the last element reset first
					else
					{
						SelectFirst();
					}
				}
			}
		}

		#endregion
	}
}