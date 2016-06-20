using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CallFlowManager.UI.Common;
using System.Globalization;
using System.Windows;
using CallFlowManager.UI.Models;

namespace CallFlowManager.UI.ViewModels.Holidays
{
    public class HolidayTimeViewModel : PropertyChangedBase, ICloneable     //, IDataErrorInfo
    {
        private DateTimeViewModel _startDate = new DateTimeViewModel();
        private DateTimeViewModel _endDate = new DateTimeViewModel(new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 23, 59, 0));

        private string _holidayName;

        public HolidayTimeViewModel()
        {
            Name = string.Empty;
            StartHolidayDate = _startDate;
            EndHolidayDate = _endDate;
        }

        public HolidayTimeViewModel(CsHoliday holiday)
        {
            _holidayName = holiday.Name;
            _startDate = new DateTimeViewModel(DateTime.Parse(holiday.StartDate));
            _endDate = new DateTimeViewModel(DateTime.Parse(holiday.EndDate));
        }

        public HolidayTimeViewModel(string name, string startDate, string startTime, string endDate, string endTime)
        {
            _holidayName = name;

            string[] patterns = { "d/MM/yy H:mm", "d/MM/yyyy H:mm" };

            try
            {
                var dateValue = DateTime.ParseExact(String.Concat(startDate, " ", startTime), patterns,
                    CultureInfo.InvariantCulture, DateTimeStyles.None);
                _startDate = new DateTimeViewModel(dateValue);
                dateValue = DateTime.ParseExact(String.Concat(endDate, " ", endTime), patterns,
                    CultureInfo.InvariantCulture, DateTimeStyles.None);
                _endDate = new DateTimeViewModel(dateValue);
            }
            catch (FormatException)
            {
                MessageBox.Show("DateTime in CSV-file has not correct format");
            }
        }

        public string Name
        {
            get { return _holidayName; }
            set
            {
                if (_holidayName != value)
                {
                    _holidayName = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public DateTimeViewModel StartHolidayDate
        {
            get { return _startDate; }
            set
            {
                if (value != _startDate)
                {
                    //if (value.DateTime > _endDate.DateTime)
                    //{
                    //    throw new ValidationException("  -- DATE VALID EXCEPTION --   ");
                    //}
                    //else
                    {
                        _startDate = value;
                        OnPropertyChanged("StartHolidayDate");
                    }
                }
                
            }
        }

        public DateTimeViewModel EndHolidayDate
        {
            get { return _endDate; }
            set
            {
                if (value != _endDate)
                {
                    _endDate = value;
                    OnPropertyChanged("EndHolidayDate");
                }
            }
        }

        public object Clone()
        {
            return new HolidayTimeViewModel
            {
                Name = Name,
                StartHolidayDate = StartHolidayDate,
                EndHolidayDate = EndHolidayDate,
            };
        }
    }
}