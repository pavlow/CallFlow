using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace CallFlowManagerv2
{
    class PsExecutor
    {
        /// <summary>
        /// Sample execution scenario 1: Synchronous
        /// </summary>
        /// <remarks>
        /// Executes a PowerShell script synchronously with input parameters and script output handling.
        /// </remarks>
        public Collection<PSObject> ExecuteSynchronously(string PSCommand)
        {
            using (PowerShell PowerShellInstance = PowerShell.Create())
            {
                // use "AddScript" to add the contents of a script file to the end of the execution pipeline.
                // use "AddCommand" to add individual commands/cmdlets to the end of the execution pipeline.

//                String PsScript1 = @"
//                                    param($param1) 
//                                    $d = get-date; 
//                                    $s = 'test string value';
//                                    $d; 
//                                    $s; 
//                                    $param1; 
//                                    get-service
//                                    ";

//                String PsScript2 = @"
//                                    param($param1) 
//                                    Get-CsUser
//                                    ";
                PowerShellInstance.AddCommand("Import-Module").AddArgument("SkypeForBusiness");
                PowerShellInstance.AddScript(PSCommand);

                //PowerShellInstance.AddScript("param($param1) $d = get-date; $s = 'test string value'; " +
                //"$d; $s; $param1; get-service");

                // use "AddParameter" to add a single parameter to the last command/script on the pipeline.
                PowerShellInstance.AddParameter("param1", "parameter 1 value!");

                // invoke execution on the pipeline (collecting output)
                Collection<PSObject> PSOutput = PowerShellInstance.Invoke();

                // check the other output streams (for example, the error stream)
                if (PowerShellInstance.Streams.Error.Count > 0)
                {
                    // error records were written to the error stream.
                    // do something with the items found.
                }



                //// loop through each output object item
                //foreach (PSObject outputItem in PSOutput)
                //{
                //    // if null object was dumped to the pipeline during the script then a null
                //    // object may be present here. check for null to prevent potential NRE.
                //    if (outputItem != null)
                //    {
                //        //TODO: do something with the output item
                //        Console.WriteLine(outputItem.BaseObject.GetType().FullName);
                //        Console.WriteLine(outputItem.BaseObject.ToString() + "\n");
                //    }
                //}

                return PSOutput;
            }
        }
    }
}
