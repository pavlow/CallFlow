using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using Lync_WCF;

namespace CallFlowManager.UI
{
    public class PsQueriesBeforePsTidy
    {
        PsExecutor pS = new PsExecutor();
        public ICollection<PSObject> Workflows;
        public ICollection<PSObject> Queues;
        public Dictionary<string, string> CsQueuesList;
        public ICollection<PSObject> Groups;
        public ICollection<PSObject> BusHours;
        public ICollection<PSObject> RegistrarPools;
        public ICollection<PSObject> Users;
        public Collection<PSObject> UserRgsGrpMembership;
        public ICollection<PSObject> Holidays;


        public Dictionary<string, string> UsersList { get; set; }
        //public Collection<PSObject> SipDomains;
        public ICollection<PSObject> SipDomains { get; set; }
        //public List<string> SipDomainsList;
        public List<string> SipDomainsList { get; set; }

        public List<string> BusinessHoursGroups { get; set; }

        public List<string> RegistrarPoolsList { get; set; }

        //AT
        public ICollection<PSObject> UnassignedNumbers;
        //AT - Andrew Voice Features testing

        private bool PsLyncModuleIsAvailable;
        private bool PsSfBModuleIsAvailable;
        private bool PsLocalCommandIsAvailable;
        //private bool PsRemotingIsAvailable;
        //private bool PSRemotingCustomCredentialsIsAvailable;

        //private string RemoteMachineFqdn = "S4BFE01.ucgeek.nz";
        //private int RemoteMachinePort = 443;
        //private string RemoteMachinePath = "/OCSPowerShell/";
        //private string Username = @"UCGEEK\dev";
        //private bool SslEnabled = true;
        //private SecureString Password = new SecureString();

        public PsQueriesBeforePsTidy()
        {
            //Check PS connection types
            //string testCommand = ReadScriptFile();
            //PsLocalCommandIsAvailable = pS.CheckLocalCommand("Get-CsPool");
            //PsLyncModuleIsAvailable = pS.CheckLocalModules("Lync");
            //PsSfBModuleIsAvailable = pS.CheckLocalModules("SkypeForBusiness");
        }
        
        private Collection<PSObject> RunQuery(string pSCommand)
        {
            if (PsLocalCommandIsAvailable)
            {
                return pS.ExecuteSynchronously(pSCommand);
            }
            else if (PsSfBModuleIsAvailable)
            {
                return pS.ExecuteSynchronously("Import-Module SkypeForBusiness; " + pSCommand);
            }
            else if (PsLyncModuleIsAvailable)
            {
                return pS.ExecuteSynchronously("Import-Module Lync; " + pSCommand);
            }
            else
            {
                return null;
            }
        }

        public void GetCsUnassignedNumbers()
        {
            //UnassignedNumbers = pS.ExecuteSynchronously("Get-CsUnassignedNumber");
            //UnassignedNumbers = RunQuery("Get-CsUnassignedNumber;");

            var lyncServerMan = new LyncServerManager();
            UnassignedNumbers = lyncServerMan.GetCsUnassignedNumber();
        }

        public void GetCsWorkflows()
        {
            //Get the Workflows
            //Workflows = RunQuery("Get-CsRgsWorkflow;");
            var lyncServerMan = new LyncServerManager();
            Workflows = lyncServerMan.GetCsRgsWorkflow();
        }

        public void GetCsQueues()
        {
            //Get the Queues
            //Queues = RunQuery("Get-CsRgsQueue;");

            var lyncServerMan = new LyncServerManager();
            Queues = lyncServerMan.GetCsRgsQueue();            
            UpdateCsQueuesList();
        }

        public void UpdateCsQueuesList()
        {
            CsQueuesList = new Dictionary<string, string>();

            //List<KeyValuePair<string, int>> sorted = (from kv in CsQueuesList orderby kv.Value select kv).ToList();

            foreach (dynamic queueName in Queues)
            {
                if (queueName != null)
                {
                    CsQueuesList.Add(queueName.Identity.ServiceId.FullName.ToString() + "/" +
                                    queueName.Identity.InstanceId.ToString(), queueName.Name.ToString());
                }
            }
        }


        public void GetCsGroups()
        {
            //Get the Groups
            //Groups = RunQuery("Get-CsRgsAgentGroup;");

            var lyncServerMan = new LyncServerManager();
            Groups = lyncServerMan.GetCsRgsAgentGroup();
        }

        public void GetCsUsers()
        {
            //Get the Workflows
            //Users = RunQuery("Get-CsUser;");

            var lyncServerMan = new LyncServerManager();
            Users = lyncServerMan.GetCsUser();
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
            //UserRgsGrpMembership = RunQuery("Get-CsRgsAgentGroup | Where {$_.AgentsByUri -Contains '" + sipAddress + "'} | Select-Object Name;");
        }


        public void GetCsBusHours()
        {
            //Get the Business Hours
            //BusHours = RunQuery("Get-CsRgsHoursOfBusiness;");

            var lyncServerMan = new LyncServerManager();
            BusHours = lyncServerMan.GetCsRgsHoursOfBusiness();
        }


        public void GetRegistrarPools()
        {
            //Get Registrar Pools
            //RegistrarPools = RunQuery("\r\nGet-CsPool | Where-Object {$_.Services -like 'Registrar:*'};\r\n");

            var lyncServerMan = new LyncServerManager();
            RegistrarPools = lyncServerMan.GetCsPool_Registrar();

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
            //SipDomains = RunQuery("\r\nGet-CsSipDomain | Select-Object Name;\r\n");

            var lyncServerMan = new LyncServerManager();
            SipDomains = lyncServerMan.GetCsSipDomain();
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
            //Holidays = RunQuery("Get-CsRgsHolidaySet;");

            var lyncServerMan = new LyncServerManager();
            Holidays = lyncServerMan.GetCsRgsHolidaySet();
        }
        
        private string ReadScriptFile(string path)
        {
            return System.IO.File.ReadAllText(path);
        }

    }
}
