/*
 *	A component added to a Scene to allow CORE Engine Scenarios to be 
 *	dynamically loaded and interacted with.
 *	
 *	When initialized in the Scene, the ScenarioManager works with the 
 *	IExtensionModule provided by the content module to provide Tasks 
 *	and environment configuration. The Tasks are then passed through 
 *	to a TaskManager
 */

#region Resources

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RemoteEducation.Networking.Telemetry;
using RemoteEducation.Helpers.Unity;
using RemoteEducation.UI;
using Lean.Gui;
using UnityEngine.Events;
using RemoteEducation.Interactions;
using UnityEngine.ResourceManagement;
using RemoteEducation.Localization;

#endregion

namespace RemoteEducation.Scenarios
{
    /// <summary>
    ///     A component added to a Scene to allow CORE Engine a <see cref="Scenario"/> to be dynamically loaded and interacted with.
    /// </summary>
    /// <remarks>
    ///     When initialized in the Scene, the ScenarioManager works with the <see cref="IExtensionModule"/> provided by the content module
    ///     to provide Tasks and environment configuration. The Tasks are then passed through to a <see cref="Scenarios.TaskManager"/>
    /// </remarks>
    public class ScenarioManager : MonoBehaviour
    {
        private const float executionDelay = .1f;
        private const int startfromZero = -1;           // used to display task index from 0

        #region Static Members

        /// <summary> Ensures exactly one ScenarioManager exists in a Unity Scene </summary>
        /// <remarks> This is an implementation of the Singleton design pattern in the Unity environment </remarks>
        public static ScenarioManager Instance;

        #endregion


        #region Fields

        /// <summary> List of all Scenarios being tracked by the ScenarioManager </summary>
        private List<Scenario> scenarios;

        /// <summary> Tracks the current <see cref="Scenario"/> </summary>
        [NonSerialized] public Scenario CurrentScenario;

        /// <summary> Object which tracks, manipulates, and updates the Tasks of the active Scenario </summary>
        [NonSerialized] public TaskManager TaskManager;

        [Header("Window Manager Properties")]
        [Tooltip("UI manager which handles displaying various Windows throughout the Scenario")]
        public WindowManager WindowManager;

        public GameObject[] DefaultWindows;

        [Header("Specialized Windows")]

        public GameObject ResultWindowResource;

        /// <summary> Event handler to be invoked when a Scenario is activated </summary>
        public UnityEvent ScenarioLoaded;

        /// <summary>
        /// This action is invoked when the scenario end. This will be right before the Result Window
        /// is shown, or the Start Scene is loaded
        /// </summary>
        public UnityEvent ScenarioEnded;


        #endregion


        #region Properties

        /// <value> Total number of Scenarios being tracked </value>
        public int TotalScenarios
        {
            get { return scenarios.Count; }
        }

        /// <value> Number of seconds the ScenarioScene has been loaded </value>
        public static int TimeSinceScenarioLoaded
        {
            get
            {
                return (int)Time.timeSinceLevelLoad;
            }
        }
        #endregion


        #region MonoBehaviour CallBacks

        /// <summary>
        ///     Ensures that only one instance of ScenarioManager exists in the current context. 
        ///     If 'Instance' is non-null when this callback runs, it will delete the attached GameObject
        /// </summary>
        void Awake()
        {
            // Get reference to Instance
            if (Instance != null)
            {
                Debug.LogError("ScenarioManager already exists. Deleting this instance's gameobject.");
                Destroy(Instance.gameObject);
            }

            // Assign this object to the static Instance property
            Instance = this;
            scenarios = new List<Scenario>();

            // Create TaskManager object and hold reference in the ScenarioManager singleton
            if (!TaskManager)
            {
                TaskManager = gameObject.AddComponent<TaskManager>();
            }

            // If the Addressables are configured improperly, a bucket is deleted by the dev team, or a content release breaks an old build somehow.
            ResourceManager.ExceptionHandler = CustomAddressablesExceptionHandler.Handler;
        }

