using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallFlowManagerv2
{
    class CsHolidayGroup
    {
        //Enforce class properties using constructor
        //http://stackoverflow.com/questions/554405/stipulating-that-a-property-is-required-in-a-class-compile-time
        public CsHolidayGroup(string name, string ownerPool, string identity)
        {
            Name = name;
            OwnerPool = ownerPool;
            Identity = identity;
        }

        [DisplayName("Name")]
        public string Name { get; set; }

        [Browsable(false)]
        public string OwnerPool { get; set; }

        [Browsable(false)]
        public string Identity { get; set; }

        [DisplayName("Holidays")]
        [Browsable(false)]
        public BindingList<CsHoliday> Holidays { get; set; }
    }
}
