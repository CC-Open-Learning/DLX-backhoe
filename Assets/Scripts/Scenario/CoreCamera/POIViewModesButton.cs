/*
 *  FILE          :	POIViewModesButton.cs
 *  PROJECT       :	VARLab CORE
 *  PROGRAMMER    :	Duane Cressman
 *  FIRST VERSION :	2020-12-2
 *  RELATED FILES : PointOfInterest.cs
 *  DESCRIPTION   : This file contains the POIViewModesButton class.
 *                  This class is used to define the properties of a button
 *                  in the view modes panel. It will also give it's corresponding 
 *                  POI a reference to the CoreCameraController in the scene.
 */

using Lean.Gui;
using RemoteEducation.Interactions;
using RemoteEducation.Scenarios;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LeanButton))]
[RequireComponent(typeof(LeanToggle))]
public class POIViewModesButton : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI ButtonText;

    private LeanButton Button;

    private LeanToggle SelectionToggle;

    public PointOfInterest POI { get; private set; }

    public void Initialize(CoreCameraController cameraController, PointOfInterest poi, string buttonText = null)
    {
        SelectionToggle = GetComponent<LeanToggle>();

        POI = poi;

        Button = GetComponent<LeanButton>();
        ButtonText.text = poi == null ? buttonText : poi.POIName;
        Button.OnClick.AddListener(() => cameraController.SendCameraToPosition(poi));
    }

    public void SetSelected(bool selected)
    {
        SelectionToggle.On = selected;
    }
}
