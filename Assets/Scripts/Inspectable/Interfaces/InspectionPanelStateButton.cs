using Lean.Gui;
using Lean.Transition.Method;
using RemoteEducation.Scenarios.Inspectable;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the buttons that are used in <see cref="InspectionPanel"/>s.
/// </summary>
public class InspectionPanelStateButton : MonoBehaviour
{
    public enum ButtonColours
    {
        Red = 0,
        Green = 1,
        Yellow = 2
    }

    [SerializeField]
    private Color Red;

    [SerializeField]
    private Color Green;

    [SerializeField]
    private Color Yellow;

    [SerializeField]
    private Image innerCap;

    [SerializeField]
    private Image cap;

    [SerializeField]
    private TextMeshProUGUI buttonText;

    [SerializeField]
    private LeanGraphicColor enterTransition;
    
    [SerializeField]
    private LeanGraphicColor exitTransition;

    private float innerCapAlpha;
    private const float innerCapBrightness = 0.5f;
    private float enterTransitionColourAlpha;

    public LeanButton LeanButton;

    private void Awake()
    {
        //Get the default values that Lean uses for colours alpha values
        innerCapAlpha = innerCap.color.a;
        enterTransitionColourAlpha = enterTransition.Data.Color.a;
    }

    /// <summary>
    /// Set the colour and text on the button. The transition colours are also updated.
    /// </summary>
    /// <param name="buttonText">The text on the button.</param>
    /// <param name="colour">The colour the button should be.</param>
    public void SetValues(string buttonText, ButtonColours colour)
    {
        Color outerColour = (new Color[] { Red, Green, Yellow })[(int)colour];
        
        Color innercolour = new Color(outerColour.r + innerCapBrightness, outerColour.g + innerCapBrightness, outerColour.g + innerCapBrightness, innerCapAlpha);

        cap.color = outerColour;

        innerCap.color = innercolour;

        exitTransition.Data.Color = innercolour;

        enterTransition.Data.Color = innercolour;
        enterTransition.Data.Color.a = enterTransitionColourAlpha;

        this.buttonText.text = buttonText;
    }
}
