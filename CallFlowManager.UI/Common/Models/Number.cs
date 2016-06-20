using System.ComponentModel;

namespace CallFlowManager.UI.Models
{
    /// <summary>
    /// http://www.codeplussoft.com/Pages/bindclasslistdgv
    /// </summary>
    public class Number
    {
        //Enforce class properties using constructor
        //http://stackoverflow.com/questions/554405/stipulating-that-a-property-is-required-in-a-class-compile-time
        public Number(string type, string ddi, string ext, string assignedTo)//, string memberOf)
        {
            Type = type;
            DDI = ddi;
            Ext = ext;
            AssignedTo = assignedTo;
        }

        //[Browsable(false)]
        [DisplayName("Type")]
        public string Type { get; set; }

        [DisplayName("DDI/DID")]
        public string DDI { get; set; }

        [DisplayName("Extension")]
        public string Ext { get; set; }

        [DisplayName("Assigned To")]
        public string AssignedTo { get; set; }
    }

}
