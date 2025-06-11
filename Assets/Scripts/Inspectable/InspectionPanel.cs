using UnityEngine;
using Lean.Gui;
using TMPro;
using System.Collections.Generic;
using ButtonColours = InspectionPanelStateButton.ButtonColours;
using System.Linq;
using Lean.Transition.Method;
using System.Collections;
using System;
using RemoteEducation.Interactions;
using RemoteEducation.Localization;
using UnityEngine.UI;

namespace RemoteEducation.Scenarios.Inspectable
{

    /// <summary>
    ///     Controller for the UI element which displays information and receives
    ///     user input for a specific <see cref="InspectableElement"/>
    /// </summary>
    [RequireComponent(typeof(InspectionPanelAnimator))]
    [RequireComponent(typeof(CanvasGroup))]
    [DisallowMultipleComponent]
    public class InspectionPanel : MonoBehaviour
    {
        /// <summary>String literal for an InspectableElement that has not yet been inspected</summary>
        private static string NO_STATE_RESPONSE => "Engine.None".Localize();

        /// <summary>The <see cref="InspectableElement"/> which provides information and receives input from this InspectionPanel </summary>
        public InspectableElement context { get; protected set; } = null;

        /// <summary>The states the element can be in that each button corresponds to.</summary>
        private Dictionary<InspectionPanelStateButton, int> stateIdentifersByButtons;

        /// <summary>The animator that controls the changes in the size of the panel.</summary>
        private InspectionPanelAnimator panelAnimator;

        /// <summary>The controller for showing the status icons.</summary>
        private StatusIconController statusIconController;

        /// <summary>Coroutine for delaying the panels from being shown.</summary>
        private Coroutine DelayedShowPanelCoroutine;

        [Header("Window Components")]

        [Tooltip("LeanWindow object which contains element controls")]
        public LeanWindow ControlWindow;

        [Tooltip("LeanToggle for the ControlWindow which defines transitions between contracted and expanded states")]
        public LeanToggle ExpansionToggle;

        [Tooltip("Offset from mouse or raycast position, relative to the pivot point of the Transform component")]
        [SerializeField] protected Vector3 panelOffset = new Vector3(0, 30);

        [Tooltip("Help Button to Create Extra Details")]
        public Transform HelpButton;

        [Header("Content")]

        [Tooltip("Text area which will display the name of the element being controlled")]
        public TextMeshProUGUI TitleField;

        [Tooltip("The button that shows the selected state")]
        public InspectionPanelStateButton SelectedStateButton;

        [Tooltip("The buttons for the possible states for the element. The order of this list MUST match the order of how they appear in the panel")]
        public List<InspectionPanelStateButton> PossibleStateButtons;

        [Header("ToolTip Options")]

        [Tooltip("How long does the user need to hover over the object before the tool tip is shown.")]
        public float DelayShowingTime = 0.75f;

        /// <summary>Buffer used to store the screen-space bounds of the <see cref="Collider"/> attached to the <see cref="context"/>.</summary>
        /// <remarks>Re-using this buffer instead of creating a new one prevents a heap allocation every frame.</remarks>
        private Vector3[] screenSpaceBounds = new Vector3[4];

        /// <summary>Buffer used to store the screen-space coordinates of each corner of the <see cref="Collider"/> attached to the <see cref="context"/>.</summary>
        /// <remarks>Re-using this buffer instead of creating a new one prevents a heap allocation every frame.</remarks>
        private Vector3[] worldCornerBuffer = new Vector3[8];

        /// <summary>Reference to the current <see cref="Camera"/> being used for raycasts.</summary>
        /// <remarks>This is used in place of <see cref="Camera.main"/> as that is an extremely slow property call.</remarks>
        private Camera cam;

        /// <summary><see cref="CanvasGroup"/> of the <see cref="InspectionPanel"/>.</summary>
        /// <remarks>Used to set whether this panel should block raycasts or not.</remarks>
        private CanvasGroup canvasGroup;

        /// <summary><see cref="CanvasScaler"/> of the parent <see cref="Canvas"/> containing this <see cref="InspectionPanel"/>.</summary>
        /// <remarks>Used to determine the reference resolution of the parent <see cref="Canvas"/>.</remarks>
        private CanvasScaler canvasScaler;

        /// <summary><see cref="RectTransform"/> of the <see cref="InspectionPanel"/>.</summary>
        /// <remarks>Used to get the current size of this <see cref="InspectionPanel"/>.</remarks>
        private RectTransform rect;

