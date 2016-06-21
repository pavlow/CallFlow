using System;
using System.Collections.ObjectModel;
using CallFlowManager.UI.Common;

namespace CallFlowManager.UI.ViewModels.Groups
{
    public class GroupViewModel : PropertyChangedBase, ICloneable
    {
        private string _name;
        private string _description;
        private string _distributionGroup;
        private int _timeout = 10;
        private bool _isDistributionGroup = false;
        private bool _isGroupAgents = true;
        private bool _isGroupAgentSignIn;
        private string _routingMethod;
        private string _identity;
        private string _owner;
        private string _user;
        private string _ownerPool;

        public GroupViewModel()
        {
            Agents = new ObservableCollection<AgentGroupViewModel>();
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

        public string DistributionGroup
        {
            get { return _distributionGroup; }
            set
            {
                if (_distributionGroup != value)
                {
                    _distributionGroup = value;
                    OnPropertyChanged("DistributionGroup");
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

        public bool IsDistributionGroup
        {
            get { return _isDistributionGroup; }
            set
            {
                if (_isDistributionGroup != value)
                {
                    _isDistributionGroup = value;
                    OnPropertyChanged("IsDistributionGroup");
                    //OnPropertyChanged("IsDistributionGroup");

                    if (!value)
                    {
                        IsGroupAgents = true;
                    }

                    if (value)
                    {
                        IsGroupAgents = false;
                    }

                }
            }
        }

        public bool IsGroupAgents
        {
            get { return _isGroupAgents; }
            set
            {
                if (_isGroupAgents != value)
                {
                    _isGroupAgents = value;
                    OnPropertyChanged("IsGroupAgents");

                    if (!value)
                    {
                        IsDistributionGroup = true;
                    }

                    if (value)
                    {
                        IsDistributionGroup = false;
                    }
                }
            }
        }

        public string User
        {
            get { return _user; }
            set
            {
                if (_user != value)
                {
                    _user = value;
                    OnPropertyChanged("User");
                }
            }
        }

        

        public bool IsGroupAgentSignIn
        {
            get { return _isGroupAgentSignIn; }
            set
            {
                if (_isGroupAgentSignIn != value)
                {
                    _isGroupAgentSignIn = value;
                    OnPropertyChanged("IsGroupAgentSignIn");
                }
            }
        }

        public string RoutingMethod
        {
            get { return _routingMethod; }
            set
            {
                if (_routingMethod != value)
                {
                    _routingMethod = value;
                    OnPropertyChanged("RoutingMethod");
                }
            }
        }

        public string Owner
        {
            get { return _owner; }
            set
            {
                if (_owner != value)
                {
                    _owner = value;
                    OnPropertyChanged("Owner");
                }
            }
        }

        public string Identity
        {
            get { return _identity; }
            set
            {
                if (_identity != value)
                {
                    _identity = value;
                    OnPropertyChanged("Identity");
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


        public string ParticipationPolicy { get; set; }

        public ObservableCollection<AgentGroupViewModel> Agents { get; private set; }

        public object Clone()
        {
            return new GroupViewModel()
            {
                Name = Name,
                Description = Description,
                DistributionGroup = DistributionGroup,
                IsDistributionGroup = IsDistributionGroup,
                IsGroupAgentSignIn = IsGroupAgentSignIn,
                IsGroupAgents = IsGroupAgents,
                Timeout = Timeout,
                Agents = Agents,
                Identity = Identity,
                Owner = Owner,
                RoutingMethod = RoutingMethod,
                ParticipationPolicy = ParticipationPolicy,
                OwnerPool = OwnerPool
            };
        }
    }
}
