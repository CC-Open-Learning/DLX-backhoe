using RemoteEducation.UI.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteEducation.Modules.HeavyEquipment
{
    internal static class HEPrompts
    {
        internal const float ShortToastDuration = 3f;
        internal const float LongToastDuration = 6f;

        internal static NotificationData CreateToast(string message, float lifeTime)
        {
            var data = NotificationManager.DefaultNotificationData;

            data.message = message;

            return NotificationManager.CreateNotification(data, NotificationManager.Position.Top);
        }
    }
}