        public void Awake()
        {
            if (!ControlWindow)
            {
                Debug.LogError("InspectionPanel : No LeanWindow component assigned for the control panel");
                return;
            }

            //set up the button handlers
            SelectedStateButton.LeanButton.OnClick.AddListener(() => HandleStateButtonClick(SelectedStateButton));
            foreach (InspectionPanelStateButton button in PossibleStateButtons)
            {
                button.LeanButton.OnClick.AddListener(() => HandleStateButtonClick(button));
            }

            panelAnimator = GetComponent<InspectionPanelAnimator>();
            statusIconController = GetComponentInChildren<StatusIconController>();

            ControlWindow.OnOff.AddListener(UnselectContext);

            cam = Camera.main;

            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = false;

            rect = GetComponent<RectTransform>();
        }

        private void Start()
        {
            canvasScaler = GetComponentInParent<CanvasScaler>();
        }

        public void Update()
        {
            // Update the position of the InspectionPanel each game frame so that it
            // moves relative to the user's cursor and camera view
            UpdatePosition();
        }

        /// <summary>
        ///     Displays the Inspection panel in informational tooltip mode when
        ///     a new <see cref="InspectableElement"/> is provided as context for the
        ///     InspectionPanel
        /// </summary>
        /// <param name="element">
        ///     The <see cref="InspectableElement"/> which will be controlled by
        ///     the <see cref="InspectionPanel"/>
        /// </param>
        public void ShowPanel(InspectableElement element)
        {
            context = element;

            // Refresh the content of the panel when a new context is provided
            UpdateContent();

            // When first showing the InspectionPanel, the InspectableElement is unselected, 
            // so the contracted "tooltip" version of the control panel should be shown
            panelAnimator.InstantClose();

            //start the coroutine for delaying when the panel is shown
            DelayedShowPanelCoroutine = StartCoroutine(DelayShowingPanel());
        }

        /// <summary>
        ///     Wrapper method for turning on <see cref="ExpansionToggle"/>
        /// </summary>
        public void ExpandPanel()
        {
            if (!ExpansionToggle)
            {
                Debug.LogError("InspectionPanel : No LeanToggle component assigned for control panel state change");
                return;
            }

            canvasGroup.blocksRaycasts = true;

            //if the coroutine for delaying showing the panel is still running, 
            // stop it and show the panel
            if (DelayedShowPanelCoroutine != null)
            {
                StopCoroutine(DelayedShowPanelCoroutine);
                DelayedShowPanelCoroutine = null;

                ControlWindow.TurnOn();
            }

            ExpansionToggle.TurnOn();
        }

        /// <summary>
        /// Delay showing the panel for <see cref="DelayShowingTime"/> seconds.
        /// </summary>
        private IEnumerator DelayShowingPanel()
        {
            yield return new WaitForSeconds(DelayShowingTime);

            if (context != null && context.HasFlags(Interactions.Interactable.Flags.MouseOver))
            {
                ControlWindow.TurnOn();
            }

            DelayedShowPanelCoroutine = null;
        }

        /// <summary>
        ///     Sets the position of the InspectionPanel relative to either the current 
        ///     mouse position or a fixed 3D-space position, depending on the selected
        ///     state of <see cref="context"/>
        /// </summary>
        public void UpdatePosition()
        {
            if (context && cam)
            {
                // The operation below uses the ternary operator to set the position of the InspectionPanel
                // relative to either the mouse position or a fixed 3D-space position, depending on the selected
                // state of the 'context' InspectableElement. The same 'panelOffset' is used in both cases

                var offset = panelOffset;

                if (context.TryGetComponent(out Collider collider))
                {
                    GetObjectBounds(collider.bounds);
                    offset = screenSpaceBounds[0];
                }

                // Get the offset to generate the position of panel window
                var pos =  context.IsSelected
                        // if inspection controls mode, position should stay relative to the inspectable, even if camera is moved
                        ? offset
                        // if tooltip mode, position should follow mouse cursor as it moves over the InspectableElement
                        : Input.mousePosition;

                // Calculate the ratio between our current resolution and the reference resolution of the parent Canvas.
                var res = canvasScaler.referenceResolution;
                var xRatio = Screen.width / res.x;
                var yRatio = Screen.height / res.y;

                // If pos is above the halfway mark on our screen, we want to flip the pivot.
                var reverse = pos.y > Screen.height / 2f;

                // We also need to add on the height multiplied by our calculated ratio here if we do flip it since our pivot will have flipped sides.
                var height = rect.sizeDelta.y * yRatio;
                if (reverse)
                    pos.y += height;

                rect.pivot = new Vector2(0.5f, reverse ? 1f : 0f);

                // Here we force the UI element within bounds and add a small buffer to those bounds.
                const float MENU_BUFFER_VERTICAL = 10f;

                var xBuffer = xRatio * rect.sizeDelta.x / 2f;
                var yBuffer = yRatio * MENU_BUFFER_VERTICAL;

                transform.position = new Vector3(
                    Mathf.Clamp(pos.x + xBuffer, xBuffer, Screen.width - xBuffer),
                    Mathf.Clamp(pos.y, yBuffer, Screen.height - yBuffer),
                    0.0f);
            }
        }

