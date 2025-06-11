using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RemoteEducation.Scenarios;
using RemoteEducation.Scenarios.Inspectable;
using RemoteEducation.Interactions;
using System.Linq;
using UnityEngine.Analytics;
using RemoteEducation.Editor.BugReporting;
using RemoteEducation.UI;
using VARLab.Analytics;
using UnityEngine.Events;
using RemoteEducation.Localization;
using System;

namespace RemoteEducation.Modules.HeavyEquipment
{
    public class HeavyEquipmentModule : MonoBehaviour, IExtensionModule
    {
        public enum ScenarioIdentifiers 
        {
            Explore = 2040,
            PreTripInspection = 2041
        }

        public enum HeavyEquipmentCheckTypes
        {
            /// <summary>
            /// Use for UserTaskVertexs that need to be waited on for a set amount of seconds.
            /// The constData must be of type Tuple(string, float), where the string is a key for the
            /// wait (you can just use the name of the vertex) and the float is the time in seconds you
            /// want to wait.
            /// PreTripInspectionData.WaitTask for more info.
            /// </summary>
            Wait = 400,

           /// <summary>
           /// bool - If the dipstick is wiped.
           /// </summary>
            DipStickIsWiped = 401,

            /// <summary>
            /// bool - If the dipstick is removed.
            /// </summary>
            DipStickIsRemoved = 402,

            /// <summary>
            /// bool - If the fuel water seperator has been drained
            /// </summary>
            FuelWaterSeparatorDrained = 403,

            /// <summary>
            /// bool - If the dipstick is returned.
            /// </summary>
            DipStickIsReturned = 404
        }

        //[SerializeField]
        //private CloudSaving saveSystem; expose via event

        /// <summary>If a Scenario Graph is being used in the current scenario.</summary>
        public static bool ScenarioAttatched;

        public bool ShowDialogOnExit => true;

        [SerializeField, Tooltip("Reference to the WindowManager in the Scene")]
        public WindowManager windowManager;

        [SerializeField, Tooltip("The UI's in the scene for the scenario graph")]
        private List<GameObject> scenarioGraphUIs;

        public List<IScenarioGraphUI> ScenarioGraphUIs => (from ui in scenarioGraphUIs select ui.GetComponent<IScenarioGraphUI>()).ToList();

        [Tooltip("Camera Controller included in the module environment")]
        [SerializeField] private CoreCameraController cameraController;

        public CoreCameraController CoreCameraController { get => cameraController; }

        public InspectableManager InspectableManager;

        public BackhoeController BackhoeController;

        public DetailsPanel DetailsPanel;

        public PreTripInspectionData preTripInspectionData;

        [Header("Bad Element Loading")]

        //The file path to load a debug JSON file, set in the bug reporter
        public static string DebugLoadPath = "";

        [SerializeField, Tooltip("True: The Bad Percent slider is used\nFalse: The BadElementStates list is used (if elements are entered).")]
        private bool UsePercentage;

        [SerializeField, Range(0, 100), Tooltip("The percent of elements that should be spawned in as bad.")]
        private float badPrecent = 50f;
        [SerializeField, Tooltip("The bad elements to load into the scene.")]
        List<InspectableElement> BadElementStates;

        /// <summary>
        /// Analytics properties
        /// </summary>

        [SerializeField]
        private AnalyticsEvent AnalyticsEvent;

        [SerializeField]
        private SegmentTracker SegmentTracker;

        /// <summary>
        /// Save system properties
        /// </summary>

        public enum PlayerLoadPreference
        {
            NotSet,
            Continue,
            Restart,
        }

        [SerializeField]
        private PlayerLoadPreference playerLoadPreference = PlayerLoadPreference.NotSet;

        /// <summary>
        /// This property is set by <see cref="WasLoadSuccess(bool)"/> which is used as a listener method to an event on the SaveDataSupport script.
        /// </summary>
        private bool? _loadSuccessful = null;

        public UnityEvent<List<InspectableElement>> StageCompleteSave;
        public UnityEvent<int> StageCompleteGetVertexID;
        public UnityEvent TriggerLoad;
        public UnityEvent DeleteSave;
        public UnityEvent<Action<List<InspectableElement>>, List<InspectableElement>> LoadSaveToInspectableElements;
        public UnityEvent<Action<List<Tuple<string, int>>>> OnLoadBackhoeStates;
        public UnityEvent<Action<int>> OnLoadSetSavePointVertexID;
        private List<Tuple<string, int>> _backhoeStates;

