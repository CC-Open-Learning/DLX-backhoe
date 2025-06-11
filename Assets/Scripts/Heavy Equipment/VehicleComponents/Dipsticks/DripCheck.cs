/*
 * SUMMARY: Defines dip stick checking system.
 */
using RemoteEducation.Interactions;
using RemoteEducation.Scenarios;
using RemoteEducation.Scenarios.Inspectable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HECheckType = RemoteEducation.Modules.HeavyEquipment.HeavyEquipmentModule.HeavyEquipmentCheckTypes;

namespace RemoteEducation.Modules.HeavyEquipment
{
    /// <summary>
    /// This class checks how much transmission/engine oil is in the system
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class DripCheck : SemiSelectable, IInitializable, IBreakable, ITaskable
    {
        public enum FluidStates
        { 
            Empty,
            Normal,
            Full,
            Overflow
        }

        public enum DripCheckTypes
        {
            EngineFluid,
            TransmissionFluid,
            SecondTransmissionFluid
        }

        /// <summary>
        /// This variable allows us to check things and do things on the backhoe. 
        /// </summary>
        private BackhoeController backhoeController;

        /// <value>
        /// To determine if the inspection menu/mode is on
        /// </value>
        private bool inspectOpen = false;

        /// <value>
        /// To determine if the stick has been wiped of previous results/decontamination
        /// </value>
        private bool isWiped;

        /// <summary>
        /// This variable allows us to set how much oil is on the stick
        /// </summary>
        [SerializeField]
        private OilStain oilIndicator;

        /// <summary>
        /// This list allows us do stuff on the dipstick
        /// </summary>
        public List<GameObject> UI;

        /// <summary>
        /// This variable is the state of the dipstick
        /// </summary>
        [SerializeField]
        private FluidStates dipState;

        /// <summary>
        /// This variable is the closed position of the dipstick
        /// </summary>
        private Vector3 resetPosition;

        /// <summary>
        /// This variable is a spawned cleaning rag that appears when we take the stick out
        /// </summary>
        [SerializeField]
        private Transform cleaningRag;

        /// <value>
        /// Boolean variable to check if we are animating or not
        /// </value>
        private bool isAnimating;

        /// <value>
        /// Variable for how much animation time it goes vertically
        /// </value>
        [SerializeField]
        private float verticalAnimationTime = 1f;

        /// <value>
        /// Variable for how much animation time it goes horizontally
        /// </value>
        [SerializeField]
        private float horizontalAnimationTime = 1f;

        [SerializeField, Tooltip("The type of Drip Check")]
        private DripCheckTypes dripCheckType;

        [SerializeField, Tooltip("Open Position")]
        private Transform openPosition;
        public DripCheckTypes DripCheckType => dripCheckType;

        /// <summary>
        /// Static Camera Position Reference
        /// </summary>
        private Transform cameraPos => CoreCameraController.Instance.MainCameraTransform;

        public TaskableObject Taskable { get; private set; }

        public DynamicInspectableElement InspectableElement { get; private set; }

        /// <summary>
        /// Checks if the stick has been removed
        /// </summary>
        private bool isRemoved;

        /// <summary>
        /// Checks if the stick has been the inital wipe has been done
        /// </summary>
        private bool firstWipeDone;

        /// <summary>
        /// Checks if the oil has been inspected
        /// </summary>
        private bool oilLevelInspected;

        [Header("Cleaning Rag Animation")]
        [SerializeField]
        private float entryATime = .5f;
        [SerializeField]
        private float wipeUpATime = .5f;
        [SerializeField]
        private float wipeDownATime = .5f;
        [SerializeField]
        private float exitATime = .5f;

        /// <summary>
        /// The canvas for the transmission symbol on the dipstick
        /// </summary>
        [SerializeField] private Canvas transmissionSymbolCanvas;

        public FluidStates GetFluidState()
        {
            return dipState;
        }

        /// <summary>
        /// Make the dipstick an inspectable and set it to broken or fixed
        /// </summary>
        public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode)
        {
            //Set the state if broken or not
            if (broken)
            {
                dipState = Random.Range(0, 1) == 1 ? FluidStates.Empty : FluidStates.Overflow;
            }
            else
            {
                dipState = Random.Range(0, 1) == 1 ? FluidStates.Normal : FluidStates.Full;
            }

            InspectableElement = inspectableElement;

            InspectableElement.OnInspect += () => oilLevelInspected = true;

            InspectableElement.OnCurrentlyInspectableStateChanged += (active) =>
            {
                ToggleFlags(!active, Flags.Highlightable | Flags.ExclusiveInteraction);
            };
        }

