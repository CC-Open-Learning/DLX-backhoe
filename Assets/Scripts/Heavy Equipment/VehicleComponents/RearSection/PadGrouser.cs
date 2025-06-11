/*
 * SUMMARY: The purpose of this class is to model the behaviour and properties of the backhoe's stabilizer leg
 *          pad and grouser. They will either have a good state, or a bad state which has the pads missing/damaged.
 */



using RemoteEducation.Interactions;
using RemoteEducation.Scenarios.Inspectable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemoteEducation.Modules.HeavyEquipment
{

    public sealed class PadGrouser : MonoBehaviour, IBreakable
    {
        [SerializeField, Tooltip("The damaged pad/grouser mesh")]
        private Mesh damagedMesh;

        private MeshFilter meshFilter;

        public DynamicInspectableElement InspectableElement { get; private set; }

        public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode)
        {
            InspectableElement = inspectableElement;

            meshFilter = GetComponent<MeshFilter>();

            if (broken)
            {
                meshFilter.sharedMesh = damagedMesh;
            }
        }
    }
}