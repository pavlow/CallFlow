using System.Collections.Generic;
using System.ComponentModel;

namespace CallFlowManager.UI.Models
{
    public class CsBusHoursGroup
    {
        //Enforce class properties using constructor
        //http://stackoverflow.com/questions/554405/stipulating-that-a-property-is-required-in-a-class-compile-time
        public CsBusHoursGroup(string name, string ownerPool, string identity)
        {
            Name = name;
            OwnerPool = ownerPool;
            Identity = identity;
            Id = identity;
        }

        [DisplayName("Name")]
        public string Name { get; set; }

        [Browsable(false)]
        public string Id { get; set; }

        [Browsable(false)]
        public string OwnerPool { get; set; }

        [Browsable(false)]
        public string Identity { get; set; }

        [DisplayName("Business Hours")]
        [Browsable(false)]
        public List<CsBusHours> BusinessHours { get; set; }
    }
}
