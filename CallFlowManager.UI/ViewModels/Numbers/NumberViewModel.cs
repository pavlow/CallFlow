using System.Collections.ObjectModel;
using System.Linq;
using CallFlowManager.UI.Common;
using CallFlowManager.UI.Models;
using CallFlowManager.UI.ViewModels.BusinessHours;

namespace CallFlowManager.UI.ViewModels.Numbers
{
    public class NumberViewModel : PropertyChangedBase
    {
        private string _name;
        private string _ownerPool;

        public NumberViewModel()
        {
            OpenCloseTimes = new ObservableCollection<OpenCloseTimeViewModel>();
        }

        public NumberViewModel(CsBusHoursGroup model)
        {
            _name = model.Name;
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
            get {  return _ownerPool; }
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