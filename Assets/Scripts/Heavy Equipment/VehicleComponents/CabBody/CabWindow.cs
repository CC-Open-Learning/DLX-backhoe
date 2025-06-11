/*
 * SUMMARY: This file contains the CabWindow class. The purpose this class is to open and close the cab windows
 *          through an animation done with coroutines. Apply this to any window that needs to be animated. 
 */

using System.Collections;
using System.Collections.Generic;
using RemoteEducation.Interactions;
using RemoteEducation.Localization;
using RemoteEducation.Scenarios.Inspectable;
using RemoteEducation.UI;
using UnityEngine;

namespace RemoteEducation.Modules.HeavyEquipment
{
    /// <summary>This class defines the basic functionality of the cab window. </summary>
    /// <remarks>Some cab windows can be open and closed, which is done through animations.
    /// This class defines those animations as well as the different states the window can
    /// have.</remarks>
    public sealed class CabWindow : SemiSelectable, IBreakable, IInitializable
    {
        public enum WindowPositions
        {
            Front,
            RightDoorTop,
            RightDoorBottom,
            RightBack,
            LeftDoorTop,
            LeftDoorBottom,
            LeftBack,
            BackUpper,
            BackLower
        }

        public enum WindowStates
        {
            Cracked,
            Dirty
        }

        [Tooltip("Dirty window mesh")]
        [SerializeField] private Mesh DirtyMesh;

        [Tooltip("Cracked window mesh")]
        [SerializeField] private Mesh CrackedMesh;

        [Tooltip("Clean window mesh")]
        [SerializeField] private Mesh CleanMesh;

        [Tooltip("Material for cracked window")]
        [SerializeField] private Material CrackedWindowMaterial;

        [Tooltip("Material for dirty window")]
        [SerializeField] private Material DirtyWindowMaterial;

        [Tooltip("Material for a clean window")]
        [SerializeField] private Material CleanWindowMaterial;

        [SerializeField] private WindowPositions Position;

        private Renderer materialRenderer;
        private MeshFilter meshFilter;

        public WindowStates WState;

        public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode)
        {
            meshFilter = GetComponent<MeshFilter>();
            materialRenderer = GetComponent<Renderer>();

            CleanMesh = meshFilter.sharedMesh;
            CleanWindowMaterial = materialRenderer.sharedMaterial;

            if(broken)
                WState = (WindowStates)breakMode;

            SetWindowState(broken, breakMode);

            inspectableElement.OnCurrentlyInspectableStateChanged += (active) =>
            {
                //When interactable, don't show the highlight for this script. Also
                //make it so that it can be interacted with at any time.
                ToggleFlags(!active, Flags.Highlightable | Flags.ExclusiveInteraction);
            };
        }

        public void Initialize(object input = null)
        {
            RemoveFlags(Flags.Highlightable);
            ToggleFlags(false, Flags.InteractionsDisabled);
        }

        public void SetClean()
        {
            meshFilter.sharedMesh = CleanMesh;
            materialRenderer.sharedMaterial = CleanWindowMaterial;
        }

        public void RecallDirty()
        {
            if(WState == WindowStates.Dirty)
                HEPrompts.CreateToast(Localizer.Localize("HeavyEquipment.WindowsRecallDirtyToast"), HEPrompts.LongToastDuration);
        }

        /// <summary>Changes window mesh depending on state.</summary>
        private void SetWindowState(bool broken, int breakMode)
        {
            if (!broken)
            {
                return;
            }

            switch ((WindowStates)breakMode)
            {
                case WindowStates.Cracked:
                    meshFilter.sharedMesh = CrackedMesh;
                    materialRenderer.sharedMaterial = CrackedWindowMaterial;
                    break;

                case WindowStates.Dirty:
                    meshFilter.sharedMesh = DirtyMesh;
                    materialRenderer.sharedMaterial = DirtyWindowMaterial;
                    break;
            }

            materialRenderer.sharedMaterial.SetOverrideTag("Glowable", "True");
        }

        /// <summary>Displays toast of the window being cleaned after being dirty.</summary>
        public void WindowsCleanedToast()
        {
            HEPrompts.CreateToast(Localizer.Localize("HeavyEquipment.WindowsCleanToast"), HEPrompts.LongToastDuration);
        }
    }
}