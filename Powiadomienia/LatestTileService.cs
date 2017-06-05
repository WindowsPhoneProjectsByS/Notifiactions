using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace Powiadomienia
{
    class LatestTileService
    {
        public static void UpdateLatestTaskTile()
        {
            ToastNotifier notifier =
                    ToastNotificationManager.CreateToastNotifier();

            List<ScheduledToastNotification> initialList = new List<ScheduledToastNotification>();
            initialList = notifier.GetScheduledToastNotifications().ToList();

            string title = "";
            string content = "";

            if (initialList.Count > 0)
            {

                DateTime latestDateTime = initialList[0].DeliveryTime.DateTime;


                foreach (ScheduledToastNotification item in initialList)
                {

                    if (latestDateTime >= item.DeliveryTime.DateTime)
                    {
                        XmlDocument docXml = item.Content;
                        XmlNodeList toastTextElement = docXml.GetElementsByTagName("text");


                        try
                        {
                            title = toastTextElement[0].FirstChild.InnerText;
                        }
                        catch (Exception e)
                        {
                            title = "";
                        }

                        try
                        {
                            content = toastTextElement[1].FirstChild.InnerText;
                        }
                        catch (Exception e)
                        {
                            content = "";
                        }
                    }
                }

                ShowLatestTaskTile(title, content);
            }
            else
            {
                ShowLatestTaskTile("Brak najbliższych powiadomień", "");
            }
            
        }

        public static void ShowLatestTaskTile(string title, string content)
        {
            XmlDocument template = TileUpdateManager.GetTemplateContent(
                TileTemplateType.TileWide310x150IconWithBadgeAndText
                );

            XmlNodeList texts = template.GetElementsByTagName("text");

            texts[0].InnerText = title;
            texts[1].InnerText = content;

            TileNotification notification = new TileNotification(template);
            TileUpdater updater = TileUpdateManager.CreateTileUpdaterForApplication();

            updater.Update(notification);
        } 

        
    }
}
