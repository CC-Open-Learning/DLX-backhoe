using RemoteEducation.UI.Notifications;
using System.Collections;
using UnityEngine;

namespace RemoteEducation.Interactions
{
    /// <summary>
    /// An auxiliary highlight is a highlight that remains on an object regardless of how the user is interacting with it.
    /// This highlight will be covered up by any other highlights applied to the object. This class can only be used on objects
    /// that already have a <see cref="HighlightObject"/> on them.
    /// </summary>
    /// <remarks>    
    /// Example Usage:
    /// 1. Add an AuxiliaryHighlight to an InspectableElement object using <see cref="AddAuxiliaryHighlight(GameObject, Color)"/>.
    /// 2. Call <see cref="EnableAuxilaryHighlight"/> to show the auxiliary highlight. 
    ///    If this is being done a the beginning of the scene (in the first frame), use <see cref="EnableAuxilaryHighlight(float)"/>.
    /// 3. The Auxiliary highlight will be shown. If the user hovers the mouse over the object or selects the object, the regular highlight 
    ///    colours will be shown. Once they move the mouse off the object and/or de-select it, the Auxiliary highlight will show up again.
    /// 4. To remove the Auxiliary highlight, call <see cref="DisableAuxilaryHighlight"/>. You could call this in from 
    ///    InspectableElement.OnInspect so that the Auxiliary highlight is removed when once the user has inspected the component.
    /// </remarks>
    [RequireComponent(typeof(HighlightObject))]
    [DisallowMultipleComponent]
    public class AuxiliaryHighlight : MonoBehaviour
    {
        /// <summary>The <see cref="HighlightObject"/> that will be used my this object.</summary>
        private HighlightObject highlightObject;

        /// <summary>If a <see cref="HighlightObject"/> has been found on the object.</summary>
        private bool highlightAttatched => highlightObject != null;

        /// <summary>If the auxiliary highlight should be applied to the object.</summary>
        private bool auxiliaryActive;

        /// <summary>If the highlight on the object is the auxiliary highlight. If the
        /// highlight on the object is default highlight, this will be false.</summary>
        public bool HighlightIsAuxiliary => highlightIsAuxiliary;

        /// <summary>If the highlight on the object is the auxiliary highlight. If the
        /// highlight on the object is default highlight, this will be false.</summary>
        private bool highlightIsAuxiliary;

        /// <summary>The colour for the auxiliary highlight.</summary>
        private Color auxiliaryColour;

        /// <summary>The highlight coroutine to stop it.</summary>
        private Coroutine highlightCoroutine;

        /// <summary> The toast message </summary>
        private NotificationData toastMessage;

        /// <summary> The toast message lifetime </summary>
        private const float TOASTLIFETIME = 60f;

