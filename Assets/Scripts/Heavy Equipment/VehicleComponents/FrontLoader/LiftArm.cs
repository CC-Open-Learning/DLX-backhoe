using RemoteEducation.Interactions;
using RemoteEducation.Scenarios;
using RemoteEducation.Scenarios.Inspectable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemoteEducation.Modules.HeavyEquipment
{
    public class LiftArm : SemiSelectable, IBreakable
    {
        [SerializeField, Tooltip("The cracked arm decal object.")]
        private GameObject CrackedDecals;


        /// <summary> Setting Lift Arm Broken States and making it interactable </summary>
        /// <param name="broken">Used to tell if it's a broken state or not.</param>
        /// <param name="breakMode">For broken states. If 0 cracked, 1 rotted, and 2 deformed</param>
        public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode)
        {
            //Set the state if broken or not
            if (!broken)
            {
                return;
            }
                    
            CrackedDecals.SetActive(true);
        }
    }
}