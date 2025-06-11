using RemoteEducation.Scenarios.Inspectable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyBreakable : MonoBehaviour, IBreakable
{
    public int CurrentBreakMode;
    public bool CurrentlyBroken;
    public DynamicInspectableElement CurrentElement;

    public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode)
    {
        CurrentBreakMode = breakMode;
        CurrentlyBroken = broken;
        CurrentElement = inspectableElement;
    }
}
