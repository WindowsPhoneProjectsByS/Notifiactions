using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Powiadomienia
{
    class BadgeService
    {
        public static void UpdateTodayNumber(int seconds)
        {
            if (isToday(seconds))
            {
                MainPage.todayNotificationNumber++;
                ShowBadgeWithNumber(MainPage.todayNotificationNumber);
            }
        } 

        public static bool isToday(int seconds)
        {
            if (seconds > 60 * 60  * 24)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static void ShowBadgeWithNumber(int number)
        {
            var badgeXML = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeNumber);
            var badge = badgeXML.SelectSingleNode("/badge") as XmlElement;
            badge.SetAttribute("value", number.ToString());
            var badgeNotification = new BadgeNotification(badgeXML);
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badgeNotification);

        }
    }
}
