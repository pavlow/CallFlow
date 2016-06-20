using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CallFlowManager.UI.Common;
using CallFlowManager.UI.ViewModels.Groups;
using CallFlowManager.UI.ViewModels.Users;
using CallFlowManager.UI.ViewModels.WorkFlows;

namespace CallFlowManager.UI.ViewModels.Queues
{
    public class QueueViewModel : PropertyChangedBase, ICloneable
    {
        private string _name;
        private string _description;
        private string _timeoutSipDomain;
        private string _timeoutUri;
        private int _timeout = 10;
        private string _overflowSipDomain;
        private string _overFlowUri;
        private string _timeoutDestination;
        private string _overFlowDestination;
        private string _overFlowCandidate;
        private int _overFlow;
        private bool _timeOutOn;
        private bool _overFlowOn;
        private string _timeoutQueueId;
        private string _overflowQueueId;
        private QueueViewModel _overflowQueue;
        private QueueViewModel _timeoutQueue;
        private string _group;
        private string _id;
        private string _ownerPool;
        private QueuesViewModel _queuesViewModel;
        private IEnumerable<GroupViewModel> _groupsFiltered;
        private IEnumerable<QueueViewModel> _queuesFiltered;

        public QueueViewModel()
        {
            Groups = new ObservableCollection<QueueGroupViewModel>();
            //_groupsFiltered = new ObservableCollection<GroupViewModel>();
            Name = string.Empty;
            Id = string.Empty;

        }

        private void UpdateFilters()
        {
            if (Groups != null && QueuesViewModel != null)
            {
                GroupsFiltered =
                    QueuesViewModel.Groups.Where(w => w.OwnerPool.Equals(OwnerPool));
            }

            if (Groups != null && QueuesViewModel != null)
            {
                QueuesFiltered =
                    QueuesViewModel.Queues.Where(w => w.OwnerPool.Equals(OwnerPool) && w.Name != Name);
            }

        }
        
        public IEnumerable<GroupViewModel> GroupsFiltered
        {
            get { return _groupsFiltered; }
            set
            {
                if (_groupsFiltered != value)
                {
                    _groupsFiltered = value;
                    OnPropertyChanged("GroupsFiltered");
                }
            }
        }

        public IEnumerable<QueueViewModel> QueuesFiltered
        {
            get { return _queuesFiltered; }
            set
            {
                if (_queuesFiltered != value)
                {
                    _queuesFiltered = value;
                    OnPropertyChanged("QueuesFiltered");
                }
            }
        }

        public QueuesViewModel QueuesViewModel
        {
            get { return _queuesViewModel; }
            set
            {
                if (_queuesViewModel != value)
                {
                    _queuesViewModel = value;
                    OnPropertyChanged("QueuesViewModel");
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

        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged("Description");
                }
            }
        }

        public string TimeoutSipDomain
        {
            get { return _timeoutSipDomain; }
            set
            {
                if (_timeoutSipDomain != value)
                {
                    _timeoutSipDomain = value;
                    OnPropertyChanged("TimeoutSipDomain");
                }
            }
        }

        public string TimeoutDestination
        {
            get { return _timeoutDestination; }
            set
            {
                if (_timeoutDestination != value)
                {
                    _timeoutDestination = value;
                    OnPropertyChanged("TimeoutDestination");
                }
            }
        }

        public string OverFlowDestination
        {
            get { return _overFlowDestination; }
            set
            {
                if (_overFlowDestination != value)
                {
                    _overFlowDestination = value;
                    OnPropertyChanged("OverFlowDestination");
                }
            }
        }

        public string OverFlowSipDomain
        {
            get { return _overflowSipDomain; }
            set
            {
                if (_overflowSipDomain != value)
                {
                    _overflowSipDomain = value;
                    OnPropertyChanged("OverFlowSipDomain");
                }
            }
        }

        public string OverFlowCandidate
        {
            get { return _overFlowCandidate; }
            set
            {
                if (_overFlowCandidate != value)
                {
                    _overFlowCandidate = value;
                    OnPropertyChanged("OverFlowCandidate");
                }
            }
        }

