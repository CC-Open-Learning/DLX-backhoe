/*
*  FILE          :	InspectableManager.cs
*  PROJECT       :	CORE Engine Inspection Module
*  PROGRAMMER    :	Duane Cressman
*  FIRST VERSION :	2020-11-02
*  DESCRIPTION   :  This file contains the InspectableManager class. 
*  
*  Class Description: This class is used to set which InspectableController objects are set to Good and Bad in 
*                     a scene. It will also keep track of what controllers have been inspected along with if the
*                     user said they were good or bad.
*                     
*/

using RemoteEducation.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace RemoteEducation.Scenarios.Inspectable
{
    //The state of a component
    public enum InspectableState
    {
        Good = 0,
        Bad = 1
    }

    public class InspectableManager : MonoBehaviour, ITaskable
    {
        public UnityEvent<Dictionary<InspectableController, List<InspectableElement>>> onStoreBadElements;
        public UnityEvent onLoadFromCloudSave;

        [SerializeField, Tooltip("The UI elements for this scene.")]
        private GameObject[] uiElements;

        public List<InspectableController> InspectableControllers { get; private set; }
        public List<InspectableElement> InspectableElements { get; private set; }

        public List<InspectableElement> ActiveElements => InspectableElements.Where(x => x.CurrentlyInspectable).ToList();

        public TaskableObject Taskable { get; private set; }

        [Tooltip("The object in the scene that is being inspected. (Forklift, Hoist...")]
        public InspectableObject InspectableObject;

        [Tooltip("If true, all Inspectable Objects will display their name in a CORE Engine standard tooltip. Disable to use custom tooltip")]
        public bool UseStandardTooltip = true;

        public delegate void ElementEventHandler(InspectableElement element);
        public delegate void ElementListEventHandler(List<InspectableElement> elements);
        public delegate void GenericEventHandler();

        /// <summary>Event fired when the mouse enters an <see cref="InspectableElement"/>.</summary>
        public event ElementEventHandler OnElementMouseEnter;

        /// <summary>Event fired when the mouse leaves an <see cref="InspectableElement"/>.</summary>
        public event ElementEventHandler OnElementMouseExit;

        /// <summary>Event fired when an <see cref="InspectableElement"/> is selected.</summary>
        public event ElementEventHandler OnElementSelected;

        /// <summary>Event fired when an <see cref="InspectableElement"/> is selected or deselected.</summary>
        public event ElementEventHandler OnElementClicked;

        /// <summary>Event fired when an <see cref="InspectableElement"/> is deselected.</summary>
        public event ElementEventHandler OnElementDeselected;

        /// <summary>Event fired when a specific <see cref="InspectableElement"/> has its state change.</summary>
        public event ElementEventHandler OnElementStateChanged;

        /// <summary>Event fired when all <see cref="InspectableElement"/> objects are told to check their current state simultaneously.</summary>
        public event ElementListEventHandler OnActiveElementsChanged;

        /// <summary>Event fired when the user chooses to have their work evaluated.</summary>
        public event GenericEventHandler OnEvaluationCompleted;

        /// <summary>The <see cref="InspectableElement"/> currently being inspected.</summary>
        public InspectableElement CurrentlySelectedElement { get; private set; }
        public InspectableElement ElementUnderMouse { get; private set; }

        public delegate void ElementSetup(List<InspectableElement> inspectableElements); //Declared Delegate     
        public static event ElementSetup OnElementSetup; //Declared Events

        public enum ContolStates
        {
            AllOn,
            AllOff
        }

        public void Initialize(string loadPath)
        {
            InitializeControllers(loadPath);
            Initialize();
        }

        /// <summary>Initialize using a preset list of bad elements.</summary>
        /// <param name="elementStates">The bad elements to be used</param>
        public void Initialize(List<InspectableElement> elementStates)
        {
            InitializeControllers(elementStates);
            Initialize();
        }

        /// <summary>Initialize using a percent of random bad elements.</summary>
        /// <param name="badPercent">The percentage of elements that should be bad (0 - 100)</param>
        public void Initialize(float badPercent)
        {
            InitializeControllers(badPercent);
            Initialize();
        }

        /// <summary> Initialize using data from save file </summary>
        /// <param name="backhoeStates">The state of backhoe: string: element name, int: state identifier</param>
        public void InitializeFromSaveData(List<Tuple<string, int>> backhoeStates)
        {
            InitializeControllersFromSaveData(backhoeStates);
            Initialize();
        }

        /// <summary>Do all other initialization for the manager.</summary>
        private void Initialize()
        {
            Taskable = new TaskableObject(this);

            OnElementStateChanged += (_) => Taskable.PokeTaskManager();

            if (uiElements == null || uiElements.Length == 0)
            {
                Debug.LogWarning("There are no UI elements on the InspectableManager");
                return;
            }

            foreach (GameObject ui in uiElements)
            {
                IInspectableUI inspectableUI = ui.GetComponent<IInspectableUI>();

                if (inspectableUI == null)
                {
                    Debug.LogError("All UI elements for the Inspectable system must implement the IInspectableUI interface. " + ui.name);
                    continue;
                }

                inspectableUI.InitializeUI(this);
            }


        }

        private void InitializeControllers(string loadPath)
        {
            //Get Debug Data
            StateLogger.DebugData debugData = StateLogger.GetDebugData(loadPath);

            //Initialize controllers
            InspectableControllers = InspectableObject.GetComponentsInChildren<InspectableController>().ToList();
            InspectableControllers.ForEach((x) => x.Initialize(this));

            //Need every spawning location to know their controller
            var inspectableControllersBySpawningData = new Dictionary<ElementSpawningData, InspectableController>();

            //populate lists
            List<ElementSpawningData> spawnLocations = new List<ElementSpawningData>();
            foreach (var iC in InspectableControllers) {
                foreach (var eSD in iC.ElementSpawningLocations) {
                    spawnLocations.Add(eSD);
                    inspectableControllersBySpawningData.Add(eSD, iC);
                }
            }

            //populate each controller based on the JSON file
            foreach (StateLogger.DebugElement debugElement in debugData.DebugElements) {

                ElementSpawningData spawnData = spawnLocations.Find(x => x.ElementName == debugElement.ElementName);
                InspectableController controller = inspectableControllersBySpawningData[spawnData];
                InspectableElement inspectableElement = controller.GetStateByIdentifer(debugElement.StateIdentifier);

                controller.InspectableElements.Add(controller.SetupElement(spawnData, inspectableElement));
            }

            //Initialize InspectableElements
            InspectableElements = new List<InspectableElement>();
            foreach (InspectableController controller in InspectableControllers) {
                InspectableElements.AddRange(controller.InspectableElements);
            }
            OnElementSetup?.Invoke(InspectableElements);

        }

        internal void ElementClicked(InspectableElement element)
        {
            OnElementClicked?.Invoke(element);
        }

        /// <summary>Initialize the controllers using a set of element of bad elements.</summary>
        /// <param name="badElements">The bad elements to use</param>
        private void InitializeControllers(List<InspectableElement> badElements)
        {
            //Get all the child inspectable controllers
            InspectableControllers = InspectableObject.GetComponentsInChildren<InspectableController>().ToList();
            InspectableControllers.ForEach((x) => x.Initialize(this));

            //organize all the elements by controllers
            Dictionary<InspectableController, List<InspectableElement>> elementsByController = new Dictionary<InspectableController, List<InspectableElement>>();
            InspectableControllers.ForEach(x => elementsByController.Add(x, new List<InspectableElement>()));
            Dictionary<InspectableController, int> controllersAndSpots = new();
            foreach (InspectableElement element in badElements)
            {
                
                if (!InspectableElement.IsElementReal(element))
                {
                    Debug.LogError("InspectableManager : An InspectableElement with a 'fake' bad state has been provided to the InspectableManager. " +
                        "This should be replaced as it may cause unexpected behaviour in the IBreakable which uses this InspectableElement. " +
                        $"InspectableElement: '{element}' ");
                }

                int elementHash = element.GetHashCode();

                //get the controller that can use this element
                InspectableController controller = InspectableControllers.Find(x => x.BadHashes.Contains(element.GetHashCode()));
                
                if (controller == null)
                {
                    Debug.LogError("The element \"" + element.gameObject.name + "\" can not be used by any of the controllers on the Inspectable Object");
                    continue;
                }
                controllersAndSpots.TryAdd(controller, controller.ElementSpawningLocations.Count);

                //this fixes crash but results in a game state change resulting in an edge case of changing the answer of what is to be inspected upon reload
                if (controllersAndSpots[controller] > 0)
                {
                    elementsByController[controller].Add(element);
                    controllersAndSpots[controller] -= 1;
                }
                
            }

            AddElementsToControllers(elementsByController);
            //dump bad elements to save
            onStoreBadElements?.Invoke(elementsByController);
        }

        /// <summary>Initialize the controllers using a percent of random bad elements.</summary>
        /// <param name="badPercent">The percentage of elements that should be bad (0 - 100)</param>
        private void InitializeControllers(float badPercent)
        {
            //Get all the child inspectable controllers
            InspectableControllers = InspectableObject.GetComponentsInChildren<InspectableController>().ToList();
            InspectableControllers.ForEach((x) => x.Initialize(this));

            //track how many elements each controller can hold
            List<Tuple<InspectableController, int>> controllersAndElementCount = new List<Tuple<InspectableController, int>>();

            //track which elements are going to which controller
            Dictionary<InspectableController, List<InspectableElement>> elementsByController = new Dictionary<InspectableController, List<InspectableElement>>();

            int totalElements = 0;

            foreach (InspectableController controller in InspectableControllers)
            {
                if (controller.ElementSpawningLocations.Count == 0)
                {
                    Debug.LogError("There are no element locations for the controller \"" + controller.gameObject.name + "\".");
                    continue;
                }

                //track the elements this controller can hold
                controllersAndElementCount.Add(new Tuple<InspectableController, int>(controller, controller.ElementSpawningLocations.Count));

                //track the total amount of elements
                totalElements += controller.ElementSpawningLocations.Count;

                elementsByController.Add(controller, new List<InspectableElement>());
            }

            int badElementCount = (int)(Mathf.Clamp(badPercent, 0f, 100f) / 100f * totalElements);

            for (int i = 0; i < badElementCount; i++)
            {
                //randomly select a controller
                int controllerIndex = Random.Range(0, controllersAndElementCount.Count);

                Tuple<InspectableController, int> current = controllersAndElementCount[controllerIndex];

                // Holds a reference to the filtered list of 'real' states which includes the good states
                // and all 'real' bad states (states which have a non-negative BadMode value)
                List<InspectableElement> possibleStates = current.Item1.RealStates;

                // Randomly choose a state that this controller has.
                // It appears that, since this was using InspectableController.ElementStates, it was possible
                // for the 'Good' state to be picked when randomly selecting bad states.
                // Before finalizing this code, I would like to confirm that we can make the change identified below
                // to ONLY draw from bad states (specifically 'real' bad states). However this will affect all current
                // simulations which randomize bad states.
                elementsByController[current.Item1].Add(possibleStates[Random.Range(0, possibleStates.Count)]);

                ////This code block below might replace the previous line:
                //// Filtered list of all possible 'real' bad states
                //List<InspectableElement> possibleBadStates = possibleStates.Where(element => element.State == InspectableState.Bad).ToList();
                //elementsByController[current.Item1].Add(possibleBadStates[Random.Range(0, possibleBadStates.Count)]);


                if (current.Item2 - 1 == 0)
                {
                    //remove it from the list of possible controllers
                    controllersAndElementCount.RemoveAt(controllerIndex);
                }
                else
                {
                    //decrement the available element locations
                    controllersAndElementCount[controllerIndex] = new Tuple<InspectableController, int>(current.Item1, current.Item2 - 1);
                }
            }

            AddElementsToControllers(elementsByController);
            //this is where we can do the dump to the SaveData badElements list.
            //onStoreBadElements?.Invoke(elementsByController);
        }

        /// <summary>
        /// A initialize method for loading progress from a save file
        /// </summary>
        /// <param name="backhoeStates">The state of backhoe: string: element name, int: state identifier</param>
        private void InitializeControllersFromSaveData(List<Tuple<string, int>> backhoeStates)
        {

            InspectableControllers = InspectableObject.GetComponentsInChildren<InspectableController>().ToList();
            InspectableControllers.ForEach((x) => x.Initialize(this));

            //Need every spawning location to know their controller
            var inspectableControllersBySpawningData = new Dictionary<ElementSpawningData, InspectableController>();

            //populate lists
            List<ElementSpawningData> spawnLocations = new List<ElementSpawningData>();
            foreach (var iC in InspectableControllers)
            {
                foreach (var eSD in iC.ElementSpawningLocations)
                {
                    spawnLocations.Add(eSD);
                    inspectableControllersBySpawningData.Add(eSD, iC);
                }
            }

            //populate each controller based on the save data
            for (int i = 0; i < backhoeStates.Count; i++)
            {
                ElementSpawningData spawnData = spawnLocations.Find(x => x.ElementName == backhoeStates[i].Item1);
                InspectableController controller = inspectableControllersBySpawningData[spawnData];
                InspectableElement inspectableElement = controller.GetStateByIdentifer(backhoeStates[i].Item2);

                controller.InspectableElements.Add(controller.SetupElement(spawnData, inspectableElement));
            }

            //Initialize InspectableElements
            InspectableElements = new List<InspectableElement>();
            foreach (InspectableController controller in InspectableControllers)
            {
                InspectableElements.AddRange(controller.InspectableElements);
            }

            //Load user progress data
            onLoadFromCloudSave?.Invoke();
        }

        /// <summary>Add the elements to each controller.
        /// This method can only be called once the controllers are initialized.</summary>
        /// <param name="pairs"></param>
        private void AddElementsToControllers(Dictionary<InspectableController, List<InspectableElement>> pairs)
        {
            InspectableElements = new List<InspectableElement>();

            foreach (InspectableController controller in pairs.Keys)
            {
                List<InspectableElement> elements = controller.CreateElements(pairs[controller]);

                InspectableElements.AddRange(elements);
            }

            if(OnElementSetup != null)
                OnElementSetup(InspectableElements);
        }

        /// <summary>
        /// This method can be used to reset how the elements on the object are
        /// set up. This method shall only be used for objects that only have
        /// <see cref="DynamicInspectableElement"/>s.
        /// This method will make it so that the <see cref="InspectableElement"/>s on each
        /// component will not match the state the component is actually in.
        /// Because of this, this method shall only be used once the inspection portion
        /// of a scenario is over.
        /// </summary>
        /// <param name="badElements">All the bad elements that should be on the object</param>
        /// <param name="specifiedLocations">If you want specific controllers to use specific elements</param>
        public List<IBreakable> ReloadControllers(List<DynamicInspectableElement> badElements, List<Tuple<InspectableController, ElementSpawningData>> specifiedLocations = null)
        {
            Dictionary<InspectableController, List<DynamicInspectableElement>> elementsByControllers = new Dictionary<InspectableController, List<DynamicInspectableElement>>();

            foreach (InspectableController inspectableController in InspectableControllers)
            {
                elementsByControllers.Add(inspectableController, new List<DynamicInspectableElement>());
            }

            foreach (DynamicInspectableElement element in badElements)
            {
                int hash = element.GetHashCode();

                InspectableController controller = InspectableControllers.Find(x => x.BadHashes.Contains(hash));

                elementsByControllers[controller].Add(element);
            }

            List<IBreakable> brokenComponents = new List<IBreakable>();

            foreach (InspectableController controller in elementsByControllers.Keys)
            {
                List<ElementSpawningData> locations = new List<ElementSpawningData>();

                if (specifiedLocations != null)
                {
                    locations = (from location in specifiedLocations where location.Item1 == controller select location.Item2).ToList();
                }

                brokenComponents.AddRange(controller.ReloadBreakables(elementsByControllers[controller], locations));
            }

            return brokenComponents;
        }

        /// <summary>Toggle if ALL the elements can be inspected or not.</summary>
        /// <param name="state"><see cref="ContolStates.AllOn"/> or <see cref="ContolStates.AllOff"/></param>
        public void UpdateActiveElements(ContolStates state)
        {
            UpdateActiveElements(state == ContolStates.AllOn ? InspectableElements : new List<InspectableElement>());
        }

        /// <summary>Set which inspectable elements in the scene are currently able to be inspected. 
        /// This will also update the UI</summary>
        /// <param name="elementsToActivate">The elements that can be inspected</param>
        public void UpdateActiveElements(List<InspectableElement> elementsToActivate)
        {
            foreach (InspectableElement element in InspectableElements)
            {
                element.SetCurrentlyInspectable(elementsToActivate.Contains(element));
            }

            OnActiveElementsChanged?.Invoke(elementsToActivate);
        }

        /// <summary>
        /// Make an InspectableElement active without affecting the rest of the
        /// active elements.
        /// </summary>
        /// <param name="element">The element to activate</param>
        public void AddActiveElement(InspectableElement element)
        {
            if (!element.CurrentlyInspectable)
            {
                element.SetCurrentlyInspectable(true);
                OnActiveElementsChanged?.Invoke(ActiveElements);
            }
        }

        /// <summary>
        /// Make an InspectableElement inactive without affecting the rest of the 
        /// active elements.
        /// </summary>
        /// <param name="element">The element to deactivate</param>
        public void RemoveActiveElement(InspectableElement element)
        {
            if (element.CurrentlyInspectable)
            {
                element.SetCurrentlyInspectable(false);
                OnActiveElementsChanged?.Invoke(ActiveElements);
            }
        }

        /// <summary>Update the element that is currently inspected.</summary>
        /// <param name="element">The element that had it's selection changed</param>
        /// <param name="isSelected"> </param>
        internal void UpdateCurrentlySelectedElement(InspectableElement element, bool isSelected)
        {
            if (!isSelected)
            {
                if (CurrentlySelectedElement == element)
                {
                    CurrentlySelectedElement = null;
                }

                OnElementDeselected?.Invoke(element);
            }
            else if (isSelected)
            {
                if (!element.CurrentlyInspectable)
                {
                    return;
                }

                CurrentlySelectedElement = element;

                OnElementSelected?.Invoke(element);
            }
        }

        /// <summary>
        /// Update the element that is under the mouse.
        /// Fire the events <see cref="OnElementMouseEnter"/> or 
        /// <see cref="OnElementMouseExit"/>.
        /// If the mouse is over the element will be determined using the
        /// <see cref="Interactable"/> flags on the element.
        /// </summary>
        /// <param name="element">The element that the mouse moved onto or off of.</param>
        public void UpdateElementUnderMouse(InspectableElement element)
        {
            if (element.HasFlags(Interactable.Flags.MouseOver))
            {
                if (!element.CurrentlyInspectable)
                {
                    return;
                }

                ElementUnderMouse = element;

                OnElementMouseEnter?.Invoke(element);
            }
            else
            {
                if (ElementUnderMouse == element)
                {
                    ElementUnderMouse = null;
                }

                OnElementMouseExit?.Invoke(element);
            }
        }

        public void RefreshElementState(InspectableElement element)
        {
            OnElementStateChanged?.Invoke(element);
        }

        /// <summary>Evaluate the elements. The status of each element will be updated.</summary>
        /// <param name="includeInactiveElements">If inactive elements should be included. (<see cref="InspectableElement.currentlyInspectable"/> == false)</param>
        public void EvaluateElements(bool includeInactiveElements = false)
        {
            (includeInactiveElements ? InspectableElements : ActiveElements).ForEach(x => x.Evaluate());

            OnEvaluationCompleted?.Invoke();

            Taskable.PokeTaskManager();
        }

        /// <summary>Evaluate the elements with no selection. The status of each element will be updated. If the selection is blank, 
        /// the selection will be considered as InspectableState.Good.</summary>
        /// <param name="includeInactiveElements">If inactive elements should be included. (<see cref="InspectableElement.currentlyInspectable"/> == false)</param>
        public void EvaluateElementsWithNoSelection(bool includeInactiveElements = false)
        {
            (includeInactiveElements ? InspectableElements : ActiveElements).ForEach(x => x.EvaluateNoSelection());

            OnEvaluationCompleted?.Invoke();

            Taskable.PokeTaskManager();
        }

        public object CheckTask(int checkType, object inputData)
        {
            switch (checkType)
            {
                case (int)TaskVertexManager.CheckTypes.ElementsEvaluated:
                    {
                        if (CheckTaskInspectables(inputData, true, out bool returned))
                        {
                            return returned;
                        }
                        else
                        {
                            Debug.LogError("For Check Type \"" + TaskVertexManager.CheckTypes.ElementsEvaluated.ToString() + "\", the input data was invalid");
                            break;
                        }
                    }

                case (int)TaskVertexManager.CheckTypes.ElementsInspected:
                    {
                        if (CheckTaskInspectables(inputData, false, out bool returned))
                        {
                            return returned;
                        }
                        else
                        {
                            Debug.LogError("For Check Type \"" + TaskVertexManager.CheckTypes.ElementsEvaluated.ToString() + "\", the input data was invalid");
                            break;
                        }
                    }

                //Check if all the elements were inspected. 
                case (int)TaskVertexManager.CheckTypes.Bool:

                    return ElementsInspected(InspectableElements);
            }

            return null;
        }

        /// <summary>
        /// Used in <see cref="CheckTask(int, object)"/> for <see cref="TaskVertexManager.CheckTypes.ElementsEvaluated"/>
        /// and <see cref="TaskVertexManager.CheckTypes.ElementsInspected"/> to interpret the data passed in 
        /// and see if they are inspected/evaluated.
        /// </summary>
        /// <param name="inputData">The input data. This shall be a <see cref="InspectableElement"/> or <see cref="List{InspectableElement}"/></param>
        /// <param name="evaluationsRequiered">If the elements need to be evaluated</param>
        /// <param name="returnValue">If all the elements passed in were inspected and/or evaluated</param>
        /// <returns>If the inputData is valid</returns>
        private bool CheckTaskInspectables(object inputData, bool evaluationsRequiered, out bool returnValue)
        {
            // Pattern matching to determine if the 'inputData' object is a single InspectableElement or
            // a collection of InspectableElements
            if (inputData is InspectableElement element)
            {
                returnValue = ElementsInspected(new List<InspectableElement>() { element }, evaluationsRequiered);
                return true;
            }
            else if (inputData is List<InspectableElement> elements)
            {
                returnValue = ElementsInspected(elements, evaluationsRequiered);
                return true;
            }

            returnValue = false;
            return false;
        }

        /// <summary>Check if the passed in <see cref="InspectableElement"/>s have been inspected.</summary>
        /// <param name="elements">The elements to check</param>
        /// <param name="requireEvaluation">If they need to be evaluated</param>
        /// <returns></returns>
        public bool ElementsInspected(List<InspectableElement> elements, bool requireEvaluation = false)
        {
            foreach (InspectableElement element in elements)
            {
                if (requireEvaluation)
                {
                    if (!element.HasBeenEvaluated)
                    {
                        return false;
                    }
                }
                else if (!element.HasBeenInspected)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>Generate a list of <see cref="TaskDetails"/> for a list of <see cref="InspectableElement"/>s.
        /// This can be used as the default option for generating the data for the result scene of a scenario scene.</summary>
        public static List<TaskDetails> GenerateDefualtInspectionResults(List<InspectableElement> elements)
        {
            List<TaskDetails> details = new List<TaskDetails>();

            foreach (InspectableElement element in elements)
            {
                TaskDetails detail = new TaskDetails();

                detail.Title = element.FullName;

                detail.Description = "Inspect this component. The possible states are:\n ";

                foreach (InspectableElement data in element.ParentController.ElementStates)
                {
                    //get all the element state messages.
                    if (data.StateDescription == null)
                    {
                        detail.Description += "<indent=5%>- " + data.StateMessage + "\n";
                    }
                    else
                    {
                        detail.Description += "<indent=5%>- " + data.StateMessage + ": " + data.StateDescription + "\n";
                    }
                }

                if (!element.HasBeenInspected)
                {
                    detail.Comments = "This item was not inspected. \n The correct answer was \"" + element.GetCorrectState().StateMessage + "\"";
                }
                else if (element.InspectionIsCorrect)
                {
                    detail.Comments = "\"" + element.GetCorrectState().StateMessage + "\" was the correct answer!";
                    detail.State = UserTaskVertex.States.Complete;
                }
                else
                {
                    detail.Comments = "The inspection \"" + element.GetSelectedState().StateMessage + "\" was incorrect.\n" +
                       "The correct answer was \"" + element.GetCorrectState().StateMessage + "\".";
                    if (element.FeedbackDetail != null)
                    {
                        detail.Comments += "\n\nTips: " + element.FeedbackDetail + ".";
                    }
                }

                details.Add(detail);
            }

            return details;
        }

        /// <summary>Generate a list of <see cref="TaskDetails"/> for a list of <see cref="InspectableElement"/>s.
        /// This can be used as to generate the data for the result scene of Heavy Equipment.</summary>
        public static List<TaskDetails> GenerateHeavyEquipmentInspectionResults(List<InspectableElement> elements)
        {
            List<TaskDetails> details = new List<TaskDetails>();

            foreach (InspectableElement element in elements)
            {
                if (!element.HasBeenInspected)
                {
                    // Don't include uninspected items in the result list.
                    // It's because in the scenario graph of the heavy equipment, users must inspect each item 
                    // in the list so the can move to the next step.
                    continue;
                }

                TaskDetails detail = new TaskDetails
                {
                    Title = element.FullName,
                    Description = "Inspect this component. The possible states are:\n "
                };

                foreach (InspectableElement data in element.ParentController.ElementStates)
                {
                    //get all the element state messages.
                    if (data.StateDescription == null)
                    {
                        detail.Description += "<indent=5%>- " + data.StateMessage + "\n";
                    }
                    else
                    {
                        detail.Description += "<indent=5%>- " + data.StateMessage + ": " + data.StateDescription + "\n";
                    }
                }


                if (element.InspectionIsCorrect)
                {
                    detail.Comments = "\"" + element.GetCorrectState().StateMessage + "\" was the correct answer!";
                    detail.State = UserTaskVertex.States.Complete;
                }
                else
                {
                    detail.Comments = "The inspection \"" + element.GetSelectedState().StateMessage + "\" was incorrect.\n" +
                       "The correct answer was \"" + element.GetCorrectState().StateMessage + "\".";
                    if (element.FeedbackDetail != null)
                    {
                        detail.Comments += "\n\nTips: " + element.FeedbackDetail + ".";
                    }
                }

                details.Add(detail);
            }

            return details;
        }
    }
}
