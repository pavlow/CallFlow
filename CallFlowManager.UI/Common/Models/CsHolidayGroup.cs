using System.Collections.Generic;
using System.ComponentModel;

namespace CallFlowManager.UI.Models
{
    public class CsHolidayGroup
    {
        //Enforce class properties using constructor
        //http://stackoverflow.com/questions/554405/stipulating-that-a-property-is-required-in-a-class-compile-time
        public CsHolidayGroup(string name, string ownerPool, string identity, List<CsHoliday> holidays)
        {
            Name = name;
            OwnerPool = ownerPool;
            Identity = identity;
            Holidays = holidays;
        }

        [DisplayName("Name")]
        public string Name { get; set; }

        [Browsable(false)]
        public string OwnerPool { get; set; }

        [Browsable(false)]
        public string Identity { get; set; }

        [DisplayName("Holidays")]
        public List<CsHoliday> Holidays { get; set; }
    }
}
