using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Crm2011.Linq;
using CRMUtility;
using Crm2011.Linq.Connection;
using System.Configuration;

namespace Lync_WCF
{
    public class CrmConnection : IDisposable
    {

        public CrmConnection()
        {
            Connect();
        }

        public CrmDataContext Context { get; private set; }

        private void Connect()
        {
            try
            {
                // Ignore SSL certificate errors
                System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);

                Context = new CrmDataContext(
                        QuickConnection.Connect(
                        ConfigurationManager.AppSettings["crm.discourl"],
                        ConfigurationManager.AppSettings["crm.domain"],
                        ConfigurationManager.AppSettings["crm.username"],
                        ConfigurationManager.AppSettings["crm.password"],
                        ConfigurationManager.AppSettings["crm.org"]
                        ));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("Failed to connect to CRM. Error: {0}", ex.Message), ex);
            }
        }

        private static bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public Guid GetCustomerReservedNumberType()
        {
            return new Guid("ee663bd1-b30d-e211-b359-00155d010d02");

            //return (from _nt in Context.NewPhonenumbertypes
            //        where _nt.Statuscode == (int)NewPhonenumbertype.Enums.Statuscode.Active
            //        && _nt.NewName == "Customer Reserved"
            //        select _nt.NewPhonenumbertypeId).SingleOrDefault();
        }

        public Guid GetUnassignedNumberType()
        {
            return new Guid("85267cea-cb97-e111-a45a-00155dc88203");

            //return (from _unt in Context.NewPhonenumbertypes
            //        where _unt.Statuscode == (int)NewPhonenumbertype.Enums.Statuscode.Active
            //        && _unt.NewName == "UNASSIGNED"
            //        select _unt.NewPhonenumbertypeId).SingleOrDefault();
        }

        // Use a static method to avoid reloading call regions multiple times
        private static Dictionary<string, Guid> _callRegions;
        public Guid GetCallingRegion(string callingRegion)
        {
            if (_callRegions == null)
            {
                var regions = Context.NewCallingregions;
                _callRegions = new Dictionary<string, Guid>();
                foreach (var region in regions)
                {
                    _callRegions.Add(region.NewName, region.NewCallingregionId);
                }
            }

            if (!_callRegions.ContainsKey(callingRegion))
            {
                throw new NotFoundException(string.Format("Could not find the calling region with the name: {0}", callingRegion));
            }
            return _callRegions[callingRegion];
        }

        public Account GetAccount(string customerShortCode)
        {
            var result = Context.Accounts.FirstOrDefault(a => a.NewCPSMShortCode == customerShortCode);
            if (result == null) throw new NotFoundException(string.Format("Could not find the CRM account with the short code: {0}", customerShortCode));
            return result;
        }

        public Account GetAccountByName(string customerName)
        {
            var result = (from _acc in Context.Accounts
                          where _acc.StatusCode == (int)Account.Enums.StatusCode.Active
                          && _acc.Name == customerName
                          select _acc).FirstOrDefault();

            if (result == null) throw new NotFoundException(string.Format("Could not find the CRM account with the name: {0}", customerName));
            return result;
        }

         public string GetAccountShortCodeByAccountName(string customerName)
        {
            var result = (from _acc in Context.Accounts
                          where _acc.StatusCode == (int)Account.Enums.StatusCode.Active
                          && _acc.Name == customerName
                          select _acc.NewCPSMShortCode).FirstOrDefault();

            if (result == null) throw new NotFoundException(string.Format("Could not find the CRM account with the name: {0}", customerName));
            return result;
        }

        public NewPhonenumber GetPhoneNumber(string number)
        {
            return Context.NewPhonenumbers.FirstOrDefault(n => n.NewNumber == number);
        }

        public Guid GetPhoneNumberType(string numberType)
        {
            return (from _ntt in Context.NewPhonenumbertypes
                    where _ntt.Statuscode == (int)NewPhonenumbertype.Enums.Statuscode.Active
                    && _ntt.NewName == numberType
                    select _ntt.NewPhonenumbertypeId).SingleOrDefault();
        }

        public Contact GetContact(string samAccountName)
        {
            var contact = (from _con in Context.Contacts
                           where _con.StatusCode == (int)Contact.Enums.StatusCode.Active
                           && _con.NewSAMAccountName == samAccountName
                           select _con).SingleOrDefault();
            if (contact == null) throw new NotFoundException(string.Format("Could not find the contact with the SamAccountName: {0}.", samAccountName));
            return contact;
        }

        public void Dispose()
        {
            if (Context != null) Context.Dispose();
        }
    }
}