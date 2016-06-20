using System;
using CallFlowManager.UI.Common;

namespace CallFlowManager.UI.ViewModels
{
    public class DateTimeViewModel : PropertyChangedBase
    {
        private DateTime _dateTime;

        public DateTimeViewModel()
        {
            var now = DateTime.UtcNow;
            _dateTime = new DateTime(now.Year, now.Month, now.Day, 0, 1, 1);
        }

        public DateTimeViewModel(DateTime dateTime)
        {
            _dateTime = dateTime;
        }

        public DateTime DateTime
        {
            get { return _dateTime; }
            set
            {
                if (_dateTime != value)
                {
                    _dateTime = value.Date.AddHours(Hour).AddMinutes(Minute);
                    OnPropertyChanged("DateTime");
                }
            }
        }

        public int Hour
        {
            get { return _dateTime.Hour; }
            set
            {
                if (_dateTime.Hour != value)
                {
                    ChangeTime(value, _dateTime.Minute);
                    OnPropertyChanged("Hour");
                }
            }
        }

        public int Minute
        {
            get { return _dateTime.Minute; }
            set
            {
                if (_dateTime.Minute != value)
                {
                    ChangeTime(_dateTime.Hour, value);
                    OnPropertyChanged("Minute");
                }
            }
        }

        public string Date
        {
            get { return _dateTime.Date.ToString(); }            
        }
        
        public void ChangeTime(int hour, int minutes)
        {
            _dateTime = _dateTime.Date.AddHours(hour).AddMinutes(minutes);
        }

        public override string ToString()
        {
            return _dateTime.ToString("HH:mm:ss");
        }
    }
}