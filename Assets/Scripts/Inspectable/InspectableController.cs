/*
*  FILE          :	InspectableManager.cs
*  PROJECT       :	CORE Engine Inspection Module
*  PROGRAMMER    :	Duane Cressman
*  FIRST VERSION :	2020-11-02
*  RELATED FILES : ElementSpawningData.cs, InspectableElement.cs, ElementAnchor.cs,
*  
*  DESCRIPTION   :  This file contains the InspectableController class. 
*  
*  Class Description: This class will be used to define a type of component that can be loaded
*                     into a inspection scenario. For example, all of the wheels in the scene can 
*                     be under one GameObject with this class on it. 
*                     The actual components and all there versions will be defined by prefabs containing 
*                     a InspectableElement script. All the possible InspectableElements should be referenced
*                     this class through the Unity Inspector. 
*                     When this class is initialized, it will find all the ElementSpawningData objects 
*                     below it, and add a InspectableElement script to those GameObjects. The 
*                     InspectableElements will them actually do the spawning of the components. 
*                     
*                     The Gameobject structure should go:
*                  
*                    >InspectableController
*                        >ElementSpawnningData (the InspectableElement will be added on this gameobject by the InspectableController
*                            >ElementAnchor
*                                >(the component will be added here)
*                                          
*/

