using System.ComponentModel;


namespace CallFlowManager.UI.Models
{
    public class CsHoliday
    {
         //Enforce class properties using constructor
        //http://stackoverflow.com/questions/554405/stipulating-that-a-property-is-required-in-a-class-compile-time
        public CsHoliday(string parent, string name, string startDate, string endDate)
        {
            Parent = parent;
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
        }

        public string Parent { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Start Date")]
        public string StartDate { get; set; }

        [DisplayName("End Date")]
        public string EndDate { get; set; }
    }
}
