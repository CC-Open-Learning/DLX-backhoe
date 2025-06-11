using RemoteEducation.Interactions;
using RemoteEducation.Scenarios.Inspectable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemoteEducation.Modules.HeavyEquipment
{

    public class HydraulicFluidReservoir : SemiSelectable, IBreakable
    {
        public DynamicInspectableElement InspectableElement { get; private set; }

        public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode)
        {
            InspectableElement = inspectableElement;
        }

        public Gauge GetFuelGauge() {
            return GetComponentInChildren<Gauge>();
        }

    }

}
