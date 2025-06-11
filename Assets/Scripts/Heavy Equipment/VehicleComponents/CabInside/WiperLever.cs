using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RemoteEducation.Interactions;
using RemoteEducation.Scenarios;

namespace RemoteEducation.Modules.HeavyEquipment
{
    /// <summary>
    /// The wiper lever class is used to animate the wiper lever and also control the signal lights on the dashboard
    /// as well as toggle the outer indicator lights
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public sealed class WiperLever : SemiSelectable, IInitializable
    {
        [SerializeField, Tooltip("Dashboard pointlight for the right signal light")] private Light leftSignalLight;
        [SerializeField, Tooltip("Dashboard pointlight for the left signal light")] private Light rightSignalLight;

        [SerializeField, Tooltip("The controller for the backhoe lights")] private BackhoeLightController backhoeLightController;


        private float blinkDelay = 0.5f;

        private Animator animator;

        void IInitializable.Initialize(object input)
        {
            animator = GetComponent<Animator>();
            leftSignalLight.enabled = false;
            rightSignalLight.enabled = false;

            OnSelect += MoveLever;
        }

        void MoveLever() {
            animator.SetTrigger("Move");

            StopAllCoroutines();
            leftSignalLight.enabled = false;
            rightSignalLight.enabled = false;
        }

        IEnumerator BlinkRightSignal()
        {
            while (true)
            {
                rightSignalLight.enabled = !rightSignalLight.enabled;
                yield return new WaitForSeconds(blinkDelay);
            }
        }

        IEnumerator BlinkLeftSignal()
        {
            while (true)
            {
                leftSignalLight.enabled = !leftSignalLight.enabled;
                yield return new WaitForSeconds(blinkDelay);
            }
        }
    }
}