        public void Initialize(object input = null)
        {
            backhoeController = input as BackhoeController;

            //Initiates the inspection
            if (HeavyEquipmentModule.ScenarioAttatched)
            {
                //This is needed for the graph in PreTripInspectionGS.BuildGraph to work.
                OnSelect += () => HeavyEquipmentModule.PokeTaskManagerOnSelect(this);
            }
            else
            {
                //if no graph is being used, initiate the inspection just by clicking on it.
                OnSelect += TakeOutStick;
            }

            resetPosition = transform.position;

            Taskable = new TaskableObject(this, dripCheckType.ToString());
        }

        public void SetEnabled(bool enabled) {
            GetComponent<BoxCollider>().enabled = enabled;
            GetComponent<MeshRenderer>().enabled = enabled;
            GetComponent<CapsuleCollider>().enabled = enabled;
            transmissionSymbolCanvas.enabled = enabled;
        }

        /// <summary>Public function to call to take the stick out and set the mode change </summary>
        public void TakeOutStick()
        {
            if(backhoeController.EngineDrip.inspectOpen || backhoeController.TransmissionDrip.inspectOpen || backhoeController.SecondTransmissionDrip.inspectOpen || isAnimating)
                return;

            isWiped = false;
            isAnimating = true;
            inspectOpen = true;
            StartCoroutine(StickRemoval());
        }

        /// <summary>
        /// Public function to return to normal functionality and disable UI elements
        /// </summary>
        public void ReturnDipstick()
        {
            if (!inspectOpen || isAnimating)
                return;

            isAnimating = true;
            StartCoroutine(StickReturn());
        }

        /// <summary>
        /// Public function to attempt to use wiping animation
        /// </summary>
        public void WipeStain()
        {
            if (!inspectOpen || isAnimating)
                return;

            isAnimating = true;
            StartCoroutine(WipeOil());
        }

        /// <summary>
        /// Oil Wipe Coroutine
        /// </summary>
        private IEnumerator WipeOil()
        {
            //Hide UI Elements until Animation is Done
            foreach (GameObject UIElement in UI)
            {
                UIElement.SetActive(false);
            }

            //Variables for animation
            float startTime = Time.time;
            float entryTime = startTime + entryATime;
            float wipeUpTime = entryTime + wipeUpATime;
            float wipeDownTime = wipeUpTime + wipeDownATime;
            float exitTime = wipeDownTime + exitATime;
            float timeRatio = 0;

            //First Position Vertical
            Vector3 initialPosition = cameraPos.position + new Vector3(-1.5f,-1f,0);

            //Final Destination
            Vector3 topWipe = oilIndicator.gameObject.transform.position + new Vector3(0, 0.15f, 0);

            //Final Destination
            Vector3 bottomWipe = oilIndicator.gameObject.transform.position;

            //Get Mesh Renderer
            MeshRenderer cleaningMesh = cleaningRag.gameObject.GetComponent<MeshRenderer>();
            cleaningMesh.enabled = true;

            while (Time.time <= exitTime)
            {
                if (Time.time <= entryTime)
                {
                    //Entry
                    timeRatio = (Time.time - startTime) / entryATime;
                    cleaningRag.position = Vector3.Lerp(initialPosition, bottomWipe, timeRatio);
                }
                else if (Time.time <= wipeUpTime)
                {
                    //Wipe Up
                    timeRatio = (Time.time - entryTime) / wipeUpATime;
                    cleaningRag.position = Vector3.Lerp(bottomWipe, topWipe, timeRatio);
                }
                else if (Time.time <= wipeDownTime)
                {
                    oilIndicator.Wipe();
                    timeRatio = (Time.time - wipeUpTime) / wipeDownATime;
                    cleaningRag.position = Vector3.Lerp(topWipe, bottomWipe, timeRatio);
                }
                else
                {
                    //Exit
                    timeRatio = (Time.time - wipeDownTime) / exitATime;
                    cleaningRag.position = Vector3.Lerp(bottomWipe, initialPosition, timeRatio);
                }
                yield return null;
            }

            //Set Final Destination and hide rag
            cleaningRag.position = initialPosition;
            cleaningMesh.enabled = false;
            isAnimating = false;


            //Toggles the "return" button text depending on if the learner
            //needs to get fluid or just return the dipstick
            //UI[1].SetActive(firstWipeDone);
            //UI[2].SetActive(!firstWipeDone);

            firstWipeDone = true;

            isWiped = true;
            Taskable.PokeTaskManager();
        }

