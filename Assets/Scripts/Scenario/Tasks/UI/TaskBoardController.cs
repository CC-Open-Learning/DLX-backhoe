/*
 *  FILE          :	TaskBoardController.cs
 *  PROJECT       :	CORE Engine
 *  PROGRAMMER    : Chowon Jung, Kieron Higgs, David Inglis
 *  FIRST VERSION :	2020-10-27
 *  DESCRIPTION   : 
 *		The TaskBoardController class is responsible for instantiating task results into the
 *		scrollview, one for each task in the scenario being summarized.
 *		Also, the tasks' completion status will be synced in real-time through the task
 *		checking methods in the TaskManager. 
 */

#region Resources

using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine;
using RemoteEducation.Helpers.Unity;
using RemoteEducation.Scenarios;
using UnityEngine.UI;
using Lean.Gui;
using RemoteEducation;
using System.Linq;
using RemoteEducation.UI;

#endregion

namespace RemoteEducation.Scenarios
{
	public class TaskBoardController : MonoBehaviour, IScenarioGraphUI
	{
		private const string TaskButtonDefaultText = "Complete Current Task";


		#region Fields

		// Expression abstraction for TaskManager instance
		TaskManager TaskManager => ScenarioManager.Instance.TaskManager;

		// Flag used to ensure that tasks are initialized only once, commented out due to lack of references, only has an assignment.
		//private bool tasksLoaded = false;

		private List<TaskBoardElement> Tasklist;        // contains the task list of the current scenario and its progress status
        private List<UserTaskVertex> tasksInUI;         // all the tasks that are in the being show in the UI.

		public bool UseStateImages;                     // this can be toggle false when not using pass & warning images

		public GameObject TaskListItemPrefab;

		public Transform TaskListAnchor;

		public TextMeshProUGUI TaskListTitle;			// Text area for displaying task list title (including Scenario name)

		[Header("Task Element Colours")]

		public Color CompletedTaskColor;		// colour of task element UI background for default and completed tasks
		public Color CompletedTaskTextColor;    // colour of task element UI text for default and completed tasks

		public Color CurrentTaskColor;          // colour of task element UI background for the current task
		public Color CurrentTaskTextColor;      // colour of task element UI text for the current task

		public Color FutureTaskColor;           // colour of task element UI background for the future tasks
		public Color FutureTaskTextColor;       // colour of task element UI text for the future tasks

		public static TaskBoardController Instance; //im sorry

		[SerializeField]
		private LeanButton ButtonTaskButton;

        private UserTaskVertex currentTask;

		#endregion


		#region MonoBehaviour Callback

		/* 
		 *	CALLBACK    : Awake
		 *	DESCRIPTION	:
		 *		Instantiates the 'TaskList' Collection
		 */
		private void Awake()
		{
			Tasklist = new List<TaskBoardElement>();
			Instance = this;
		}

        #endregion

        public void InitializeUI(TaskManager taskManager)
        {
            //add this window to the window manager
            ScenarioManager.Instance.WindowManager.Add(GetComponent<WindowLoader>());

            try
            {
                TaskListTitle.text = string.Format("{0} Tasks", ScenarioManager.Instance.CurrentScenario.Title);
            }
            catch (Exception e)
            {
                Debug.LogError("TaskBoardController: " + e.ToString());
                return;
            }

            InitializeTaskElements(taskManager.AllTaskVertices);

            taskManager.OnCurrentTaskUpdated += RefreshTaskBoard;
            TaskManager.OnTaskCompletionStateChanged += UpdateElementIcon;
        }

		/* 
		 *	METHOD		: InitializeTaskElements
		 *	DESCRIPTION	:
		 *		Loops over tasks available in the task manager and creates 
		 *		the relevant list items.
		 *		
		 *	PARAMETERS	:
		 *		void
		 *	RETURNS		:
		 *		bool : Returns true if the initialization is successful, and
		 *			false if an exception occurs.
		 */
		private bool InitializeTaskElements(List<UserTaskVertex> tasks)
		{ 
            try
            {
                foreach (UserTaskVertex task in tasks.Where(x => !x.BlankTitleAndDescription))
                {
                    if (task != null)
                    {
                        // Create TaskListItemPrefab instance and grab TaskBoardElement controller reference
                        TaskBoardElement taskElement = Instantiate(TaskListItemPrefab).GetComponent<TaskBoardElement>();
                        taskElement.transform.SetParent(TaskListAnchor, false);
                        taskElement.SetTaskDetails(task);
                        Tasklist.Add(taskElement);

						foreach (UserTaskVertex subTask in TaskManager.GetSubTasks(task).Where(x => !x.BlankTitleAndDescription))
						{
							TaskBoardElement subTaskElement = Instantiate(TaskListItemPrefab).GetComponent<TaskBoardElement>();
							subTaskElement.transform.SetParent(TaskListAnchor, false);
							subTaskElement.SetTaskDetails(subTask);
							subTaskElement.Indent(20);
							Tasklist.Add(subTaskElement);
						}
					}
                }

                foreach(TaskBoardElement element in Tasklist)
                {
                    element.Background.color = FutureTaskColor;
                    element.TaskText.color = FutureTaskTextColor;
                }

                //keep a list of all the tasks that are in the UI
                tasksInUI = (from task in Tasklist select task.Task).ToList();
            }
            catch (Exception e)
            {
                Debug.LogError("TaskBoardController: " + e.ToString());
                return false;
            }

            return true;
        }


