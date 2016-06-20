using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CallFlowManager.UI.Common;
using CallFlowManager.UI.Models;

namespace CallFlowManager.UI.ViewModels.Logs
{
    public class FilterLogViewModel : PropertyChangedBase
    {
        private FilterLog _filter = new FilterLog();

        public event EventHandler RequestCloseDialogEvent = delegate { };

        public event EventHandler DialogResultTrueEvent = delegate { };
        
        public FilterLogViewModel()
        {
            OkClickCommand = new RelayCommand(_ => OkClick());
            ExitCommand = new RelayCommand(_ => RequestCloseDialogEvent(this, EventArgs.Empty));
            _filter.IsInfo = true;
            _filter.IsPs = true;
        }

        public ICommand OkClickCommand { get; private set; }

        public ICommand ExitCommand { get; private set; }

        public FilterLog Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
                OnPropertyChanged("Filter");
            }
        }

        private void OkClick()
        {
            RequestCloseDialogEvent(this, EventArgs.Empty);
            DialogResultTrueEvent(this, EventArgs.Empty);
        }
    }
}
