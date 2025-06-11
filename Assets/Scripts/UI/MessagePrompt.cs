/*
 *  FILE          :	MessagePrompt.cs
 *  PROJECT       :	VARLab CORE Template
 *  PROGRAMMER    :	Chowon Jung, David Inglis
 *  FIRST VERSION :	2020-08-07
 *  DESCRIPTION   : This file contains the MessagePrompt class which is responsible for displaying and hiding
 *					2D UI prompt message on the screen when it is called.
 *					The 2D window persists and fade out on click.
 *					
 *					Fade in/out related codes copied from InstructionCanvas.cs written by Michael Hilts
 */


#region Resources

using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using RemoteEducation.Helpers.Unity;
using Behaviour = RemoteEducation.UI.PromptManager.Behaviour;
using Status = RemoteEducation.UI.PromptManager.Status;
using VerticalPosition = RemoteEducation.UI.PromptManager.VerticalPosition;
using System.Collections;

#endregion

namespace RemoteEducation.UI
{
    /// <summary>
    /// Defines the attributes and functionality of a Prompt.
    /// </summary>
    /// <remarks>
    /// Prompts are messages that appear on screen for a set amount of time or when they are clicked.
    /// This class is usually called from the PromptManager. Developers should instantiate and interact with prompts through the PromptManager.
    /// This is the base class for a Prompt, it does the actual work for creating and defining Prompts as specified by PromptManager.
    /// </remarks>
    public class MessagePrompt : Prompt, IPointerClickHandler
    {
        #region Constants

        // Position and anchor coordinates for each of the VerticalPosition enum options
        static readonly Vector2 TopPosition = new Vector3(0, -60);
        static readonly Vector2 TopAnchor = new Vector2(0.5f, 1f);

        static readonly Vector2 MiddlePosition = new Vector3(0, 0);
        static readonly Vector2 MiddleAnchor = new Vector2(0.5f, 0.5f);

        static readonly Vector2 BottomPosition = new Vector3(0, 60);
        static readonly Vector2 BottomAnchor = new Vector2(0.5f, 0f);

        #endregion


        #region Fields

        /// <value>Stores the key that the PromptManager uses to reference this MessagePrompt instance.</value>
        public int Key;

        /// <value>Delegate which defines callback actions to take when the MessagePrompt is clicked (if it has a StatusType of Status.Click).</value>
        private Action<int> callback;

        [Tooltip("Text field to provide the message")]
        [SerializeField]
        private TextMeshProUGUI messageTMP;

        [Tooltip("Image placeholder for status icon")]
        [SerializeField]
        private Image statusIcon;

        [Tooltip("Sprites used for the status icons.")]
        [SerializeField]
        private Sprite [] statusIcons;

       

        /// <value>The Status of the MessagePrompt determines the icon displayed next to the message text.</value>
        public Status StatusType { get; private set; }

        /// <value>The Behaviour of the MessagePrompt determines how it is interacted with by the user/system.</value>
        public Behaviour BehaviourType { get; private set; }

        #endregion


        #region MonoBehaviour Callbacks

