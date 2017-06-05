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
using Windows.System.Threading;
using Windows.UI.Core;
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
        ThreadPoolTimer periodicTimer;
        public static ObservableCollection<NotificationItem> NotificationList;
        public static int todayNotificationNumber = 0;
       
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;


            Debug.WriteLine("Przygotowanie list z powiadomieniami.");
            PrepareNotificationListView();

            StartPeriodicTimer();
            LatestTileService.UpdateLatestTaskTile();
        }

        

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            
        }

        private void RaiseNotification_Click(object sender, RoutedEventArgs e)
        {
            DeleteNotification();
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
                        IdService.Remove(item.Id);
                        break;
                    }
                }

                if (notItem.SecondsToEnd <= 3600 * 240)
                {
                    todayNotificationNumber--;
                    BadgeService.ShowBadgeWithNumber(todayNotificationNumber);
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

        private void StartPeriodicTimer()
        {
            periodicTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.High,
                () =>
                {
                    ChangeSeconds();
                });

            }, TimeSpan.FromSeconds(1));
        }

        private void ChangeSeconds()
        {
            List<NotificationItem> itemsToDelete = new List<NotificationItem>();
            foreach (NotificationItem notItem in NotificationList)
            {

                notItem.SecondsToEnd--;

                if (notItem.SecondsToEnd < 0)
                {
                    itemsToDelete.Add(notItem);
                    todayNotificationNumber--;
                    BadgeService.ShowBadgeWithNumber(todayNotificationNumber);
                    LatestTileService.UpdateLatestTaskTile();
                }
            }

            foreach (NotificationItem item in itemsToDelete)
            {
                NotificationList.Remove(item);
                IdService.Remove(item.Id);
            }
        }

       

        private List<NotificationItem> ParseInitialListToNotificationItemList(List<ScheduledToastNotification> list)
        {
            List<NotificationItem> notItemList = new List<NotificationItem>();
            
            if (list.Count > 0)
            {

                foreach (ScheduledToastNotification item in list)
                {
                    NotificationItem notItem = new NotificationItem();
                    XmlDocument docXml = item.Content;

                    XmlNodeList toastTextElement = docXml.GetElementsByTagName("text");

                    notItem.Id = item.Id;

                    try
                    {
                        notItem.Title = toastTextElement[0].FirstChild.InnerText;
                    }
                    catch (Exception e)
                    {
                        notItem.Title = "";
                    }

                    try
                    {
                        notItem.Content = toastTextElement[1].FirstChild.InnerText;
                    }
                    catch (Exception e)
                    {
                        notItem.Content = "";
                    }
                   

                    notItem.DeliveryTime = item.DeliveryTime.DateTime;

                    notItemList.Add(notItem);

                    IdService.AddNewId(Int32.Parse(item.Id));

                    DateTime deliveryTime = item.DeliveryTime.DateTime;
                    TimeSpan secondsSpan = deliveryTime.Subtract(DateTime.Now);
                    int seconds = (int)secondsSpan.TotalSeconds;

                    Debug.WriteLine("Iloość sekund powiadomienia: {0}", seconds);

                    if (seconds <= 3600 * 24)
                    {
                        todayNotificationNumber++;
                    }

                }
            }     


            Debug.WriteLine("Pokazanie ilości numerków na dzisiaj: {0}", todayNotificationNumber);
            BadgeService.ShowBadgeWithNumber(todayNotificationNumber);

            return notItemList;
        }

        private async void ShowMessage(string message)
        {
            MessageDialog msg = new MessageDialog(message);
            await msg.ShowAsync();
        }
    }
}
