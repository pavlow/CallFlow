using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CallFlowManager.UI.Common;

namespace CallFlowManager.UI.ViewModels.Users
{
    public class UserViewModel : PropertyChangedBase, ICloneable, IDataErrorInfo
    {
        private string _identity;
        private string _voicePolicy;
        private string _voiceRoutingPolicy;
        private string _conferencingPolicy;
        private string _presencePolicy;
        private string _dialPlan;
        private string _locationPolicy;
        private string _clientPolicy;
        private string _clientVersionPolicy;
        private string _archivingPolicy;
        private string _exchangeArchivingPolicy;
        private string _pinPolicy;
        private string _externalAccessPolicy;
        private string _mobilityPolicy;
        private string _persistentChatPolicy;
        private string _userServicesPolicy;
        private string _callViaWorkPolicy;
        private string _thirdPartyVideoSystemPolicy;
        private string _hostedVoiceMail;
        private string _hostedVoicemailPolicy;
        private string _hostingProvider;
        private string _registrarPool;
        private bool _enabled;
        private string _sipAddress;
        private string _lineUri;
        private string _privateLine;
        private bool _enterpriseVoiceEnabled;
        private bool _exUmEnabled;
        private string _homeServer;
        private string _displayName;
        private string _samAccountName;
        private string _UserPolicyTemplate;

        public string UserPolicyTemplate
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

        public string VoicePolicy
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

        public string VoiceRoutingPolicy
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

        public string ConferencingPolicy
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

        public string PresencePolicy
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

        public string DialPlan
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

        public string LocationPolicy
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

        public string ClientPolicy
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

        public string ClientVersionPolicy
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

        public string ArchivingPolicy
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

        public string ExchangeArchivingPolicy
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

        public string PinPolicy
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

        public string ExternalAccessPolicy
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

        public string MobilityPolicy
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

        public string PersistentChatPolicy
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

        public string UserServicesPolicy
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

        public string CallViaWorkPolicy
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

        public string ThirdPartyVideoSystemPolicy
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

        public string HostedVoicemailPolicy
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

        public string RegistrarPool
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

        public bool Enabled
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

        public bool EnterpriseVoiceEnabled
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

        public bool ExUmEnabled
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
            return new UserViewModel()
            {
                Identity = _identity,
                //VoicePolicy = _voicePolicy,
                //VoiceRoutingPolicy = _voiceRoutingPolicy,
                //ConferencingPolicy = _conferencingPolicy,
                //PresencePolicy = _presencePolicy,
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

        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public string this[string propertyName]
        {
            get
            {
                string validationResult = null;
                switch (propertyName)
                {
                    case "DisplayName":
                        validationResult = ValidateName();
                        break;
                    case "SipAddress":
  //                      validationResult = ValidateSipAddress();
                        break;
                    case "LineUri":
  //                      validationResult = ValidateLineUri();
                        break;
                    default:
                        throw new ApplicationException("Unknown Property being validated on User");
                }
                return validationResult;
            }
        }

        private string ValidateName()
        {
            if (String.IsNullOrEmpty(this.DisplayName))
                return "Display Name needs to be entered";
            else if (this.DisplayName.Length < 5)
                return "Display Name should have more than 5 letters.";
            else
                return String.Empty;
        }
    }
}