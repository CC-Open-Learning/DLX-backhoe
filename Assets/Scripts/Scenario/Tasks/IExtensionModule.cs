/*
 *  FILE          :	IExtensionModule.cs
 *  PROJECT       :	CORE Engine (Scenarios)
 *  PROGRAMMER    :	Duane Cressman
 *  FIRST VERSION :	2020-10-07
 *  DESCRIPTION   :  
 *     
 *     This class defines the IExtensionModule interface. 
 *     This interface will define some of the functionality for how a module will interface with CORE-ENGINE.
 *     
 *     The main functionality achieved here is the extension for the TaskManager. Each module will be able to
 *     define its own types of tasks.
 *     This interface will also be used when building the scenario from the XML.
 *     
 */

using System.Collections;
using System.Collections.Generic;

namespace RemoteEducation.Scenarios
{
    public interface IExtensionModule
    {
        bool ShowDialogOnExit { get; }

        CoreCameraController CoreCameraController { get; }

        /// <summary>
        /// A list of the UIs for the graph scenario in the scene.
        /// The classes that implement this interface shall also have: 
        /// [SerializeField] private List<IScenarioGraphUI> scenarioGraphUIs;
        /// This field on the interface should be an accessors for the Serialized list. 
        /// This is so that the list can show up in the inspector.
        /// </summary>
        List<IScenarioGraphUI> ScenarioGraphUIs { get; }

        /// <summary>
        /// This method will be used to override the TaskManagers way of 
        /// generating <see cref="TaskResultItem"/>s for the result screen.
        /// If the module chooses to implement its own way of generating these 
        /// tasks, the method should return true, and then return the list in
        /// the out parameter. If the module choose to use the default way of
        /// generating result items, this method should return false. 
        /// </summary>
        /// <param name="items">The list of task result items</param>
        /// <returns>If the module wants to generate its own result items</returns>
        bool GenerateTaskResults(out List<TaskDetails> items);

        //this method will be used to interpret any module specific data from the XML
        IEnumerator Attach(Scenario scenario);

        //CreateElements all the game objects that are needed in the scene.
        void InitializeGameObjects();

        //do any final set up before the user has access to the scene.
        void OnAllTasksLoaded();

        /// <summary>
        /// This method shall be called after the <see cref="TaskManager"/> has built the task graph.
        /// All the <see cref="IScenarioGraphUI"/>s that this class holds shall be initialized in this method.
        /// </summary>
        /// <param name="taskManager">The TaskManager in the scene.</param>
        void InitializeScenarioGraphUI(TaskManager taskManager);

        void LoadSavedProgress();
    }
}