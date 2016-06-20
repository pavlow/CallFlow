using System.ComponentModel;

namespace CallFlowManager.UI.Models
{
    /// <summary>
    /// http://www.codeplussoft.com/Pages/bindclasslistdgv
    /// </summary>
    public class CsBusHours
    {
        //Enforce class properties using constructor
        //http://stackoverflow.com/questions/554405/stipulating-that-a-property-is-required-in-a-class-compile-time
        public CsBusHours(string parent, string day, string openTime1, string closeTime1, string openTime2, string closeTime2)//, string memberOf)
        {
            Parent = parent;
            Day = day;
            OpenTime1 = openTime1;
            CloseTime1 = closeTime1;
            OpenTime2 = openTime2;
            CloseTime2 = closeTime2;
        }

        [Browsable(false)]
        public string Parent { get; set; }

        [DisplayName("Day")]
        public string Day { get; set; }

        [DisplayName("Open Time 1")]
        public string OpenTime1 { get; set; }

        [DisplayName("Close Time 1")]
        public string CloseTime1 { get; set; }

        [DisplayName("Open Time 2")]
        public string OpenTime2 { get; set; }

        [DisplayName("Close Time 2")]
        public string CloseTime2 { get; set; }
    }

}