        public IEnumerator Attach(Scenario scenario)
        {
            Debug.Log("Attaching Heavy Equipment module...");

            DestroyImmediate(ScenarioManager.Instance.WindowManager.gameObject);
            ScenarioManager.Instance.DefaultWindows = new GameObject[0];
            ScenarioManager.Instance.WindowManager = windowManager;

            ScenarioAttatched = scenario.Identifier == (int)ScenarioIdentifiers.PreTripInspection;

            //trigger load here
            TriggerLoad?.Invoke();

            Debug.Log("USE Percentage: " + UsePercentage.ToString());

            if (InspectableManager)
            {
                yield return new WaitUntil(() => _loadSuccessful != null);

                yield return DisplayLoadPrompt();

                SelectInitializer();
            }
            ScenarioManager.Instance.TaskManager.OnLastTaskFinished?.AddListener(EndStringChange);
        }

        /// <summary>
        /// This function initializes BackHoe based upon the conditions of certain properties to either initialize with a list of known "bad elements" or 
        /// to initialize using the float percent to generate new bad elements
        /// </summary>
        private void SelectInitializer()
        {
            if (DebugFileLoader.DebugFile != "")
            {
                InspectableManager.Initialize(DebugFileLoader.DebugFile);
            }

            //this is the path that will be taken for first time run, and for new game being selected
            else if (UsePercentage && playerLoadPreference == PlayerLoadPreference.Restart || BadElementStates.Count == 0 && playerLoadPreference == PlayerLoadPreference.Restart)
            {
                DeleteSave?.Invoke();
                InspectableManager.Initialize(badPrecent);
            }

            //this is the path that will be taken for loading the saved back hoe state
            else if (playerLoadPreference == PlayerLoadPreference.Continue)
            {
                /// <see cref="LoadedBadElements(List{InspectableElement})"/> which is triggered on a successful load, which sets the bad element states
                InspectableManager.InitializeFromSaveData(_backhoeStates);
            }

            //this was here before so I will leave it
            else
            {
                InspectableManager.Initialize(BadElementStates);
            }
        }

        /// <summary>
        /// This function is used to display the loading prompt if a load has happened or to set the PlayerPreference to restart to allow for the generation
        /// of new bad elements as if it was a new game. It returns an IEnumerator to yield until the prompt selection is made.
        /// </summary>
        /// <returns></returns>
        private IEnumerator DisplayLoadPrompt()
        {
            //this needs to only trigger if the load finds save data in the cloud.
            if (_loadSuccessful == true)
            {
                yield return LoadPrompt();
            }
            else
            {
                //since load was not successful treat the preference as a restart
                playerLoadPreference = PlayerLoadPreference.Restart;
            }
        }

        /// <summary>
        /// This is the code for the actual load prompt itself and sets the various text areas in the prompt as well as adding listeners to set a property
        /// based upon the user choice then display until a choice has been made.
        /// </summary>
        /// <returns></returns>
        private IEnumerator LoadPrompt()
        {
            Dialog loadDialogWindow = ScenarioManager.Instance.WindowManager.LoadDialogWindow;
            //set up load dialog window
            loadDialogWindow.SetDialogText("Engine.LoadProgress".Localize(), "Engine.LoadProgressMessage".Localize(), "Engine.Continue".Localize(), "Engine.Restart".Localize());
            loadDialogWindow.OnConfirm.RemoveAllListeners();
            loadDialogWindow.OnCancel.RemoveAllListeners();

            loadDialogWindow.OnConfirm.AddListener(() => { 
                playerLoadPreference = PlayerLoadPreference.Continue;
                InspectableManager.onLoadFromCloudSave.AddListener(() => LoadSaveToInspectableElements?.Invoke(LoadInspectableElements, InspectableManager.InspectableElements));
                OnLoadBackhoeStates?.Invoke(LoadBackhoeStates);
            });
            loadDialogWindow.OnCancel.AddListener(() => { 
                playerLoadPreference = PlayerLoadPreference.Restart;
                ScenarioManager.Instance.TaskManager.GraphController.SavePointVertexID = 0;
            });
            loadDialogWindow.TurnOn();

            yield return new WaitUntil(() => playerLoadPreference != PlayerLoadPreference.NotSet);
        }

        public void LoadBackhoeStates(List<Tuple<string,int>> backhoeStates)
        {
            Debug.Log("Insepctable element list after load:" + backhoeStates.ToString());
            _backhoeStates = backhoeStates;
        }

        public void LoadInspectableElements(List<InspectableElement> elements)
        {
            for (int i = 0; i < InspectableManager.InspectableElements.Count; i++)
            {
                InspectableManager.InspectableElements[i].CurrentStatus = elements[i].CurrentStatus;
                InspectableManager.InspectableElements[i].MakeUserSelection(elements[i].UserSelectedIdentifier);
                InspectableManager.InspectableElements[i].State = elements[i].State;
                InspectableManager.InspectableElements[i].SetCurrentlyInspectable(elements[i].CurrentlyInspectable);
            }
        }

