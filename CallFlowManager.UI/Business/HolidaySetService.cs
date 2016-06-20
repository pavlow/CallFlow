using System.Collections.ObjectModel;
using System.Management.Automation;
using CallFlowManager.UI.ViewModels.BusinessHours;
using CallFlowManager.UI.ViewModels.Holidays;

namespace CallFlowManager.UI.Business
{
    class HolidaySetService
    {
        /// <summary>
        /// Prepares Business Hours set command by creating a PS object that will be passed to the script file
        /// http://stackoverflow.com/questions/12472718/pass-collection-of-c-sharp-psobjects-to-powershell-pipeline
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ownerPool"></param>
        /// <param name="holTimes"></param>
        public PSObject PrepareSetCsRgsHoliday(string name, string ownerPool, ObservableCollection<HolidayTimeViewModel> holTimes)
        {
            //Create open/close time PS object
            Collection<PSObject> holTimesObj = new Collection<PSObject>();
            foreach (var item in holTimes)
            {
                PSObject obj1 = new PSObject();
                obj1.Properties.Add(new PSNoteProperty("Name", item.Name));
                obj1.Properties.Add(new PSNoteProperty("StartDate", item.StartHolidayDate.DateTime));
                obj1.Properties.Add(new PSNoteProperty("EndDate", item.EndHolidayDate.DateTime));
                holTimesObj.Add(obj1);
            }
            
            //Create PS object and return it
            PSObject holObj = new PSObject();
            holObj.Properties.Add(new PSNoteProperty("Name", name));
            holObj.Properties.Add(new PSNoteProperty("OwnerPool", ownerPool));
            holObj.Properties.Add(new PSNoteProperty("HolidayTimes", holTimesObj));
            
            return holObj;
        }
    }
}
