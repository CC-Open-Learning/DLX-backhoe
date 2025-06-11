/*
 * SUMMARY: This file contains the Fuel Water Seperator class which keeps track of 
 *          the inspection and draining of the fuel water separator.
 */

using System.Collections;
using System.Collections.Generic;
using RemoteEducation.Scenarios;
using RemoteEducation.Interactions;
using RemoteEducation.Scenarios.Inspectable;
using UnityEngine;
using RemoteEducation.UI;
using RemoteEducation.UI.Tooltip;
using RemoteEducation.Localization;
using RemoteEducation.Audio;
using HECheckType = RemoteEducation.Modules.HeavyEquipment.HeavyEquipmentModule.HeavyEquipmentCheckTypes;


namespace RemoteEducation.Modules.HeavyEquipment
{
    /// <summary>This class defines the basic functionality of the fuel water separator. </summary>
    public class FuelWaterSeparator : SemiSelectable, IInitializable, IBreakable, ITaskable
    {
        /// <summary>Tracks the current task </summary>
        private Coroutine activeCheck;

        private bool inspected = false;

        [SerializeField, Tooltip("Bowl Object")]
        private GameObject bowl;

        [SerializeField, Tooltip("How long the draining takes")]
        private float drainTime;

        private bool noBowl;

        private bool drained;

        [Tooltip("Draining water effect played when the water serarator is drained.")]
        [SerializeField] private SoundEffect DrainSoundEffect;

        public DynamicInspectableElement InspectableElement { get; private set; }

        public TaskableObject Taskable { get; private set; }

        public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode)
        {
            //noBowl = broken;
            //if(noBowl)
            //{
            //    bowl.SetActive(false);
            //}
            InspectableElement = inspectableElement;

            InspectableElement.OnInspect += () => inspected = true;

            if (gameObject.GetComponent<SimpleTooltip>() == null)
            {
                var tooltip = gameObject.AddComponent<SimpleTooltip>();
                tooltip.tooltipText = Localizer.Localize("HeavyEquipment.FuelWaterSeparatorTooltip");
            }
        }

        public void Initialize(object input = null)
        {
            Taskable = new TaskableObject(this);

            if (HeavyEquipmentModule.ScenarioAttatched)
            {
                OnSelect += () => HeavyEquipmentModule.PokeTaskManagerOnSelect(this);
            }
            else
            {
                OnSelect += DrainFuelWaterSeparator;
            }      
        }

        /// <summary>Initiates fuel water separator task</summary>
        private void DrainFuelWaterSeparator()
        {
            if(activeCheck != null)
            {
                return;
            }

            activeCheck = StartCoroutine(BowlCheck());
        }

        /// <summary>Used to check if the glass bowl exists</summary>
        private IEnumerator BowlCheck()
        {       
            while (true)
            {
                if (inspected)
                {
                    break;
                }

                yield return null;
            }

            if(!noBowl)
                activeCheck = StartCoroutine(DrainPress());
        }

        /// <summary>Drains the fuel water separator</summary>
        public IEnumerator DrainPress()
        {
            while(true)
            {
                if(noBowl)
                {
                    HEPrompts.CreateToast(Localizer.Localize("HeavyEquipment.NoBowl"), drainTime);
                    yield return new WaitForSeconds(drainTime);
                    break;
                }
                else
                {
                    DrainSoundEffect.Play();
                    yield return new WaitForSeconds(drainTime);
                    break;
                }

            }

 
            drained = true;
            Taskable.PokeTaskManager();
        }

        public object CheckTask(int checkType, object inputData)
        {
            switch(checkType)
            {
                case (int)HECheckType.FuelWaterSeparatorDrained:
                    return drained;

                default:
                    Debug.LogError("A check type was passed to Fuel Water Separator that it could not handle");
                    return null;
            }
        }

        public bool CheckDrained()
        {
            return drained;
        }
    }

}


