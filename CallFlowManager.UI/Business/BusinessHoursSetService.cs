using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;
using CallFlowManager.UI.ViewModels.BusinessHours;

namespace CallFlowManager.UI.Business
{
    class BusinessHoursSetService
    {
        /// <summary>
        /// Prepares Business Hours set command by creating a PS object that will be passed to the script file
        /// http://stackoverflow.com/questions/12472718/pass-collection-of-c-sharp-psobjects-to-powershell-pipeline
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ownerPool"></param>
        /// <param name="openClose"></param>
        public PSObject PrepareSetCsRgsHoursOfBusiness(string name, string ownerPool, ObservableCollection<OpenCloseTimeViewModel> openClose)
        {
            //Create open/close time PS object
            Collection<PSObject> openCloseObj = new Collection<PSObject>();
            foreach (var item in openClose)
            {
                PSObject obj1 = new PSObject();
                obj1.Properties.Add(new PSNoteProperty("DayOfWeek", item.DayOfWeek));
                obj1.Properties.Add(new PSNoteProperty("OpenTime1", item.OpenTime1));
                obj1.Properties.Add(new PSNoteProperty("CloseTime1", item.CloseTime1));
                obj1.Properties.Add(new PSNoteProperty("OpenTime2", item.OpenTime2));
                obj1.Properties.Add(new PSNoteProperty("CloseTime2", item.CloseTime2));
                obj1.Properties.Add(new PSNoteProperty("OpenCloseTime1Enabled", item.OpenCloseTime1Enabled));
                obj1.Properties.Add(new PSNoteProperty("OpenCloseTime2Enabled", item.OpenCloseTime2Enabled));
                openCloseObj.Add(obj1);
            }
            
            //Create PS object and return it
            PSObject busHoursObj = new PSObject();
            busHoursObj.Properties.Add(new PSNoteProperty("Name", name));
            busHoursObj.Properties.Add(new PSNoteProperty("OwnerPool", ownerPool));
            busHoursObj.Properties.Add(new PSNoteProperty("OpenCloseTimes", openCloseObj));
            
            return busHoursObj;
        }
    }
}
