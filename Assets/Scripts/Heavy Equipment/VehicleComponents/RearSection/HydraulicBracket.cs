/*
 *  SUMMARY:            The purpose of this class is to model the behaviour of inspecting a 
 *                      bracket for hydraulic lines on the backhoe. This includes a jiggle 
 *                      animation when the bracket is loose, and the 4 states a bracket can 
 *                      be in, (Good, Missing, Loose, and Damaged)
 */



using RemoteEducation.Interactions;
using RemoteEducation.Modules.HeavyEquipment;
using RemoteEducation.Scenarios.Inspectable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>This class defines the basic functionality of the hydraulic line brackets found around the backhoe.</summary>
/// <remarks>Brackets can be in 1 of 4 states, all good, damaged, loose, or missing. If the bracket is loose, then it will 
/// use the good mesh and play an animation OnSelect.</remarks>
public sealed class HydraulicBracket : SemiSelectable, IBreakable
{
    [SerializeField, Tooltip("The damaged hydraulic bracket mesh")]
    private Mesh BadMesh;

    [SerializeField, Tooltip("The damaged hydraulic bracket physics model for colliders.")]
    private Mesh BadPhys;

    [SerializeField, Tooltip("Rotation When loose")]
    private Vector3 looseRotation;

    /// <summary>Matches the values of the IE (Inspectable Element) Hydraulic Line Bracket prefabs for breakmode</summary>
    private enum BreakMode
    {
        Damaged = 0,
        Loose = 1,
        Missing = 2
    }

    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode)
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        inspectableElement.OnCurrentlyInspectableStateChanged += (active) =>
        {
            ToggleFlags(!active, Flags.Highlightable | Flags.ExclusiveInteraction);
        };

        SetBracketState(broken, breakMode);

    }

    /// <summary>Setter for the brackets state</summary>
    /// <remarks>If broken is passed in we show the broken bracket, else we show the good bracket. Regardless of state, bracket can be loose</remarks>
    /// <param name="broken">Used to tell if the broken bracket mesh needs to be active or the good bracket mesh.</param>
    public void SetBracketState(bool broken, int breakMode)
    {
        if (!broken)
        {
            return;
        }

        //Set extra properties/change game object states if bracket is loose, or missing
        switch ((BreakMode)breakMode)
        {
            //breakMode == 2
            case BreakMode.Missing:
                //Disable brackets and box collider
                gameObject.SetActive(false);
                break;

            //breakMode == 1
            case BreakMode.Loose:
                //Use good mesh for loose bracket
                transform.Rotate(looseRotation);
                break;

            //breakMode == 0
            case BreakMode.Damaged:
                meshFilter.sharedMesh = BadMesh;
                meshCollider.sharedMesh = BadPhys;
                break;
        }
    }
}