		/// <summary>
        /// This method will update the appearance of the Task Board when a new vertex becomes the 
        /// current vertex.
        /// It will update the icon next to the old current task to a check mark.
        /// Also the background of all the task will change colour to show current, past, and 
        /// future tasks.
        /// </summary>
        /// <param name="currentVertex">The vertex that is now the current vertex.</param>
		public void RefreshTaskBoard(GraphVertex currentVertex)
		{
            //if the new current vertex isn't a UserTaskVertex, this will be null.
            UserTaskVertex newCurrent = currentVertex as UserTaskVertex;

            //use null for a task that is not shown in the UI.
            if(!tasksInUI.Contains(newCurrent))
            {
                newCurrent = null;
            }

            //update the icon for the old current task.
             UpdateElementIcon(currentTask);

            // refresh UI appearance
            SetTaskProgressAppearance(newCurrent, currentTask);

            currentTask = newCurrent;
        }

        /// <summary>
        /// the above function but calls an update that will only update the icon to a pass.
        /// </summary>
        /// <param name="currentVertex"></param>
        public void RefreshTaskBoardLoad(GraphVertex currentVertex)
        {
            //if the new current vertex isn't a UserTaskVertex, this will be null.
            UserTaskVertex newCurrent = currentVertex as UserTaskVertex;

            //use null for a task that is not shown in the UI.
            if (!tasksInUI.Contains(newCurrent))
            {
                newCurrent = null;
            }

            //update the icon for the old current task.
            UpdateElementIconPass(currentTask);

            // refresh UI appearance
            SetTaskProgressAppearance(newCurrent, currentTask);

            currentTask = newCurrent;
        }


        /// <summary>
        /// Update all the back ground colours for all the task items to 
        /// show current, future, and past tasks. 
        /// </summary>
        /// <param name="newCurrent">The task that is now the current task.</param>
        /// <param name="oldCurrent">The last task that was the current task.</param>
        public void SetTaskProgressAppearance(UserTaskVertex newCurrent, UserTaskVertex oldCurrent)
		{
            //update the appearance of the last current task.
            if (oldCurrent != null)
            {
                TaskBoardElement element = Tasklist.Find(x => x.Task == oldCurrent);

                if (element == null)
                {
                    Debug.LogWarning($"No Task Board Element for task : {oldCurrent}");
                    return;
                }

                element.Background.color = CompletedTaskColor;
                element.TaskText.color = CompletedTaskTextColor;

                //update the colour for any subtasks of the task.
                IEnumerable<TaskBoardElement> subElements = from subTask in TaskManager.GetSubTasks(oldCurrent) select Tasklist.Find(x => x.Task == subTask);

                foreach (TaskBoardElement subElement in subElements)
                {
                    subElement.Background.color = CompletedTaskColor;
                    subElement.TaskText.color = CompletedTaskTextColor;
                }
            }

            if (newCurrent != null)
            {
                TaskBoardElement element = Tasklist.Find(x => x.Task == newCurrent);

                if (element == null)
                {
                    Debug.LogWarning($"No Task Board Element for task : {newCurrent}");
                    return;
                }

                element.Background.color = CurrentTaskColor;
                element.TaskText.color = CurrentTaskTextColor;
            }
        }

        public void UpdateElementIcon(GraphVertex vertex)
        {
            if (!(vertex is UserTaskVertex task))
            {
                return;
            }

            TaskBoardElement element = Tasklist.Find((x) => x.Task == task);

            // only modify pass & warning images when decided to use them
            if (element != null && UseStateImages)
			{
                TaskBoardElement.IconType type = element.State switch {
                    UserTaskVertex.States.Complete => TaskBoardElement.IconType.Pass,
                    UserTaskVertex.States.Failed => TaskBoardElement.IconType.Fail,
                    UserTaskVertex.States.UnDone => TaskBoardElement.IconType.Warn,
                    _ => TaskBoardElement.IconType.Blank
                }; 

				element.SetIcon(type);
			}
		}

        /// <summary>
        /// the above function but always changes icon to pass
        /// </summary>
        /// <param name="vertex"></param>
        public void UpdateElementIconPass(GraphVertex vertex)
        {
            if (!(vertex is UserTaskVertex task))
            {
                return;
            }

            TaskBoardElement element = Tasklist.Find((x) => x.Task == task);

            // only modify pass & warning images when decided to use them
            if (element != null && UseStateImages)
            {
                TaskBoardElement.IconType type = TaskBoardElement.IconType.Pass;
                element.SetIcon(type);
            }
        }

        public static void UpdateButtonTaskButton(bool showButton, string text = null)
        {
			if (showButton)
			{
				Instance.ButtonTaskButton.gameObject.SetActive(true);

				Instance.ButtonTaskButton.GetComponentInChildren<TextMeshProUGUI>().text = text == null ? TaskButtonDefaultText : text;
			}
			else
			{
				Instance.ButtonTaskButton.gameObject.SetActive(true);
			}
        }



		#region Task List Visibility

		public void Show()
		{
			SetTaskWindowActive(true);
		}

		public void Hide()
		{
			SetTaskWindowActive(false);
		}

		private void SetTaskWindowActive(bool active)
		{
			LeanWindow window = GetComponent<LeanWindow>();

			if (window)
            {
				window.On = active;
            }
		}

        #endregion
    }
}