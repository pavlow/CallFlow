using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Lync_WCF
{   
    [ServiceContract]
    public interface ILyncService
    {
        [OperationContract]
        List<PhoneNumberType> GetPhoneNumberTypes();

        [OperationContract]
        string GetAccountShortCode(string customerName);

        [OperationContract]
        string GetAccountName(string customerShortCode);

        [OperationContract]
        List<PhoneNumber> GetAssignedNumbers();

        [OperationContract]
        List<PhoneNumber> GetAvailableNumbers(string customerShortCode, string callingRegion);

        [OperationContract]
        void ReleaseNumberToGenericPool(string number);

        [OperationContract]
        void ReleaseNumberBackToCustomerPool(string number, string customerShortCode);

        [OperationContract]
        void ReleaseNumber(string number, string customerShortCode);

        [OperationContract]
        void SetNumber(string number, string numberType, string customerShortCode, string samAccountName, string sipUri,
                       string trunkFailForwardNumber, bool umEnabled, bool pilotNumber);

        [OperationContract]
        void UmDialPlanCreate(string domainController, string name, string regionCode, string accessNumber);

        [OperationContract]
        void UmDialPlanRemove(string domainController, string name);

        [OperationContract]
        void AddSubscriberContact(string name, string sipAddress, string registrarPool, string tenantOu, string accessNumber, string description, string ipPhone, string domainController);

        [OperationContract]
        void RemoveSubscriberContact(string sipAddress, string domainController);

        [OperationContract]
        void UpdateCsComputer();

        [OperationContract]
        string[] GetRegistrarPools();

        [OperationContract]
        void AddCallLocation(string PstnUsageName, string VoiceRouteName, string DialPlanName, string DialPlanIdentity,
                             string DialPlanDescription, string DialinConferencingRegion, string VoicePolicyName,
                             string VoicePolicyDescription, string DialPlanArea, string PstnGateways);

        [OperationContract]
        void RemoveCallLocation(string PstnUsageName, string VoiceRouteName, string DialPlanIdentity, string VoicePolicyName);

        [OperationContract]
        void AddSimpleUrl(string component, string domain, string url);

        [OperationContract]
        void RemoveSimpleUrl(string component, string domain);

        [OperationContract]
        void EnableLyncUser(string registrarPool, string identity, string sipAddress, string domainController);

        [OperationContract]
        void DisableLyncUser(string identity, string domainController);

        [OperationContract]
        void SetUserTelephonyOption(string identity, string domainController, TelephonyOption option, string lineUri, string lineServerUri);

        [OperationContract]
        void SetLyncUserGroupingId(string username, string customerOuPath, string primaryDomain);

        [OperationContract]
        void ClearLyncUserGroupingId(string username, string customerOuPath, string primaryDomain);

        [OperationContract]
        void GrantLyncUserPolicies(string identity, string domainController, string dialplan, string voice, string conferencing);

        [OperationContract]
        Policies GetLyncPolicy(PolicyType policy);

        [OperationContract]
        void EnableUMMailbox(string identity, string mailboxPolicy, string extensions, string pin, string sipAddress, string domainController);

        [OperationContract]
        void DisableUMMailbox(string identity, string domainController);

        [OperationContract]
        void ResetUMMailboxPin(string identity, string pin, string domainController);

        [OperationContract]
        List<Region> GetRegions();

        [OperationContract]
        void AddSipDomain(string domain);

        [OperationContract]
        void RemoveSipDomain(string domain);

   
    }
}
