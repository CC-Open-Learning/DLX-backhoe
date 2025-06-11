using Lean.Transition.Method;
using RemoteEducation.Scenarios.Inspectable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the changes to the animations for an <see cref="InspectionPanel"/>.
/// </summary>
/// <remarks>
/// Right now the animator can only handle up to 4 buttons in the States Panel.
/// This means if we have an <see cref="InspectableElement"/> that can have more than 4 states, we 
/// will need to update this class. Setting it up dynamically so that it can handle any number of
/// states is in theory possible. These kinds of run time changes to UI animations has always been
/// very difficult to do. So right now it is just hard coded.
/// To make this class handle more button states you would:
/// 1. Add a new element to the <see cref="PanelSizes"/> enum for 6 buttons.
/// 2. In the scene, manually size the window and states transforms to fit more buttons.
/// 3. Take those heights and add them to the <see cref="WindowHeightValues"/> and <see cref="ButtonStatesHeightValues"/> arrays.
/// 4. Update the <see cref="SetAnimationHeights"/> methods to handle more buttons.
/// </remarks>
public class InspectionPanelAnimator : MonoBehaviour
{
    [SerializeField, Tooltip("The open animation for the size of the whole window")]
    private LeanRectTransformSizeDelta WindowOpen;
    [SerializeField, Tooltip("The open animation for the size of the state button panel")]
    private LeanRectTransformSizeDelta StatesOpen;
    [SerializeField, Tooltip("The close animation for the size of the whole window")]
    private LeanRectTransformSizeDelta WindowClose;
    [SerializeField, Tooltip("The close animation for the size of the state button panel")]
    private LeanRectTransformSizeDelta StatesClose;

    [SerializeField, Tooltip("The transform for the whole window")]
    private RectTransform windowTransform;
    [SerializeField, Tooltip("The transform for the state button panel")]
    private RectTransform statesTransform;
    [SerializeField, Tooltip("The close button for the window")]
    private GameObject closeButton;

    /// <summary>
    /// The indexes for the <see cref="WindowHeightValues"/> and <see cref="ButtonStatesHeightValues"/> arrays.</summary>
    public enum PanelSizes
    {
        Closed = 0,
        Open2Buttons = 1,
        Open4Buttons = 2
    }

    /// <summary>
    /// The heights for the window when the panel is in different states.</summary>
    public float[] WindowHeightValues = new float[] { 125f, 211f, 270f };

    /// <summary>
    /// The heights for the states window when the panel is in different states.</summary>
    public float[] ButtonStatesHeightValues = new float[] { 66f, 152f, 210f };

    /// <summary>The original width of the window.</summary>
    private float windowWidth;
    /// <summary>
    /// The original width of the states window.</summary>
    private float statesWidth;

    private void Awake()
    {
        windowWidth = WindowClose.Data.SizeDelta.x;
        statesWidth = StatesClose.Data.SizeDelta.x;
    }

    /// <summary>
    /// Close the window and hide the close button.
    /// This will setup the window to shown as a tool tip.
    /// </summary>
    public void InstantClose()
    {
        windowTransform.sizeDelta = new Vector2(windowWidth, WindowClose.Data.SizeDelta.y);
        statesTransform.sizeDelta = new Vector2(statesWidth, StatesClose.Data.SizeDelta.y);

        closeButton.SetActive(false);
    }

    /// <summary>
    /// Set the height for the open animations.
    /// </summary>
    /// <param name="visibleStateButtons">How many buttons are used by the context</param>
    public void SetAnimationHeights(int visibleStateButtons)
    {
        SetAnimationHeights(visibleStateButtons < 3 ? PanelSizes.Open2Buttons : PanelSizes.Open4Buttons);
    }

    /// <summary>
    /// Set the height for the open animations.
    /// </summary>
    /// <param name="panelSize">The size for the panel.</param>
    public void SetAnimationHeights(PanelSizes panelSize)
    {
        WindowOpen.Data.SizeDelta = new Vector2(windowWidth, WindowHeightValues[(int)panelSize]);
        StatesOpen.Data.SizeDelta = new Vector2(statesWidth, ButtonStatesHeightValues[(int)panelSize]);
    }
}
