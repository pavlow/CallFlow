using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Crm2011.Linq;
using CRMUtility;
using Crm2011.Linq.Connection;
using System.Configuration;
using Microsoft.Xrm.Sdk;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.IO;
using System.Xml;


namespace Lync_WCF
{
    public class Service : ILyncService
    {
        #region "CRM Methods"

        public List<Region> GetRegions()
        {
            using (var connection = new CrmConnection())
            {
                var results = (from _cr in connection.Context.NewCallingregions
                               where _cr.Statuscode == (int)NewCallingregion.Enums.Statuscode.Active
                               && _cr.NewPublished == true
                               select new Region() { Display = _cr.NewName, AreaCode = _cr.NewPrefix }).ToList();
                // Only return regions with an area code
                return results.Where(r => !string.IsNullOrWhiteSpace(r.AreaCode)).ToList();
            }
        }

        public List<PhoneNumberType> GetPhoneNumberTypes()
        {
            using (var connection = new CrmConnection())
            {
                var results = (from _nt in connection.Context.NewPhonenumbertypes
                               where _nt.Statuscode == (int)NewPhonenumbertype.Enums.Statuscode.Active
                               select new PhoneNumberType() { NumberType = _nt.NewName }).ToList();
                return results;
            }
        }

        public List<PhoneNumber> GetAssignedNumbers()
        {
            using (var connection = new CrmConnection())
            {
                List<PhoneNumber> assignedNumbers = new List<PhoneNumber>();

                //Query Assigned Numbers 
                Guid CustomerReservedNumberType = connection.GetCustomerReservedNumberType();
                Guid UnassignedNumberType = connection.GetUnassignedNumberType();

                var assignedCustomerNumbers = (from acn in connection.Context.NewPhonenumbers
                                                where acn.Statuscode == (int)NewPhonenumber.Enums.Statuscode.Active
                                                && acn.NewAccountId != null
                                                && (acn.NewNumberTypeId != CustomerReservedNumberType || acn.NewNumberTypeId != UnassignedNumberType)                                             
                                                select acn).ToList().OrderBy(a => a.NewAccountIdName);

                foreach (NewPhonenumber number in assignedCustomerNumbers)
                {
                    assignedNumbers.Add(new PhoneNumber() { Number = number.NewNumber, AccountName = number.NewAccountIdName });
                }
                return assignedNumbers;
            }
        }

        public List<PhoneNumber> GetAvailableNumbers(string customerShortCode, string callingRegion)
        {
            using (var connection = new CrmConnection())
            {
                List<PhoneNumber> availableNumbers = new List<PhoneNumber>();
                Account crmAccount = connection.GetAccount(customerShortCode);
                Guid CallingRegion = connection.GetCallingRegion(callingRegion);

                //Query Available Numbers from the Pool reserved for the customer
                Guid CustomerReservedNumberType = connection.GetCustomerReservedNumberType();
                var availableCustomerNumbers = (from acn in connection.Context.NewPhonenumbers
                                                where acn.Statuscode == (int)NewPhonenumber.Enums.Statuscode.Active
                                                && acn.NewAccountId == crmAccount.AccountId
                                                && acn.NewNumberTypeId == CustomerReservedNumberType
                                                && acn.NewLocalCallingRegionId == CallingRegion
                                                select acn).ToList();

                foreach (NewPhonenumber number in availableCustomerNumbers)
                {
                    availableNumbers.Add(new PhoneNumber() { Number = number.NewNumber, Pool = PoolType.Customer, AccountName = number.NewAccountIdName });
                }

                //Query Available Numbers from the common Number Pool
                Guid UnassignedNumberType = connection.GetUnassignedNumberType();
                var availablePoolNumbers = (from apn in connection.Context.NewPhonenumbers
                                            where apn.Statuscode == (int)NewPhonenumber.Enums.Statuscode.Active
                                            && apn.NewAccountId == null
                                            && apn.NewNumberTypeId == UnassignedNumberType
                                            && apn.NewLocalCallingRegionId == CallingRegion
                                            select apn).ToList();

                foreach (NewPhonenumber number in availablePoolNumbers)
                {
                    availableNumbers.Add(new PhoneNumber() { Number = number.NewNumber, Pool = PoolType.General, AccountName = number.NewAccountIdName });
                }

                return availableNumbers;
            }
        }

