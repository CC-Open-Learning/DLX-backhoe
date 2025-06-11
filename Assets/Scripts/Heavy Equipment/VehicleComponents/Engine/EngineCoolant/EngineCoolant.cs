/*
 * SUMMARY: This file contains the Engine Coolant class. The purpose this class is to act as a walkthrough
 *          for the inspection of the engine coolant. The tasks, states, messages and animations are all called here.
 *          This is done through coroutines and the Prompt Manager.
 */

using System.Collections;
using System.Collections.Generic;
using RemoteEducation.Interactions;
using RemoteEducation.Scenarios.Inspectable;
using UnityEngine;
using RemoteEducation.UI;
using RemoteEducation.Scenarios;

namespace RemoteEducation.Modules.HeavyEquipment
{
    [DisallowMultipleComponent]
    /// <summary>This class defines the inspection logic for the engine coolant. </summary>
    public sealed class EngineCoolant : SemiSelectable, IInitializable, IBreakable, ITaskable
    {
        public enum Temperature
        {
            Hot,
            Cold
        }

        [Header("Visuals")]

        [SerializeField, Tooltip("A visual effect to represent heat")]
        private GameObject heatEffect; //Avoid making heat effect a child because of highlighting interference
        [SerializeField, Tooltip("How long the radiator should take to cool down.")]
        private float timeToCoolDownRadiator = 4f;

        [Header("Interactable Scripts")]

        [SerializeField, Tooltip("Interactable script for the cap")]
        private EngineCoolantCap cap;
        public EngineCoolantCap Cap => cap;

        [SerializeField, Tooltip("Interactable script for the fluid")]
        private CoolantFluid fluid;
        public CoolantFluid Fluid => fluid;

        public TaskableObject Taskable { get; private set; }

        public DynamicInspectableElement InspectableElement { get; private set; }
        
        /// <summary> The collider on this object.</summary>
        private Collider coolantCollider;

        private Temperature radiatorTempurature;
        private bool fluidIsClean;

        public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode)
        {
            fluidIsClean = !broken;

            InspectableElement = inspectableElement;
        }

        public void Initialize(object input = null)
        {
            radiatorTempurature = Random.Range(0, 2) == 0 ? Temperature.Hot : Temperature.Cold;

            // uncomment to randomly set full state
            //bool isFull = Random.Range(0, 2) == 0;
            bool isFull = true;
            fluid.SetFluidState(isFull, fluidIsClean);

            Taskable = new TaskableObject(this);

            coolantCollider = GetComponent<Collider>();

            //This will start the coolant check if the scenario is attached.
            if(HeavyEquipmentModule.ScenarioAttatched)
            {
                OnSelect += () => HeavyEquipmentModule.PokeTaskManagerOnSelect(this);
            }

            HideHeatEffect();
        }

        public void ShowHeatEffectIfAny()
        {
            heatEffect.SetActive(radiatorTempurature == Temperature.Hot);
            highlight = new SoftEdgeHighlight(gameObject, HighlightObject.OUTLINE_WIDTH_THIN, HighlightObject.Mode.OutlineAll);
        }

        public void HideHeatEffect()
        {
            heatEffect.SetActive(false);
            highlight = new SoftEdgeHighlight(gameObject, HighlightObject.OUTLINE_WIDTH_THIN, HighlightObject.Mode.OutlineVisible);
        }

        /// <summary>Toggles if the user can click on the fluid in the tank.</summary>
        /// <remarks>Since the fluid is inside of the tank, we just need to disable the tank 
        /// collider to make it so that the fluid can be clicked on.</remarks>
        /// <param name="isClickable"></param>
        public void ToggleFluidIsClickable(bool isClickable)
        {
            coolantCollider.enabled = !isClickable;
        }

        ///// <summary>Calls initial coroutine for task.</summary>
        //public void StartCheck()
        //{
        //    if (activeCheck != null)
        //    {
        //        return;
        //    }

        //    activeCheck = StartCoroutine(TemperatureCheck());
        //}

        ///// <summary>This method is for temperature check</summary>
        ///// <remarks>Here the learner is told that they can not remove the cap until the radiator is cold</remarks>
        //private IEnumerator TemperatureCheck()
        //{
        //    int key = PromptManager.Instance.CreateStatic("Check the radiator temperature (Press C to check)", PromptManager.VerticalPosition.Bottom);
        //    PromptManager.Instance.Show(key);

        //    while (true)
        //    {
        //        if (Hotkeys.GetKeyDown(KeyCode.C)) 
        //        {  
        //            break;
        //        }

        //        yield return null;
        //    }

        //    PromptManager.Instance.Destroy(key);

        //    PromptManager.Instance.CreateToast("The radiator is currently " + radiatorTempurature.ToString().ToLower(), 3f);
        //    yield return new WaitForSeconds(3f);

        //    PromptManager.Instance.CreateToast("Always make sure the radiator is cold before removing the cap", 3f);
        //    yield return new WaitForSeconds(3f);

