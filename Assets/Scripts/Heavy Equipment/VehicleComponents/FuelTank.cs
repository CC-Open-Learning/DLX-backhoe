/*
 * SUMMARY: This file contains the Fuel Tank class. The purpose this class is for the user 
 *          to fill the fuel tank and be reminded that the tank must be full at the end of the day.
 */

using RemoteEducation.Interactions;
using RemoteEducation.Scenarios;
using RemoteEducation.Scenarios.Inspectable;
using System.Collections;
using System.Collections.Generic;
using RemoteEducation.Localization;
using UnityEngine;
using RemoteEducation.UI;
using RemoteEducation.UI.Tooltip;
using RemoteEducation.Audio;

namespace RemoteEducation.Modules.HeavyEquipment
{
    [DisallowMultipleComponent]
    /// <summary>This class contains the logic for filling the tank and the animation </summary>
    public sealed class FuelTank : SemiSelectable, IInitializable, ITaskable
    {
        /// <summary>Starting position </summary>
        private Vector3 initialPosition;

        /// <summary>Starting rotation </summary>
        private Quaternion initialRotation;

        /// <summary>Mesh of the fuel can, must be disabled at the beginning of the simulation </summary>
        private MeshRenderer fuelCanMesh;

        /// <summary>Checks if fuel can is animating</summary>
        private Coroutine activeAnimation;

        /// <summary>Position of inital camera used for fuel can animation starting point</summary>
        private Transform cameraPosition => CoreCameraController.Instance.MainCameraTransform;

        [SerializeField, Tooltip("Fuel Gauge script tp keep track of fuel level")] Gauge fuelGauge;

        [SerializeField, Tooltip("Transform of the fuel can")]
        private Transform fuelCan;

        [SerializeField, Tooltip("Transform for the fill position and rotation")]
        private Transform fillPoint;

        [Header("Fill Animation")]

        [SerializeField, Tooltip("Length of entry animation")] float entryTimeLength;

        [SerializeField, Tooltip("Length of pouring animation")] float fillTimeLength;

        [SerializeField, Tooltip("Length of exit animation")] float exitTimeLength;

        [SerializeField, Tooltip("Cap of the fuel tank")]
        private EngineCoolantCap fuelCap;

        [SerializeField, Tooltip("Sound for filling")]
        private SoundEffect fillSound;

        private bool soundPlaying = false;
        public TaskableObject Taskable { get; private set; }

        public void Initialize(object input = null)
        {
            if (gameObject.GetComponent<SimpleTooltip>() == null)
            {
                var tooltip = gameObject.AddComponent<SimpleTooltip>();
                tooltip.tooltipText = Localizer.Localize("HeavyEquipment.FuelTankTooltip");
            }

            //Initial data stored for exit
            initialPosition = cameraPosition.position + new Vector3(0, -1f, 0);
            initialRotation = fuelCan.localRotation;
            fuelCanMesh = fuelCan.gameObject.GetComponent<MeshRenderer>();
            fuelCanMesh.enabled = false;

            OnSelect += FillFuelTank;
            Taskable = new TaskableObject(this);
        }

        public bool FuelGaugeCheck()
        {
            return fuelGauge.Full;
        }

        public Gauge GetFuelGauge()
        {
            return fuelGauge;
        }

        /// <summary>Checks fuel status and calls animation if fuel level is low</summary>
        private void FillFuelTank()
        {
            if (activeAnimation != null)
                return;

            if(!fuelGauge.GaugeChecked) //Fuel Gauge not checked, do nothing
            {
                HEPrompts.CreateToast(Localizer.Localize("HeavyEquipment.FuelGaugeToastCheck"), HEPrompts.LongToastDuration);
            }
            else if (fuelGauge.GaugeChecked && fuelGauge.Full) //Fuel Gauge checked but full, do nothing
            {
                HEPrompts.CreateToast(Localizer.Localize("HeavyEquipment.FuelTankToastAlreadyFull"), HEPrompts.LongToastDuration);
            }
            else if(fuelGauge.GaugeChecked && !fuelGauge.Full) //Fuel Gauge checked but low, play animation and set to full
            {
                fuelGauge.SetLevel(true);
                activeAnimation = StartCoroutine(FillAnimation());
            }
        }

        /// <summary>Filling animation</summary>
        private IEnumerator FillAnimation()
        {
            fuelCanMesh.enabled = true;

            //Animation times
            float startTime = Time.time;
            float entryTime = startTime + entryTimeLength;
            float fillTime = entryTime + fillTimeLength;
            float exitTime = fillTime + exitTimeLength;
            float timeRatio = 0;

            fuelCap.RemoveCapCall();

            while (Time.time <= exitTime)
            {
                if (Time.time <= entryTime)
                {
                    //Entry
                    timeRatio = (Time.time - startTime) / entryTimeLength;
                    fuelCan.position = Vector3.Lerp(initialPosition, fillPoint.position, timeRatio);
                }
                else if(Time.time <= fillTime)
                {
                    //Fill
                    timeRatio = (Time.time - entryTime) / fillTimeLength;
                    fuelCan.localRotation = Quaternion.Lerp(initialRotation, fillPoint.localRotation, timeRatio);

                    if(!soundPlaying)
                    {
                        soundPlaying = true;
                        fillSound.PlayFor(fillTimeLength - 0.5f);
                    }
                }
                else
                {
                    //Exit
                    timeRatio = (Time.time - fillTime) / exitTimeLength;
                    fuelCan.localRotation = Quaternion.Lerp(fillPoint.localRotation, initialRotation, timeRatio);
                    fuelCan.position = Vector3.Lerp(fillPoint.position, initialPosition, timeRatio);
                }

                yield return null;
            }

            soundPlaying = false;
            fuelCap.CloseCapCall();

            fuelCanMesh.enabled = false;

            HEPrompts.CreateToast(Localizer.Localize("HeavyEquipment.FuelTankToastReminder"), HEPrompts.LongToastDuration);

            yield return new WaitForSeconds(5f);

            activeAnimation = null;
            Taskable.PokeTaskManager();
        }

        public object CheckTask(int checkType, object inputData)
        {
            switch (checkType)
            {
                case (int)TaskVertexManager.CheckTypes.Bool:
                    if (FuelGaugeCheck())
                    {
                        return true;
                    }
                    else
                    {
                        return fuelGauge.Full;
                    }
                default:
                    Debug.LogError($"Invalid check type passed into {GetType().Name}");
                    break;
            }

            return null;
        }
    }
}