        /// <summary>
        /// Attach this object to the <see cref="HighlightObject"/> on the object.
        /// </summary>
        /// <returns>If the <see cref="HighlightObject"/> was found</returns>
        public bool Attach()
        {
            highlightObject = gameObject.GetComponent<HighlightObject>();

            if(highlightObject != null)
            {
                highlightObject.OnHighLightToggled += UpdateHighlightState;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Enable the Auxiliary highlight after <paramref name="delay"/> seconds.
        /// </summary>
        /// <remarks>This can be useful for highlighting objects right away in a scene.</remarks>
        public void EnableAuxilaryHighlight(float delay)
        {
            StartCoroutine(DelayActivation(delay));
        }

        /// <summary>
        /// Enable the Auxiliary highlight after <paramref name="delay"/> seconds.
        /// Enable the Auxiliary toast message <paramref name="message"/> with the highlight.
        /// </summary>
        /// <remarks>This can be useful for highlighting objects right away in a scene.</remarks>
        public void EnableAuxilaryHighlight(float delay, string message)
        {
            highlightCoroutine = StartCoroutine(DelayActivation(delay, message));
        }

        /// <summary>
        /// Enable the auxiliary highlight.
        /// If the object is already highlighted, then the auxiliary highlight will not
        /// be shown until the original highlight is removed.
        /// </summary>
        public void EnableAuxilaryHighlight()
        {
            if (!highlightAttatched && !Attach())
            {
                Debug.LogError($"{gameObject.name}: Unable to activate the Auxiliary Highlight. No HighlightObject was found");
                return;
            }

            if (!auxiliaryActive)
            {
                auxiliaryActive = true;

                if (!highlightObject.IsHighlighted)
                {
                    highlightObject.Glow(true, auxiliaryColour);

                    highlightIsAuxiliary = true;
                }
            }
        }

        /// <summary>
        /// Disable the auxiliary highlight.
        /// </summary>
        public void DisableAuxilaryHighlight()
        {
            if (auxiliaryActive)
            {
                auxiliaryActive = false;

                if (highlightIsAuxiliary)
                {
                    highlightObject.Glow(false);
                    highlightIsAuxiliary = false;
                }
            }
        }

        /// <summary>
        /// Ensure that the highlight on the object is in the correct state anytime the highlight changes.
        /// </summary>
        /// <param name="onOrOff">If the highlight was just turned on or off.</param>
        private void UpdateHighlightState(bool onOrOff)
        {
            if(onOrOff && highlightIsAuxiliary)
            {
                highlightIsAuxiliary = false;
            }
            else if (!onOrOff && !highlightIsAuxiliary && auxiliaryActive)
            {
                highlightObject.Glow(true, auxiliaryColour);
                highlightIsAuxiliary = true;
            }
        }

        /// <summary>
        /// Activate the highlight after <paramref name="delay"/> seconds.
        /// </summary>
        public IEnumerator DelayActivation(float delay)
        {
            yield return new WaitForSeconds(delay);
            EnableAuxilaryHighlight();
        }

        /// <summary>
        /// Activate the highlight and the message <paramref name="message"/> after <paramref name="delay"/> seconds.
        /// </summary>
        public IEnumerator DelayActivation(float delay, string message)
        {
            yield return new WaitForSeconds(delay);

            EnableAuxilaryHighlight();
             
            // Setup the toast message and create it in the scene
            toastMessage = NotificationManager.DefaultNotificationData;
            toastMessage.color = new Color32(0, 0, 0, 180);
            toastMessage.message = message;
            toastMessage.lifeTime = TOASTLIFETIME;
            NotificationManager.CreateNotification(toastMessage, NotificationManager.Position.Top);
        }

        /// <summary>
        /// Stop the DelayActivation coroutine and disable the toast message.
        /// </summary>
        public void StopDelayActivation()
        {
            DisableAnimation();
            DisableNotification();
        }

        /// <summary>
        /// Disable the toast message.
        /// </summary>
        public void DisableNotification()
        {
            NotificationManager.DisableNotification(toastMessage);
        }

        /// <summary>
        /// Stop the DelayActivation coroutine.
        /// </summary>
        public void DisableAnimation()
        {
            StopCoroutine(highlightCoroutine);
        }

        /// <summary>
        /// Attach a <see cref="AuxiliaryHighlight"/> to a GameObject. It is expected that the <paramref name="gameObject"/> will have
        /// its own <see cref="HighlightObject"/> added before the auxiliary highlight is activated.
        /// </summary>
        /// <param name="gameObject">The object to add the <see cref="AuxiliaryHighlight"/> component to</param>
        /// <param name="color">The colour for the Auxiliary highlight</param>
        /// <returns>The <see cref="AuxiliaryHighlight"/> object on the GameObject</returns>
        /// <remarks>The highlight starts turned off, use <see cref="EnableAuxilaryHighlight"/> to toggle it 
        /// on or off.</remarks>
        public static AuxiliaryHighlight AddAuxiliaryHighlight(GameObject gameObject, Color color)
        {
            AuxiliaryHighlight auxiliaryHighlight = gameObject.GetComponent<AuxiliaryHighlight>();

            if(auxiliaryHighlight == null)
            {
                auxiliaryHighlight = gameObject.AddComponent<AuxiliaryHighlight>();
            }

            auxiliaryHighlight.auxiliaryColour = color;

            return auxiliaryHighlight;
        }
    }
}