        public string TimeoutUri
        {
            get { return _timeoutUri; }
            set
            {
                if (_timeoutUri != value)
                {
                    _timeoutUri = value;
                    OnPropertyChanged("TimeoutUri");
                }
            }
        }

        public bool TimeoutOn
        {
            get { return _timeOutOn; }
            set
            {
                if (_timeOutOn != value)
                {
                    _timeOutOn = value;
                    OnPropertyChanged("TimeoutOn");
                }
            }
        }

        public string TimeoutQueueId
        {
            get { return _timeoutQueueId; }
            set
            {
                if (_timeoutQueueId != value)
                {
                    _timeoutQueueId = value;
                    OnPropertyChanged("TimeoutQueueId");
                }
            }
        }

        public QueueViewModel TimeoutQueue
        {
            get { return _timeoutQueue; }
            set
            {
                if (_timeoutQueue != value)
                {
                    _timeoutQueue = value;
                    OnPropertyChanged("TimeoutQueue");
                }
            }
        }


        public string Group
        {
            get { return _group; }
            set
            {
                if (_group != value)
                {
                    _group = value;
                    OnPropertyChanged("Agent");
                }
            }
        }

        public bool OverflowOn
        {
            get { return _overFlowOn; }
            set
            {
                if (_overFlowOn != value)
                {
                    _overFlowOn = value;
                    OnPropertyChanged("OverflowOn");
                }
            }
        }

        public string OverFlowUri
        {
            get { return _overFlowUri; }
            set
            {
                if (_overFlowUri != value)
                {
                    _overFlowUri = value;
                    OnPropertyChanged("OverFlowUri");
                }
            }
        }

         public string OverFlowQueueId
        {
            get { return _overflowQueueId; }
            set
            {
                if (_overflowQueueId != value)
                {
                    _overflowQueueId = value;
                    OnPropertyChanged("OverFlowQueueId");
                }
            }
        }

        public QueueViewModel OverFlowQueue
        {
            get { return _overflowQueue; }
            set
            {
                if (_overflowQueue != value)
                {
                    _overflowQueue = value;
                    OnPropertyChanged("OverFlowQueue");
                }
            }
        }

        
        public int Timeout
        {
            get { return _timeout; }
            set
            {
                if (_timeout != value)
                {
                    _timeout = value;
                    OnPropertyChanged("Timeout");
                }
            }
        }

        public int OverFlow
        {
            get { return _overFlow; }
            set
            {
                if (_overFlow != value)
                {
                    _overFlow = value;
                    OnPropertyChanged("OverFlow");
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

        public string OwnerPool
        {
            get { return _ownerPool; }
            set
            {
                if (_ownerPool != value)
                {
                    _ownerPool = value;
                    UpdateFilters();
                    OnPropertyChanged("OwnerPool");
                }
            }
        }

        public ObservableCollection<QueueGroupViewModel> Groups { get; private set; }

        public object Clone()
        {
            return new QueueViewModel()
            {
                QueuesViewModel = QueuesViewModel,
                Name = Name,
                Id = Id,
                Description = Description,
                Timeout = Timeout,
                TimeoutDestination = TimeoutDestination,
                TimeoutSipDomain = TimeoutSipDomain,
                TimeoutUri = TimeoutUri,
                TimeoutQueueId = TimeoutQueueId,
                TimeoutQueue = TimeoutQueue,
                OverFlow = OverFlow,
                OverFlowDestination = OverFlowDestination,
                OverFlowSipDomain = OverFlowSipDomain,
                OverFlowUri = OverFlowUri,
                OverFlowCandidate = OverFlowCandidate,
                OverFlowQueueId = OverFlowQueueId,
                OverFlowQueue = OverFlowQueue,
                TimeoutOn = TimeoutOn,
                OverflowOn = OverflowOn,
                //OverFlowQueue = string.IsNullOrEmpty(OverFlowQueue.Id) ? null : OverFlowQueue,
                //TimeoutQueue = string.IsNullOrEmpty(TimeoutQueue.Id) ? null : TimeoutQueue,
                //OverFlowQueue = OverFlowQueue,
                //TimeoutQueue = TimeoutQueue,
                Groups = Groups,
                Group = Group,
                OwnerPool = OwnerPool
            };
        }
    }
}
