/*
 *  FILE          :	PromptManager.cs
 *  PROJECT       :	CORE Engine
 *  PROGRAMMER    :	Michael Hilts, David Inglis
 *  FIRST VERSION :	2019-10-20
 *  DESCRIPTION   : 
 *      This file contains the PromptManager class which is responsible for managing and controlling
 *      prompts of various types to be displayed to the user.
 *      
 *      Currently the only supported prompt is the MessagePrompt which is a 2D text message displayed
 *      in the UI layer
 */

#region Using Statements

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace RemoteEducation.UI
{
    /// <summary>
    /// PromptManager class which is responsible for Manages and controls prompts of various types to be displayed to the user.
    /// </summary>
    /// <remarks>
    /// Calls upon the MessagePrompt class to do the actual work of displaying and creating Prompts.
    /// Devs should use this class to interact with Prompts.
    /// </remarks>
    public class PromptManager : MonoBehaviour
    {

        /// <summary>Enumerator for assigning vertical position of UI prompts.</summary>
        public enum VerticalPosition
        {
            Bottom,
            Middle,
            Top
        }

        /// <summary>Enumerator for assigning status type of prompts to change their style or appearance.</summary>
        public enum Status
        {
            Info = 0,
            Positive = 1,
            Negative = 2,
            Warning = 3,
            Interactive = 4,
            Help = 5
        }

        /// <summary>Enumerator for assigning the behaviour of the prompt.</summary>
        /// <remarks>Behaviour defines if a prompt is Static, Clickable (click to hide or to complete action), 
        /// or a Toast (a message that appears for a set period of time).</remarks>
        public enum Behaviour
        {
            Static,
            Clickable,
            Toast
        }

        /// <summary>Enumerator for assigning the time until a prompt automatically hides itself.</summary>
        public enum Timeout
        {
            None,
            Short,
            Long
        }

        [Tooltip("Transform containers for the top, middle, and bottom messages respectively.")]
        [SerializeField] private Transform[] containers;

        public Transform TopContainer => containers[0];
        public Transform MiddleContainer => containers[1];
        public Transform BottomContainer => containers[2];

        /// <summary>If a new prompt cannot be created, InvalidKey is returned as its key value.</summary>
        public const int InvalidKey = -1;

        /// <value>Short Toast timeout</value>
        public const float ToastTimeoutShort = 3f;
        /// <value>Long Toast timeout</value>
        public const float ToastTimeoutLong = 6f;

        /// <summary>Ensures the 'prompts' Dictionary does not contain more than MaxPromptCount objects.</summary>
        private static readonly int MaxPromptCount = short.MaxValue;
        /// <summary>Ensures that the prompt key is not over the max allowed value for ints.</summary>
        private static readonly int MaxKeyValue = int.MaxValue;

        /// <summary>Reference to the GameObject used by the MessagePrompt class.</summary>
        [SerializeField] private GameObject Resource;

        /// <summary>Tracks an instance of the PromptManager class to form a Singleton pattern</summary>
        public static PromptManager Instance = null;

        /// <value>Dictionary used to store all managed prompts</value>
        public Dictionary<int, MessagePrompt> prompts;


        /// <summary>
        /// Initializes the PromptManager.
        /// </summary>
        /// <remarks>
        /// Uses the Instance member to form a Singleton pattern.
        /// </remarks>
        public void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("PromptManager : An instance already exists");
                Destroy(this);
                return;
            }

            UnitTestSafetyCheck();

            Instance = this;
            prompts = new Dictionary<int, MessagePrompt>();
        }

        /// <summary>
        /// Generates a unique integer key to use as reference in the 'prompts' Dictionary. 
        /// If the Dictionary is full, the 'InvalidKey' constant is returned.
        /// </summary>
        /// <remarks>
        /// The Dictionary has an arbitrary maximum capacity of MaxPromptCount (defined to be 2^15), 
        /// while the key has an arbitrary maximum value of 2^31.
        /// </remarks>
        /// <returns></returns>
        private int GenerateKey()
        {
            if (prompts.Count >= MaxPromptCount)
            {
                return InvalidKey;
            }

            int key;

            // Ensure the key random key is unique
            do
            {
                key = UnityEngine.Random.Range(0, MaxKeyValue);
            }
            while (prompts.ContainsKey(key));

            return key;
        }

        /// <summary>
        /// Creates the based MessagePrompt object controlled by this manager.
        /// </summary>
        /// <remarks>
        /// This method is called by multiple public methods which themselves define a prompt of common types.
        /// i.e. Toasts, static prompts, etc.
        /// </remarks>
        /// <param name="message">Message to display as prompt.</param>
        /// <param name="key">Prompt identifier or key.</param>
        /// <param name="behaviour">How the prompt should behave (i.e. clickable, static, etc.).</param>
        /// <param name="position">Where on the screen to display the prompt.</param>
        /// <param name="status">Prompt status icon (i.e. check, x, etc.).</param>
        /// <param name="color">Prompt colour (i.e. Color.black, or nothing for default colour specified in gameObject).</param>
        /// <returns></returns>
        private MessagePrompt CreateMessagePrompt(string message, int key, 
            Behaviour behaviour = Behaviour.Static, VerticalPosition position = VerticalPosition.Bottom, 
            Status status = Status.Info, Color? color = null)
        {
            MessagePrompt prompt = Instantiate(Resource).GetComponent<MessagePrompt>();

            // Assign reference key to be stored by the MessagePrompt
            prompt.Key = key;

            // Set message with provided or defaulted values
            prompt.SetMessage(message);
            prompt.SetBehaviour(behaviour);
            prompt.SetPosition(position);
            prompt.SetStatus(status);

            if (color != null) { prompt.SetBackgroundColour(color.Value); }

            return prompt;
        }

        /// <summary>
        /// Generic interface for creating MessagePrompt instances.
        /// </summary>
        /// <remarks>
        /// Based on the specified 'behaviour', the appropriate specialized method to Create a new MessagePrompt is called.
        /// If this method is used to create a 'Clickable' MessagePrompt, no callback action is assigned.
        /// If this method is used to create a 'Toast' MessagePrompt, the 'ToastTimeoutShort' constant is used as the timeout value.
        /// </remarks>
        /// <param name="message">Message to display as prompt.</param>
        /// <param name="behaviour">How the prompt should behave (i.e. clickable, static, etc.).</param>
        /// <param name="position">Where on the screen to display the prompt.</param>
        /// <param name="status">Prompt status icon (i.e. check, x, etc.).</param>
        /// <param name="color">Prompt colour (i.e. Color.black, or nothing for default colour specified in gameObject).</param>
        /// <returns></returns>
        public int Create(string message, Behaviour behaviour = Behaviour.Static, VerticalPosition position = VerticalPosition.Bottom, Status status = Status.Info, Color? color = null)
        {
            switch(behaviour)
            {
                case Behaviour.Clickable:
                    return CreateClickable(message, null, position, status, color);
                case Behaviour.Toast:
                    CreateToast(message, ToastTimeoutShort, position, status, color);
                    return InvalidKey;
                case Behaviour.Static:
                default:
                    return CreateStatic(message, position, status, color);
            }
        }

        /// <summary>
        /// Creates a 'Static' MessagePrompt.
        /// </summary>
        /// <remarks>
        ///  Creates a MessagePrompt instance with 'Static' behaviour which is intended to be displayed to the user for an indefinite amount of time.
        /// </remarks>
        /// <param name="message">Message to display as prompt.</param>
        /// <param name="position">Where on the screen to display the prompt.</param>
        /// <param name="status">Prompt status icon (i.e. check, x, etc.).</param>
        /// <param name="color">Prompt colour (i.e. Color.black, or nothing for default colour specified in gameObject).</param>
        /// <returns>
        /// int : The key which corresponds to the newly created MessagePrompt. 
        /// External classes will reference their MessagePrompt through the key.
        /// </returns>
        public int CreateStatic(string message, VerticalPosition position = VerticalPosition.Bottom, Status status = Status.Info, Color? color = null)
        {
            int key = GenerateKey();

            if (key != InvalidKey)
            {
                // Add the message to the 'prompts' Dictionary
                prompts.Add(key, CreateMessagePrompt(message, key, Behaviour.Static, position, status, color));
            }

            return key;
        }

        /// <summary>
        /// Creates a 'Clickable' prompt.
        /// </summary>
        /// <remarks>
        /// Creates a specialized MessagePrompt instance with 'Clickable' behaviour which
        /// is intended to react to user input through a mouse or pointer click action.
        /// The default status of a Clickable MessagePrompt is 'Interactive'.
        /// If the custom action 'customAction' is specified (non null), the callback is 
        /// invoked when the MessagePrompt is clicked by the user.
        /// </remarks>
        /// <param name="message">Message to display as prompt.</param>
        /// <param name="customAction">Callback to invoke onClick (if specified).</param>
        /// <param name="position">Where on the screen to display the prompt.</param>
        /// <param name="status">Prompt status icon (i.e. check, x, etc.).</param>
        /// <param name="color">Prompt colour (i.e. Color.black, or nothing for default colour specified in gameObject).</param>
        /// <returns>
        /// int : The key which corresponds to the newly created MessagePrompt. 
        /// External classes will reference their MessagePrompt through the key.
        /// </returns>
        public int CreateClickable(string message, Action<int> customAction, VerticalPosition position = VerticalPosition.Bottom, Status status = Status.Interactive, Color? color = null)
        {
            int clickableKey = GenerateKey();

            if (clickableKey != InvalidKey)
            {
                // Add the message to the 'prompts' Dictionary
                prompts.Add(clickableKey, CreateMessagePrompt(message, clickableKey, Behaviour.Clickable, position, status, color));
                
                // Set up callback action
                AddCallback(clickableKey, customAction);
            }

            return clickableKey;
        }

        /// <summary>
        /// Creates a MessagePrompt instance that will be displayed after creation 
        /// and will disappear after the specified 'timeout' number of seconds.
        /// </summary>
        /// <remarks>
        /// Unlike the standard MessagePrompts, this toast-style MessagePrompt may not 
        /// be managed by the external class which created it after its instantiation 
        /// </remarks>
        /// <param name="message">Message to display as prompt.</param>
        /// <param name="timeout">How long to keep the prompt on screen.</param>
        /// <param name="position">Where on the screen to display the prompt.</param>
        /// <param name="status">Prompt status icon (i.e. check, x, etc.).</param>
        /// <param name="color">Prompt colour (i.e. Color.black, or nothing for default colour specified in gameObject).
        ///     An optional <see cref="Nullable{T}"/> parameter that allows the color of the toast to be specified.
        ///     If <see langword="null"/>, the background <see cref="Color"/> will be unchanged from the color
        ///     in the <see cref="Resource"/> prefab.
        /// </param>
        public void CreateToast(string message, float timeout, VerticalPosition position = VerticalPosition.Bottom, Status status = Status.Info, Color? color = null)
        {
            int toastKey = GenerateKey();

            if (toastKey != InvalidKey)
            {
                // Add the message to the 'prompts' Dictionary
                prompts.Add(toastKey, CreateMessagePrompt(message, toastKey, Behaviour.Toast, position, status, color));

                // Invoke coroutine to display then hide Toast
                StartCoroutine(ToastMessageCoroutine(toastKey, timeout));
            }
        }

        /// <summary>
        /// Sets the vertical position of the MessagePrompt associated with the given key.
        /// </summary>
        /// <remarks>
        /// The explicit canvas positions corresponding to each VerticalPosition
        /// enum are defined within the MessagePrompt.SetPosition() method.
        /// </remarks>
        /// <param name="key">Prompt identifier.</param>
        /// <param name="position">The new position of the specified prompt.</param>
        /// <returns>true if prompt 'key' is valid and position was set, false if otherwise.</returns>
        public bool SetPosition(int key, VerticalPosition position)
        {
            if (prompts.TryGetValue(key, out MessagePrompt prompt))
            {
                prompt.SetPosition(position);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the Status icon for the prompt associated with the given key.
        /// </summary>
        /// <remarks>
        ///  The explicit sprite resources corresponding to each Status enum are defined within the MessagePrompt.SetStatus() method.
        /// </remarks>
        /// <param name="key">Prompt identifier.</param>
        /// <param name="status">The new Status icon for the associated prompt.</param>
        /// <returns>true if prompt 'key' is valid and Status was set, false if otherwise.</returns>
        public bool SetStatus(int key, Status status)
        {
            if (prompts.TryGetValue(key, out MessagePrompt prompt))
            {
                prompt.SetStatus(status);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the message of the prompt associated with the given key.
        /// </summary>
        /// <param name="key">Prompt identifier.</param>
        /// <param name="message">New message to apply to associated prompt.</param>
        /// <returns>true if prompt 'key' is valid and message was set, false if otherwise.</returns>
        public bool SetMessage(int key, string message)
        {
            if (prompts.TryGetValue(key, out MessagePrompt prompt))
            {
                prompt.SetMessage(message);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Generic interface to switch MessagePrompt behaviour.
        /// </summary>
        /// <remarks>
        /// Switching the behaviour to 'Toast' is not supported.
        /// If switching the MessagePrompt to 'Clickable', no callback action is provided.
        /// It is recommended to use SetClickable(int, Action<int>) if a callback action is required.
        /// </remarks>
        /// <param name="key">Prompt identifier.</param>
        /// <param name="behaviour">New Behaviour for prompt associated with given key.</param>
        /// <returns>true if prompt 'key' is valid and Behaviour was set, false if otherwise.</returns>
        public bool SetBehaviour(int key, Behaviour behaviour)
        {
            switch (behaviour)
            {
                case Behaviour.Toast:
                    return false;
                case Behaviour.Clickable:
                    return SetClickable(key, null);
                case Behaviour.Static:
                default:
                    return SetStatic(key);
            }
        }

        /// <summary>
        /// Sets the Behaviour of the specified MessagePrompt to 'Static'.
        /// </summary>
        /// <remarks>
        /// Sets the 'callback' delegate to null.
        /// </remarks>
        /// <param name="key">Identifier for pompt to set as 'Static'.</param>
        /// <returns>true if prompt 'key' is valid set 'Static', false if otherwise.</returns>
        public bool SetStatic(int key)
        {
            if (prompts.TryGetValue(key, out MessagePrompt prompt))
            {
                prompt.SetBehaviour(Behaviour.Static);
                prompt.ClearCallbacks();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the Behaviour of the MessagePrompt with the specified 'key' to 'Clickable'.
        /// </summary>
        /// <remarks>
        /// Any existing callbacks are cleared, but a new custom callback action can be specified by the 'callback' parameter.
        /// Additional custom callback actions can be specified with the AddCallback(int, Action<int>) method.
        /// </remarks>
        /// <param name="key">Prompt identifier.</param>
        /// <param name="callback">New callback method for onClick of specified prompt.</param>
        /// <returns>true if prompt 'key' is valid and prompt was set as 'Clickable', false if otherwise.</returns>
        public bool SetClickable(int key, Action<int> callback)
        {
            if (prompts.TryGetValue(key, out MessagePrompt prompt))
            {
                prompt.SetBehaviour(Behaviour.Clickable);
                prompt.ClearCallbacks();
                prompt.AddCallback(callback);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds custom callback action to the prompt associated with the given key.
        /// </summary>
        /// <remarks>
        /// The referenced MessagePrompt must already have the 'Clickable' Behaviour in order to specify an additional callback action.
        /// Use the SetClickable(int, Action<int>) method if the MessagePrompt is not already 'Clickable'.
        /// </remarks>
        /// <param name="key">Prompt identifier.</param>
        /// <param name="callback">New callback for prompt associated with given 'key'.</param>
        /// <returns>true if prompt 'key' is valid and prompt's callback was set, false if otherwise.</returns>
        public bool AddCallback(int key, Action<int> callback)
        {
            if (prompts.TryGetValue(key, out MessagePrompt prompt) && prompt.BehaviourType == Behaviour.Clickable)
            {
                prompt.AddCallback(callback);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Clears the callback for the prompt associated with the given 'key'.
        /// </summary>
        /// <param name="key">Prompt identifier.</param>
        /// <returns>true if prompt 'key' is valid and callbacks were cleared, false if otherwise.</returns>
        public bool ClearCallbacks(int key)
        {
            if (prompts.TryGetValue(key, out MessagePrompt prompt))
            {
                prompt.ClearCallbacks();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Hides the prompt associated with the given 'key'.
        /// </summary>
        /// <remarks>
        /// Calls the Hide() method on the MessagePrompt referenced by the provided 'key', if such MessagePrompt exists in the 'prompts' Dictionary.
        /// </remarks>
        /// <param name="key">Prompt identifier.</param>
        /// <returns>true if prompt 'key' is valid and prompt was hidden, false if otherwise.</returns>
        public bool Hide(int key)
        {
            if (prompts.TryGetValue(key, out MessagePrompt prompt))
            {
                prompt.Hide();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Hide all the prompts
        /// </summary>
        public void HideAll()
        {
            foreach(int i in prompts.Keys)
            {
                Hide(i);
            }
        }

        /// <summary>
        /// Shows prompt associated with given 'key'.
        /// </summary>
        /// <remarks>
        /// Calls the Show() method on the MessagePrompt referenced by the provided 'key', if such MessagePrompt exists in the 'prompts' Dictionary.
        /// </remarks>
        /// <param name="key">Prompt identifier.</param>
        /// <returns>true if prompt 'key' is valid and prompt was shown, false if otherwise.</returns>
        public bool Show(int key)
        {
            if (prompts.TryGetValue(key, out MessagePrompt prompt))
            {
                prompt.Show();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Destroys prompt associated with given 'key'.
        /// </summary>
        /// <remarks>
        /// Invokes the DelayedDestroy coroutine, which hides and destroys the MessagePrompt referenced by the provided 'key', 
        /// if such MessagePrompt exists in the 'prompts' Dictionary.
        /// </remarks>
        /// <param name="key">Prompt identifier.</param>
        public void Destroy(int key)
        {
            StartCoroutine(DelayedDestroy(key));
        }

        /// <summary>
        /// Destroys prompt associated with given 'key' in a delayed fashion.
        /// </summary>
        /// <remarks>
        /// Intended to be invoked as a coroutine, this method hides then destroys and removes the MessagePrompt referenced by the provided 'key'.
        /// </remarks>
        /// <param name="key">Prompt identifier.</param>
        /// <returns>true if the prompt 'key' is valid and the prompt was destroyed, false if otherwise.</returns>
        private IEnumerator DelayedDestroy(int key)
        {
            if (prompts.TryGetValue(key, out MessagePrompt prompt))
            {
                // Calling Hide() causes the Toast to fade out, if the appropriate behaviour is attached
                Hide(key);
                yield return new WaitForSeconds(1f);

                // Destroy the MessagePrompt GameObject
                Destroy(prompt.gameObject);

                // Remove the MessagePrompt reference from 'prompts' Dictionary
                prompts.Remove(key);
            }

            yield return null;
        }

        /// <summary>
        /// Displays Toasts using a Coroutine.
        /// </summary>
        /// <remarks>
        /// A Coroutine is used to do this as Toasts are messages which only appear on screen, in a static manner, for a set period of time.
        /// As such, they have to be created, displayed, and then a wait period is entered before the Toast is destroyed. 
        /// Given that the processing thread is required to wait, it is best to run this operation from a Coroutine.
        /// </remarks>
        /// <param name="key">Prompt identifier.</param>
        /// <param name="timeout">How long to display the Toast on screen before it is destroyed.</param>
        /// <returns>null</returns>
        public IEnumerator ToastMessageCoroutine(int key, float timeout)
        {
            // Display the Toast, since it is hidden on creation
            Show(key);

            // Wait for the specified timeout length
            yield return new WaitForSeconds(timeout);

            // Uses the attached fader to remove the MessagePrompt
            StartCoroutine(DelayedDestroy(key));

            yield return null;
        }

        /// <summary>Unit test workaround since they don't have a mocked Prompt Manager asset.</summary>
        /// <remarks>Should be reworked to use a test addressable.</remarks>
        private void UnitTestSafetyCheck()
        {
            if (containers == null)
            {
                containers = new Transform[3];
                containers[0] = new GameObject("Dummy").transform;
                containers[1] = new GameObject("Dummy").transform;
                containers[2] = new GameObject("Dummy").transform;

                containers[0].SetParent(transform);
                containers[1].SetParent(transform);
                containers[2].SetParent(transform);
            }
        }
    }
}