        public string GetAccountShortCode(string customerName)
        {
            using (var connection = new CrmConnection())
            {
                return connection.GetAccountShortCodeByAccountName(customerName);
            }
        }

        public string GetAccountName(string customerShortCode)
        {
            using (var connection = new CrmConnection())
            {
                return connection.GetAccount(customerShortCode).Name;
            }
        }
       
        /// <summary>
        /// Release a number without changing the pool type. Use this method to release public and customer numbers.
        /// </summary>
        /// <param name="number">The number to release</param>
        /// <param name="customerShortCode">The customer short code, just in case the number is dedicated to a customer</param>
        public void ReleaseNumber(string number, string customerShortCode)
        {
            // Release a number without changing the pool type (ie: Customer pool / Public pool)
            bool isPublicNumber;
            System.Web.HttpContext.Current.Trace.Write(string.Format("Looking for the number {0} to release back to the pool", number));

            // Determine if the number is public or customer specific
            using (var connection = new CrmConnection())
            {
                var foundNumber = connection.GetPhoneNumber(number);
                if (foundNumber == null) return;
                Guid CustomerReservedNumberType = connection.GetCustomerReservedNumberType();
                isPublicNumber = foundNumber.NewNumberTypeId != CustomerReservedNumberType;
            }

            // Release the number accordingly
            if (isPublicNumber)
            {
                 System.Web.HttpContext.Current.Trace.Write(string.Format("Releasing the generic pool number {0}", number));
                 ReleaseNumberToGenericPool(number);
            }
            else
            {
                System.Web.HttpContext.Current.Trace.Write(string.Format("Releasing the pool number {0} back for customer {1}", number, customerShortCode));
                ReleaseNumberBackToCustomerPool(number, customerShortCode);
            }
        }

        public void ReleaseNumberBackToCustomerPool(string number, string customerShortCode)
        {
            using (var connection = new CrmConnection())
            {
                Account crmAccount = connection.GetAccount(customerShortCode);
                var foundNumber = connection.GetPhoneNumber(number);
                if (foundNumber == null) return;

                //set things to generic settings
                foundNumber.NewNumberTypeId = connection.GetCustomerReservedNumberType();
                foundNumber.NewServiceName = crmAccount.Name + " - Customer Reserved";
                foundNumber.NewSIPURI = "";
                foundNumber.NewPilotNumber = false;
                foundNumber.NewUMEnabled = false;

                //clear the Contact field
             
                EntityReference customerEntityReference = new EntityReference("new_phonenumber", foundNumber.NewPhonenumberId);
                if (customerEntityReference != null && customerEntityReference.Id != Guid.Empty)
                {
                    Entity customerEntity = new Entity(customerEntityReference.LogicalName);
                    customerEntity.Id = customerEntityReference.Id;
                    customerEntity.Attributes["new_contactid"] = null;
                    connection.Context.Sdk.Update(customerEntity);
                }
              
                connection.Context.Update(foundNumber);
            }
        }

        public void ReleaseNumberToGenericPool(string number)
        {
            using (var connection = new CrmConnection())
            {
                var foundNumber = connection.GetPhoneNumber(number);
                if (foundNumber == null) return;

                //set things to generic settings
                foundNumber.NewNumberTypeId = connection.GetUnassignedNumberType();
                foundNumber.NewServiceName = "UNASSIGNED";
                foundNumber.NewSIPURI = "";
                foundNumber.NewPilotNumber = false;
                foundNumber.NewUMEnabled = false;

                //clear the Contact field
                EntityReference customerEntityReference = new EntityReference("new_phonenumber", foundNumber.NewPhonenumberId);
                if (customerEntityReference != null && customerEntityReference.Id != Guid.Empty)
                {
                    Entity customerEntity = new Entity(customerEntityReference.LogicalName);
                    customerEntity.Id = customerEntityReference.Id;
                    customerEntity.Attributes["new_contactid"] = null;
                    connection.Context.Sdk.Update(customerEntity);
                }

                //clear the Account field
                EntityReference customerEntityReference2 = new EntityReference("new_phonenumber", foundNumber.NewPhonenumberId);
                if (customerEntityReference2 != null && customerEntityReference2.Id != Guid.Empty)
                {
                    Entity customerEntity2 = new Entity(customerEntityReference2.LogicalName);
                    customerEntity2.Id = customerEntityReference.Id;
                    customerEntity2.Attributes["new_accountid"] = null;
                    connection.Context.Sdk.Update(customerEntity2);
                }

                connection.Context.Update(foundNumber);
            }
        }

