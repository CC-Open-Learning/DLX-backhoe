using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace RemoteEducation.UI.Notifications
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    public class Notification : MonoBehaviour, IPointerClickHandler
    {
        /// <summary>How long it takes a notification to fade out.</summary>
        private const float FADETIME = 1.5f;

        [Tooltip("Image used to display the status icons.")]
        [SerializeField] private Image statusIconImage;

        [Tooltip("Background image of the notification.")]
        [SerializeField] private Image bg;

        [Tooltip("Text object used to display the message.")]
        [SerializeField] private TextMeshProUGUI messageText;

        /// <summary>Canvas group of the entire notification.</summary>
        /// <remarks>Used for fading the notification in and out as a whole.</remarks>
        private CanvasGroup group;
        /// <summary>Time the current state was first started at.</summary>
        private float stateStartTime;
        /// <summary>Progress through the current state from 0 (start) to 1 (finished).</summary>
        private float t;

        [Tooltip("Sprites used for the status icons.")]
        [SerializeField] private Sprite[] statusIcons;

        /// <summary>The <see cref="NotificationManager"/> used to create this <see cref="Notification"/>.</summary>
        private NotificationManager manager;

        /// <summary>Most recent <see cref="NotificationData"/> used to set up this <see cref="Notification"/>.</summary>
        private NotificationData data;

        public enum StatusType
        {
            Info = 0,
            Positive = 1,
            Negative = 2,
            Warning = 3,
            Interactive = 4,
            Help = 5
        }

        enum State
        {
            NewMsg = 0,
            Fading = 1,
            Static = 2
        }
        private State state;

        /// <summary>If this <see cref="Notification"/> is static or timed.</summary>
        internal bool IsStatic => data.lifeTime <= 0f;

        void Awake()
        {
            group = GetComponent<CanvasGroup>();
            gameObject.SetActive(false); // Auto disable on load, these are created in NotificationManager's Awake function.
        }

        void Update()
        {
            switch (state)
            {
                case State.NewMsg:
                    t = (Time.time - stateStartTime) / data.lifeTime;

                    if (t >= 1)
                    {
                        stateStartTime = Time.time;
                        state = State.Fading;
                    }
                    break;

                case State.Fading:
                    t = (Time.time - stateStartTime) / FADETIME;

                    group.alpha = Mathf.Lerp(1.0f, 0.0f, t);

                    if (t >= 1)
                    {
                        Release();
                    }
                    break;
            }
        }

        /// <summary>Uses the given <paramref name="data"/> to set up this <see cref="Notification"/> and enables it for display.</summary>
        internal void SetData(NotificationData data, NotificationManager manager)
        {
            this.manager = manager;
            this.data = data;

            messageText.text = data.message;
            bg.color = data.color;
            statusIconImage.sprite = statusIcons[(int)data.statusType];

            bg.raycastTarget = data.callback != null; // Disable raycast catching if this is not a clickable toast.

            stateStartTime = Time.time;
            state = State.NewMsg;
            group.alpha = 1f;

            gameObject.SetActive(true);

            if (IsStatic)
                state = State.Static;
        }

        /// <summary>Disables this <see cref="Notification"/> and releases it from its <see cref="NotificationManager"/>.</summary>
        private void Release()
        {
            gameObject.SetActive(false);
            manager.Release(data);
        }

        /// <summary>Disables this <see cref="Notification"/>.</summary>
        internal void Disable()
        {
            gameObject.SetActive(false);
        }

        /// <summary>Handles callbacks for clickable notifications.</summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (data.callback != null)
                data.callback.Invoke();
        }
    }
}
