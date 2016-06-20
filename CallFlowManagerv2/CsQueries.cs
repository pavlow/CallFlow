using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CallFlowManagerv2
{
    public class CsQueries
    {
        PsExecutor pS = new PsExecutor();
        public Collection<PSObject> Workflows;
        public Collection<PSObject> Queues;
        public Dictionary<string, string> CsQueuesList;
        public Collection<PSObject> Groups;
        public Collection<PSObject> BusHours;
        public Collection<PSObject> RegistrarPools;
        public Collection<PSObject> Users;
        public Collection<PSObject> UserRgsGrpMembership;
        public Collection<PSObject> Holidays;

        public Dictionary<string, string> UsersList { get; set; }
        //public Collection<PSObject> SipDomains;
        public Collection<PSObject> SipDomains { get; set; }
        //public List<string> SipDomainsList;
        public List<string> SipDomainsList { get; set; }

        public List<string> BusinessHoursGroups { get; set; }

        public List<string> RegistrarPoolsList { get; set; }

        public void GetCsWorkflows()
        {
            //Get the Workflows
            Workflows = pS.ExecuteSynchronously(@"Get-CsRgsWorkflow");
        }

        public void GetCsQueues()
        {
            //Get the Queues
            Queues = pS.ExecuteSynchronously(@"Get-CsRgsQueue");
            UpdateCsQueuesList();
        }

        public void UpdateCsQueuesList()
        {
            //if (CsQueuesList != null)
            //{
            //    CsQueuesList.Clear();
            //}
            CsQueuesList = new Dictionary<string, string>();
            foreach (dynamic queueName in Queues)
            {
                if (queueName != null)
                {
                    //CsQueuesList.Add(queueName.Identity.ToString(), queueName.Name.ToString());
                    CsQueuesList.Add(queueName.Identity.ServiceId.FullName.ToString() + "/" +
                                    queueName.Identity.InstanceId.ToString(), queueName.Name.ToString());

                    //MessageBox.Show(queueName.Identity.ServiceId.FullName.ToString() + "/" +
                    //                queueName.Identity.InstanceId.ToString());

                }
            }
        }


        public void GetCsGroups()
        {
            //Get the Groups
            Groups = pS.ExecuteSynchronously(@"Get-CsRgsAgentGroup");
        }

        public void GetCsUsers()
        {
            //Get the Workflows
            Users = pS.ExecuteSynchronously(@"Get-CsUser");
        }

        public void UpdateUsersList()
        {
            UsersList = new Dictionary<string, string>();

            foreach (dynamic user in Users)
            {
                if (user != null)
                {
                    UsersList.Add(user.Name + " (" + user.SipAddress + ")", user.SipAddress);
                }
            }
        }

        public void GetCsUserRgsGrpMembership(string sipAddress)
        {
            UserRgsGrpMembership = pS.ExecuteSynchronously(@"Get-CsRgsAgentGroup | Where {$_.AgentsByUri -Contains '" + sipAddress + "'} | Select-Object Name");
        }


        public void GetCsBusHours()
        {
            //Get the Business Hours
            BusHours = pS.ExecuteSynchronously(@"Get-CsRgsHoursOfBusiness");
        }


        public void GetRegistrarPools()
        {
            //Get Registrar Pools
            RegistrarPools = pS.ExecuteSynchronously(@"Get-CsPool | Where {$_.Services -like 'Registrar:*'}");

            if (RegistrarPools != null)
            {
                UpdateRegistrarPoolsList();
            }
        }

        public void UpdateRegistrarPoolsList()
        {
            RegistrarPoolsList = new List<string>();

            foreach (dynamic registrarPools in RegistrarPools)
            {
                if (registrarPools != null)
                {
                    RegistrarPoolsList.Add(registrarPools.Fqdn);
                }
            }
        }

        public void GetSipDomains()
        {
            //Get Sip Domains
            SipDomains = pS.ExecuteSynchronously(@"Get-CsSipDomain | Select Name");
        }

        public void UpdateSipDomainsList()
        {
            SipDomainsList = new List<string>();

            foreach (dynamic sipDomain in SipDomains)
            {
                if (sipDomain != null)
                {
                    SipDomainsList.Add(sipDomain.Name);
                }
            }
        }

        public void UpdateBusinessHoursGroups()
        {
            BusinessHoursGroups = new List<string>();

            foreach (dynamic busHour in BusHours)
            {
                if (busHour != null)
                {
                    BusinessHoursGroups.Add(busHour.Name);
                }
            }
        }

        public void GetCsHolidays()
        {
            //Get the holidays
            Holidays = pS.ExecuteSynchronously(@"Get-CsRgsHolidaySet");
        }
    }
}
