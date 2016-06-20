using System;
using CallFlowManager.UI.Common;
using CallFlowManager.UI.Models;

namespace CallFlowManager.UI.ViewModels.BusinessHours
{
    public class OpenCloseTimeViewModel : PropertyChangedBase, ICloneable
    {
        private string _dayOfWeek;
        private DateTimeViewModel _openTime1;
        private DateTimeViewModel _closeTime1;

        private DateTimeViewModel _openTime2;
        private DateTimeViewModel _closeTime2;

        private bool _openCloseTime1Enabled;
        private bool _openCloseTime2Enabled;

        public new event EventHandler PropertyDayOfWeekChanged = delegate { };

        public OpenCloseTimeViewModel()
        {
            DayOfWeek = "MonSun";
            OpenTime1 = new DateTimeViewModel { Hour = 8, Minute = 0 };
            OpenTime2 = new DateTimeViewModel { Hour = 8, Minute = 0 };
            CloseTime1 = new DateTimeViewModel { Hour = 17, Minute = 0 };
            CloseTime2 = new DateTimeViewModel { Hour = 17, Minute = 0 }; 
            OpenCloseTime1Enabled = true;
        }

        public OpenCloseTimeViewModel(CsBusHours hours)
        {
            _dayOfWeek = hours.Day;
            _openTime1 =
               new DateTimeViewModel(string.IsNullOrEmpty(hours.OpenTime1)
                   ? DateTime.MinValue
                   : DateTime.ParseExact(hours.OpenTime1, "HH:mm:ss", null));
         
             _closeTime1 =
                new DateTimeViewModel(string.IsNullOrEmpty(hours.CloseTime1)
                    ? DateTime.MinValue
                    : DateTime.ParseExact(hours.CloseTime1, "HH:mm:ss", null));
         
            _openTime2 =
                new DateTimeViewModel(string.IsNullOrEmpty(hours.OpenTime2)
                    ? DateTime.MinValue
                    : DateTime.ParseExact(hours.OpenTime2, "HH:mm:ss", null));

            _closeTime2 = new DateTimeViewModel(string.IsNullOrEmpty(hours.CloseTime2) ? DateTime.MinValue : DateTime.ParseExact(hours.CloseTime2, "HH:mm:ss", null));
            OpenCloseTime1Enabled = true;
        }

        public string DayOfWeek
        {
            get { return _dayOfWeek; }
            set
            {
                if (_dayOfWeek != value)
                {
                    _dayOfWeek = value;
                    OnPropertyChanged("DayOfWeek");
                    PropertyDayOfWeekChanged(this, EventArgs.Empty);
                }
            }
        }

        public bool OpenCloseTime1Enabled
        {
            get
            {
                return _openCloseTime1Enabled;
            }
            set
            {
                if (value != _openCloseTime1Enabled)
                {
                    _openCloseTime1Enabled = value;
                    OnPropertyChanged("OpenCloseTime1Enabled");
                }
            }
        }

        public bool OpenCloseTime2Enabled
        {
            get
            {
                return _openCloseTime2Enabled;
            }
            set
            {
                if (value != _openCloseTime2Enabled)
                {
                    _openCloseTime2Enabled = value;
                    OnPropertyChanged("OpenCloseTime2Enabled");
                }
            }
        }

        public DateTimeViewModel OpenTime1
        {
            get { return _openTime1; }
            set
            {
                if (value != _openTime1)
                {
                    _openTime1 = value;
                    OnPropertyChanged("OpenTime1");
                }
            }
        }

        public DateTimeViewModel CloseTime1
        {
            get { return _closeTime1; }
            set
            {
                if (value != _closeTime1)
                {
                    _closeTime1 = value;
                    OnPropertyChanged("CloseTime1");
                }
            }
        }

        public DateTimeViewModel OpenTime2
        {
            get { return _openTime2; }
            set
            {
                if (value != _openTime2)
                {
                    _openTime2 = value;
                    OnPropertyChanged("OpenTime2");
                }
            }
        }

        public DateTimeViewModel CloseTime2
        {
            get { return _closeTime2; }
            set
            {
                if (value != _closeTime2)
                {
                    _closeTime2 = value;
                    OnPropertyChanged("CloseTime2");
                }
            }
        }

        public object Clone()
        {
            return new OpenCloseTimeViewModel
            {
                DayOfWeek = DayOfWeek,
                OpenTime1 = OpenTime1,
                OpenTime2 = OpenTime2,
                CloseTime1 = CloseTime1,
                CloseTime2 = CloseTime2,
                OpenCloseTime1Enabled = OpenCloseTime1Enabled,
                OpenCloseTime2Enabled = OpenCloseTime2Enabled
            };
        }
    }
}