using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Forms;

namespace CallFlowManagerv2
{
    //NOTES
    //Error Handling:
    /// http://stackoverflow.com/questions/14973642/how-using-try-catch-for-exception-handling-is-best-practice
    //throw - bubbles exception up the stack - error will originate from the original throw
    //throw ex - bubbles exception up the stack - error will appear to originate from the throw ex, meaning if it bubble to the throw ex it could be confusing as to where it originated from
    //throw new xxx - bubbles user defined error up the stack
    //throw new ApplicationException("There was an error starting the event log in Form1.cs", ex);
    //System.Environment.Exit(exitCode);
    //
    //
    ////Test exeception:
    //var extest = new CsQueries();
    //foreach (var item in extest.Groups)
    //{
    //    MessageBox.Show("DDD");
    //}  

    public static class ErrorHandler
    {
        public static void ShowMessage(string message, string title)
        {
           MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void ShowMessage(string message, string exception, string title)
        {
            MessageBox.Show(message + Environment.NewLine + Environment.NewLine + exception, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
