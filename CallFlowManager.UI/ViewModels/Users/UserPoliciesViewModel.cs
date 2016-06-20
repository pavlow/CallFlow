using System;
using System.Collections.ObjectModel;
using CallFlowManager.UI.Common;

namespace CallFlowManager.UI.ViewModels.Users
{
    public class UserPoliciesViewModel : PropertyChangedBase, ICloneable
    {
        private string _identity;
        private ObservableCollection<string> _voicePolicy = new ObservableCollection<string>();
        private ObservableCollection<string> _voiceRoutingPolicy = new ObservableCollection<string>();
        private ObservableCollection<string> _conferencingPolicy = new ObservableCollection<string>();
        private ObservableCollection<string> _presencePolicy = new ObservableCollection<string>();
        private ObservableCollection<string> _dialPlan = new ObservableCollection<string>();
        private ObservableCollection<string> _locationPolicy = new ObservableCollection<string>();
        private ObservableCollection<string> _clientPolicy = new ObservableCollection<string>();
        private ObservableCollection<string> _clientVersionPolicy = new ObservableCollection<string>();
        private ObservableCollection<string> _archivingPolicy = new ObservableCollection<string>();
        private ObservableCollection<string> _exchangeArchivingPolicy = new ObservableCollection<string>();
        private ObservableCollection<string> _pinPolicy = new ObservableCollection<string>();
        private ObservableCollection<string> _externalAccessPolicy = new ObservableCollection<string>();
        private ObservableCollection<string> _mobilityPolicy = new ObservableCollection<string>();
        private ObservableCollection<string> _persistentChatPolicy = new ObservableCollection<string>();
        private ObservableCollection<string> _userServicesPolicy = new ObservableCollection<string>();
        private ObservableCollection<string> _callViaWorkPolicy = new ObservableCollection<string>();
        private ObservableCollection<string> _thirdPartyVideoSystemPolicy = new ObservableCollection<string>();
        private string _hostedVoiceMail;
        private ObservableCollection<string> _hostedVoicemailPolicy = new ObservableCollection<string>();
        private string _hostingProvider;
        private ObservableCollection<string> _registrarPool = new ObservableCollection<string>();
        private string _enabled;
        private string _sipAddress;
        private string _lineUri;
        private string _privateLine;
        private string _enterpriseVoiceEnabled;
        private string _exUmEnabled;
        private string _homeServer;
        private string _displayName;
        private string _samAccountName;

        private ObservableCollection<string> _UserPolicyTemplate = new ObservableCollection<string>();

