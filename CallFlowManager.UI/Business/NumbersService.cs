using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;
using CallFlowManager.UI.Models;
using CallFlowManager.UI.Views;

namespace CallFlowManager.UI.Business
{
    public class NumbersService
    {
        //Local copy of passed in Ps data object
        private ICollection<PSObject> _numbers { get; set; }
        public IList<Number> LoadedNumbers { get; private set; }

        public NumbersService()
        {
            LoadedNumbers = new List<Number>();
        }

        /// <summary>
        /// Processes SfB numbers retrieved from PowerShell
        /// </summary>
        public void ProcessPsNumbersInventory(ICollection<PSObject> numbers)
        {
            _numbers = numbers;

            foreach (dynamic number in _numbers)
            {
                //Deal with null/empty extensions
                string ext;
                if (number.Ext == null || number.Ext == "")
                {
                    ext = "";
                }
                else
                {
                    ext = number.ext.ToString();
                }
                               
                string type = number.Type;

                string ddi;
                if (number.DDI == null)
                {
                    ddi = "";
                }
                else
                {
                    ddi = number.DDI.ToString();
                }                              

                string assignedTo = number.AssignedTo;

                var num = new Number(type,ddi,ext,assignedTo);
                LoadedNumbers.Add(num);
            }
        }
    }
}
