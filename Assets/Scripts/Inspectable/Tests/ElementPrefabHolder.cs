using RemoteEducation.Scenarios.Inspectable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementPrefabHolder : MonoBehaviour
{
    public InspectableElement LightsGood;
    public InspectableElement LightsMissing;
    public InspectableElement LightsBurntOut;

    public InspectableElement WheelsGood;
    public InspectableElement WheelsFlat;
    public InspectableElement FakeElementWheelsMissing;
}
