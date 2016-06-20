using System.ComponentModel;

namespace CallFlowManager.UI.Models
{
    /// <summary>
    /// http://www.codeplussoft.com/Pages/bindclasslistdgv
    /// </summary>
    public class CsQAgentGroups
    {
        //Enforce class properties using constructor
        //http://stackoverflow.com/questions/554405/stipulating-that-a-property-is-required-in-a-class-compile-time
        public CsQAgentGroups(string name, string alertTime, string participationPolicy, string routingMethod)
        {
            Name = name;
            AlertTime = alertTime;
            ParticipationPolicy = participationPolicy;
            RoutingMethod = routingMethod;
        }

        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Alert Time")]
        public string AlertTime { get; set; }

        [DisplayName("Participation Policy")]
        public string ParticipationPolicy { get; set; }

        [DisplayName("Routing Method")]
        public string RoutingMethod { get; set; }
  
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