        public void EndStringChange()
        {
            string ExploreDescription = "You have completed the inspection.";
            string ExploreCancel = "See Results";
            string ExploreConfirm = "Explore Mode";
            ScenarioManager.Instance.WindowManager.DialogWindow.SetDialogText("Paused", ExploreDescription, ExploreConfirm, ExploreCancel);
        }

        public bool GenerateTaskResults(out List<TaskDetails> items)
        {
            if (InspectableManager.InspectableElements == null)
            {
                items = null;
                return false;
            }

            items = InspectableManager.GenerateHeavyEquipmentInspectionResults(InspectableManager.InspectableElements);
            return true;
        }

        public void InitializeGameObjects()
        {
            Debug.Log("Heavy Equipment : Initializing objects");

            CoreCameraController.Initialize();
            Debug.Log("Camera Controller is initialized");

            BackhoeController.Initialize();
            Debug.Log("Backhoe is initialized");

            DetailsPanel.Initialize();
            Debug.Log("Details Panel is initialized");
        }

        public void OnAllTasksLoaded()
        {
            Debug.Log("Heavy Equipment : This is called when all tasks are loaded");

            CoreCameraController.StartCameraController();

            Debug.Log("Camera Controller has started");

            PreTripInspectionGS.OnStageComplete += PreTripInspectionGS_OnStageComplete;
            PreTripInspectionGS.OnDeleteSave += DeleteSaveDelegate;
        }

        private void DeleteSaveDelegate()
        {
            DeleteSave?.Invoke();
        }
        private void PreTripInspectionGS_OnStageComplete(string stageName, int stageEnum)
        {
            //SegmentCompleteContainer data = new SegmentCompleteContainer(stageName, SegmentTracker.GetSegmentIndex());
            //AnalyticsEvent.Raise(this, data);
            SegmentTracker.IncreamentSegmentIndex();

            int CurrentVertexID = ScenarioManager.Instance.TaskManager.GraphController.CurrentVertex.Id;
#if UNITY_EDITOR
            Debug.Log($"Current Vertex ID On Stage Complete: {CurrentVertexID}");
#endif
            StageCompleteGetVertexID?.Invoke( CurrentVertexID );
            StageCompleteSave?.Invoke(InspectableManager.InspectableElements);
        }

        /// <summary>
        /// This method is used to check if a save was successful to prompt the user with a toast if their progress is saving successfully
        /// </summary>
        /// <param name="isSaveSuccess"></param>
        public void OnProgressSaveSuccess(bool isSaveSuccess)
        {
            if (isSaveSuccess == true)
            {
                HEPrompts.CreateToast("Progress saved.", HEPrompts.ShortToastDuration);
            }
            else
            {
                HEPrompts.CreateToast("Progress save error. Saving will be attempted again at the next checkpoint.", HEPrompts.ShortToastDuration);
            }
        }

        /// <summary>
        /// This method is added to an event that happens in <see cref="SaveDataSupport"/> to pass back if the load was successful
        /// </summary>
        /// <param name="loaded"></param>
        /// <returns></returns>
        public void WasLoadSuccess(bool loaded)
        {
            _loadSuccessful = loaded;
        }

        /// <summary>
        /// This method is added as a listener to an event that happens in <see cref="SaveDataSupport"/> to pass back the loaded prefabs from the names of the bad elements
        /// </summary>
        /// <param name="elements"></param>
        public void LoadedBadElements(List<InspectableElement> elements)
        {
            BadElementStates.Clear();
            BadElementStates = elements;
        }

        public static void PokeTaskManagerOnSelect(Interactable interactable)
        {
            ScenarioManager.Instance.TaskManager.GraphController.PokeCurrentVertex(interactable);
        }

        public void InitializeScenarioGraphUI(TaskManager taskManager)
        {
            foreach (IScenarioGraphUI ui in ScenarioGraphUIs)
            {
                ui.InitializeUI(taskManager);
            }
        }
        void OnDestroy()
        {
            PreTripInspectionGS.OnStageComplete -= PreTripInspectionGS_OnStageComplete;
        }

        public void LoadSavedProgress()
        {
            if (playerLoadPreference == PlayerLoadPreference.Continue)
                OnLoadSetSavePointVertexID?.Invoke((vertexID) => ScenarioManager.Instance.TaskManager.GraphController.SavePointVertexID = vertexID);
        }
    }
}