        private void GetObjectBounds(Bounds bounds)
        {
            Vector3 c = bounds.center;
            Vector3 e = bounds.extents;

            worldCornerBuffer[0] = new Vector3(c.x + e.x, c.y + e.y, c.z + e.z);
            worldCornerBuffer[1] = new Vector3(c.x + e.x, c.y + e.y, c.z - e.z);
            worldCornerBuffer[2] = new Vector3(c.x + e.x, c.y - e.y, c.z + e.z);
            worldCornerBuffer[3] = new Vector3(c.x + e.x, c.y - e.y, c.z - e.z);
            worldCornerBuffer[4] = new Vector3(c.x - e.x, c.y + e.y, c.z + e.z);
            worldCornerBuffer[5] = new Vector3(c.x - e.x, c.y + e.y, c.z - e.z);
            worldCornerBuffer[6] = new Vector3(c.x - e.x, c.y - e.y, c.z + e.z);
            worldCornerBuffer[7] = new Vector3(c.x - e.x, c.y - e.y, c.z - e.z);

            IEnumerable<Vector3> screenCorners = worldCornerBuffer.Select(corner => Camera.main.WorldToScreenPoint(corner));
            float maxX = screenCorners.Max(corner => corner.x);
            float minX = screenCorners.Min(corner => corner.x);
            float maxY = screenCorners.Max(corner => corner.y);
            float minY = screenCorners.Min(corner => corner.y);

            screenSpaceBounds[0] = new Vector3(maxX, maxY, 0);
            screenSpaceBounds[1] = new Vector3(minX, maxY, 0);
            screenSpaceBounds[2] = new Vector3(maxX, minY, 0);
            screenSpaceBounds[3] = new Vector3(minX, minY, 0);
        }

        /// <summary>
        ///     Hide the InspectionPanel and discard the current <see cref="context"/>
        /// </summary>
        public void HidePanel()
        {
            // Hide the window and discard the current InspectableElement context
            context = null;
            ControlWindow.TurnOff();

            ContractPanel();
        }

        /// <summary>
        ///     Wrapper method for turning off <see cref="ExpansionToggle"/>
        /// </summary>
        public void ContractPanel()
        {
            if (!ExpansionToggle)
            {
                Debug.LogError("InspectionPanel : No LeanToggle component assigned for control panel state change");
                return;
            }

            ExpansionToggle.TurnOff();
            canvasGroup.blocksRaycasts = false;
        }

        /// <summary>Unselect the context</summary>
        /// <remarks>This should be called when the close button is pressed</remarks>
        private void UnselectContext()
        {
            if (context != null)
            {
                context.Select(false, Interactions.Interactable.SelectionModes.Internal);
            }
        }

