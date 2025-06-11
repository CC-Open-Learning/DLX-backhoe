/*
 *  FILE          :	TaskResultItem.cs
 *  PROJECT       :	CORE Engine
 *  PROGRAMMER    : 
 *  FIRST VERSION :	2020-2-8
 *  DESCRIPTION   : 
 *		The TaskResult class is responsible for displaying the name and pass or fail image for
 *		an interactor on the results scene. It also provides the ability to click on the result
 *		to display a more detailed view.
 */

#region Resources

using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#endregion

namespace RemoteEducation.Scenarios
{
	public class TaskResultItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
	{
		#region Fields

		public TaskDetails TaskDetails;

		private bool pass = false;                  // true if task was passed

		[NonSerialized] public TaskResultList Parent;

		[NonSerialized] public bool selected = false;              // true if this element is currently selected

		[Header("Text")]

		[Tooltip("Text field to display the Task title")]
		[SerializeField] private TextMeshProUGUI titleText = null;


		[Header("Visuals")]

		[Tooltip("The background of the UI element")]
		[SerializeField] private Image background;

		[Tooltip("Color block for TaskResultItem background")]
		[SerializeField] private ColorBlock backgroundColors;

		[Tooltip("The passImage of this taskresult")]
		[SerializeField] private GameObject passImage = null;

		[Tooltip("The failImage of this taskresult")]
		[SerializeField] private GameObject failImage = null;

        #endregion

        #region Properties

        

        // display proper image when pass is set
        public bool Pass
		{
			get
			{
				return pass;
			}
			set
			{
				pass = value;
				if (pass)
				{
					passImage.SetActive(true);
				}
				else
				{
					failImage.SetActive(true);
				}
			}
		}

		#endregion

		#region MonoBehaviour Callbacks

		/*
		 * FUNCTION    : Awake()
		 * DESCRIPTION : 
		 *		This method will get reference to the background image and set the
		 *		oass and fail images inactive
		 * PARAMETERS  :
		 *		void
		 * RETURNS     :
		 *		void
		 */

		private void Awake()
		{
			passImage.SetActive(false);
			failImage.SetActive(false);

			Reset();
		}

		#endregion

		#region Private Methods

		/*
		 * FUNCTION    : SetAlpha()
		 * DESCRIPTION : 
		 *		This method will set the alpha of the background to the value passed in
		 * PARAMETERS  :
		 *		float newAlpha: value to set the alpha to
		 * RETURNS     :
		 *		void
		 */

		private void SetAlpha(float newAlpha)
		{
			Color tmp = background.color;
			tmp.a = newAlpha;
			background.color = tmp;
		}

		#endregion

		#region Interface Implementations

		/*
		 * FUNCTION    : OnPointerEnter()
		 * DESCRIPTION : 
		 *		This method will set the background alpha to 0.6 to signify the mouse is
		 *		over it when the user mouses over. If the element is already selected it will not.
		 * PARAMETERS  :
		 *		PointerEventData: event data associated with the mouse over event
		 * RETURNS     :
		 *		void
		 */

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (!selected)
			{
				Highlight();
			}
		}

		/*
		 * FUNCTION    : OnPointerExit()
		 * DESCRIPTION : 
		 *		This method will set the background alpha to 0 to signify the mouse is
		 *		no longer over it when the users mouse exits. If the element is already selected it will not.
		 * PARAMETERS  :
		 *		PointerEventData: event data associated with the mouse exit event
		 * RETURNS     :
		 *		void
		 */

		public void OnPointerExit(PointerEventData eventData)
		{
			if (!selected)
			{
				Reset();
			}
		}

		/*
		 * FUNCTION    : OnPointerClick()
		 * DESCRIPTION : 
		 *		This method will set the element as selected 
		 * PARAMETERS  :
		 *		PointerEventData: event data associated with the mouse exit event
		 * RETURNS     :
		 *		void
		 */

		public void OnPointerClick(PointerEventData eventData)
		{
			if (!selected && Parent)
			{
				Parent.Select(this);
			}
		}

		#endregion

		#region Public Methods

		public void SetDetails(TaskDetails taskDetails)
        {
			TaskDetails = taskDetails;
			titleText.text = taskDetails.Title;
			Pass = taskDetails.State == UserTaskVertex.States.Complete;
		}

		/*
		 * FUNCTION    : Highlight()
		 * DESCRIPTION : 
		 *		This method will set the background color to the 'highlighted' color of the 
		 *		'backgroundColors' ColorBlock to signify that the user has moved their cursor
		 *		over the element, but has not yet selected it
		 * PARAMETERS  :
		 *		Void
		 * RETURNS     :
		 *		void
		 */
		public void Highlight()
		{
			background.color = backgroundColors.highlightedColor;
		}

		/*
		 * FUNCTION    : Select()
		 * DESCRIPTION : 
		 *		This method will set the background color to the 'selected' color of the 
		 *		'backgroundColors' ColorBlock to signify that the element is selected,
		 *		as well as set the selected flag to true.
		 * PARAMETERS  :
		 *		Void
		 * RETURNS     :
		 *		void
		 */
		public void Select()
		{
			background.color = backgroundColors.selectedColor;
			selected = true;
		}

		/*
		 * FUNCTION    : Reset()
		 * DESCRIPTION : 
		 *		This method will set the background color to the 'normal' color of the 
		 *		'backgroundColors' ColorBlock to signify that the element is no longer selected, 
		 *		as well as set the selected flag to false.
		 * PARAMETERS  :
		 *		Void
		 * RETURNS     :
		 *		void
		 */
		public void Reset()
		{
			background.color = backgroundColors.normalColor;
			selected = false;
		}

		#endregion
	}
}