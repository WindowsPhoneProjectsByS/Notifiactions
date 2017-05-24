using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace Powiadomienia
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DispatcherTimer dTimer;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            Debug.WriteLine("Przygotowanie list z powiadomieniami.");
            PrepareNotificationListView();
            StartDTimer();
        }

        public static ObservableCollection<NotificationItem> NotificationList;

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private void RaiseNotification_Click(object sender, RoutedEventArgs e)
        {
            DeleteNotification();
        }

        private void showHarderWay()
        {
            ToastTemplateType toastType = ToastTemplateType.ToastText02;

            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastType);

            XmlNodeList toastTextElement = toastXml.GetElementsByTagName("text");
            toastTextElement[0].AppendChild(toastXml.CreateTextNode("Hello C# Corner"));
            toastTextElement[1].AppendChild(toastXml.CreateTextNode("Jakaś treść"));

            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            ((XmlElement)toastNode).SetAttribute("duration", "long");

            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        private void DeleteNotification()
        {
            if (NotificationListView.SelectedItem == null)
            {
                ShowMessage("Brak wybranego elementu.");
            }
            else
            {
                NotificationItem notItem = NotificationListView.SelectedItem as NotificationItem;
                String id = notItem.Id;

                ToastNotifier notifier =
                    ToastNotificationManager.CreateToastNotifier();

                List<ScheduledToastNotification> initialList = new List<ScheduledToastNotification>();
                initialList = notifier.GetScheduledToastNotifications().ToList();

                foreach (ScheduledToastNotification item in initialList)
                {
                    if (item.Id == id)
                    { 
                        notifier.RemoveFromSchedule(item);
                        break;
                    }
                }

                NotificationList.Remove(notItem);
            
            }
        }

        private void GoToTimerPageButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(TimerPage));
        }

        private void GoToDatePageButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DatePage));
        }

        private void PrepareNotificationListView()
        {
            ToastNotifier notifier =
                    ToastNotificationManager.CreateToastNotifier();

            List<ScheduledToastNotification> initialList = new List<ScheduledToastNotification>();
            initialList = notifier.GetScheduledToastNotifications().ToList();

            var collection = ParseInitialListToNotificationItemList(initialList);
            NotificationList = new ObservableCollection<NotificationItem>(collection);

            NotificationListView.ItemsSource = NotificationList;

        }

        private void StartDTimer()
        {
            dTimer = new DispatcherTimer();
            dTimer.Interval = new TimeSpan(0, 0, 1);
            dTimer.Tick += ChangeSeconds;
            dTimer.Start();
        }


        private List<NotificationItem> ParseInitialListToNotificationItemList(List<ScheduledToastNotification> list)
        {
            List<NotificationItem> notItemList = new List<NotificationItem>();
                 
            foreach (ScheduledToastNotification item in list)
            {
                NotificationItem notItem = new NotificationItem();
                XmlDocument docXml = item.Content;

                XmlNodeList toastTextElement = docXml.GetElementsByTagName("text");

                notItem.Id = item.Id;
                notItem.Title = toastTextElement[0].FirstChild.InnerText;
                notItem.Content = toastTextElement[1].FirstChild.InnerText;

                notItem.DeliveryTime = item.DeliveryTime.DateTime;

                notItemList.Add(notItem);
            }

            return notItemList;
        }

        private void ChangeSeconds(object sender, object e)
        {
            List<NotificationItem> itemsToDelete = new List<NotificationItem>();
            foreach (NotificationItem notItem in NotificationList)
            {
                
                notItem.SecondsToEnd--;
                
                if (notItem.SecondsToEnd == 0)
                {
                    itemsToDelete.Add(notItem);
                }
            }

            foreach (NotificationItem item in itemsToDelete)
            {
                NotificationList.Remove(item);
            }
        }

        private async void ShowMessage(string message)
        {
            MessageDialog msg = new MessageDialog(message);
            await msg.ShowAsync();
        }
    }
}
