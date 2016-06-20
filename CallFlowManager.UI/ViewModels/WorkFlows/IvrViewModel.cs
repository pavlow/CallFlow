using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CallFlowManager.UI.Common;
using CallFlowManager.UI.ViewModels.Queues;
using Microsoft.Win32;

namespace CallFlowManager.UI.ViewModels.WorkFlows
{
    public class IvrViewModel : PropertyChangedBase
    {
        private int _number;
        private string _name = "";
        private string _selectedInvoiceQueue;
        private IvrViewModel _parent;
        private readonly WorkFlowViewModel _parentViewModel;
        private QueueViewModel _invoiceQueue;
        private string _audioIvrTree;
        private string _audioIvrTreeFilePath;
        private string _textIvrMessage = "Required";

        public IvrViewModel(WorkFlowViewModel parentViewModel)
        {
            AddIvrCommand = new RelayCommand(_ => AddChildIvr());
            RemoveIvrCommand = new RelayCommand(_ => RemoveIvr());
            AudioIvrTreeSelectorCommand = new RelayCommand(_ => AudioIvrTreeSelector());
            AudioIvrTreeRemoveCommand = new RelayCommand(_ => AudioIvrTreeRemove());
            ChildIvrNodes = new ObservableCollection<IvrViewModel>();
            Numbers = Enumerable.Range(1, 9);
            _parentViewModel = parentViewModel;
            InvoiceQueue = new QueueViewModel();
        }

        public IEnumerable<int> Numbers { get; set; }

        public ObservableCollection<QueueViewModel> Queues { get; set; }

        public ObservableCollection<IvrViewModel> ChildIvrNodes { get; private set; }

        public int Number
        {
            get { return _number; }
            set
            {
                if (_number != value)
                {
                    _number = value;
                    OnPropertyChanged("Number");
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

        public string SelectedInvoiceQueue
        {
            get { return _selectedInvoiceQueue; }
            set
            {
                if (_selectedInvoiceQueue != value)
                {
                    _selectedInvoiceQueue = value;
                    OnPropertyChanged("SelectedInvoiceQueue");
                }
            }
        }

        public QueueViewModel InvoiceQueue
        {
            get { return _invoiceQueue; }
            set
            {
                if (_invoiceQueue != value)
                {
                    _invoiceQueue = value;
                    OnPropertyChanged("InvoiceQueue");
                }
            }
        }

        public IvrViewModel Parent
        {
            get { return _parent; }
            set
            {
                if (_parent != value)
                {
                    _parent = value;
                    OnPropertyChanged("Parent");
                }
            }
        }

        public bool CanAddChildNodes
        {
            get { return _parent == null; }
        }

        public bool ParentHasChild
        {
            get
            {
                if (_parent == null && ChildIvrNodes.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        public string AudioIvrTree
        {
            get { return _audioIvrTree; }
            set
            {
                if (_audioIvrTree != value)
                {
                    _audioIvrTree = value;
                    OnPropertyChanged("AudioIvrTree");
                }
            }
        }

        public string TextIvrMessage
        {
            get { return _textIvrMessage; }
            set
            {
                if (_textIvrMessage != value)
                {
                    _textIvrMessage = value;
                    OnPropertyChanged("TextIvrMessage");
                }
            }
        }

        public string AudioIvrTreeFilePath
        {
            get { return _audioIvrTreeFilePath; }
            set
            {
                if (_audioIvrTreeFilePath != value)
                {
                    _audioIvrTreeFilePath = value;
                    OnPropertyChanged("AudioIvrTreeFilePath");
                }
            }
        }
        public ICommand RemoveIvrCommand { get; private set; }

        public ICommand AddIvrCommand { get; private set; }

        public ICommand AudioIvrTreeSelectorCommand { get; private set; }

        public ICommand AudioIvrTreeRemoveCommand { get; private set; }
        

        private void AddChildIvr()
        {
            if (Parent == null)
            {
                ChildIvrNodes.Add(new IvrViewModel(_parentViewModel) { Parent = this });
                OnPropertyChanged("ParentHasChild");
            }
        }

        private void RemoveIvr()
        {
            if (Parent != null)
            {
                Parent.ChildIvrNodes.Remove(this);
                Parent.OnPropertyChanged("ParentHasChild");
            }
            else
            {
                _parentViewModel.RemoveParentIvr(this);
            }
        }

        private void AudioIvrTreeSelector()
        {
            var audioFilePath = FileHelper.AudioFileSelector();
            var audioFileName = audioFilePath.Substring(audioFilePath.LastIndexOf(@"\") + 1);
            AudioIvrTreeFilePath = audioFilePath;
            AudioIvrTree = audioFileName;
        }

        private void AudioIvrTreeRemove()
        {
            AudioIvrTreeFilePath = null;
            AudioIvrTree = null;
        }
        
        


    }
}
