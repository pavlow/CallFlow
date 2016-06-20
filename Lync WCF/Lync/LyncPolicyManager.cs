using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace Lync_WCF
{
    public class LyncPolicyManager
    {
        private readonly PsFactory _factory = new LyncPsFactory();
        private string[] _getPoliciesScripts;

        public LyncPolicyManager()
        {
            int length = Enum.GetNames(typeof(PolicyType)).Length;
            _getPoliciesScripts = new string[length];
            _getPoliciesScripts[(int)PolicyType.Archiving] = "GetArchivingPolicy.ps1";
            _getPoliciesScripts[(int)PolicyType.Client] = "GetClientPolicy.ps1";
            _getPoliciesScripts[(int)PolicyType.ClientVersion] = "GetClientVersionPolicy.ps1";
            _getPoliciesScripts[(int)PolicyType.Conferencing] = "GetConferencingPolicy.ps1";
            _getPoliciesScripts[(int)PolicyType.ExternalAccess] = "GetExternalAccessPolicy.ps1";
            _getPoliciesScripts[(int)PolicyType.HostedVoiceMail] = "GetHostedVoiceMailPolicy.ps1";
            _getPoliciesScripts[(int)PolicyType.Location] = "GetLocationPolicy.ps1";
            _getPoliciesScripts[(int)PolicyType.Pin] = "GetPinPolicy.ps1";
            _getPoliciesScripts[(int)PolicyType.Presence] = "GetPresencePolicy.ps1";
            _getPoliciesScripts[(int)PolicyType.Voice] = "GetVoicePolicy.ps1";
            _getPoliciesScripts[(int)PolicyType.DialPlan] = "GetDialPlans.ps1";
            _getPoliciesScripts[(int)PolicyType.UserExperience] = "GetUserExperiencePolicy.ps1";
        }

        public Policies GetPolicies(PolicyType policyType)
        {
            string policyScript = GetGetPoliciesScript(policyType);
            var results = _factory.RunPowerShellScriptFile(policyScript);
            string[] policyNames = results.Select(item => item.Properties["identity"].Value.ToString().Replace("Tag:", "")).ToArray();
            return new Policies(policyType, policyNames);
        }

        private string GetGetPoliciesScript(PolicyType type)
        {
            return _getPoliciesScripts[(int)type];
        }


        #region "File System Methods"

        public List<LyncPolicyTemplate> GetPolicyTemplates()
        {
            var PolicyTemplates = new List<LyncPolicyTemplate>();

            string[] TemplateFiles = null;
            try
            {
                //TemplateFiles = Directory.GetFiles(@ConfigurationManager.AppSettings["PolicyTemplatesFolder"], "*.xml");
            }
            catch (Exception ex)
            {
                //    throw ex.Message;
            }

            if (TemplateFiles != null)
            {
                foreach (var TemplateFile in TemplateFiles)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(TemplateFile);
                    LyncPolicyTemplate Policy = new LyncPolicyTemplate();

                    Policy.PolicyTemplateName = (doc.DocumentElement.SelectSingleNode("/Policies/Name") != null) ? (doc.DocumentElement.SelectSingleNode("/Policies/Name").InnerText) : "";
                    Policy.ArchivingPolicy = (doc.DocumentElement.SelectSingleNode("/Policies/ArchivingPolicy") != null) ? (doc.DocumentElement.SelectSingleNode("/Policies/ArchivingPolicy").InnerText) : "";
                    Policy.CallViaWorkPolicy = (doc.DocumentElement.SelectSingleNode("/Policies/CallViaWorkPolicy") != null) ? (doc.DocumentElement.SelectSingleNode("/Policies/CallViaWorkPolicy").InnerText) : "";
                    Policy.ClientPolicy = (doc.DocumentElement.SelectSingleNode("/Policies/ClientPolicy") != null) ? (doc.DocumentElement.SelectSingleNode("/Policies/ClientPolicy").InnerText) : "";
                    Policy.ClientVersionPolicy = (doc.DocumentElement.SelectSingleNode("/Policies/ClientVersionPolicy") != null) ? (doc.DocumentElement.SelectSingleNode("/Policies/ClientVersionPolicy").InnerText) : "";
                    Policy.ConferencingPolicy = (doc.DocumentElement.SelectSingleNode("/Policies/ConferencingPolicy") != null) ? (doc.DocumentElement.SelectSingleNode("/Policies/ConferencingPolicy").InnerText) : "";
                    Policy.DialPlan = (doc.DocumentElement.SelectSingleNode("/Policies/DialPlan") != null) ? (doc.DocumentElement.SelectSingleNode("/Policies/DialPlan").InnerText) : "";
                    Policy.ExchangeArchivingPolicy = (doc.DocumentElement.SelectSingleNode("/Policies/ExchangeArchivingPolicy") != null) ? (doc.DocumentElement.SelectSingleNode("/Policies/ExchangeArchivingPolicy").InnerText) : "";
                    Policy.HostedVoicemailPolicy = (doc.DocumentElement.SelectSingleNode("/Policies/HostedVoicemailPolicy") != null) ? (doc.DocumentElement.SelectSingleNode("/Policies/HostedVoicemailPolicy").InnerText) : "";
                    Policy.LocationPolicy = (doc.DocumentElement.SelectSingleNode("/Policies/LocationPolicy") != null) ? (doc.DocumentElement.SelectSingleNode("/Policies/LocationPolicy").InnerText) : "";
                    Policy.MobilityPolicy = (doc.DocumentElement.SelectSingleNode("/Policies/MobilityPolicy") != null) ? (doc.DocumentElement.SelectSingleNode("/Policies/MobilityPolicy").InnerText) : "";
                    Policy.PersistentChatPolicy = (doc.DocumentElement.SelectSingleNode("/Policies/PersistentChatPolicy") != null) ? (doc.DocumentElement.SelectSingleNode("/Policies/PersistentChatPolicy").InnerText) : "";
                    Policy.PresencePolicy = (doc.DocumentElement.SelectSingleNode("/Policies/PresencePolicy") != null) ? (doc.DocumentElement.SelectSingleNode("/Policies/PresencePolicy").InnerText) : "";
                    Policy.ThirdPartyVideoSystemPolicy = (doc.DocumentElement.SelectSingleNode("/Policies/ThirdPartyVideoSystemPolicy") != null) ? (doc.DocumentElement.SelectSingleNode("/Policies/ThirdPartyVideoSystemPolicy").InnerText) : "";
                    Policy.UserServicesPolicy = (doc.DocumentElement.SelectSingleNode("/Policies/UserServicesPolicy") != null) ? (doc.DocumentElement.SelectSingleNode("/Policies/UserServicesPolicy").InnerText) : "";
                    Policy.VoicePolicy = (doc.DocumentElement.SelectSingleNode("/Policies/VoicePolicy") != null) ? (doc.DocumentElement.SelectSingleNode("/Policies/VoicePolicy").InnerText) : "";
                    Policy.VoiceRoutingPolicy = (doc.DocumentElement.SelectSingleNode("/Policies/VoiceRoutingPolicy") != null) ? (doc.DocumentElement.SelectSingleNode("/Policies/VoiceRoutingPolicy").InnerText) : "";

                    PolicyTemplates.Add(Policy);
                }
            }
            return PolicyTemplates;
        }

        public void SetPolicyTemplate(LyncPolicyTemplate PolicyTemplate)
        {
            string NewTemplateName = PolicyTemplate.PolicyTemplateName;
            string NewTemplateFileName = MakeSafeFilename(NewTemplateName) + ".xml";

            //Create folder if it doesnt already exist
            CreatePolicyFolderIfDoesntExist();

            //Create the XML Template from current ComboBox Settings
            try
            {

                XmlDocument xmlDoc = new XmlDocument();
                XmlNode rootNode = xmlDoc.CreateElement("Policies");
                xmlDoc.AppendChild(rootNode);

                XmlNode xmlNode = xmlDoc.CreateElement("Name");
                xmlNode.InnerText = PolicyTemplate.PolicyTemplateName ?? "";
                rootNode.AppendChild(xmlNode);

                xmlNode = xmlDoc.CreateElement("ArchivingPolicy");
                xmlNode.InnerText = PolicyTemplate.ArchivingPolicy ?? "";
                rootNode.AppendChild(xmlNode);

                xmlNode = xmlDoc.CreateElement("CallViaWorkPolicy");
                xmlNode.InnerText = PolicyTemplate.CallViaWorkPolicy ?? "";
                rootNode.AppendChild(xmlNode);

                xmlNode = xmlDoc.CreateElement("ClientPolicy");
                xmlNode.InnerText = PolicyTemplate.ClientPolicy ?? "";
                rootNode.AppendChild(xmlNode);

                xmlNode = xmlDoc.CreateElement("ClientVersionPolicy");
                xmlNode.InnerText = PolicyTemplate.ClientVersionPolicy ?? "";
                rootNode.AppendChild(xmlNode);

                xmlNode = xmlDoc.CreateElement("ConferencingPolicy");
                xmlNode.InnerText = PolicyTemplate.ConferencingPolicy ?? "";
                rootNode.AppendChild(xmlNode);

                xmlNode = xmlDoc.CreateElement("DialPlan");
                xmlNode.InnerText = PolicyTemplate.DialPlan ?? "";
                rootNode.AppendChild(xmlNode);

                xmlNode = xmlDoc.CreateElement("ExchangeArchivingPolicy");
                xmlNode.InnerText = PolicyTemplate.ExchangeArchivingPolicy ?? "";
                rootNode.AppendChild(xmlNode);

                xmlNode = xmlDoc.CreateElement("HostedVoicemailPolicy");
                xmlNode.InnerText = PolicyTemplate.HostedVoicemailPolicy ?? "";
                rootNode.AppendChild(xmlNode);

                xmlNode = xmlDoc.CreateElement("LocationPolicy");
                xmlNode.InnerText = PolicyTemplate.LocationPolicy ?? "";
                rootNode.AppendChild(xmlNode);

                xmlNode = xmlDoc.CreateElement("MobilityPolicy");
                xmlNode.InnerText = PolicyTemplate.MobilityPolicy ?? "";
                rootNode.AppendChild(xmlNode);

                xmlNode = xmlDoc.CreateElement("PersistentChatPolicy");
                xmlNode.InnerText = PolicyTemplate.PersistentChatPolicy ?? "";
                rootNode.AppendChild(xmlNode);

                xmlNode = xmlDoc.CreateElement("PolicyTemplateName");
                xmlNode.InnerText = PolicyTemplate.PolicyTemplateName ?? "";
                rootNode.AppendChild(xmlNode);

                xmlNode = xmlDoc.CreateElement("PresencePolicy");
                xmlNode.InnerText = PolicyTemplate.PresencePolicy ?? "";
                rootNode.AppendChild(xmlNode);

                xmlNode = xmlDoc.CreateElement("ThirdPartyVideoSystemPolicy");
                xmlNode.InnerText = PolicyTemplate.ThirdPartyVideoSystemPolicy ?? "";
                rootNode.AppendChild(xmlNode);

                xmlNode = xmlDoc.CreateElement("UserServicesPolicy");
                xmlNode.InnerText = PolicyTemplate.UserServicesPolicy ?? "";
                rootNode.AppendChild(xmlNode);

                xmlNode = xmlDoc.CreateElement("VoicePolicy");
                xmlNode.InnerText = PolicyTemplate.VoicePolicy ?? "";
                rootNode.AppendChild(xmlNode);

                xmlNode = xmlDoc.CreateElement("VoiceRoutingPolicy");
                xmlNode.InnerText = PolicyTemplate.VoiceRoutingPolicy ?? "";
                rootNode.AppendChild(xmlNode);

                xmlDoc.Save(System.IO.Path.Combine(@ConfigurationManager.AppSettings["PolicyTemplatesFolder"], NewTemplateFileName));

                //Looks like the version of LINQ used byt the LyncWCF service doesnt support the nicer to use XDocument way  :(

                ////new XDocument(
                ////new XElement("Policies",
                ////   new XElement("Name", NewTemplateName),
                ////   new XElement("Policy1", comboBox1.SelectedItem.ToString().Trim()),
                ////   new XElement("Policy2", comboBox2.SelectedItem.ToString().Trim()),
                ////   new XElement("Policy3", comboBox3.SelectedItem.ToString().Trim()),
                ////   new XElement("Policy4", comboBox4.SelectedItem.ToString().Trim()),
                ////   new XElement("Policy5", comboBox5.SelectedItem.ToString().Trim())
                //   )
                //)
                //.Save(System.IO.Path.Combine(@ConfigurationManager.AppSettings["PolicyTemplatesFolder"], NewTemplateFileName));
                //  MessageBox.Show("Created New Template: " + NewTemplateName);
            }
            catch (Exception ex)
            {
                //  MessageBox.Show(ex.Message);
            }


        }

        public static string MakeSafeFilename(string filename)
        {
            try
            {
                foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                {
                    filename = filename.Replace(c, '_');
                }
                return filename;
            }
            catch (Exception ex)
            {
                //  MessageBox.Show(ex.Message);
                return null;
            }
        }

        private void CreatePolicyFolderIfDoesntExist()
        {
            //Check if Folder Exists - Create if not
            try
            {
                bool exists = System.IO.Directory.Exists(@ConfigurationManager.AppSettings["PolicyTemplatesFolder"]);
                if (!exists) System.IO.Directory.CreateDirectory(@ConfigurationManager.AppSettings["PolicyTemplatesFolder"]);
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);
            }
        }

        #endregion
    }
}