using UnityEngine;
using System;

namespace RemoteEducation.UI.Notifications
{
    public struct NotificationData
    {
        public float lifeTime;
        public Color color;
        public string message;
        public Notification.StatusType statusType;
        public Action callback;

        public NotificationData(float lifeTime, Color color, string message, Notification.StatusType statusType, Action callback)
        {
            this.lifeTime = lifeTime;
            this.color = color;
            this.message = message;
            this.statusType = statusType;
            this.callback = callback;
        }
    }
}