        public enum NumberType
        {
            LyncUser,
            LyncResponseGroup,
            ExchangeAutoAttendant,
            ExchangeSubscriberAccess,
            ForwardUnassignedNumber,
            CustomerReserved
        }

        /// <summary>
        /// Assign a number in CRM
        /// </summary>
        /// <param name="number">Number to assign</param>
        /// <param name="numberType">IE: Lync User, Exchange Subscriber Access</param>
        /// <param name="customerShortCode"></param>
        /// <param name="samAccountName"></param>
        /// <param name="sipUri">IE: sip:user@something.local</param>
        /// <param name="trunkFailForwardNumber">The user specified fail forward number</param>
        /// <param name="umEnabled">Set to true when the user has UM</param>
        /// <param name="pilotNumber">False by default</param>
        public void SetNumber(string number, string numberType, string customerShortCode, string samAccountName, string sipUri,
                              string trunkFailForwardNumber, bool umEnabled, bool pilotNumber)
        {
            using (var connection = new CrmConnection())
            {
                Account crmAccount = connection.GetAccount(customerShortCode);
                var numberTypeToSet = connection.GetPhoneNumberType(numberType);
                var numberToSet = connection.GetPhoneNumber(number);
                if (numberTypeToSet == null || numberToSet == null) throw new NotFoundException(string.Format("Unable to find the number {0} of type {1}.", number, numberType));
                numberToSet.NewNumberTypeId = numberTypeToSet;
                numberToSet.NewAccountId = crmAccount.AccountId;
                numberToSet.NewSIPURI = sipUri ?? "";
                numberToSet.NewPilotNumber = pilotNumber;

                switch (numberType)
                {
                    case "Lync User":
                        var contact = connection.GetContact(samAccountName);
                        numberToSet.NewContactId = contact.ContactId;
                        numberToSet.NewUMEnabled = umEnabled;
                        numberToSet.NewTrunkFailForwardNumber = trunkFailForwardNumber ?? "";

                        numberToSet.NewServiceName = crmAccount.Name + " - " +
                                                    numberType + " - " +
                                                    contact.FullName;

                        connection.Context.Update(numberToSet);
                        break;
                    case "Lync Response Group":
                        numberToSet.NewUMEnabled = false;
                        numberToSet.NewTrunkFailForwardNumber = trunkFailForwardNumber ?? "";

                        numberToSet.NewServiceName = crmAccount.Name + " - " +
                                                    numberType + " - " +
                                                    sipUri;

                        if (numberToSet.NewContactId != null)
                        {
                            //clear the Contact field
                            EntityReference customerEntityReference = new EntityReference("new_phonenumber", numberToSet.NewPhonenumberId);
                            if (customerEntityReference != null && customerEntityReference.Id != Guid.Empty)
                            {
                                Entity customerEntity = new Entity(customerEntityReference.LogicalName);
                                customerEntity.Id = customerEntityReference.Id;
                                customerEntity.Attributes["new_contactid"] = null;
                                connection.Context.Sdk.Update(customerEntity);
                            }
                        }
                        connection.Context.Update(numberToSet);
                        break;
                    case "Exchange Auto Attendant":
                        numberToSet.NewUMEnabled = true;
                        numberToSet.NewTrunkFailForwardNumber = trunkFailForwardNumber ?? "";

                        numberToSet.NewServiceName = crmAccount.Name + " - " +
                                                    numberType + " - " +
                                                    sipUri;

                        if (numberToSet.NewContactId != null)
                        {
                            //clear the Contact field
                            EntityReference customerEntityReference = new EntityReference("new_phonenumber", numberToSet.NewPhonenumberId);
                            if (customerEntityReference != null && customerEntityReference.Id != Guid.Empty)
                            {
                                Entity customerEntity = new Entity(customerEntityReference.LogicalName);
                                customerEntity.Id = customerEntityReference.Id;
                                customerEntity.Attributes["new_contactid"] = null;
                                connection.Context.Sdk.Update(customerEntity);
                            }
                        }

                        connection.Context.Update(numberToSet);
                        break;
                    case "Exchange Subscriber Access":
                        numberToSet.NewUMEnabled = false;
                        numberToSet.NewTrunkFailForwardNumber = trunkFailForwardNumber ?? "";
                        numberToSet.NewServiceName = crmAccount.Name + " - " + numberType;

                        if (numberToSet.NewContactId != null)
                        {
                            //clear the Contact field
                            EntityReference customerEntityReference = new EntityReference("new_phonenumber", numberToSet.NewPhonenumberId);
                            if (customerEntityReference != null && customerEntityReference.Id != Guid.Empty)
                            {
                                Entity customerEntity = new Entity(customerEntityReference.LogicalName);
                                customerEntity.Id = customerEntityReference.Id;
                                customerEntity.Attributes["new_contactid"] = null;
                                connection.Context.Sdk.Update(customerEntity);
                            }
                        }
                        connection.Context.Update(numberToSet);
                        break;
                    case "Forward (Unassigned Number)":
                        numberToSet.NewUMEnabled = false;
                        numberToSet.NewTrunkFailForwardNumber = trunkFailForwardNumber ?? "";

                        numberToSet.NewServiceName = crmAccount.Name + " - " +
                                                    numberType + " - " +
                                                    sipUri;

                        if (numberToSet.NewContactId != null)
                        {
                            //clear the Contact field
                            EntityReference customerEntityReference = new EntityReference("new_phonenumber", numberToSet.NewPhonenumberId);
                            if (customerEntityReference != null && customerEntityReference.Id != Guid.Empty)
                            {
                                Entity customerEntity = new Entity(customerEntityReference.LogicalName);
                                customerEntity.Id = customerEntityReference.Id;
                                customerEntity.Attributes["new_contactid"] = null;
                                connection.Context.Sdk.Update(customerEntity);
                            }
                        }
                        connection.Context.Update(numberToSet);
                        break;
                    case "Customer Reserved":
                        numberToSet.NewUMEnabled = false;
                        numberToSet.NewTrunkFailForwardNumber = trunkFailForwardNumber ?? "";

                        numberToSet.NewServiceName = crmAccount.Name + " - " +
                                                    numberType + " - " +
                                                    sipUri;

                        if (numberToSet.NewContactId != null)
                        {
                            //clear the Contact field
                            EntityReference customerEntityReference = new EntityReference("new_phonenumber", numberToSet.NewPhonenumberId);
                            if (customerEntityReference != null && customerEntityReference.Id != Guid.Empty)
                            {
                                Entity customerEntity = new Entity(customerEntityReference.LogicalName);
                                customerEntity.Id = customerEntityReference.Id;
                                customerEntity.Attributes["new_contactid"] = null;
                                connection.Context.Sdk.Update(customerEntity);
                            }
                        }
                        connection.Context.Update(numberToSet);
                        break;
                    default:
                        throw new InvalidOperationException(string.Format("An invalid number type was specified: {0}", numberType));
                }
            }
        }
        
