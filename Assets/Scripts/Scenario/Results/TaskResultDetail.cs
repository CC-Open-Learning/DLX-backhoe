/*
*  FILE          :	ResultDetail.cs
*  PROJECT       :	JWCProject
*  PROGRAMMER    :	Michael Hilts
*  FIRST VERSION :	2019-10-20
*  DESCRIPTION   : This file contains the ResultDetail class which is responsible for displaying
*					the result of a single task.
*/

#region Using Statements

using TMPro;
using UnityEngine;

#endregion

namespace RemoteEducation.Scenarios
{
	/*
	* CLASS:       ResultDetail
	* DESCRIPTION: This class is responsible for displaying the result of a single task.
	*/

	public class TaskResultDetail : MonoBehaviour
	{
		//private TaskInfo info;                      // the task info to display

		[Tooltip("The title text component of the detail panel")]
		[SerializeField]
		private TextMeshProUGUI titleText;


		[Header("Description")]
		[Tooltip("The description area of the detail panel")]
		[SerializeField]
		private GameObject descriptionBlock;

		[Tooltip("The description text component of the detail panel")]
		[SerializeField]
		private TextMeshProUGUI descriptionText;


		[Header("Comment")]
		[Tooltip("The comment area of the detail panel")]
		[SerializeField]
		private GameObject commentBlock;

		[Tooltip("The comment text component of the detail panel")]
		[SerializeField]
		private TextMeshProUGUI commentText;


		[Header("Score")]
		[Tooltip("The result area of the detail panel")]
		[SerializeField]
		private GameObject scoreBlock;

		[Tooltip("The pass/fail text component of the detail panel")]
		[SerializeField]
		private ScoreText scoreText;

        //[Tooltip("The result progress bar")]
        //[SerializeField]
        //private ProgressBar resultProgress;


        /*
		* FUNCTION    : SetDetailsFromTask()
		* DESCRIPTION : displays the info from the TaskInfo struct.
		* PARAMETERS  :
		*		VOID
		* RETURNS     :
		*		VOID
		*/
        public void SetDetailsFromTask(TaskDetails task)
        {
            bool taskPasses = task.State == UserTaskVertex.States.Complete;

            // Display Task Title
            titleText.text = task.Title;

            SetDescription(task.Description);

            SetComment(task.Comments);
            
			SetScore(true, taskPasses);
        }

        private void SetDescription(string description)
        {
			if (description == null || description.Equals(string.Empty))
            {
				descriptionBlock.SetActive(false);
				return;
            }

			descriptionText.text = description;
			descriptionBlock.SetActive(true);
        }

		private void SetComment(string comment)
        {
			if (comment == null || comment.Equals(string.Empty))
			{
				commentBlock.SetActive(false);
				return;
			}

			commentText.text = comment;
			commentBlock.SetActive(true);
		}

		private void SetScore(bool active, bool value)
        {
			scoreText.Pass = value;
			scoreBlock.SetActive(active);
        }
	}
}