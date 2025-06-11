using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemoteEducation.Modules.HeavyEquipment
{
    [RequireComponent(typeof(BackhoeLight))]
    [RequireComponent(typeof(Light))]
    [DisallowMultipleComponent]
    public class BlinkLight : MonoBehaviour
    {
        [SerializeField, Tooltip("Light Blink Delay")] private float delay;
        [SerializeField] private MeshRenderer litMesh;

        private BackhoeLight backhoeLight;
        private Light pointLight;
        private float timeSinceLastBlink;

        // Start is called before the first frame update
        void Start()
        {
            backhoeLight = GetComponent<BackhoeLight>();
            pointLight = GetComponent<Light>();
            timeSinceLastBlink = 0;
        }

        // Update is called once per frame
        void Update()
        {
            if (backhoeLight.Broken || !backhoeLight.TurnedOn) return;

            timeSinceLastBlink += Time.deltaTime;

            if (timeSinceLastBlink >= delay)
            {
                pointLight.enabled = !pointLight.enabled;
                litMesh.enabled = pointLight.enabled;
                timeSinceLastBlink = 0;
            }
        }
    }
}
