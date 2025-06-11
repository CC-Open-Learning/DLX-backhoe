/*
 *  FILE          :	ScenarioResult.cs
 *  PROJECT       :	CORE Engine
 *  PROGRAMMER    :	David Inglis
 *  FIRST VERSION :	2020-12-01
 *  DESCRIPTION   : 
 *		The ScenarioResult class defines the overall behaviour for the ResultsScene. 
 *		
 *		ScenarioResult will call upon the TaskResultList class to instantiate a list of
 *		TaskResultItem objects which correlate to the Tasks of the performed Scenario
 *		
 *		The Scenario information will be gathered from the Scene-persistent ScenarioData GameObject
 *
 */

#region Resources

using TMPro;
using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

#endregion

namespace RemoteEducation.Scenarios
{
	/*
	* CLASS:       ScenarioResult
	* DESCRIPTION: This class is responsible for displaying the scenario summary on the results screen
	*				as well as instantiating all task and penalty summaries on the same page. 
	*/
	public class ScenarioResult : MonoBehaviour
	{
		#region Fields

		private Scenario scenario;

		[Header("Page Setup")]

		[Tooltip("Text field for the Results Scene title")]
		[SerializeField] private TextMeshProUGUI header;

		[Tooltip("Controller responsible for displaying Tasks in a scrollable list view")]
		[SerializeField] private TaskResultList taskResultList;

		[Tooltip("Dynamic text field for the final score of the Scenario")]
		[SerializeField] private ScoreText scoreText;

		[Tooltip("Display percent next to number of completed tasks?")]
		[SerializeField] private bool displayPercent = true;

		public GameObject completeButton;

		[SerializeField] private TextMeshProUGUI networkConnectionString;

		#endregion

		/*
		 *	FUNCTION    : LoadResults()
		 *	DESCRIPTION : 
		 *		Updates various text fields on the ResultsScene with Scenario-specific information, then
		 *		passes the provided Scenario to the TaskResultList referenced by the ScenarioResult object to
		 *		load a list view of the Scenario's Tasks
		 *		
		 *		After retrieving the 'score' of the Scenario, a percentage grade out of 100 is passed
		 *		to SendGradeAsync, which calls upon the APIGrades library
		 *		
		 *	PARAMETERS  :
		 *		Scenario scenario
		 *	RETURNS     :
		 *		void
		 */
		public void LoadResults(Scenario scenario, List<TaskDetails> details)
		{
			if (taskResultList)
			{
				//get the TaskManager to generate a list of all the tasks in the scenario.
				taskResultList.GenerateTaskResults(details);
			}

			// Set the Results Scene header text
			if (header)
			{
				header.text = string.Format("Results - {0}", scenario.Title);
			}

			int maxScore = details.Count;
			int currentScore = details.Where(x => x.State == UserTaskVertex.States.Complete).Count();

			float scorePercent = (currentScore / (float)maxScore);

			scoreText.SetScore(currentScore, maxScore, displayPercent);
		}
	}
}
