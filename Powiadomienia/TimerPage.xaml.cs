﻿using Powiadomienia.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Powiadomienia
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TimerPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public TimerPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            initComboBox();
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void initComboBox()
        {
            initHourCombobox();
            initMinuteComboBox();
            initSecondComboBox();
        }

        private void initHourCombobox()
        {
            for (int i = 0; i < 25; i++)
            {
                HourComboBox.Items.Add(i.ToString());
            }

            HourComboBox.SelectedIndex = 0;
        }

        private void initMinuteComboBox()
        {
            for (int i = 0; i < 60; i++)
            {
                MinuteComboBox.Items.Add(i.ToString());
            }

            MinuteComboBox.SelectedIndex = 0;
        }

        private void initSecondComboBox()
        {
            for (int i = 0; i < 60; i++)
            {
                SecondComboBox.Items.Add(i.ToString());
            }

            SecondComboBox.SelectedIndex = 1;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddSheduleNotification();
        }

        private void AddSheduleNotification()
        {
            string title = Title.Text;
            string content = Content.Text;

            int seconds = PrepareTimeSpan();

            ToastTemplateType toastType = ToastTemplateType.ToastText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastType);

            XmlNodeList toastTextElement = toastXml.GetElementsByTagName("text");
            toastTextElement[0].AppendChild(toastXml.CreateTextNode(title));
            toastTextElement[1].AppendChild(toastXml.CreateTextNode(content));

            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            ((XmlElement)toastNode).SetAttribute("duration", "long");

            ScheduledToastNotification sheduledToast = new ScheduledToastNotification(toastXml, DateTimeOffset.Now.AddSeconds(seconds));
            sheduledToast.Id = IdService.GetNewId();

            ToastNotifier notifier =
                    ToastNotificationManager.CreateToastNotifier();

            notifier.AddToSchedule(sheduledToast);

            NotificationItem notItem = new NotificationItem();
            notItem.Title = title;
            notItem.Content = content;
            notItem.SecondsToEnd = seconds;
            notItem.Id = sheduledToast.Id;

            MainPage.NotificationList.Add(notItem);

            Frame.GoBack();

            //ShowMessage("Dodano powiadowmienie.");
        }

        private int PrepareTimeSpan()
        {
            int hours = Int32.Parse(HourComboBox.SelectedItem.ToString());
            int minutes = Int32.Parse(MinuteComboBox.SelectedItem.ToString());
            int seconds = Int32.Parse(SecondComboBox.SelectedItem.ToString());
            int secondsResult = hours * 60 * 60 + minutes * 60 + seconds;

            return secondsResult;
        }

        private async void ShowMessage(string message)
        {
            MessageDialog msg = new MessageDialog(message);
            await msg.ShowAsync();
        }
    }
}
