using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Gui;
using TMPro;
using RemoteEducation.Helpers.Unity;

/// <summary>
/// The class is used to pass UI button clicks to the <see cref="GraphController"/>s.
/// GraphControllers can add them selves to the static list of tracked graph controllers. 
/// This class should be added as a component to any UI button that will want its clicks 
/// sent to the graphs. The string in the <see cref="ButtonIdentifier"/> will be used by the Buttons as 
/// well as the <see cref="GraphEdge"/> to tell which button should activate with edge.
/// 
/// This class is only set up to work with <see cref="LeanButton"/>s, but it can be expanded
/// very easily.
/// </summary>
/// 

[DisallowMultipleComponent]
public class ButtonGraphInterface : MonoBehaviour
{
    public const string IdentifierPrefix = "Button : ";

    #region Static

    /// <summary>
    /// All of the <see cref="GraphController"/>s that are being tracked /// </summary>
    private static List<GraphController> GraphControllers;

    private static Dictionary<string, LeanButton> ToggleableButtons;
    private static Dictionary<string, LeanToggle> DeactivatableButtons; 

    static ButtonGraphInterface()
    {
        Initialize();

        FadeSceneLoader.OnLoadScene += FadeSceneLoader_OnLoadScene;
    }

    private static void Initialize()
    {
        GraphControllers = new List<GraphController>();
        ToggleableButtons = new Dictionary<string, LeanButton>();
        DeactivatableButtons = new Dictionary<string, LeanToggle>();
    }

    private static void FadeSceneLoader_OnLoadScene(string sceneName)
    {
        Initialize();
    }

    public static void ToggleButtonVisibility(string identifier, bool visible)
    {
        if(ToggleableButtons.ContainsKey(identifier))
        {
            ToggleableButtons[identifier].gameObject.SetActive(visible);
        }
        else
        {
            Debug.LogError("ButtonGraphInterface : Unable to find the specified button");
        }
    }

    /// <summary>
    ///     Waits <paramref name="delay"/> seconds before calling <see cref="ToggleButtonVisibility(string, bool)"/>
    ///     with the provided <paramref name="identifier"/> and <paramref name="visible"/> parameters
    /// </summary>
    /// <param name="identifier">The unique string used to identify the button in the <see cref="ButtonGraphInterface"/></param>
    /// <param name="visible">Indicates whether the button should be made visible</param>
    /// <param name="delay">Number of seconds to wait before toggling the visibility of the button</param>
    /// <returns>IEnumerator as required by a Unity <see cref="Coroutine"/></returns>
    public static IEnumerator ToggleButtonVisibilityDelayed(string identifier, bool visible, float delay)
    {
        yield return new WaitForSeconds(delay);
        ToggleButtonVisibility(identifier, visible);
        yield return null;
    }

    public static void DeactivateButton(string idetifier, bool deactivated)
    {
        if(DeactivatableButtons.ContainsKey(idetifier))
        {
            DeactivatableButtons[idetifier].On = !deactivated;
        }
        else
        {
            Debug.LogError("ButtonGraphInterface : Unable to find the specified button");
        }
    }

    public static bool SetButtonText(string identifier, string text)
    {
        if (ToggleableButtons.ContainsKey(identifier))
        {
            TextMeshProUGUI textFeild = ToggleableButtons[identifier].gameObject.GetComponentInChildren<TextMeshProUGUI>();

            if(textFeild != null)
            {
                textFeild.text = text;
                return true;
            }
        }

        Debug.LogError("ButtonGraphInterface : Unable to set the text on the specified button");
        return false;
    }

    public static void SetButtonText(string identifier, string text, bool visable)
    {
        //if the text was changed, then toggle the viability
        if(SetButtonText(identifier, text))
        {
            ToggleButtonVisibility(identifier, visable);
        }
    }

    public static LeanButton GetButton(string identifier)
    {
        if (ToggleableButtons.ContainsKey(identifier))
        {
            return ToggleableButtons[identifier];
        }
        else
        {
            Debug.LogError("ButtonGraphInterface : Unable to find the specified button");
        }

        return null;
    }

    /// <summary>
    /// Add a new graph controller to the list of graph controllers.
    /// This means that the graph controller will be "poked" when ever a
    /// UI button is clicked.
    /// </summary>
    /// <param name="controller"></param>
    public static void AddGraphController(GraphController controller)
    {
        GraphControllers.Add(controller);
    }

    #endregion

    [SerializeField, Tooltip("The identifier for this Button")]
    private string ButtonIdentifier;

    [Tooltip("This button can be toggled visible through static ButtonGraphInterface function calls")]
    public bool CanBeHidden = true;

    [Tooltip("This button can be enabled and disabled through static ButtonGraphInterface calls. Requires a LeanToggle component." +
        "A disabled button is still visible but has no response on hover or click.")]
    [UnityEngine.Serialization.FormerlySerializedAs("CanBeDeavtivated")]
    public bool CanBeDeactivated;

    [Tooltip("The button must be set active on Awake to add itself to the list. This will deactivate it right away.")]
    public bool StartHidden;

    public LeanToggle LeanToggle { get; private set; }


    private void Awake()
    {
        LeanButton button = GetComponent<LeanButton>();

        if (button)
        {
            //set up the listener for the button click
            button.OnClick.AddListener(OnButtonClick);

            //add it to the list of toggleable buttons
            if (CanBeHidden)
            {
                if (ToggleableButtons.ContainsKey(ButtonIdentifier))
                {
                    ToggleableButtons.Remove(ButtonIdentifier);
                }

                ToggleableButtons.Add(ButtonIdentifier, button);
            }

            if(CanBeDeactivated)
            {
                if(DeactivatableButtons.ContainsKey(ButtonIdentifier))
                {
                    DeactivatableButtons.Remove(ButtonIdentifier);
                }

                DeactivatableButtons.Add(ButtonIdentifier, GetComponent<LeanToggle>());
            }

            if (StartHidden)
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning(gameObject.name + " : No LeanButton found");
        }
    }

    /// <summary>
    /// Tell all the tracked graph controllers that this button was clicked.
    /// </summary>
    public void OnButtonClick()
    {
        foreach(GraphController controller in GraphControllers)
        {
            controller.PokeCurrentVertex(IdentifierPrefix + ButtonIdentifier);
        }
    }
}