        #endregion

        #region "Exchange Methods"

        private static string RemotingUrl = ConfigurationManager.AppSettings["RemotingUrl"].Trim();

        private string GetRemotingUrl()
        {
            if (string.IsNullOrWhiteSpace(RemotingUrl)) throw new ApplicationException("The remoting URL is not specified. Please configure the RemotingURL in the web.config");
            return RemotingUrl;
        }

        public void UmDialPlanCreate(string domainController, string name, string regionCode, string accessNumber)
        {
            var manager = new UmServerManager(domainController, GetRemotingUrl());
            manager.AddDialPlan(name, regionCode, accessNumber);
        }

        public void UmDialPlanRemove(string domainController, string name)
        {
            var manager = new UmServerManager(domainController, GetRemotingUrl());
            manager.RemoveDialPlan(name);
        }

        public void AddSubscriberContact(string name, string sipAddress, string registrarPool, string tenantOu, string accessNumber, string description, string ipPhone, string domainController)
        {
            var manager = new LyncServerManager();
            manager.AddSubscriberContact(name, sipAddress, registrarPool, tenantOu, accessNumber, description, ipPhone, domainController);
        }

        public void RemoveSubscriberContact(string sipAddress, string domainController)
        {
            var manager = new LyncServerManager();
            manager.RemoveSubscriberContact(sipAddress, domainController);
        }

        public void EnableUMMailbox(string identity, string mailboxPolicy, string extensions, string pin, string sipAddress, string domainController)
        {
            var manager = new UmUserManager(identity, domainController, GetRemotingUrl());
            manager.Enable(mailboxPolicy, extensions, pin, sipAddress);
        }

