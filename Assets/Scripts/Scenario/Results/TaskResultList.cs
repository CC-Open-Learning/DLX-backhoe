/*
 *  FILE          :	TaskResultList.cs
 *  PROJECT       :	CORE Engine
 *  PROGRAMMER    : Michael Hilts, David Inglis
 *  FIRST VERSION :	2020-02-08
 *  DESCRIPTION   : 
 *		The TaskResultList class is responsible for displaying the Tasks of a given Scenario
 *		as TaskResultItem objects in a list view. In the list view, the name of the Task, 
 *		as well as a 'checkmark' or 'cross' to indicate if the Task was completed successfully, 
 *		are displayed. 
 *		
 *		Each TaskResultItem can be clicked on to display a more detailed view, which is
 *		managed the referenced TaskResultDetail object
 */

#region Resources

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace RemoteEducation.Scenarios
{
	public class TaskResultList : MonoBehaviour
	{
		#region Fields

		List<TaskResultItem> resultItems;

		[Header("Resources")]
		[Tooltip("The PassFail GameObject which is a child of the background of the canvas.")]
		[SerializeField] private GameObject taskResultResource;

		[Header("Panel Setup")]
		[SerializeField] private RectTransform taskResultContent;

		[Tooltip("Controller for 'Task Details' content area")]
		[SerializeField] private TaskResultDetail taskResultDetail;

		#endregion


		#region Public Methods

		/// <summary>
		/// Generate all the <see cref="TaskResultItem"/>s that will be shown 
		/// on the results screen. 
		/// </summary>
		/// <param name="tasks">All of the <see cref="TaskDetails"/> objects that represent 
		/// all the tasks that were in the scene.</param>
		public void GenerateTaskResults(List<TaskDetails> tasks)
		{
			resultItems = new List<TaskResultItem>();

            // loop through each visible Task
            foreach (TaskDetails current in tasks)
            {
                // instantiate a taskresult as a child of this transform and get reference to the component
                GameObject taskResult = Instantiate(taskResultResource, taskResultContent);

                // Set relevant information in TaskResultItem
                TaskResultItem item = taskResult.GetComponent<TaskResultItem>();
                item.SetDetails(current);

				item.Parent = this;

                resultItems.Add(item);
            }

            // Set the first Task as selected
            Select(resultItems.FirstOrDefault());
		}


		/*
		 * FUNCTION    : Select()
		 * DESCRIPTION : 
		 *		Call the 'Select' method on the provided TaskResultItem, and deselect 
		 *		all other TaskResultItem objects.
		 *		
		 *		If this object has a TaskResultDetail reference, the specified TaskResultItem
		 *		is passed through to the TaskResultDetail
		 *		
		 * PARAMETERS  :
		 *		TaskResultItem item : the TaskResultItem to be selected
		 * RETURNS     :
		 *		void
		 */
		public void Select(TaskResultItem item)
		{
			if (!item) { return; }

			item.Select();

			ResetAll(item);

			if (taskResultDetail)
			{
                taskResultDetail.SetDetailsFromTask(item.TaskDetails);
            }
        }


		/*
		 * FUNCTION    : ResetAll()
		 * DESCRIPTION : 
		 *		This method will call the Reset method of each TaskResultItem, except for the
		 *		one 'exception' if it is provided
		 * PARAMETERS  :
		 *		TaskResultItem exception : the TaskResultItem not to reset
		 * RETURNS     :
		 *		void
		 */
		public void ResetAll(TaskResultItem exception = null)
		{
			foreach (TaskResultItem item in resultItems.Except(new List<TaskResultItem>() { exception }))
			{
				item.Reset();
			}
		}

		#endregion
	}
}
