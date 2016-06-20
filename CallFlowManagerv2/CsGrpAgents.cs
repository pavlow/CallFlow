using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallFlowManagerv2
{
    /// <summary>
    /// http://www.codeplussoft.com/Pages/bindclasslistdgv
    /// </summary>
    class CsGrpAgents
    {
        //Enforce class properties using constructor
        //http://stackoverflow.com/questions/554405/stipulating-that-a-property-is-required-in-a-class-compile-time
        public CsGrpAgents(string name, string sipAddress)//, string memberOf)
        {
            Name = name;
            SipAddress = sipAddress;
            //MemberOf = memberOf;
        }

        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("SIP Address")]
        public string SipAddress { get; set; }

        [DisplayName("Member Of")]
        public string MemberOf { get; set; }
    }

    //Validate
    //http://stackoverflow.com/questions/554405/stipulating-that-a-property-is-required-in-a-class-compile-time
    //public void PerformAction()
    //{
    //if(this.Validate())
    //    // Perform action
    //}
    //protected bool Validate()
    //{
    //    if(!EmploymentType.HasValue)
    //        throw new InvalidOperationException("EmploymentType must be set.");
    //}
}
