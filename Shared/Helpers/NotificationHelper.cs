using System;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Shared.Helpers
{
    public class NotificationHelper
    {

        public static void ShowNotification(String notificationText)
        {
            // template to load for showing Toast Notification
            var xmlToastTemplate = "<toast launch=\"app-defined-string\">" +
                                     "<visual>" +
                                       "<binding template =\"ToastGeneric\">" +
                                         "<text>Palette</text>" +
                                         "<text>" +
                                           notificationText +
                                         "</text>" +
                                       "</binding>" +
                                     "</visual>" +
                                   "</toast>";

            // load the template as XML document
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlToastTemplate);

            // create the toast notification and show to user
            var toastNotification = new ToastNotification(xmlDocument);
            toastNotification.NotificationMirroring = NotificationMirroring.Disabled;
            toastNotification.Group = "toast";
            var notification = ToastNotificationManager.CreateToastNotifier();
            notification.Show(toastNotification);
        }

    }
}
