/*
 * SUMMARY: This file contains the BackhoeLightController class. 
 *          The purpose of this file is to handle light change requests and control how light works.
 */

using System.Collections;
using System.Collections.Generic;
using RemoteEducation.Interactions;
using RemoteEducation.Scenarios;
using UnityEngine;
using System;

namespace RemoteEducation.Modules.HeavyEquipment
{

    public class BackhoeLightController : SemiSelectable, IInitializable, ITaskable
    {
        public enum Light
        {
            Front = 1 << 0,
            Rear = 1 << 1,
            Top = 1 << 2,
            Indicator = 1 << 3,
            None = 0,
            All = 15
        }
        private Light lightsOn = Light.None;

        [SerializeField] private LightGroup frontRightLights;
        [SerializeField] private LightGroup frontLeftLights;
        [SerializeField] private LightGroup rearRightLights;
        [SerializeField] private LightGroup rearLeftLights;
        [SerializeField] private LightGroup topLight;
        [SerializeField] private List<IndicatorLight> indicatorLights;

        [SerializeField] private List<LightSwitch> lightSwitches;
        [SerializeField] private IndicatorLightSwitch indicatorLightSwitch;

        public TaskableObject Taskable { get; private set; }

        object ITaskable.CheckTask(int checkType, object inputData)
        {
            switch (checkType)
            {
                //Are all the lights on?
                case (int)TaskVertexManager.CheckTypes.Int:
                    if (lightsOn == Light.All)
                    {
                        return 1;
                    }
                    else if (lightsOn == Light.None)
                    {
                        return 2;
                    }

                    return 3;

                default:
                    Debug.LogError("Invalid check type passed into BackhoeLightController");
                    break;
            }

            return null;
        }

        public void EnableLightSwitchColliders(bool enable) {
            foreach (LightSwitch lS in lightSwitches) lS.GetComponent<MeshCollider>().enabled = enable;
            indicatorLightSwitch.GetComponent<MeshCollider>().enabled = enable;
        }

        void IInitializable.Initialize(object input)
        {
            Taskable = new TaskableObject(this);
        }

        public bool ToggleLights(Light light)
        {
            var shouldTurnOn = !IsFlagSet(light);

            TurnOnLights(light, shouldTurnOn);

            return shouldTurnOn;
        }

        private void TurnOnLights(Light light, bool on)
        {
            switch (light)
            {
                case Light.Front:
                    TurnOnFrontLights(on);
                    break;

                case Light.Rear:
                    TurnOnRearLights(on);
                    break;

                case Light.Top:
                    TurnOnTopLight(on);
                    break;

                case Light.Indicator:
                    TurnOnIndicators(on);
                    break;
            }

            SetLightFlag(light, on);

            Taskable.PokeTaskManager();
        }

        private void SetLightFlag(Light light, bool on)
        {
            if (on)
            {
                lightsOn |= light;
            }
            else
            {
                lightsOn &= ~light;
            }
            Debug.Log((int)lightsOn);
        }

        private bool IsFlagSet(Light light)
        {
            return (lightsOn & light) == light;
        }

        private void TurnOnFrontLights(bool on) 
        {
            frontRightLights.ToggleLights(on);
            frontLeftLights.ToggleLights(on);
        }

        private void TurnOnRearLights(bool on)
        {
            rearRightLights.ToggleLights(on);
            rearLeftLights.ToggleLights(on);
        }

        private void TurnOnTopLight(bool on)
        {
            topLight.ToggleLights(on);
        }

        private void TurnOnIndicators(bool on) 
        {
            foreach (IndicatorLight iL in indicatorLights)
            {
                iL.ToggleLight(on);
            }
        }
    }
}