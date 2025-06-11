/*
 * SUMMARY: The purpose of this class is to model the behaviour and properties of the backhoe's engine belt. 
 * They will either have a good state, or a bad state which has the pads missing/damaged.
 */

using RemoteEducation.Interactions;
using RemoteEducation.Scenarios.Inspectable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class EngineBelt : MonoBehaviour, IBreakable
{
    [SerializeField, Tooltip("Damaged Engine Belt Mesh")]
    private Mesh DamagedBeltMesh;

    [SerializeField, Tooltip("Damaged Engine Belt Material")]
    Material DamagedEngineBeltMaterial;

    [SerializeField, Tooltip("Decals for the broken state")]
    GameObject DamageDecals;

    [SerializeField, Tooltip("Light For the engine belt")]
    Light pointLight;


    public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode)
    {

        if (broken)
        {
            this.gameObject.GetComponent<MeshRenderer>().material = DamagedEngineBeltMaterial;
            this.gameObject.GetComponent<MeshFilter>().sharedMesh = DamagedBeltMesh;
            DamageDecals.SetActive(true);
        }
    }

    public void EnableLight(bool enabled) { pointLight.enabled = enabled; }
}
