using RemoteEducation.Interactions;
using RemoteEducation.Scenarios.Inspectable;
using System.Collections.Generic;
using UnityEngine;

namespace RemoteEducation.Modules.HeavyEquipment
{ 
    [DisallowMultipleComponent]
    public sealed class LightGroup : SemiSelectable, IBreakable
    {
        /// <summary>All lights that are part of this lightgroup.</summary>
        private List<BackhoeLight> lights = new List<BackhoeLight>();

        public DynamicInspectableElement InspectableElement { get; private set; }

        public bool LightOn { get; private set; }

        void Start()
        {
            foreach (Transform child in this.transform) 
            {
                if (child.GetComponent<BackhoeLight>()) 
                    lights.Add(child.GetComponent<BackhoeLight>());
            }
        }

        public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode)
        {
            InspectableElement = inspectableElement;

            if (broken)
                lights[Random.Range(0, lights.Count)].Broken = true;
        }

        public void ToggleLights(bool on) 
        {
            LightOn = on;

            foreach (BackhoeLight l in lights)
            {
                l.ToggleObject(on);
            }
        }
    }
}
