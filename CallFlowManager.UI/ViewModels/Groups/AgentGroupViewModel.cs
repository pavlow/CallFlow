using CallFlowManager.UI.Common;

namespace CallFlowManager.UI.ViewModels.Groups
{
    public class AgentGroupViewModel : PropertyChangedBase
    {
        private string _name;
        private string _sipAddress;
        private string _routingMethod;
        private string _memberOff;

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

        public string SipAddress
        {
            get { return _sipAddress; }
            set
            {
                if (_sipAddress != value)
                {
                    _sipAddress = value;
                    OnPropertyChanged("SipAddress");
                }
            }
        }

        public string MemberOff
        {
            get { return _memberOff; }
            set
            {
                if (_memberOff != value)
                {
                    _memberOff = value;
                    OnPropertyChanged("MemberOff");
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