        public void DisableUMMailbox(string identity, string domainController)
        {
            var manager = new UmUserManager(identity, domainController, GetRemotingUrl());
            manager.Disable();
        }

        public void ResetUMMailboxPin(string identity, string pin, string domainController)
        {
            var manager = new UmUserManager(identity, domainController, GetRemotingUrl());
            manager.ResetPin(pin);
        }

        #endregion

        #region "Lync Methods"

        public void SetCsRgsAgentGroup(string Identity, string Name, string Description, string ParticipationPolicy, string AgentAlertTime, 
            string RoutingMethod, string DistributionGroupAddress, string OwnerPool, string AgentsByUri)
        {
            var manager = new LyncServerManager();
            manager.SetCsRgsAgentGroup(Identity, Name, Description, ParticipationPolicy, AgentAlertTime,
                RoutingMethod, DistributionGroupAddress, OwnerPool, AgentsByUri);
        }

        public void UpdateCsComputer()
        {
            var manager = new LyncServerManager();
            manager.EnableComputer();
        }

        public string[] GetRegistrarPools()
        {
            var manager = new LyncServerManager();
            return manager.GetRegistrarPools().ToArray();
        }

        public void AddSimpleUrl(string component, string domain, string url)
        {
            var manager = new LyncServerManager();
            manager.AddSimpleUrl(component, domain, url);
        }

        public void RemoveSimpleUrl(string component, string domain)
        {
            var manager = new LyncServerManager();
            manager.RemoveSimpleUrl(component, domain);
        }

        public void AddCallLocation(string PstnUsageName, string VoiceRouteName, string DialPlanName, string DialPlanIdentity,
                                    string DialPlanDescription, string DialinConferencingRegion, string VoicePolicyName,
                                    string VoicePolicyDescription, string DialPlanArea, string PstnGateways)
        {
            var manager = new LyncServerManager();
            manager.AddCallLocation(PstnUsageName, VoiceRouteName, DialPlanName, DialPlanIdentity, DialPlanDescription, DialinConferencingRegion, VoicePolicyName, VoicePolicyDescription, DialPlanArea, PstnGateways);
        }

        public void RemoveCallLocation(string PstnUsageName, string VoiceRouteName, string DialPlanIdentity, string VoicePolicyName)
        {
            var manager = new LyncServerManager();
            manager.RemoveCallLocation(PstnUsageName, VoiceRouteName, DialPlanIdentity, VoicePolicyName);
        }

        public void EnableLyncUser(string registrarPool, string identity, string sipAddress, string domainController)
        {
            var manager = new LyncUserManager(identity, domainController);
            manager.Enable(registrarPool, sipAddress);
        }

        public void DisableLyncUser(string identity, string domainController)
        {
            var manager = new LyncUserManager(identity, domainController);
            manager.Disable();
        }

        public void SetUserTelephonyOption(string identity, string domainController, TelephonyOption option, string lineUri, string lineServerUri)
        {
            var manager = new LyncUserManager(identity, domainController);
            manager.SetUserTelephonyOption(option, lineUri, lineServerUri);
        }

        public void SetLyncUserGroupingId(string username, string customerOuPath, string primaryDomain)
        {
            var manager = new LyncUserManager(username);
            manager.SetUserGroupingId(customerOuPath, primaryDomain);
        }

        public void ClearLyncUserGroupingId(string username, string customerOuPath, string primaryDomain)
        {
            var manager = new LyncUserManager(username);
            manager.ClearUserGroupingId(customerOuPath, primaryDomain);
        }

        public void GrantLyncUserPolicies(string identity, string domainController, string dialplan, string voice, string conferencing)
        {
            var manager = new LyncUserManager(identity, domainController);
            manager.GrantPolicies(dialplan, voice, conferencing);
        }

        public Policies GetLyncPolicy(PolicyType policy)
        {
            var manager = new LyncPolicyManager();
            return manager.GetPolicies(policy);
        }

        public void AddSipDomain(string domain)
        {
            var manager = new LyncDomainManager();
            manager.Add(domain);
        }

        public void RemoveSipDomain(string domain)
        {
            var manager = new LyncDomainManager();
            manager.Remove(domain);
        }
                
        #endregion     


    }
}