        /// <summary>
        /// Update all the data in the panel to reflect the data in the <see cref="InspectableElement"/>
        /// this panel is attached to.
        /// </summary>
        public void UpdateContent()
        {
            TitleField.text = context.ChecklistName;
            UpdateButtons();
            panelAnimator.SetAnimationHeights(context.ParentController.ElementStates.Count);
            UpdateStatusIcon();

            if (context.GetComponent<ExtraDetails>())
            {
                HelpButton.gameObject.SetActive(true);
            }
            else
            {
                HelpButton.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Update the buttons on the panel.
        /// The colour, text, and position of each button is updated
        /// to reflect the possible and chosen states for the element.
        /// Which button corresponds to which state in the element is 
        /// also updated.
        /// </summary>
        private void UpdateButtons()
        {
            stateIdentifersByButtons = new Dictionary<InspectionPanelStateButton, int>();

            Dictionary<InspectableElement, int> identifiersByElement = context.ParentController.IdentifiersByElement;

            List<InspectableElement> buttonsToMake = GetOrderedButtons(context);

            //first setup the selected button
            if (context.UserSelectedIdentifier != InspectableElement.NO_STATE_SELECTION)
            {
                UpdateStateButton(SelectedStateButton, buttonsToMake[0]);

                stateIdentifersByButtons.Add(SelectedStateButton, identifiersByElement[buttonsToMake[0]]);

                buttonsToMake.RemoveAt(0);
            }
            else
            {
                UpdateStateButton(SelectedStateButton, null);
                stateIdentifersByButtons.Add(SelectedStateButton, InspectableElement.NO_STATE_SELECTION);
            }

            for (int i = 0; i < PossibleStateButtons.Count; i++)
            {
                InspectableElement state = null;

                if (buttonsToMake.Count > 0)
                {
                    state = buttonsToMake[0];

                    stateIdentifersByButtons.Add(PossibleStateButtons[i], identifiersByElement[state]);

                    buttonsToMake.RemoveAt(0);
                }

                UpdateStateButton(PossibleStateButtons[i], state);
            }

            if (buttonsToMake.Count != 0)
            {
                Debug.LogError("There are more states than the inspection panel can handle. If we need more " +
                    "than 4 states, we will need to update this panel");
            }
        }

        /// <summary>
        /// Create a list of values that will be used to create the buttons in the panel.
        /// </summary>
        /// <remarks>
        /// The order of the list is set to match the order of the buttons in the panel.
        /// The order goes:
        /// 1. The selected state, if a state is selected.
        /// 2. The good state, if the selected state isn't the good state.
        /// 3. The rest of the possible bad states.
        /// </remarks>
        /// <param name="element">The element to make the buttons for.</param>
        /// <returns>An ordered list of inspectable elements to make buttons for.</returns>
        private List<InspectableElement> GetOrderedButtons(InspectableElement element)
        {
            List<InspectableElement> states = new List<InspectableElement>();
            states.AddRange(element.ParentController.ElementStates);

            List<InspectableElement> buttons = new List<InspectableElement>();

            //first, check if there is a selected state
            InspectableElement selectedState = element.GetSelectedState();
            if (selectedState != null)
            {
                buttons.Add(selectedState);
                states.Remove(selectedState);
            }

            //next, put in the good state
            if (!(selectedState != null && selectedState.State == InspectableState.Good))
            {
                InspectableElement goodElement = states.Find(x => x.State == InspectableState.Good);

                buttons.Add(goodElement);
                states.Remove(goodElement);
            }

            //add the rest of the buttons
            buttons.AddRange(states);

            return buttons;
        }

        /// <summary>
        /// Set the values for a <see cref="InspectionPanelStateButton"/> based on an <see cref="InspectableElement"/>.</summary>
        /// <param name="button">The button to set the values for</param>
        /// <param name="state">The state for the button</param>
        /// <param name="isSelectedStateButton">If the button passed in is the <see cref="SelectedStateButton"/></param>
        /// <remarks>If the <see cref="SelectedStateButton"/> is passed in and the state is null, it will be yellow. If any other button 
        /// is passed in and the state is null, that button will be set inactive.</remarks>
        private void UpdateStateButton(InspectionPanelStateButton button, InspectableElement state)
        {
            if (state == null)
            {
                if (button == SelectedStateButton)
                {
                    button.SetValues(NO_STATE_RESPONSE, ButtonColours.Yellow);
                }
                else
                {
                    button.gameObject.SetActive(false);
                    return;
                }
            }
            else
            {
                button.SetValues(state.StateMessage, state.State == InspectableState.Good ? ButtonColours.Green : ButtonColours.Red);
            }

            button.gameObject.SetActive(true);
        }

        /// <summary>
        /// Execute the actions needed for when a button on the panel is pressed.
        /// </summary>
        /// <param name="button">The button that was pressed.</param>
        /// <remarks>If the new state button is pressed, that inspection will be made on <see cref="InspectableElement"/> for this panel.
        /// If the button for the state that is already selected is pressed, a inspection of nothing will be made on the element.
        /// If the button that shows no state is selected is pressed, nothing happens</remarks>
        public void HandleStateButtonClick(InspectionPanelStateButton button)
        {
            int buttonsStateIdentifer;

            if (!stateIdentifersByButtons.TryGetValue(button, out buttonsStateIdentifer))
            {
                Debug.LogError("There was no state for the button that was selected.");
            }

            if (button == SelectedStateButton)
            {
                if (buttonsStateIdentifer == InspectableElement.NO_STATE_SELECTION)
                {
                    //they pressed the button that means there's no selection made
                    return;
                }

                context.MakeUserSelection(InspectableElement.NO_STATE_SELECTION);
            }
            else
            {
                context.MakeUserSelection(buttonsStateIdentifer);
            }
        }

        /// <summary>Update the status icon</summary>
        public void UpdateStatusIcon()
        {
            if (context != null)
            {
                statusIconController.SetIcon(context.CurrentStatus);
            }
        }

        /// <summary>Opens up details panel manager singleton on scene</summary>
        public void CanCheckExtraDetails()
        {
            DetailsPanelManager.Instance.ShowPanel();
        }
    }
}