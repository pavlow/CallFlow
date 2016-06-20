using System.Collections.ObjectModel;
using System.Linq;
using CallFlowManager.UI.Common;
using CallFlowManager.UI.Models;

namespace CallFlowManager.UI.ViewModels.BusinessHours
{
    public class BusinessHourGroupViewModel : PropertyChangedBase
    {
        private string _name;
        private string _ownerPool;

        public BusinessHourGroupViewModel()
        {
            OpenCloseTimes = new ObservableCollection<OpenCloseTimeViewModel>();
        }

        public BusinessHourGroupViewModel(CsBusHoursGroup model)
        {
            _name = model.Name;
            Id = model.Identity;
            _ownerPool = model.OwnerPool;

            OpenCloseTimes = new ObservableCollection<OpenCloseTimeViewModel>();

            if (model.BusinessHours != null)
            {
                foreach (var businessHour in model.BusinessHours)
                {
                    OpenCloseTimes.Add(new OpenCloseTimeViewModel(businessHour));
                }
            }
        }

        public string Id { get; set; }

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

        public string OwnerPool
        {
            get { return _ownerPool; }
            set
            {
                if (_ownerPool != value)
                {
                    _ownerPool = value;
                    OnPropertyChanged("OwnerPool");
                }
            }
        }

        public ObservableCollection<OpenCloseTimeViewModel> OpenCloseTimes { get; private set; }

        public void ResetOpenCloseTimes()
        {
            var allItems = OpenCloseTimes.ToList();
            foreach (var openCloseTimeViewModel in allItems)
            {
                OpenCloseTimes.Remove(openCloseTimeViewModel);
            }
        }
    }
}