using RemoteEducation.Interactions;
using System;
using UnityEngine;

namespace RemoteEducation.Modules.HeavyEquipment
{
    [Serializable]
    public class WiperRotations
    {
        [SerializeField] public Transform transform;
        [SerializeField] public Transform extendedTransform;
        public Quaternion RetractedRotation { get; private set; }
        public Quaternion ExtendedRotation { get; private set; }

        /// <summary>Save the starting positions for these rotations.</summary>
        public void Init()
        {
            RetractedRotation = transform.localRotation;
            ExtendedRotation = extendedTransform.localRotation;
        }
    }
}