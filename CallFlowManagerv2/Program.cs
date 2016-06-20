using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CallFlowManagerv2
{
    static class Program
    {
        private static LoggingOld log;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                //Start application log
                try
                {
                    log = LoggingOld.Instance;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "There was an error starting the application event log in Program.cs:" + Environment.NewLine +
                        Environment.NewLine + ex.ToString(), "Application Error");
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                System.Environment.Exit(0);
            }
        }

        /// <summary>
        /// Program exception handler
        /// </summary>
        /// <param name="ex"></param>
        private static void HandleException(Exception ex)
        {
            MessageBox.Show(ex.Message + Environment.NewLine + Environment.NewLine + "For more information see the application log.", "Application Error");
            if (log != null)
            log.WriteToLog(ex.ToString());
        }
    }

   
}
