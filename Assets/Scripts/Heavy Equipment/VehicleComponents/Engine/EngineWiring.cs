/*
 * SUMMARY: The purpose of this class is to model the behaviour and properties of the wiring in the engine compartment.
            They will either have a good state, or a bad state which has some exposed wiring.
 */



using RemoteEducation.Interactions;
using RemoteEducation.Scenarios.Inspectable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public sealed class EngineWiring : MonoBehaviour, IBreakable
{
    [SerializeField, Tooltip("The exposed wiring mesh")]
    private Mesh damagedMesh;

    [SerializeField, Tooltip("The exposed wiring material")]
    private Material damagedMat;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    public DynamicInspectableElement InspectableElement { get; private set; }

    public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode)
    {
        InspectableElement = inspectableElement;

        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        if (broken)
        {
            meshRenderer.sharedMaterial = damagedMat;
            meshFilter.sharedMesh = damagedMesh;
        }
    }
}