        public ObservableCollection<string> UserPolicyTemplate
        {
            get { return _UserPolicyTemplate; }
            set
            {
                if (_UserPolicyTemplate != value)
                {
                    _UserPolicyTemplate = value;
                    OnPropertyChanged("UserPolicyTemplate");
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



        public ObservableCollection<string> VoicePolicy// = new ObservableCollection<string>();
        {
            get { return _voicePolicy; }
            set
            {
                if (_voicePolicy != value)
                {
                    _voicePolicy = value;
                    OnPropertyChanged("VoicePolicy");
                }
            }
        }


        public ObservableCollection<string> VoiceRoutingPolicy
        {
            get { return _voiceRoutingPolicy; }
            set
            {
                if (_voiceRoutingPolicy != value)
                {
                    _voiceRoutingPolicy = value;
                    OnPropertyChanged("VoiceRoutingPolicy");
                }
            }
        }


        public ObservableCollection<string> ConferencingPolicy
        {
            get { return _conferencingPolicy; }
            set
            {
                if (_conferencingPolicy != value)
                {
                    _conferencingPolicy = value;
                    OnPropertyChanged("ConferencingPolicy");
                }
            }
        }


        public ObservableCollection<string> PresencePolicy
        {
            get { return _presencePolicy; }
            set
            {
                if (_presencePolicy != value)
                {
                    _presencePolicy = value;
                    OnPropertyChanged("PresencePolicy");
                }
            }
        }

        public ObservableCollection<string> DialPlan
        {
            get { return _dialPlan; }
            set
            {
                if (_dialPlan != value)
                {
                    _dialPlan = value;
                    OnPropertyChanged("DialPlan");
                }
            }
        }


        public ObservableCollection<string> LocationPolicy
        {
            get { return _locationPolicy; }
            set
            {
                if (_locationPolicy != value)
                {
                    _locationPolicy = value;
                    OnPropertyChanged("LocationPolicy");
                }
            }
        }

        public ObservableCollection<string> ClientPolicy
        {
            get { return _clientPolicy; }
            set
            {
                if (_clientPolicy != value)
                {
                    _clientPolicy = value;
                    OnPropertyChanged("ClientPolicy");
                }
            }
        }


        public ObservableCollection<string> ClientVersionPolicy
        {
            get { return _clientVersionPolicy; }
            set
            {
                if (_clientVersionPolicy != value)
                {
                    _clientVersionPolicy = value;
                    OnPropertyChanged("ClientVersionPolicy");
                }
            }
        }


        public ObservableCollection<string> ArchivingPolicy
        {
            get { return _archivingPolicy; }
            set
            {
                if (_archivingPolicy != value)
                {
                    _archivingPolicy = value;
                    OnPropertyChanged("ArchivingPolicy");
                }
            }
        }


        public ObservableCollection<string> ExchangeArchivingPolicy
        {
            get { return _exchangeArchivingPolicy; }
            set
            {
                if (_exchangeArchivingPolicy != value)
                {
                    _exchangeArchivingPolicy = value;
                    OnPropertyChanged("ExchangeArchivingPolicy");
                }
            }
        }


        public ObservableCollection<string> PinPolicy
        {
            get { return _pinPolicy; }
            set
            {
                if (_pinPolicy != value)
                {
                    _pinPolicy = value;
                    OnPropertyChanged("PinPolicy");
                }
            }
        }


        public ObservableCollection<string> ExternalAccessPolicy
        {
            get { return _externalAccessPolicy; }
            set
            {
                if (_externalAccessPolicy != value)
                {
                    _externalAccessPolicy = value;
                    OnPropertyChanged("ExternalAccessPolicy");
                }
            }
        }


        public ObservableCollection<string> MobilityPolicy
        {
            get { return _mobilityPolicy; }
            set
            {
                if (_mobilityPolicy != value)
                {
                    _mobilityPolicy = value;
                    OnPropertyChanged("MobilityPolicy");
                }
            }
        }

        public ObservableCollection<string> PersistentChatPolicy
        {
            get { return _persistentChatPolicy; }
            set
            {
                if (_persistentChatPolicy != value)
                {
                    _persistentChatPolicy = value;
                    OnPropertyChanged("PersistentChatPolicy");
                }
            }
        }


        public ObservableCollection<string> UserServicesPolicy
        {
            get { return _userServicesPolicy; }
            set
            {
                if (_userServicesPolicy != value)
                {
                    _userServicesPolicy = value;
                    OnPropertyChanged("UserServicesPolicy");
                }
            }
        }


        public ObservableCollection<string> CallViaWorkPolicy
        {
            get { return _callViaWorkPolicy; }
            set
            {
                if (_callViaWorkPolicy != value)
                {
                    _callViaWorkPolicy = value;
                    OnPropertyChanged("CallViaWorkPolicy");
                }
            }
        }


        public ObservableCollection<string> ThirdPartyVideoSystemPolicy
        {
            get { return _thirdPartyVideoSystemPolicy; }
            set
            {
                if (_thirdPartyVideoSystemPolicy != value)
                {
                    _thirdPartyVideoSystemPolicy = value;
                    OnPropertyChanged("ThirdPartyVideoSystemPolicy");
                }
            }
        }


    public string HostedVoiceMail
        {
            get { return _hostedVoiceMail; }
            set
            {
                if (_hostedVoiceMail != value)
                {
                    _hostedVoiceMail = value;
                    OnPropertyChanged("HostedVoiceMail");
                }
            }
        }


    public ObservableCollection<string> HostedVoicemailPolicy
        {
            get { return _hostedVoicemailPolicy; }
            set
            {
                if (_hostedVoicemailPolicy != value)
                {
                    _hostedVoicemailPolicy = value;
                    OnPropertyChanged("HostedVoicemailPolicy");
                }
            }
        }


    public string HostingProvider
        {
            get { return _hostingProvider; }
            set
            {
                if (_hostingProvider != value)
                {
                    _hostingProvider = value;
                    OnPropertyChanged("HostingProvider");
                }
            }
        }


    public ObservableCollection<string> RegistrarPool
        {
            get { return _registrarPool; }
            set
            {
                if (_registrarPool != value)
                {
                    _registrarPool = value;
                    OnPropertyChanged("RegistrarPool");
                }
            }
        }


    public string Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    OnPropertyChanged("Enabled");
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


    public string LineUri
        {
            get { return _lineUri; }
            set
            {
                if (_lineUri != value)
                {
                    _lineUri = value;
                    OnPropertyChanged("LineURI");
                }
            }
        }

        public string PrivateLine
        {
            get { return _privateLine; }
            set
            {
                if (_privateLine != value)
                {
                    _privateLine = value;
                    OnPropertyChanged("PrivateLine");
                }
            }
        }


    public string EnterpriseVoiceEnabled
        {
            get { return _enterpriseVoiceEnabled; }
            set
            {
                if (_enterpriseVoiceEnabled != value)
                {
                    _enterpriseVoiceEnabled = value;
                    OnPropertyChanged("EnterpriseVoiceEnabled");
                }
            }
        }


    public string ExUmEnabled
        {
            get { return _exUmEnabled; }
            set
            {
                if (_exUmEnabled != value)
                {
                    _exUmEnabled = value;
                    OnPropertyChanged("ExUmEnabled");
                }
            }
        }


    public string HomeServer
        {
            get { return _homeServer; }
            set
            {
                if (_homeServer != value)
                {
                    _homeServer = value;
                    OnPropertyChanged("HomeServer");
                }
            }
        }


    public string DisplayName
        {
            get { return _displayName; }
            set
            {
                if (_displayName != value)
                {
                    _displayName = value;
                    OnPropertyChanged("DisplayName");
                }
            }
        }

    public string SamAccountName
        {
            get { return _samAccountName; }
            set
            {
                if (_samAccountName != value)
                {
                    _samAccountName = value;
                    OnPropertyChanged("SamAccountName");
                }
            }
        }

        public object Clone()
        {
            return new UserPoliciesViewModel()
            {
                Identity = _identity,
                VoicePolicy = _voicePolicy,
                VoiceRoutingPolicy = _voiceRoutingPolicy,
                ConferencingPolicy = _conferencingPolicy,
                PresencePolicy = _presencePolicy,
                DialPlan = _dialPlan,
                LocationPolicy = _locationPolicy,
                ClientPolicy = _clientPolicy,
                ClientVersionPolicy = _clientVersionPolicy,
                ArchivingPolicy = _archivingPolicy,
                ExchangeArchivingPolicy = _exchangeArchivingPolicy,
                PinPolicy = _pinPolicy,
                ExternalAccessPolicy = _externalAccessPolicy,
                MobilityPolicy = _mobilityPolicy,
                PersistentChatPolicy = _persistentChatPolicy,
                UserServicesPolicy = _userServicesPolicy,
                CallViaWorkPolicy = _callViaWorkPolicy,
                ThirdPartyVideoSystemPolicy = _thirdPartyVideoSystemPolicy,
                HostedVoiceMail = _hostedVoiceMail,
                HostedVoicemailPolicy = _hostedVoicemailPolicy,
                HostingProvider = _hostingProvider,
                RegistrarPool = _registrarPool,
                Enabled = _enabled,
                SipAddress = _sipAddress,
                LineUri = _lineUri,
                PrivateLine = _privateLine,
                EnterpriseVoiceEnabled = _enterpriseVoiceEnabled,
                ExUmEnabled = _exUmEnabled,
                HomeServer = _homeServer,
                DisplayName = _displayName,
                SamAccountName = _samAccountName

            };
        }
    }
}