        //    if(radiatorTempurature == Temperature.Hot)
        //    {
        //        PromptManager.Instance.CreateToast("Since it is hot we will need to wait a bit until it's cold", 5f);
        //        yield return new WaitForSeconds(5f);
        //        Destroy(heatEffect);
        //    }

        //    activeCheck = StartCoroutine(OpenCap());

        //}

        ///// <summary>This method is for opening the cap, which calls the open animation when selected</summary>
        //private IEnumerator OpenCap()
        //{     
        //    int key = PromptManager.Instance.CreateStatic("Remove the cap", PromptManager.VerticalPosition.Bottom);
        //    PromptManager.Instance.Show(key);

        //    while(true)
        //    {
        //        cap.RemoveFlags(Flags.InteractionsDisabled);

        //        if (cap.Removed)
        //        {
        //            yield return StartCoroutine(cap.RemoveCap());
        //            break;
        //        }

        //        yield return null;
        //    }

        //    cap.AddFlags(Flags.InteractionsDisabled);
        //    PromptManager.Instance.Destroy(key);

        //    activeCheck = StartCoroutine(ContaminantsCheck());
        //}

        ///// <summary>This method is for opening the cap, which calls the open animation when selected</summary>
        //private IEnumerator ContaminantsCheck()
        //{
        //    PromptManager.Instance.CreateToast("Now we need to inspect the coolant", 3f);
        //    yield return new WaitForSeconds(3f);

        //    //Make object inspectable.

        //    int key = PromptManager.Instance.CreateStatic("Check off if the coolant is clean or dirty in the Inspection Checklist", PromptManager.VerticalPosition.Bottom);
        //    PromptManager.Instance.Show(key);
        //    while (true)
        //    {
        //        if(fluidChecked)
        //        {
        //            break;
        //        }

        //        yield return null;
        //    }

        //    //Make object un-inspectable.

        //    PromptManager.Instance.Destroy(key);
        //    StartCoroutine(CheckFluidLevels());
        //}

        ///// <summary>This method is to check the fluid levels. If it's low, the learner can press on it to fill it up</summary>
        //private IEnumerator CheckFluidLevels()
        //{

        //   PromptManager.Instance.CreateToast("Check the coolant fluid level", 3f);
        //   yield return new WaitForSeconds(3f);


        //   int key = PromptManager.Instance.CreateStatic("The coolant level is low, so you will need to fill it up", PromptManager.VerticalPosition.Bottom);
        //   if (fluidState == FluidStates.Low)
        //   {
        //        PromptManager.Instance.Show(key);
        //   }
        //   else
        //   {
        //        PromptManager.Instance.CreateToast("The coolant level is already full, so you can move forward", 3f);
        //        yield return new WaitForSeconds(3f);
        //   }          

        //    while (fluidState != FluidStates.Full)
        //    {
        //        lowFluidAction.RemoveFlags(Flags.InteractionsDisabled);
        //        if (lowFluidAction.Fill)
        //        {
        //            lowFluid.SetActive(false);
        //            fullFluid.SetActive(true);
        //            fluidState = FluidStates.Full;
        //        }

        //        yield return null;
        //    }

        //    lowFluidAction = null;
        //    PromptManager.Instance.Destroy(key);

        //    activeCheck = StartCoroutine(CloseCap());
        //}

        ///// <summary>This method is for closing the cap, which calls the open animation when selected</summary>
        ///// <remarks>After this task is finished the inspection is done</remarks>
        //private IEnumerator CloseCap()
        //{
        //    int key = PromptManager.Instance.CreateStatic("Close the cap and the coolant inspection is done", PromptManager.VerticalPosition.Bottom);
        //    PromptManager.Instance.Show(key);

        //    cap.RemoveFlags(Flags.InteractionsDisabled);

        //    while (true)
        //    {
        //        if (cap.Removed == false)
        //        {
        //            yield return StartCoroutine(cap.CloseCap());
        //            break;
        //        }

        //        yield return null;
        //    }

        //    PromptManager.Instance.Destroy(key);

        //    cap.AddFlags(Flags.InteractionsDisabled);

        //    //make inspectable 

        //    coolantInspectionCompleted = true;
        //    Taskable.PokeTaskManager();
        //}

        public Temperature GetTemperature()
        {
            return radiatorTempurature;
        }

        public void StartCoolDownRadiator()
        {
            if(radiatorTempurature == Temperature.Hot)
            {
                StartCoroutine(CoolDownRadiator());
            }
        }

        private IEnumerator CoolDownRadiator()
        {
            //TODO : do some stuff that makes the heat block fade

            //but for now
            yield return new WaitForSecondsRealtime(timeToCoolDownRadiator);
            heatEffect.SetActive(false);

            radiatorTempurature = Temperature.Cold;
            Taskable.PokeTaskManager();
        }

        public object CheckTask(int checkType, object inputData)
        {
            switch (checkType)
            {
                case (int)TaskVertexManager.CheckTypes.Bool:
                    return radiatorTempurature == Temperature.Hot;

                default:
                    Debug.LogError("A Check Type was passed to the EngineCoolant that it could not handle");
                    return null;
            }
        }
    }
}
