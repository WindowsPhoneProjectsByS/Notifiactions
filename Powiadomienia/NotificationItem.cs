using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Powiadomienia
{
    public class NotificationItem : INotifyPropertyChanged
    {
        public String Id { get; set; }
        private string title;
        private string content;
        private DateTime deliveryTime;
        private int secondsToEnd;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        public DateTime DeliveryTime
        {
            set
            {
                deliveryTime = value;
                secondsToEnd = ToSeconds();
            }
        }

        public int SecondsToEnd
        {
            get { return secondsToEnd; }
            set
            {
                secondsToEnd = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SecondsToEnd"));
                OnPropertyChanged(new PropertyChangedEventArgs("RemaingTime")); 
            }
        }

        public String RemaingTime
        {
            get
            {
                string result = "";
                if (secondsToEnd > 3600 * 24)
                {
                    result = deliveryTime.Day + "/" 
                        + deliveryTime.Month + "/" 
                        + deliveryTime.Year + " " 
                        + deliveryTime.Hour + ":" 
                        + deliveryTime.Minute;
                }
                else
                {
                    int allSeconds = secondsToEnd;
                    int hours = allSeconds / 3600;
                    allSeconds = allSeconds % 3600;
                    int minutes = allSeconds / 60;
                    allSeconds = allSeconds % 60;
                    int seconds = allSeconds;

                    if (hours < 10) { result += "0" + hours;  }
                    else { result += hours; }

                    result += ":";

                    if (minutes < 10) { result += "0" + minutes; }
                    else { result += minutes; }

                    result += ":";

                    if (seconds < 10) { result += "0" + seconds; }
                    else { result += seconds; }

                }

                return result;
            }
        }

        private int ToSeconds()
        {
            TimeSpan timeSpace = deliveryTime - DateTime.Now;

            int seconds = timeSpace.Days * 24 * 60 * 60 +
                timeSpace.Hours * 60 * 60 +
                timeSpace.Minutes * 60 +
                timeSpace.Seconds;

            return seconds;
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}
