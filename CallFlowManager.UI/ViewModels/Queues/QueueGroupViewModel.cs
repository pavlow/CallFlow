using System.Collections.Generic;
using CallFlowManager.UI.Common;
using CallFlowManager.UI.ViewModels.BusinessHours;

namespace CallFlowManager.UI.ViewModels.Queues
{
    public class QueueGroupViewModel : PropertyChangedBase
    {
        private string _alertTime;
        private string _name;
        private string _participationPolicy;
        private string _routingMethod;

        public QueueGroupViewModel(string name, string alertTime, string participationPolicy, string routingMethod)
        {
            Name = name;
            AlertTime = alertTime;
            ParticipationPolicy = participationPolicy;
            RoutingMethod = routingMethod;
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

        public string AlertTime
        {
            get { return _alertTime; }
            set
            {
                if (_alertTime != value)
                {
                    _alertTime = value;
                    OnPropertyChanged("AlertTime");
                }
            }
        }

        public string ParticipationPolicy
        {
            get { return _participationPolicy; }
            set
            {
                if (_participationPolicy != value)
                {
                    _participationPolicy = value;
                    OnPropertyChanged("ParticipationPolicy");
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
    }
}