        /// <summary>
        ///     Calls methods to gather information about all Scenarios relevant to the 
        ///     current simulation and load the initial Scenario and all of its resources
        /// </summary>
        public IEnumerator Start()
        {
            AggregateScenarios();

            yield return StartCoroutine(ActivateCurrentScenario());

            LoadWindows();

            // Send analytics event to indicate that a Scenario has been started
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Utilizes the ScenarioBuilder to locate the Scenario objects 
        ///     relevant to the current simulation.
        /// </summary>
        /// <remarks>
        ///     As CORE Engine still only supports a single Scenario being loaded 
        ///     in a single ScenarioScene instance, only the first Scenario in the 
        ///     collection returned through the ScenarioBuilder will be loaded.
        /// </remarks>
        private void AggregateScenarios()
        {
            // Use the SceneBuilder to get Scenario objects from memory
            scenarios = ScenarioBuilder.GetScenarios();

            if (scenarios == null || scenarios.Count == 0)
            {
                Debug.LogError("ScenarioManager : No Scenarios in this scene. Cannot continue");
                return;
            }

            CurrentScenario = scenarios[0];

            Debug.Log("ScenarioManager : Scenarios have been aggregated");
        }

        
        /// <summary>
        ///     Instructs the <see cref="ScenarioBuilder"/> to initialize the environment and
        ///     Task objects of the Scenario currently selected in the ScenarioManager.
        /// </summary>
        /// <remarks>
        ///     If no Scenario is currently selected, or if the "current" Scenario cannot be built,
        ///     the ResultsScene will be loaded to display the results of the Scenario(s)
        /// </remarks>
        private IEnumerator ActivateCurrentScenario()
        {
            // If no Scenario is currently selected, or if the "current" Scenario cannot be built,
            // load the ResultsScene to display the results of the Scenario(s)
            if (CurrentScenario == null)
            {
                Debug.LogError("ScenarioManager : Attempted to load an invalid Scenario");
                Invoke(nameof(EndScenario), executionDelay);
                yield break;
            }

            yield return StartCoroutine(ScenarioBuilder.Build(CurrentScenario));

           
            Debug.Log("ScenarioManager : Invoking ScenarioLoaded Action");
            Instance.ScenarioLoaded?.Invoke();
        }


        /// <summary>
        ///     Sets up the <see cref="SlideMenuManager"/> with the appropriate <see cref="SlideMenuTab"/> items 
        ///     that should be displayed in the Scenario. The Magnifier Slide Menu is currently the only generic SlideMenuTab
        ///     handled by this method
        /// </summary>
        /// <remarks>
        ///     A future consideration is providing options in the Scenario configuration files to 
        ///     enable or disable certain UI elements.
        /// </remarks>
        private void LoadWindows()
        {
            if (!WindowManager)
            {
                WindowManager = FindObjectOfType<WindowManager>();
            }

            if (WindowManager)
            {
                foreach (GameObject resource in DefaultWindows)
                {
                    WindowLoader loader = Instantiate(resource, WindowManager.transform).GetComponent<WindowLoader>();
                    if (loader)
                    {
                        WindowManager.Add(loader.Window, loader.Button);
                    }
                }

                //set up quit dialog window
                WindowManager.DialogWindow.SetDialogText("Engine.Paused".Localize(), "Engine.QuitScenario".Localize(), "Engine.Quit".Localize());
                WindowManager.DialogWindow.OnConfirm.RemoveAllListeners();
                WindowManager.DialogWindow.OnCancel.RemoveAllListeners();

                WindowManager.DialogWindow.OnConfirm.AddListener(QuitScenario);
            }
        }

        /// <summary>
        /// The <see cref="ScenarioEnded"/> action is invoked. 
        /// Then if the <see cref="TaskManager"/> has any task details to show, 
        /// they will be shown in a result window. If there are no tasks to
        /// be shown, then the start scene will be loaded.
        /// </summary>
        private void EndScenario()
        {
            ScenarioEnded?.Invoke();

            if (TaskManager.GenerateTaskResults(out List<TaskDetails> details))
            {
                ShowResultCanvas(details);
            }
            else
            {
                LeaveScenarioScene();
            }
        }

        /// <summary>
        /// Show the result window for this scenario. 
        /// This method will instantiate the window and add it to the WindowManager.
        /// It will then pass the data for each task into the <see cref="ScenarioResult"/>
        /// class to populate the window.
        /// </summary>
        /// <param name="details"></param>
        private void ShowResultCanvas(List<TaskDetails> details)
        {
            GameObject resultWindowResource = Instantiate(ResultWindowResource);
            
            if(resultWindowResource == null)
            {
                Debug.LogError("ScenarioManager : Unable to instantiate the Result Window");
                return;
            }

            //add the window to the window manager
            LeanWindow resultsWindow = resultWindowResource.GetComponentInChildren<LeanWindow>();
            WindowManager.Add(resultWindowResource);

            //make it so that pressing Esc wont bring up the quit dialog
            WindowManager.WindowCloser.enabled = false;

            //set up the results screen with data from the task manager
            ScenarioResult scenarioResult = resultsWindow.GetComponentInChildren<ScenarioResult>();
            scenarioResult.LoadResults(CurrentScenario, details);
            scenarioResult.completeButton.GetComponent<LeanButton>().OnClick.AddListener(LeaveScenarioScene);

            resultsWindow.TurnOn();
        }

        /// <summary>
        /// Load the start scene or quit the application.
        /// If the start scene is loading, the static <see cref="InputManager"/> is
        /// refreshed.</summary>
        private void LeaveScenarioScene()
        {
            if (ScenarioBuilder.TryGetDefaultScenario(out int scenarioId) && scenarioId == CurrentScenario.Identifier)
            {
                Application.Quit();
            }
            else
            {
                //allow classes to reset any static functionality
                InputManager.Refresh();
                Interactable.ResetSubsetActivatedObjects();

                // Load StartScene
                FadeSceneLoader.LoadSceneWithFade(Constants.START_SCENE_NAME);
            }
        }

        #endregion


        #region Public Methods

        /// <summary>
        ///     Returns the <see cref="Scenario"/> from the 'scenarios' list with an 'Identifier'
        ///     field that matches the <paramref name="id" /> parameter
        /// </summary>
        /// <param name="id">Identifier of the Scenario to retrieve</param>
        /// <returns>
        ///     The Scenario object matching the <paramref name="id" /> parameter, or
        ///     <c>null</c> if <paramref name="id" /> does not match a managed Scenario
        /// </returns>
        public Scenario GetScenario(int id)
        {
            return scenarios.Find(x => x.Identifier == id);
        }
        

        /// <summary>
        ///     A very rudimentary way of defining the order of Scenarios. 
        /// </summary>
        /// <remarks>
        ///     When called, the next Scenario in the 'scenarios' list is set as the 
        ///     current Scenario. If there are no more Scenarios in the list, the 
        ///     currentScenarioId field is set to Scenario.None
        ///     <para />
        ///     This method is in need of review, since the new Graph structure implementation
        ///     of Tasks within Scenarios is likely a good implementation for organizing multiple
        ///     Scenarios as well
        /// </remarks>
        public void NextScenario()
        {
            Debug.Log("ScenarioManager : CORE currently supports only one Scenario at a time. The Scenario will now be finished");
            FinishScenario();
            //
        }


        /// <summary>
        ///     Ends the current <see cref="Scenario"/> indicating it has been completed.
        /// </summary>
        /// <remarks>
        ///     This method fires an Analytics Event when the <see cref="Scenario"/> is complete,
        ///     indicating whether the user has successfully completed or failed. 
        ///     <para/>
        ///     This event needs to be fired before switching to the ResultsScene 
        ///     in order to track 'Time.timeSinceLevelLoaded'
        /// </remarks>
        public void FinishScenario()
        {
            EndScenario();
        }


        /// <summary>
        ///     Indicates that the current scenario should be ended immediately. 
        /// </summary>
        /// <remarks>
        ///     Before calling the appropriate method to load the "ResultsScene",
        ///     an analytics event is fired indicating that the current scenario
        ///     has been quit
        /// </remarks>
        public void QuitScenario()
        {
            EndScenario();
        }


        /// <summary>
        ///     A direct call to load the ScenarioScene.unity Scene
        /// </summary>
        public static void LoadScenarioScene()
        {
            Debug.Log("Loading Scenario Scene");
            FadeSceneLoader.LoadSceneWithFade("ScenarioScene");
        }

        
        /// <summary>
        ///     Coroutine used to load the ScenarioScene after a short delay
        /// </summary>
        /// <param name="wait">Number of seconds to wait before loading ScenarioScene</param>
        /// <returns></returns>
        public static IEnumerator LazyLoad(float wait = 0.5f)
        {
            yield return new WaitForSeconds(wait);
            LoadScenarioScene();
            yield return null;
        }

        #endregion
    }
}