        /// <summary>
        /// Sets up the message with default values.
        /// </summary>
        /// <remarks>
        /// The message is hidden on instantiation.
        /// </remarks>
        private void Awake()
        {
            gameObject.SetActive(false);

            if (!messageTMP)
            {
                messageTMP = GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        private void Start()
        {
            Canvas.ForceUpdateCanvases();

            var layout = GetComponentInParent<VerticalLayoutGroup>();

            if (layout == null)
                return;

            // Hack to force the canvas layout to update properly.
            layout.enabled = false;
            layout.enabled = true;
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Sets the message for the prompt.
        /// </summary>
        /// <param name="message">The message to display as a prompt.</param>
        public void SetMessage(string message)
        {
            if (messageTMP)
            {
                messageTMP.text = message;
            }
            else
            {
                Debug.LogError("MessagePrompt : No TextMeshPro found in GameObject");
            }
        }

        /// <summary>
        /// Converts the provided VerticalPosition enumerator into a position and anchor coordinate pairing.
        /// </summary>
        /// <remarks>
        /// Used by SetPosition(Vector3, Vector2) to set the message's position on the screen accordingly.
        /// If an invalid VerticalPosition enum is provided, VerticalPosition.Bottom is used as the default.
        /// </remarks>
        /// <param name="anchorPosition"> VerticalPosition enumerator corresponding to one of the available vertical positions (i.e. where you want the prompt to show).</param>
        public void SetPosition(VerticalPosition anchorPosition)
        {
            SetContainer(anchorPosition);
            /*switch (anchorPosition)
            {
                case VerticalPosition.Top:
                    SetPosition(TopPosition, TopAnchor);
                    break;
                case VerticalPosition.Middle:
                    SetPosition(MiddlePosition, MiddleAnchor);
                    break;
                case VerticalPosition.Bottom:
                default:
                    SetPosition(BottomPosition, BottomAnchor);
                    break;
            }*/
        }

        private void SetContainer(VerticalPosition anchorPosition)
        {
            switch (anchorPosition)
            {
                case VerticalPosition.Top:
                    transform.SetParent(PromptManager.Instance.TopContainer);
                    break;
                case VerticalPosition.Middle:
                    transform.SetParent(PromptManager.Instance.MiddleContainer);
                    break;
                case VerticalPosition.Bottom:
                default:
                    transform.SetParent(PromptManager.Instance.BottomContainer);
                    break;
            }

            transform.localScale = Vector3.one;
        }

        /// <summary>
        ///  Assigns the message position according to the provided Vector2 'position' and 'anchor' parameters.
        /// </summary>
        /// <param name="position">The X, Y coordinates to be used as the center point of the prompt message.</param>
        /// <param name="anchor">Coordinates used to set the min and max anchor positions of the message.</param>
        public void SetPosition(Vector2 position, Vector2 anchor)
        {
            RectTransform rect = transform.Find("Background") as RectTransform;

            if (rect)
            {
                rect.anchorMin = rect.anchorMax = anchor;
                rect.anchoredPosition = position;
            }
        }

        /// <summary>
        /// Assigns the status icon sprite of the message to one of:
        /// Info, Warning, Success, Fail
        /// </summary>
        /// <param name="status">Status enumerator indicating one of the available status types.</param>
        public void SetStatus(Status status)
        {
            StatusType = status;
            
            if (!statusIcon)
            {
                return;
            }

            statusIcon.sprite = statusIcons[(int)StatusType];
        }

        /// <summary>
        /// Sets the background colour for the prompt to that which is specified.
        /// </summary>
        /// <param name="colour">The background colour for the prompt</param>
        public void SetBackgroundColour(Color colour)
        {
            Image img;
            img = gameObject.GetComponentInChildren<Image>();
            if (img)
            {
                img.color = colour;
            }
        }

        /// <summary>
        /// Assigns the behaviour of the MessagePrompt, where the available.
        /// </summary>
        /// <remarks>
        /// behaviours are as follows:
        /// 'Static' prompts are displayed indefinitely, must be modified or destroyed by an external class through the PromptManager.
        /// 'Clickable' prompts are displayed indefinitely, may be modified by an external class through the PromptManager, and may be assigned
        /// callback actions to the 'callback' delegate, which are invoked when the MessagePrompt area is clicked.
        /// </remarks>
        /// <param name="behaviour">Status enumerator indicating one of the available status types</param>
        public void SetBehaviour(Behaviour behaviour)
        {
            BehaviourType = behaviour;
        }

        /// <summary>
        /// Adds the specified Action<int> as a callback action to be invoked at a specified time.
        /// </summary>
        /// <remarks>
        /// Currently the 'callback' Action<int> is invoked when the MessagePrompt has StatusType 'Status.Click' and is clicked 
        /// (as defined by IPointerClickHandler interface).
        /// </remarks>
        /// <param name="action">The callback action to specify.</param>
        public void AddCallback(Action<int> action)
        {
            callback += action;
        }

        /// <summary>
        /// Sets the 'callback' Action<int> delegate to 'null'
        /// </summary>
        public void ClearCallbacks()
        {
            callback = null;
        }

        /// <summary>
        /// Sets the GameObject to active.
        /// </summary>
        /// <remarks>
        /// If there is an ActivationFade component attached to the GameObject as
        /// well, then the message will fade in when set to active.
        /// </remarks>
        public void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Fades out the message and then hides it
        /// </summary>
        public void Hide()
        {
            ActivationFade fader = GetComponent<ActivationFade>();
            if (fader && gameObject.activeSelf)
            {
                fader.FadeOut(ManualDisable);
            }
        }

        /// <summary>
        /// Instantly sets the gameObject to inactive. 
        /// Overrides any fade options.
        /// </summary>
        private void ManualDisable()
        {
            gameObject.SetActive(false);
        }

        #endregion


        #region IPointerClickHandler Interface

        /// <summary>
        /// This method defines the behaviour when the MessagePrompt UI element is clicked.
        /// </summary>
        /// <remarks>
        /// If the MessagePrompt has the StatusType (defined in PromptManager)
        /// of Status.Click, then the attached 'callback' actions will be invoked.
        /// </remarks>
        /// <param name="eventData">Data from event invoke.</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (BehaviourType == Behaviour.Clickable)
            {
                callback?.Invoke(Key);
            }
        }
        #endregion
    }
}
