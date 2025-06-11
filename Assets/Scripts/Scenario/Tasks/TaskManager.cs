/*
 *  FILE            : TaskManager.cs
 *  PROJECT         : CORE
 *  PROGRAMMER      : Duane Cressman
 *  FIRST VERSION   : 2020-09-29
 *  RELATED FILES   : Task.cs, ITaskable.cs
 *  DESCRIPTION     : This class will handle all the tasks that must be completed in a scenario. 
 *                    All the tasks (held as Task objects) will be added to this class be the SceneBuilder.
 *                    The TaskManager will have the ability to ask taskable components (any class the implements
 *                    the ITaskable interface) "questions" and determine if the answer means that the task is 
 *                    completed.
 *                    To steam line which components are being check for if the task is complete, a system of 
 *                    events has been set up. Each taskable component has a event that is fired anytime changed 
 *                    state in a way that could complete the task. The TaskManager, based on the current task, 
 *                    will set what method that event points to. If the component is able to be part of the task,
 *                    the event will point to a method on the TaskManager that checks if the task is complete. The 
 *                    taskable component that sent the event will be the one that is checked for completion. 
 *                    
 *                    This class interfaces with the Scenario, ScenarioManager, and SceneBuilder. These connection
 *                    allow the tasks to be read in from the XML and for the rest of the scene to react to the tasks.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace RemoteEducation.Scenarios
{
    public class TaskManager : MonoBehaviour
    {
        // String resources for the end-of-Scenario Dialog window
        public string ExploreDescription = "You have completed all tasks.\nWould you like to continue in Explore Mode?";
        public string ExploreCancel = "Exit Lab";
        public string ExploreConfirm = "Explore Mode";

        #region Attributes

        //The module that is plugged into the TaskManager.
        public IExtensionModule TaskModule;

        public GraphController GraphController { get; private set; }

        public List<UserTaskVertex> AllTaskVertices { get; private set; }

        /// <summary>
        ///     Invoked when the last user task has been completed, and the graph is at the 'end' node
        /// </summary>
        public UnityEvent OnLastTaskFinished = new UnityEvent();

        /// <summary>
        ///     Enable this variable so user can press C key to skip a task
        /// </summary>
        public bool SkipTask { get; set; } = false;

        #endregion

        #region Events

        /// <summary>
        /// A delegate to handle when a update happens that is related to a task.
        /// This could mean that the <param name="newVertex"> itself has updated in some way.
        /// It could also mean that the current task changed and the <param name="newVertex"> 
        /// is the new current task.
        /// </summary>
        /// <param name="newVertex">The new current task.</param>
        public delegate void taskUpdate(GraphVertex newVertex);

        public delegate void genericTaskEvent();

        /// <summary>
        /// The previous current task was completed and there is a new current task.
        /// The vertex passed along with this event is the new current task. 
        /// </summary>
        public event taskUpdate OnCurrentTaskUpdated;

        /// <summary>
        /// Fired when the last task in the graph has completed.
        /// </summary>
        public event genericTaskEvent OnLastTaskCompleted;

        /// <summary>
        /// Fired in specific cases when the completion state of a <see cref="UserTaskVertex"/> changes.
        /// 1. A task that was already completed, but became undone when it was checked as a reliant task.
        /// 2. A sub task is completed.
        /// Basically this event only fires when the state of a <see cref="UserTaskVertex"/> that is pointed to 
        /// by a <see cref="ReliantEdge"/> is changed
        /// </summary>
        public event taskUpdate OnTaskCompletionStateChanged;

        #endregion

        #region Setup Methods

        /// <summary>
        /// This method shall be called by <see cref="ScenarioBuilder.Build(Scenario)"/>.
        /// It will attach the scenario and extension module to the Task Manager. 
        /// The Graph Controller in the TaskManager will build and hold the graph vertices 
        /// in the scenario passed in. 
        /// The extension module for this scenario will also be saved. It will interpret the module
        /// specific data from the scenario.
        /// </summary>
        /// <param name="scenario">The <see cref="ITaskableGraphBuilder"/> that defines the graph for the scenario</param>
        /// <param name="extensionModule">The extension module that is needed for this scenario</param>
        public IEnumerator Attach(Scenario scenario)
        {
            if (TaskModule == null)
            {
                Debug.LogError("TaskManager : No IExtensionModule has been loaded into the Task Manager");
                yield break;
            }

            //Attach the scenario to the task module
            yield return TaskModule.Attach(scenario);

            //Only build the graph controller if there is a task graph provided.
            if (scenario.TaskGraph != null)
            {
                //create the graph controller using the scenario
                GraphController = new GraphController(scenario.TaskGraph, TaskModule);

                //create the task vertex manager
                TaskVertexManager.Initialize(() => GraphController.PokeCurrentVertex(null));

                //get a path of vertices between the start and end vertices
                List<GraphVertex> graphVertices = GraphController.Graph.GetPathBetweenNodes(GraphController.StartVertex, GraphController.EndVertex);

                if (graphVertices == null)
                {
                    Debug.LogError("There is no path between the start and end vertex. The Scenario Graph is invalid. Can not continue");
                    yield break;
                }

                //pull out all the user task vertices
                AllTaskVertices = graphVertices.OfType<UserTaskVertex>().ToList();

                if (AllTaskVertices.Count > 0)
                {
                    TaskModule.InitializeScenarioGraphUI(this);
                }

                //set up the event handlers
                GraphController.OnCurrentVertexUpdated += HandleCurrentTaskUpdated;
                GraphController.OnVertexCompletionStateChanged += HandleVertexStateChanged;
                GraphController.OnEndVertexHit += OnLastTaskReached;
            }
            else
            {
                TaskVertexManager.Initialize(null);
            }

            //set up the rest of the task module
            TaskModule.InitializeGameObjects();
            TaskModule.OnAllTasksLoaded();

            ScenarioManager.Instance.ScenarioEnded.AddListener(() =>
            {
                // Clear the SendersTurnedOn collection in the TaskVertexManager 
                // at the end of a Scenario, to avoid exceptions being thrown 
                // by task creation in a subsequent Scenario 
                TaskVertexManager.ClearSenders();

                //hide all the UI
                TaskModule.CoreCameraController.ToggleAllUIVisability(false);
            });

            if (scenario.TaskGraph != null)
            {
                TaskModule.LoadSavedProgress();
                GraphController.StartGraph();
            }
        }

        /// <summary>
        /// This function is used for switching the graph scenario to update the task manager and graph controller.
        /// It will attach the extension module to the Task Manager. 
        /// The Graph Controller in the TaskManager will build and hold the graph vertices 
        /// in the scenario passed in. 
        /// The extension module for this scenario will also be saved. It will interpret the module
        /// specific data from the scenario.
        /// </summary>
        /// <param name="scenario">The <see cref="ITaskableGraphBuilder"/> that defines the graph for the scenario</param>
        public void AttachAgain(Scenario scenario)
        {
            if (GraphController != null)
            {
                // Remove the listner to avoid the TurnOnSender is executed after AttachAgain, and execute it mannualy here
                GraphController.OnCurrentVertexUpdated -= HandleCurrentTaskUpdated;
                HandleCurrentTaskUpdated(GraphController.CurrentVertex);
            }

            if (TaskModule == null)
            {
                Debug.LogError("TaskManager : No IExtensionModule has been loaded into the Task Manager");
                return;
            }

            //Only build the graph controller if there is a task graph provided.
            if (scenario.TaskGraph != null)
            {
                //create or update the graph controller using the scenario
                if (GraphController == null)
                {
                    GraphController = new GraphController(scenario.TaskGraph, TaskModule);
                }
                else
                {
                    GraphController.UpdateGraphController(scenario.TaskGraph, TaskModule);
                }

                GraphController.EndCurrentProgress(false);

                //create the task vertex manager
                TaskVertexManager.Initialize(() => GraphController.PokeCurrentVertex(null));

                //get a path of vertices between the start and end vertices
                List<GraphVertex> graphVertices = GraphController.Graph.GetPathBetweenNodes(GraphController.StartVertex, GraphController.EndVertex);

                if (graphVertices == null)
                {
                    Debug.LogError("There is no path between the start and end vertex. The Scenario Graph is invalid. Can not continue");
                    return;
                }

                //pull out all the user task vertices
                AllTaskVertices = graphVertices.OfType<UserTaskVertex>().ToList();

                if (AllTaskVertices.Count > 0)
                {
                    TaskModule.InitializeScenarioGraphUI(this);
                }

                //set up the event handlers
                GraphController.OnCurrentVertexUpdated += HandleCurrentTaskUpdated;
                GraphController.OnVertexCompletionStateChanged += HandleVertexStateChanged;
                GraphController.OnEndVertexHit += OnLastTaskReached;
            }
            else
            {
                GraphController.EndCurrentProgress(true);
                TaskVertexManager.Initialize(null);
            }

            //set up the rest of the task module
            TaskModule.OnAllTasksLoaded();

            ScenarioManager.Instance.ScenarioEnded.AddListener(() =>
            {
                // Clear the SendersTurnedOn collection in the TaskVertexManager 
                // at the end of a Scenario, to avoid exceptions being thrown 
                // by task creation in a subsequent Scenario 
                TaskVertexManager.ClearSenders();

                //hide all the UI
                TaskModule.CoreCameraController.ToggleAllUIVisability(false);
            });

            if (scenario.TaskGraph != null)
            {
                GraphController.StartGraph();
            }
        }

        #endregion

        #region GraphController Event Handlers

        /// <summary>
        /// This is the event handler for <see cref="GraphController.OnCurrentVertexUpdated"/>.
        /// </summary>
        /// <param name="newCurrentVertex">The new current vertex.</param>
        private void HandleCurrentTaskUpdated(GraphVertex newCurrentVertex)
        {
            OnCurrentTaskUpdated?.Invoke(newCurrentVertex);

            TurnOnSendersForCurrentVertex(newCurrentVertex);
        }

        /// <summary>The event handler for <see cref="GraphController.OnVertexCompletionStateChanged"/>.</summary>
        /// <param name="updatedVertex">The vertex that had it's completion state changed.</param>
        private void HandleVertexStateChanged(GraphVertex updatedVertex)
        {
            OnTaskCompletionStateChanged?.Invoke(updatedVertex);
        }

        /// <summary>
        /// This method will exit the scenario. It is called when the Current Scenario Graph 
        /// reaches its end vertex.
        /// When we start joining scenarios together, we will need to add the functionality here.
        /// </summary>
        private void OnLastTaskReached()
        {
            OnLastTaskCompleted?.Invoke();

            //show the exit scene dialog
            if (TaskModule.ShowDialogOnExit)
            {
                ScenarioManager.Instance.WindowManager.DialogWindow.SetDialogText("Paused", ExploreDescription, ExploreConfirm, ExploreCancel);
                ScenarioManager.Instance.WindowManager.DialogWindow.OnCancel.RemoveAllListeners();
                ScenarioManager.Instance.WindowManager.DialogWindow.OnConfirm.RemoveAllListeners();

                ScenarioManager.Instance.WindowManager.DialogWindow.OnCancel
                    .AddListener(delegate
                    {
                        ScenarioManager.Instance.NextScenario();
                    });

                ScenarioManager.Instance.WindowManager.DialogWindow.TurnOn();
            }
            else
            {
                ScenarioManager.Instance.QuitScenario();
            }

            OnLastTaskFinished?.Invoke();
        }

        #endregion


        #region Task Methods

        /// <summary>
        /// This method will get the Sub Tasks for a Task. A sub task is defined
        /// as a <see cref="UserTaskVertex"/> that is pointed to by a reliant edge that is marked 
        /// with <see cref="ReliantEdge.IsSubEdge"/> as true.
        /// </summary>
        /// <param name="vertex">The task to get the sub tasks off of</param>
        /// <returns>A list of all the sub tasks</returns>
        public List<UserTaskVertex> GetSubTasks(UserTaskVertex vertex)
        {
            return GraphController.GetSubVertices<UserTaskVertex>(vertex);
        }

        /// <summary>
        /// This method will activate all the components in the scene that are used to complete the current vertex.
        /// The components will have there event senders activated. This means that when they did something that could 
        /// possibly complete part of the current vertex, they will tell the graph controller to check the current vertex.
        /// Which components are activated is determined by the component types in the vertices that the passed in vertex relies on.
        /// The <see cref="TaskVertexManager"/> is then used to activate any components that were not already active and deactivate any that aren't 
        /// needed anymore.
        /// </summary>
        /// <param name="newCurrentNode">The vertex to activate components based on</param>
        public void TurnOnSendersForCurrentVertex(GraphVertex newCurrentNode)
        {
            // Sometimes the TurnOnSendersForCurrentVertex is executed after the AttachAgain, it will cause bugs.
            // We cannot control the execution order, so just skip this function in the specific case to avoid bugs.
            if (GraphController == null)
            {
                Debug.LogWarning("The graph controller is null since it has been initialized in AttachAgain.");
                return;
            }
            if (newCurrentNode is UserTaskVertex userTask)
            {
                List<Type> typesNeeded = new List<Type>();

                if (userTask.ComponentType != null)
                {
                    typesNeeded.Add(userTask.ComponentType);
                }

                List<UserTaskVertex> verticesReliedOn = GraphController.GetReliedOnVertexies(userTask).OfType<UserTaskVertex>().ToList();

                foreach (UserTaskVertex vertex in verticesReliedOn)
                {
                    if (vertex.ComponentType != null && !typesNeeded.Contains(vertex.ComponentType))
                    {
                        typesNeeded.Add(vertex.ComponentType);
                    }
                }

                TaskVertexManager.UpdateSendersByType(typesNeeded);
            }
        }

        /// <summary>
        /// Generate the task results that will be shown in the results window.
        /// This method will first check if the <see cref="TaskModule"/> has it's
        /// own way of generating results. If it doesn't, then this method generates
        /// default task details.
        /// </summary>
        /// <param name="details">The details generated</param>
        /// <returns>If there were any task details to generate.</returns>
        public bool GenerateTaskResults(out List<TaskDetails> details)
        {
            if (TaskModule.GenerateTaskResults(out List<TaskDetails> items))
            {
                details = items;
                return true;
            }

            if (AllTaskVertices == null || AllTaskVertices.Count == 0)
            {
                details = null;
                return false;
            }

            details = new List<TaskDetails>();

            foreach (UserTaskVertex vertex in AllTaskVertices.Where(x => !x.BlankTitleAndDescription))
            {
                details.Add(new TaskDetails(vertex.State, vertex.Title, vertex.Description));
            }

            // Return false if there are no items to be displayed in the results panel.
            return details.Count > 0;
        }


        /// <summary>
        ///     Update loop for the TaskManager
        /// </summary>
        /// <remarks>
        ///     When in the Unity Editor or running a build marked as a
        ///     "development build" users can skip the current task by
        ///     pressing the 'Z' key (<see cref="KeyCode.Z"/>)
        /// </remarks>
        private void Update()
        {

            // The 'Z' key can be used to skip the current task only when
            // run in the Unity Editor
            // OR on a "Development" build (when the "development build" option
            // is selected in the CORE Builder menu)
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (Hotkeys.GetKeyDown(KeyCode.Z))
            {
                GraphController.BeACheater();
            }
#endif

            // Any non-Editor or non-development build specific code
            // should be added below

            if (SkipTask)
            {
                if (Hotkeys.GetKeyDown(KeyCode.C))
                {
                    GraphController.BeACheater();
                }
            }
        }

        #endregion
    }
}