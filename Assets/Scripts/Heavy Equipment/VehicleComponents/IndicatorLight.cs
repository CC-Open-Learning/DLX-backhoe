using RemoteEducation.Interactions;
using RemoteEducation.Scenarios.Inspectable;
using System.Collections;
using UnityEngine;

namespace RemoteEducation.Modules.HeavyEquipment
{
    /// <summary>Indicator Light Containing a box collider for the Inspectable Manager.</summary>
    [DisallowMultipleComponent]
    public sealed class IndicatorLight : SemiSelectable, IBreakable
    {
        [Tooltip("Delay in between blinks.")]
        [SerializeField] private float blinkDelay = 0.5f;

        private bool isBroken;

        private BackhoeLight pointLight;

        public DynamicInspectableElement InspectableElement { get; private set; }

        public bool LightOn { get; private set; }

        void Start() 
        {
            pointLight = GetComponentInChildren<BackhoeLight>();
        }

        void IBreakable.AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode)
        {
            InspectableElement = inspectableElement;
            isBroken = broken;
        }

        public void ToggleLight(bool on)
        {
            StopAllCoroutines();
            LightOn = on;

            if (LightOn && !isBroken) 
                StartCoroutine(Blink());
        }

        IEnumerator Blink()
        {
            while (true)
            {
                pointLight.ToggleObject(!pointLight.TurnedOn);
                
                yield return new WaitForSeconds(blinkDelay);
            }
        }
    }
}