        /// <summary>
        /// Coroutine for stick removal
        /// </summary>
        private IEnumerator StickRemoval()
        {

            GetComponent<CapsuleCollider>().enabled = false;

            //Only set dip stick properties if the first wipe has been done
            if (firstWipeDone)
            {
                oilIndicator.SetDipstickProperties(dipState);
            }

            //First Position Vertical
            Vector3 verticalDestination = new Vector3(resetPosition.x, openPosition.position.y, resetPosition.z);

            //Final Destination
            Vector3 finalDestination = openPosition.position;

            //Time Elements
            float verticalTime = Time.time;
            float verticalEndTime = verticalTime + verticalAnimationTime;
            float horizontalEndTime = verticalEndTime + horizontalAnimationTime;
            float timeRatio = 0;

            //Animate our removal
            while (Time.time <= horizontalEndTime)
            {
                if(Time.time <= verticalEndTime)
                {
                    //Vertical Movement
                    timeRatio = (Time.time - verticalTime) / verticalAnimationTime;
                    transform.position = Vector3.Lerp(resetPosition, verticalDestination, timeRatio);
                }
                else
                {
                    //Horizontal Movement
                    timeRatio = (Time.time - verticalEndTime) / horizontalAnimationTime;
                    transform.position = Vector3.Lerp(verticalDestination, finalDestination, timeRatio);
                }
                yield return null;
            }

            //Set Final Destination
            transform.position = finalDestination;
            isAnimating = false;
            GetComponent<CapsuleCollider>().enabled = true;

            //Wipe UI won't activate until the oil level has been inspected
            if (firstWipeDone)
            {
                while(!oilLevelInspected)
                {
                    yield return null;
                }
            }

            //UI[0].SetActive(true);

            isRemoved = true;
            Taskable.PokeTaskManager();
        }

        /// <summary>
        /// Coroutine for returning the stick
        /// </summary>
        private IEnumerator StickReturn()
        {
            //Remove UI elements when returning stick
            /*foreach (GameObject UIElement in UI)
            {
                UIElement.SetActive(false);
            }*/

            //First Position Vertical
            Vector3 horizontalDestination = new Vector3(resetPosition.x, transform.position.y, resetPosition.z);

            //Final Destination
            Vector3 finalDestination = resetPosition;

            Vector3 removedPosition = transform.position;

            //Time Elements
            float horizontalTime = Time.time;
            float horizontalEndTime = horizontalTime + horizontalAnimationTime;
            float verticalEndTime = horizontalEndTime + verticalAnimationTime;
            float timeRatio = 0;

            while (Time.time <= verticalEndTime)
            {
                if (Time.time <= horizontalEndTime)
                {
                    //Horizontal Movement
                    timeRatio = (Time.time - horizontalTime) / horizontalAnimationTime;
                    transform.position = Vector3.Lerp(removedPosition, horizontalDestination, timeRatio);
                }
                else
                {
                    //Vertical Movement
                    timeRatio = (Time.time - horizontalEndTime) / verticalAnimationTime;
                    transform.position = Vector3.Lerp(horizontalDestination, finalDestination, timeRatio);
                }
                yield return null;
            }

            transform.position = finalDestination;

            //Set that we are not animating or inspecting
            isAnimating = false;
            inspectOpen = false;

            isRemoved = false;
            Taskable.PokeTaskManager();
        }

        public object CheckTask(int checkType, object inputData)
        {
            switch (checkType)
            {
                case (int)HECheckType.DipStickIsReturned:
                    return isRemoved ? false : true;
                case (int)HECheckType.DipStickIsRemoved:
                    return isRemoved;

                case (int)HECheckType.DipStickIsWiped:
                    return isWiped;

                default:
                    Debug.LogError("A check type was passed to DripCheck that it could not handle");
                    return null;
            }

        }

        // Resets the inspection for the dripcheck, needed for some occurences of loading in from a save.
        public void resetInspection()
        {
            InspectableElement.MakeUserSelection(RemoteEducation.Scenarios.Inspectable.InspectableElement.NO_STATE_SELECTION);
        }

    }
}