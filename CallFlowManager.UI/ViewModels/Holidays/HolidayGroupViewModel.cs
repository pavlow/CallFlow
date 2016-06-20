using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CallFlowManager.UI.Common;
using CallFlowManager.UI.Models;

namespace CallFlowManager.UI.ViewModels.Holidays
{
    public class HolidayGroupViewModel : PropertyChangedBase
    {
        private string _name;
        private string _ownerPool;
        private string _id;
//        private List<string> _pools;

        public HolidayGroupViewModel()
        {
            Holidays = new ObservableCollection<HolidayTimeViewModel>();
        }

        public HolidayGroupViewModel(CsHolidayGroup model)
        {
            _name = model.Name;
            _ownerPool = model.OwnerPool;
            _id = model.Identity;

            Holidays = new ObservableCollection<HolidayTimeViewModel>();

            if (model.Holidays != null)
            {
                foreach (var holidays in model.Holidays)
                {
                    Holidays.Add(new HolidayTimeViewModel(holidays));
                }
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public string Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged("Id");
                }
            }
        }

        public List<string> Pools { get; set; }

        public ObservableCollection<HolidayTimeViewModel> Holidays { get; private set; }

        public void ResetHolidayTimes()
        {
            var allItems = Holidays.ToList();
            foreach (var item in allItems)
            {
                Holidays.Remove(item);
            }
        }

        public string OwnerPool
        {
            get 
            {
                    return _ownerPool;
            }

            set
            {
                if (_ownerPool != value)
                {
                    _ownerPool = value;
                    OnPropertyChanged("OwnerPool");
                }
            }
        }

        public void AddHoliday(HolidayTimeViewModel holiday, bool mute)
        {
            if (Holidays.Any(p => p.Name.Contains(holiday.Name)))
            {
                if (!mute)
                {
                    MsgBox.Error(string.Format("Holday {0} EXIST AND WILL BE EDIT - TODO", holiday.Name));
                }

                HolidayTimeViewModel editHoliday = Holidays.SingleOrDefault(p => p.Name.Equals(holiday.Name));

                //                   from holi in Holidays where holi.Name.Contains(holiday.Name) select new {(HolidayTimeViewModel)holi};

                //                List<Teacher> teachersMal = teachers.FindAll(t => t.name.StartsWith("Мал"));  

                // TODO - EDIT !
            }
            else
            {
                Holidays.Add(holiday);
            }
        }
    }
}
