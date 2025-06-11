/*
 * SUMMARY: This file contains a generic backhoe light properties and functionality.
 *          The purpose is to act like a single entity and change upon requested.
 */
using RemoteEducation.Interactions;
using RemoteEducation.Scenarios.Inspectable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemoteEducation.Modules.HeavyEquipment
{
    /// <summary>
    ///     Class that defines what a light is
    /// </summary>
    /// 
    [DisallowMultipleComponent]
    public sealed class BackhoeLight : MonoBehaviour
    {

        private Light pointLight;
        private bool broken;
        private bool turnedOn;
        public bool Broken { get { return broken; } set { broken = value; } }
        public bool TurnedOn { get { return turnedOn; } }

        private void Start()
        {
            pointLight = GetComponent<Light>();
            ToggleObject(false);
        }
        public void ToggleObject(bool lightOn)
        {
            if (broken) return; 
            
            pointLight.enabled = lightOn;
            turnedOn = lightOn;
        }
    }
}