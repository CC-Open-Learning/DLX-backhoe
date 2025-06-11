/*
 * SUMMARY: This file contains the Wheel class. 
 *          The purpose of this file is to inspect the wheels for damage.
 */
using RemoteEducation.Interactions;
using RemoteEducation.Scenarios.Inspectable;
using UnityEngine;

namespace RemoteEducation.Modules.HeavyEquipment
{
    /// <summary>Class that defines behaviour of how to inspect the wheels for damage.</summary>
    public sealed class Wheel : MonoBehaviour, IBreakable
    {
        [SerializeField, Tooltip("The rim MeshFilter.")]
        private MeshFilter rimMeshFilter;
        // Created for UT purpose
        public MeshFilter RimMeshFilter => rimMeshFilter;

        [SerializeField, Tooltip("The tire MeshFilter.")]
        private MeshFilter tireMeshFilter;
        // Created for UT purpose
        public MeshFilter TireMeshFilter => tireMeshFilter;

        [Tooltip("The deflated tire mesh.")]
        [SerializeField] private Mesh deflatedTire;
        // Created for UT purpose
        public Mesh DeflatedTire => deflatedTire;

        [Tooltip("The damaged rim mesh.")]
        [SerializeField] private Mesh damagedRim;
        // Created for UT purpose
        public Mesh DamagedRim => damagedRim;

        [Tooltip("Relative position of this wheel on the Backhoe.")]
        [SerializeField] private Positions Position;

        [Tooltip("The nuts on this object")]
        [SerializeField] private WheelNuts WheelNuts;
        // Created for UT purpose
        public WheelNuts AllWheelNuts => WheelNuts;

        /// <summary>Wheel break modes.</summary>
        private enum WheelBreakMode
        {
            Deflated = 0,
            Loose = 1,
            //DamagedRim = 2
        }

        /// <summary>Allows the student to inspect the wheels.</summary>
        /// <param name="inspectableElement">The element being inspected.</param>
        /// <param name="broken">If the wheel is broken.</param>
        /// <param name="breakMode">Unused.</param>
        public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode)
        {
            if (broken)
            {
                switch (breakMode)
                {
                    case (int)WheelBreakMode.Deflated:
                        tireMeshFilter.sharedMesh = deflatedTire;
                        rimMeshFilter.sharedMesh = damagedRim;
                        WheelNuts.SetAllNutsTight();
                        break;

                    case (int)WheelBreakMode.Loose:
                        WheelNuts.SetLooseWheelNuts();
                        break;

                    /*case (int)WheelBreakMode.DamagedRim:
                        rimMeshFilter.sharedMesh = damagedRim;
                        WheelNuts.SetAllNutsTight();
                        break;*/
                }
            }
            else
            {
                WheelNuts.SetAllNutsTight();
            }

            WheelNuts.SetupToolTips();
            WheelNuts.SetupInteractable(inspectableElement);
        }
    }
}