using System;
using UnityEngine;

namespace RemoteEducation.Interactions
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    /// <summary>Disables all the lights in the scene while the camera this script is placed on is rendering.</summary>
    /// <remarks>Does not support lights that are added after this object is loaded as an optimization.</remarks>
    public partial class DisableLightsForCamera : MonoBehaviour
    {
        private Light[] lights;
        private bool[] lightStates;

        private void Awake()
        {
            GetAllLights();
        }

        private void GetAllLights()
        {
            lights = FindObjectsOfType<Light>(true);
            lightStates = new bool[lights.Length];
        }

        private void ToggleLights(bool on)
        {
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].enabled = on;
            }
        }

        private void OnPreCull()
        {
            GetLightStates();
            ToggleLights(false);
        }

        private void OnPostRender()
        {
            SetLightStates();
        }

        private void GetLightStates()
        {
            for (int i = 0; i < lights.Length; i++)
            {
                lightStates[i] = lights[i].enabled;
            }
        }

        private void SetLightStates()
        {
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].enabled = lightStates[i];
            }
        }
    }
}