using System;
using System.Linq;
using RemoteEducation.UI.Tooltip;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RemoteEducation.Scenarios.Inspectable
{
    public class InspectableController : MonoBehaviour
    {
        public const string DEFAULT_GOOD_MESSAGE = "Good";

        /// <summary>The elements created at runtime that were placed in the scene.</summary>
        public List<InspectableElement> InspectableElements { get; private set; }

        [Tooltip("Name of this group of elements for use as a checklist heading label.")]
        public string ElementGroupName;

        [Tooltip("The good state for this controller")]
        public InspectableElement GoodElementPrefab;

        [SerializeField, Tooltip("All the bad states this controller can have")]
        public List<InspectableElement> BadElementPrefabs;

        /// <summary>Gets all good and bad element states.</summary>
        public List<InspectableElement> GetAllElements()
        {
            var elements = new List<InspectableElement>();

            elements.Add(GoodElementPrefab);

            for (int i = 0; i < BadElementPrefabs.Count; i++)
            {
                elements.Add(BadElementPrefabs[i]);
            }

            return elements;
        }

        /// <summary>The element states that this controller has with their Identifiers.</summary>
        public Dictionary<InspectableElement, int> IdentifiersByElement { get; private set; }

        /// <summary>
        ///     Collection of all element states provided to this Inspectable Controller.<br/>
        ///     This includes possible 'fake' bad states for <see cref="DynamicInspectableElement"/> objects
        ///     which have been indicated with a <see cref="DynamicInspectableElement.BadMode"/> value of <c>-1</c>
        /// </summary>
        /// <remarks>
        ///     These elements are only for referencing the possible states. 
        ///     They are not in the scene.
        /// </remarks>
        public List<InspectableElement> ElementStates
        {
            get
            {
                if(IdentifiersByElement == null)
                {
                    return null;
                }

                return IdentifiersByElement.Keys.ToList();
            }
        }

        /// <summary>
        ///     Collection of element states which can be used for assignment to
        ///     an InspectableElement in the scene. This collection contains no 'fake'
        ///     bad states, as a 'fake' bad state should only ever be shown in UI elements
        ///     and not used as an actual state on the InspectableObject
        /// </summary>
        public List<InspectableElement> RealStates => ElementStates.Where(InspectableElement.IsElementReal).ToList();


        [Tooltip("Set the spawning locations here if they are not a child of this game object.")]
        public List<ElementSpawningData> ElementSpawningLocations;

        /// <summary>Inspectable manager used to initialize this controller.</summary>
        public InspectableManager InspectableManager { get; private set; }

        /// <summary>The hashes for the possible elements this controller can use.</summary>
        public List<int> BadHashes { get; private set; }

        public void Initialize(InspectableManager inspectableManager)
        {
            InspectableManager = inspectableManager;

            SetDefaultGoodMessage(GoodElementPrefab);

            SetupStateIdentifiers();

            if (ElementSpawningLocations == null || ElementSpawningLocations.Count == 0)
            {
                //find all the spawning locations for the elements
                ElementSpawningLocations = GetComponentsInChildren<ElementSpawningData>().ToList();
            }

            //Get a list of the hashes for each bad prefab.
            //These will be use when dynamically spawning in elements.
            BadHashes = new List<int>();
            BadElementPrefabs.ForEach(x => BadHashes.Add(x.GetHashCode()));

            InspectableElements = new List<InspectableElement>();
        }

        /// <summary>Create the identifiers that will be used to reference the different element states.</summary>
        private void SetupStateIdentifiers()
        {
            int currentIdentifier = 0;

            IdentifiersByElement = new Dictionary<InspectableElement, int>() { { GoodElementPrefab, currentIdentifier } };
            BadElementPrefabs.ForEach(x => IdentifiersByElement.Add(x, ++currentIdentifier));
        }

        /// <summary>If the element is good and no state message has been entered, add <see cref="DEFAULT_GOOD_MESSAGE"/></summary>
        /// <param name="element">The good element prefab</param>
        private void SetDefaultGoodMessage(InspectableElement element)
        {
            if (!element) { Debug.LogError($"{name} has a null element"); return; }
            if(element.State == InspectableState.Good && element.StateMessage.Trim().Equals(string.Empty))
            {
                element.StateMessage = DEFAULT_GOOD_MESSAGE;
            }
        }

        /// <summary>Get the element state that corresponds to the identifier passed in.</summary>
        /// <param name="identifier">The identifier to find the element for</param>
        /// <returns>The element that corresponds to the identifier. Null if no element is found.</returns>
        public InspectableElement GetStateByIdentifer(int identifier)
        {
            if(identifier == InspectableElement.NO_STATE_SELECTION)
            {
                return null;
            }

            foreach (InspectableElement element in IdentifiersByElement.Keys)
            {
                if (IdentifiersByElement[element] == identifier)
                {
                    return element;
                }
            }

            if(identifier != InspectableElement.NO_STATE_SELECTION)
            {
                Debug.LogError("There is no InspectableElement for the identifier \'" + identifier + "\' in the \"" + gameObject.name + "\" controller.");
            }

            return null;
        }

        /// <summary>
        /// This method can be used to spawn in all the elements randomly based on 
        /// how many element should be good.
        /// </summary>
        /// <param name="numOfGoodElements">How many elements should be good</param>
        /// <returns>All the elements that were created</returns>
        public List<InspectableElement> CreateElements(int numOfGoodElements)
        {
            int badElementCount = ElementSpawningLocations.Count - numOfGoodElements;

            List<InspectableElement> badElements = new List<InspectableElement>();

            for(int i = 0; i < badElementCount; i++)
            {
                badElements.Add(BadElementPrefabs[Random.Range(0, BadElementPrefabs.Count)]);
            }

            return CreateElements(badElements);
        }

        /// <summary>
        /// Initialize the spawning locations. First all the bad elements
        /// will be randomly placed on a spawning location. Then all the 
        /// other spawning locations will be set as good.
        /// <see cref="UninitializedSpawningLocations"/> is used in this method.
        /// Make sure that <see cref="RefreshElementLoading"/> is called before this method.
        /// </summary>
        /// <param name="badElements">The bad elements to use</param>
        /// <returns></returns>
        public List<InspectableElement> CreateElements(List<InspectableElement> badElements)
        {
            if(badElements.Count > ElementSpawningLocations.Count)
            {
                Debug.LogError("InspectableController : There are not enough element locations for all the bad elements" +
                    "that are to be setup. Check that the elements entered to be bad can all fit on the InspectableObject");

                return null;
            }

            InspectableElements = new List<InspectableElement>();

            List<ElementSpawningData> avalibleLocations = new List<ElementSpawningData>();
            avalibleLocations.AddRange(ElementSpawningLocations);

            //create all the bad elements
            foreach (InspectableElement badElement in badElements)
            {
                //get a random element to use
                int index = Random.Range(0, avalibleLocations.Count);

                InspectableElements.Add(SetupElement(avalibleLocations[index], badElement));

                //remove it from the list of possible element locations
                avalibleLocations.RemoveAt(index);
            }

            //create all the good elements
            foreach (ElementSpawningData data in avalibleLocations)
            {
                InspectableElements.Add(SetupElement(data, GoodElementPrefab));
            }

            return InspectableElements;
        }

        /// <summary>
        /// This method will set up the element in the spawning location.
        /// It will also setup the tool tip.
        /// Once the element is added to the game object, the element will be
        /// initialized with <see cref="InspectableElement.Initialize(InspectableController, InspectableObject, string, string, int)"/>.
        /// </summary>
        /// <param name="spawningLocation">The location where the element should be added.</param>
        /// <param name="elementPrefab">The element to add.</param>
        /// <returns></returns>
        public InspectableElement SetupElement(ElementSpawningData spawningLocation, InspectableElement elementPrefab)
        {
            //Add the element to the gameobject the spawning location is on
            InspectableElement element = elementPrefab.AddElementToGameObject(spawningLocation.gameObject);

            //get the full name for the element. If non is provided, use the element name
            string fullName = spawningLocation.ElementTooltip.Equals(string.Empty) ? spawningLocation.ElementName : spawningLocation.ElementTooltip;

            //initialize the element
            element.Initialize(this, InspectableManager.InspectableObject, spawningLocation.ElementName, fullName, IdentifiersByElement[elementPrefab]);

            // Add a tooltip of the element's name if tooltips have been enabled 
            // on the InspectableManager and on the individual InspectableElement
            if (InspectableManager.UseStandardTooltip && spawningLocation.AddTooltip)
            {
                element.Tooltip = element.gameObject.AddComponent<SimpleTooltip>();

                if (element.Tooltip) { element.Tooltip.tooltipText = fullName; }
            }

            return element;
        }

        /// <summary>
        /// Reload how the <see cref="IBreakable"/>s on this controller is setup. This does not change how the
        /// corresponding <see cref="DynamicInspectableElement"/> is setup. This means that the UI will not be updated.
        /// Also the <see cref="IBreakable"/> will appear to differently to what the inspections are expecting. Therefore
        /// the user will not be able to make accurate inspections.
        /// This method should only be used once no more inspections need to be made.
        /// Good elements will be placed where ever a new bad element wasn't.
        /// </summary>
        /// <param name="badElements">The bad elements to be placed.</param>
        /// <param name="specifiedLocations">Any specified locations</param>
        /// <returns></returns>
        public List<IBreakable> ReloadBreakables(List<DynamicInspectableElement> badElements, List<ElementSpawningData> specifiedLocations)
        {
            if(specifiedLocations != null)
            {
                //error checking
                foreach (ElementSpawningData location in specifiedLocations)
                {
                    if (!ElementSpawningLocations.Contains(location))
                    {
                        Debug.LogError("When reloading a controller, a location was specified for a controller, but the controller doesn't have that location." +
                            " The elements will be spawned in at random locations");

                        specifiedLocations.Clear();

                        break;
                    }
                }
            }

            if(GoodElementPrefab as DynamicInspectableElement == null)
            {
                Debug.LogError("The elements can not be reloaded if the GoodElementPrefab is not a DynamicInspectableElement");
                return null;
            }

            List<ElementSpawningData> avalibleLocations = new List<ElementSpawningData>();
            avalibleLocations.AddRange(ElementSpawningLocations);

            List<IBreakable> reloadedComponents = new List<IBreakable>();

            //reload all the bad elements
            foreach(DynamicInspectableElement element in badElements)
            {
                ElementSpawningData spawningLocation;

                //check if a specified location was specified
                if(specifiedLocations != null && specifiedLocations.Count > 0)
                {
                    spawningLocation = specifiedLocations[Random.Range(0, specifiedLocations.Count)];
                }
                else
                {
                    spawningLocation = avalibleLocations[Random.Range(0, avalibleLocations.Count)];
                }

                avalibleLocations.Remove(spawningLocation);

                reloadedComponents.Add(ReloadBreakable(element, spawningLocation));
            }

            //reload all the good elements in all the left over locations
            foreach(ElementSpawningData location in avalibleLocations)
            {
                reloadedComponents.Add(ReloadBreakable(GoodElementPrefab as DynamicInspectableElement, location));
            }

            return reloadedComponents;
        }

        /// <summary>
        /// Reload the <see cref="IBreakable"/> at a location.
        /// See <see cref="ReloadBreakables"/> for warnings about this method.
        /// </summary>
        /// <param name="newElement">The new state to put the <see cref="IBreakable"/> in</param>
        /// <param name="location">The location of the <see cref="IBreakable"/> to change</param>
        /// <returns></returns>
        private IBreakable ReloadBreakable(DynamicInspectableElement newElement, ElementSpawningData location)
        {
            IBreakable breakable = location.gameObject.GetComponent<IBreakable>();
            DynamicInspectableElement existingElement = location.gameObject.GetComponent<DynamicInspectableElement>();

            breakable.AttachInspectable(existingElement, newElement.State == InspectableState.Bad, newElement.BadMode);

            return breakable;
        }
    }
}