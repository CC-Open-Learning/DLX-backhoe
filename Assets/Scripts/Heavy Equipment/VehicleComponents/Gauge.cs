/*
 * SUMMARY: This file contains the Gauge class. The purpose of this class is to keep track
 *          of the fluid levels for gauges and to make sure the gauge has been checked before being able to
 *          fill the tank.
 */

using System.Collections;
using System.Collections.Generic;
using RemoteEducation.Interactions;
using RemoteEducation.Scenarios.Inspectable;
using RemoteEducation.Localization;
using UnityEngine;
using RemoteEducation.UI;
using RemoteEducation.UI.Tooltip;
using RemoteEducation.Scenarios;

namespace RemoteEducation.Modules.HeavyEquipment
{
    public class Gauge : SemiSelectable, IInitializable, ITaskable
    {
        /// <summary>True if the gauge has been pressed </summary>
        public bool GaugeChecked { get; private set; } = false;

        /// <summary>Checks if tank is full or not</summary>
        public bool Full { get; private set; }

        [SerializeField, Tooltip("Starting Level of the Gauge")] private bool startsFull;

        [SerializeField, Tooltip("Point of rotation for gauge pin")]
        private Transform GaugeAnchor;

        [SerializeField, Tooltip("Pin rotation when full/adequate")]
        private Transform FullRotation;

        [SerializeField, Tooltip("Pin rotation when low")]
        private Transform LowRotation;

        [SerializeField, Tooltip("Tooltip text link")]
        private string ToolTipText;

        [SerializeField, Tooltip("The location of this gauge in the backhoe")]
        Locations location;

        public TaskableObject Taskable { get; private set; }

        /// <summary>Whether the student has clicked on this gauge.</summary>
        private bool clicked = false;

        public enum Locations
        { 
            HydraulicFluidReservoir,
            GaugePanel
        }

        public enum Temperature
        {
            Hot,
            Cold
        }

        private Temperature fluidTemp = Temperature.Cold;

        public void Initialize(object input = null)
        {
            Full = startsFull;

            if (GetComponent<SimpleTooltip>() == null)
            {
                var tooltip = gameObject.AddComponent<SimpleTooltip>();
                tooltip.tooltipText = Localizer.Localize(ToolTipText);
            }

            if (Full)
            {
                GaugeAnchor.transform.localRotation = FullRotation.localRotation;
            }
            else
            {
                GaugeAnchor.transform.localRotation = LowRotation.localRotation;
            }

            Taskable = new TaskableObject(this, location.ToString());

            OnSelect += CheckedFuelGauge;
        }

        /// <summary>The fuel gauge has been checked</summary>
        private void CheckedFuelGauge()
        {
            GaugeChecked = true;
            clicked = true;

            Taskable.PokeTaskManager();
        }

        /// <summary>Updates fuel gauge pin to match fuel state</summary>
        public void SetLevel(bool full)
        {
            Full = full;

            if (full)
            {
                GaugeAnchor.transform.localRotation = FullRotation.localRotation;
            }
        }

        /// <summary>Because the fuel tank has no mechanics/scripts we can only attach the temp on the gauge</summary>
        public Temperature GetTemperature()
        {
            return fluidTemp;
        }


        public object CheckTask(int checkType, object inputData)
        {
            switch (checkType)
            {
                case (int)TaskVertexManager.CheckTypes.Bool:
                    return clicked;
                default:
                    Debug.LogError("Invalid check type passed into FrontHood");
                    break;
            }

            return null;
        }
    }
}
