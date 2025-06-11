using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Collections;

namespace RemoteEducation.UI.Notifications
{
    [DisallowMultipleComponent]
    public class NotificationManager : MonoBehaviour
    {
        /// <summary>Maximum number of simultaneous notifications before re-using the oldest.</summary>
        private const int MAX_SIMULTANEOUS_NOTES = 10;

        [Tooltip("Prefab to use as a notification.")]
        [SerializeField] private GameObject notificationPrefab;

        [Tooltip("Transform containers for the top, middle, and bottom messages respectively.")]
        [SerializeField] private Transform[] containers;

        /// <summary>Collection of all currently active notifications.</summary>
        /// <remarks>Upon expiring, the notification is removed from this collection.</remarks>
        private Dictionary<NotificationData, Notification> notifications;

        /// <summary>Pool of notifications that get re-used.</summary>
        /// <remarks>This prevents allocations and garbage collection in the middle of a sim.</remarks>
        private Notification[] objectPool;

        /// <summary>Layout groups corresponding to each of the <see cref="containers"/>.</summary>
        private VerticalLayoutGroup[] layoutGroups;

        /// <summary>Singleton instance of this class.</summary>
        private static NotificationManager instance;

        /// <summary>A preset of notification values to help reduce redundant code.</summary>
        public static readonly NotificationData DefaultNotificationData = new NotificationData
        {
            lifeTime = 5.0f,
            callback = null,
            message = "",
            statusType = Notification.StatusType.Info,
            color = new Color32(0, 2, 36, 225)
        };

        /// <summary>Current index of the <see cref="objectPool"/> to pull the next <see cref="Notification"/> from.</summary>
        private int currentPoolIndex = 0;

        /// <summary>Enumerator for assigning vertical position of UI prompts.</summary>
        public enum Position
        {
            Top = 0,
            Middle = 1,
            Bottom = 2
        }

        private void Awake()
        {
            if (!SingletonSetupAndDuplicateCheck())
                return;

            notifications = new Dictionary<NotificationData, Notification>();

            objectPool = new Notification[MAX_SIMULTANEOUS_NOTES];

            for (int i = 0; i < objectPool.Length; i++)
            {
                objectPool[i] = (Instantiate(notificationPrefab) as GameObject).GetComponent<Notification>();
            }

            GetLayoutGroups(); 
        }

        /// <summary>Creates a <see cref="Notification"/> with the given parameters.</summary>
        /// <returns>A copy of the <paramref name="data"/> generated.</returns>
        public static NotificationData CreateNotification(string message, Color color, Position position, Notification.StatusType statusType, float lifeTime, Action callback)
        {
            var data = new NotificationData(lifeTime, color, message, statusType, callback);

            var notification = instance.GetNotification(data);

            instance.SetNotificationContainer(notification, position);

            return data;
        }

        /// <summary>Creates a <see cref="Notification"/> with the given <paramref name="data"/> and moves it to the given <paramref name="position"/>.</summary>
        /// <returns>A copy of the <paramref name="data"/> used.</returns>
        public static NotificationData CreateNotification(NotificationData data, Position position)
        {
            var notification = instance.GetNotification(data);

            instance.SetNotificationContainer(notification, position);

            return data;
        }

        /// <summary>Gets a new <see cref="Notification"/> from the pool or refreshes an existing one if the <paramref name="data"/> they share is identical.</summary>
        /// <returns>A reference to the <see cref="Notification"/> used.</returns>
        private Notification GetNotification(NotificationData data)
        {
            Notification notification;

            if(!notifications.TryGetValue(data, out notification))
            {
                notification = GetFreeNotificationFromPool();
                notifications.Add(data, notification);
            }

            notification.SetData(data, this);
            return notification;
        }

        /// <summary>Finds the first free <see cref="Notification"/> in the total pool of re-usable objects.</summary>
        /// <returns>First free <see cref="Notification"/> in the pool or the oldest if none are free.</returns>
        private Notification GetFreeNotificationFromPool()
        {
            var index = currentPoolIndex;

            do
            {
                if (!objectPool[index].gameObject.activeSelf)
                {
                    currentPoolIndex = NextIndex(index);
                    return objectPool[index];
                }

                index = NextIndex(index);
            } while (currentPoolIndex != index);

            currentPoolIndex = NextIndex(index);
            return objectPool[index];

            int NextIndex(int index)
            {
                return (index + 1) % objectPool.Length;
            }
        }

        /// <summary>Forces a <see cref="Notification"/> to immediately disable.</summary>
        /// <remarks>Should mostly be used to disable static notifications that otherwise will not disable themselves.</remarks>
        /// <returns><see langword="true"/> if the <see cref="NotificationData"/> was found in the <see cref="notifications"/> collection.</returns>
        public static bool DisableNotification(NotificationData data)
        {
            var found = instance.notifications.TryGetValue(data, out Notification notification);

            if (found)
                notification.Disable();

            return found;
        }

        /// <summary>Frees a finished <see cref="Notification"/> from the <see cref="notifications"/> collection.</summary>
        internal void Release(NotificationData data)
        {
            notifications.Remove(data);
        }

        /// <summary>Moves the <paramref name="notification"/> to the desired <paramref name="position"/> on the screen.</summary>
        private void SetNotificationContainer(Notification notification, Position position)
        {
            var t = notification.transform;

            t.SetParent(containers[(int)position]);
            t.localScale = Vector3.one;

            LayoutBugWorkaround(position);
        }

        /// <summary>A workaround for the layout bug that causes items to overlap.</summary>
        private void LayoutBugWorkaround(Position position)
        {
            Canvas.ForceUpdateCanvases();

            // Hack to force the canvas layout to update properly.
            var i = (int)position;
            layoutGroups[i].enabled = false;
            layoutGroups[i].enabled = true;
        }

        /// <summary>Gets the <see cref="VerticalLayoutGroup"/> components attached to each of the <see cref="containers"/>.</summary>
        private void GetLayoutGroups()
        {
            layoutGroups = new VerticalLayoutGroup[containers.Length];
            for (int i = 0; i < layoutGroups.Length; i++)
            {
                layoutGroups[i] = containers[i].GetComponent<VerticalLayoutGroup>();
            }
        }

        /// <summary>Sets up the class singleton and checks if this instance is a duplicate.</summary>
        /// <returns><see langword="true"/> if this instance is not a duplicate, <see langword="false"/> otherwise.</returns>
        bool SingletonSetupAndDuplicateCheck()
        {
            if (instance != null)
            {
                Debug.LogError("NotificationManager: Duplicate singleton error, there should never be two of these in one scene! Deleting this duplicate.");
                Destroy(this);
                return false;
            }

            instance = this;
            return true;
        }
    }
}
