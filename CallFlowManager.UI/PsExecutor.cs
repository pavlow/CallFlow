using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Remoting;
using System.Management.Automation.Runspaces;
using System.Security;

namespace CallFlowManager.UI
{
    /// <summary>
    /// HELPFUL LINKS:
    /// http://stackoverflow.com/questions/16329448/hosting-powershell-powershell-vs-runspace-vs-runspacepool-vs-pipeline/21053751#21053751
    /// http://stackoverflow.com/questions/13760795/how-to-pass-pscredential-object-from-c-sharp-code-to-powershell-function
    /// https://com2kid.wordpress.com/2011/09/22/remotely-executing-commands-in-powershell-using-c/
    /// http://social.technet.microsoft.com/wiki/contents/articles/4546.working-with-passwords-secure-strings-and-credentials-in-windows-powershell.aspx
    /// http://stackoverflow.com/questions/28844580/using-c-sharp-to-return-data-from-powershell
    /// http://stackoverflow.com/questions/20399186/what-is-the-difference-between-pipeline-invoke-and-powershell-invoke
    /// http://stackoverflow.com/questions/3216594/create-remote-powershell-session-in-c
    /// http://stackoverflow.com/questions/20131551/how-to-execute-powershell-invoke-twice
    /// http://stackoverflow.com/questions/7294765/problem-with-calling-a-powershell-function-from-c-sharp
    /// http://stackoverflow.com/questions/10919055/code-to-run-a-powershell-cmdlet-function-that-lives-in-a-ps1-file-of-cmdlets
    /// http://stackoverflow.com/questions/10270985/the-term-is-not-recognized-as-the-name-of-a-cmdlet
    /// http://www.codeproject.com/Articles/18229/How-to-run-PowerShell-scripts-from-C
    /// http://stackoverflow.com/questions/7564778/problems-using-literals-and-codeblocks-with-c-sharp-to-interact-with-powershell
    /// http://stackoverflow.com/questions/11405384/how-to-pass-a-parameter-from-c-sharp-to-a-powershell-script-file
    /// http://stackoverflow.com/questions/21856240/powershell-to-exchange-2013-restricted-language-mode-error
    /// https://social.technet.microsoft.com/Forums/office/en-US/46728788-9850-4374-ab08-5fcb30772c93/script-block-literals-are-not-allowed-in-restricted-language-mode-or-a-data-section?forum=winserverpowershell
    /// https://social.technet.microsoft.com/Forums/windowsserver/en-US/46767d4b-e338-4bff-a38e-63b4d4903408/remote-powershell-scriptblock-execution-question?forum=winserverpowershell
    /// https://social.msdn.microsoft.com/Forums/vstudio/en-US/13fdce35-1643-4f16-bda4-757fa4994a92/how-to-set-formatenumerationlimit1-in-powershell-when-it-called-from-c?forum=csharpgeneral
    /// http://stackoverflow.com/questions/13760795/how-to-pass-pscredential-object-from-c-sharp-code-to-powershell-function
    /// http://www.get-exchange.info/2012/12/30/powershell-scripting-for-exchange-server-some-tips/
    /// http://stackoverflow.com/questions/14104222/powershell-whereobjectcommand-from-c-sharp
    /// https://technet.microsoft.com/en-us/library/ee706578(v=vs.85).aspx
    /// https://social.technet.microsoft.com/Forums/windowsserver/en-US/159771e3-699d-4270-ba27-cd74d28ea0b7/serialization-depth-on-remote-powershell?forum=winserverpowershell
    /// http://stackoverflow.com/questions/33553463/are-powershell-objects-returned-from-a-remote-session-different
    /// http://stackoverflow.com/questions/33556855/receive-full-psobject-un-serialised-from-remote-session
    /// http://blogs.msdn.com/b/powershell/archive/2010/01/07/how-objects-are-sent-to-and-from-remote-sessions.aspx
    /// http://stackoverflow.com/questions/12472718/pass-collection-of-c-sharp-psobjects-to-powershell-pipeline
    /// </summary>
    class PsExecutor : IDisposable
    {
        //Create PS Remoting session variable
        private object psSessionConnection;
        //Create PS Remoting runspace
        private Runspace runspace;
        private RunspacePool rrp;

        public PsExecutor()
        {
            //Create PS Remoting runspace
            runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();
        }

        public bool CheckPSRemoting(string psCommand, string RemoteMachineFqdn, int RemoteMachinePort, string RemoteMachinePath, bool SslEnabled)
        {
            var psRemote = ExecuteSynchronously(psCommand, RemoteMachineFqdn, RemoteMachinePort, RemoteMachinePath, SslEnabled);

            if (psRemote.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        public bool CheckPSRemotingCustomCredentials(string psCommand, string RemoteMachineFqdn, int RemoteMachinePort, string RemoteMachinePath, bool SslEnabled,
            string Username, SecureString Password)
        {
            var psRemote = ExecuteSynchronously(psCommand, RemoteMachineFqdn, RemoteMachinePort, RemoteMachinePath, SslEnabled,
                Username, Password);

            if (psRemote.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        public bool CheckLocalModules(string moduleName)
        {
            var module = ExecuteSynchronously(@"Import-Module " + moduleName);

            if (module.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool CheckLocalCommand(string psCommand)
        {
            var command = ExecuteSynchronously(psCommand);

            if (command.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        /// <summary>
        /// Execute PowerShell syncronously using a local session
        /// </summary>
        /// <param name="PSCommand"></param>
        /// <returns></returns>
        public Collection<PSObject> ExecuteSynchronously(string PSCommand)
        {
            using (PowerShell ps = PowerShell.Create())
            {
                ps.Streams.Error.DataAdded += (sender, e) => PsError_DataAdded(sender, e, ps);
                ps.AddScript(PSCommand);
                Collection<PSObject> psOutput = ps.Invoke();

                // check the other output streams (for example, the error stream)
                if (ps.Streams.Error.Count > 0)
                {
                    // error records were written to the error stream.
                    // do something with the items found.
                }
                return psOutput;
            }
        }

        /// <summary>
        /// Execute PowerShell syncronously using a remote session with the logged on users credentials
        /// </summary>
        /// <param name="PSCommand"></param>
        /// <param name="RemoteMachineFqdn"></param>
        /// <param name="RemoteMachinePort"></param>
        /// <param name="RemoteMachinePath"></param>
        /// <param name="SslEnabled"></param>
        /// <returns></returns>
        public Collection<PSObject> ExecuteSynchronously(string PSCommand, string RemoteMachineFqdn, int RemoteMachinePort, string RemoteMachinePath,
            bool SslEnabled)
        {
            string shellUri = @"http://schemas.microsoft.com/powershell/Microsoft.PowerShell";
            WSManConnectionInfo connectionInfo = new WSManConnectionInfo(SslEnabled, RemoteMachineFqdn, RemoteMachinePort, RemoteMachinePath, shellUri, null);

            connectionInfo.SkipRevocationCheck = true;
            connectionInfo.SkipCACheck = true;
            connectionInfo.SkipCNCheck = true;
            //connectionInfo.IdleTimeout = 43200000; // Set the IdleTimeout using a TimeSpan object

            using (Runspace runspace = RunspaceFactory.CreateRunspace(connectionInfo))
            {
                var psResult = new Collection<PSObject>();
                runspace.Open();

                using (PowerShell powershell = PowerShell.Create())
                {
                    powershell.Streams.Error.DataAdded += (sender, e) => PsError_DataAdded(sender, e, powershell);
                    powershell.Runspace = runspace;
                    powershell.AddScript(PSCommand);

                    try
                    {
                        psResult = powershell.Invoke();
                    }
                    catch (Exception ex)
                    {
                        var message = string.Format(@"PowerShell command failed: {0}. Error: {1}", PSCommand,
                            ex.Message);
                        //Trace(message);
                        throw new ApplicationException(message, ex);
                    }
                    return psResult;
                }
            }
        }


        /// <summary>
        /// Execute PowerShell syncronously using a remote session with custom credentials
        /// </summary>
        /// <param name="PSCommand"></param>
        /// <param name="RemoteMachineFqdn"></param>
        /// <param name="RemoteMachinePort"></param>
        /// <param name="RemoteMachinePath"></param>
        /// <param name="SslEnabled"></param>
        /// <param name="Username"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public Collection<PSObject> ExecuteSynchronously(string PSCommand, string RemoteMachineFqdn, int RemoteMachinePort, string RemoteMachinePath,
            bool SslEnabled, string Username, SecureString Password)
        {
            string shellUri = @"http://schemas.microsoft.com/powershell/Microsoft.PowerShell";
            PSCredential remoteCredential = new PSCredential(Username, Password);
            WSManConnectionInfo connectionInfo = new WSManConnectionInfo(SslEnabled, RemoteMachineFqdn, RemoteMachinePort, RemoteMachinePath, shellUri, remoteCredential);

            connectionInfo.SkipRevocationCheck = true;
            connectionInfo.SkipCACheck = true;
            connectionInfo.SkipCNCheck = true;
            //connectionInfo.IdleTimeout = 43200000; // Set the IdleTimeout using a TimeSpan object

            using (Runspace runspace = RunspaceFactory.CreateRunspace(connectionInfo))
            {
                Collection<PSObject> psResult = new Collection<PSObject>();
                runspace.Open();

                using (Pipeline pipeLine = runspace.CreatePipeline())
                {
                    Command command = new Command(PSCommand, true);
                    //PassParametersToCommand(command, scriptParameters);
                    pipeLine.Commands.Add(command);

                    psResult = pipeLine.Invoke();

                    return psResult;
                }

                //using (PowerShell powershell = PowerShell.Create())
                //{
                //    powershell.Streams.Error.DataAdded += (sender, e) => PsError_DataAdded(sender, e, powershell);
                //    powershell.Runspace = runspace;
                //    powershell.AddScript(PSCommand);

                //    try
                //    {
                //        psResult = powershell.Invoke();
                //    }
                //    catch (Exception ex)
                //    {
                //        var message = string.Format(@"PowerShell command failed: {0}. Error: {1}", PSCommand,
                //            ex.Message);
                //        //Trace(message);
                //        throw new ApplicationException(message, ex);
                //    }
                //    return psResult;
                //}
            }
        }


        
        /// <summary>
        /// Execute PowerShell syncronously using a remote session imported to local PS instance using custom credentials 
        /// </summary>
        /// <param name="PSCommand"></param>
        /// <param name="RemoteMachineFqdn"></param>
        /// <param name="RemoteMachinePort"></param>
        /// <param name="RemoteMachinePath"></param>
        /// <param name="SslEnabled"></param>
        /// <param name="Username"></param>
        /// <param name="Password"></param>
        /// <param name="importSessionToLocalPSInstance"></param>
        /// <returns></returns>
        public Collection<PSObject> ExecuteSynchronously(string PSCommand, string RemoteMachineFqdn, int RemoteMachinePort, string RemoteMachinePath,
           bool SslEnabled, string Username, SecureString Password, bool importSessionToLocalPSInstance)
        {
            //object psSessionConnection;
            //var runspace = RunspaceFactory.CreateRunspace();
            //runspace.Open();

            if (psSessionConnection == null)
            {
                PSCredential remoteCredential = new PSCredential(Username, Password);
                PSSessionOption sessionOption = new PSSessionOption();
                sessionOption.SkipCACheck = true;
                sessionOption.SkipCNCheck = true;
                sessionOption.SkipRevocationCheck = true;

                // Create a powershell session for remote server
                using (var powershell = PowerShell.Create())
                {
                    powershell.Streams.Error.DataAdded += (sender, e) => PsError_DataAdded(sender, e, powershell);
                    var command = new PSCommand();
                    command.AddCommand("New-PSSession");
                    //command.AddParameter("ConfigurationName", "Microsoft.Exchange");
                    command.AddParameter("ConnectionUri", new Uri("https://S4BFE01.ucgeek.nz/OCSPowerShell/"));
                    command.AddParameter("Authentication", "Default");
                    command.AddParameter("Credential", remoteCredential);
                    command.AddParameter("SessionOption", sessionOption);
                    powershell.Commands = command;
                    powershell.Runspace = runspace;

                    // TODO: Handle errors
                    var result = powershell.Invoke();
                    psSessionConnection = result[0];
                }

                //// Set ExecutionPolicy on the process to unrestricted
                //using (var powershell = PowerShell.Create())
                //{
                //    powershell.Streams.Error.DataAdded += (sender, e) => PsError_DataAdded(sender, e, powershell);
                //    var command = new PSCommand();
                //    command.AddCommand("Set-ExecutionPolicy");
                //    command.AddParameter("Scope", "Process");
                //    command.AddParameter("ExecutionPolicy", "Unrestricted");
                //    powershell.Commands = command;
                //    powershell.Runspace = runspace;

                //    powershell.Invoke();
                //}

                //// Import remote exchange session into runspace
                //using (var powershell = PowerShell.Create())
                //{
                //    powershell.Streams.Error.DataAdded += (sender, e) => PsError_DataAdded(sender, e, powershell);
                //    var command = new PSCommand();
                //    command.AddCommand("Import-PSSession");
                //    command.AddParameter("Session", psSessionConnection);
                //    powershell.Commands = command;
                //    powershell.Runspace = runspace;

                //    powershell.Invoke();
                //}
            }

            Collection<PSObject> psOutput;

            using (Runspace rs = RunspaceFactory.CreateRunspace())
            {

                rs.Open();
                // Import remote exchange session into runspace
                using (var powershell = PowerShell.Create())
                {
                    powershell.Streams.Error.DataAdded += (sender, e) => PsError_DataAdded(sender, e, powershell);
                    var command = new PSCommand();
                    command.AddCommand("Import-PSSession");
                    command.AddParameter("Session", psSessionConnection);
                    powershell.Commands = command;
                    powershell.Runspace = rs;

                    powershell.Invoke();
                }

                // Run command
                using (var powershell = PowerShell.Create())
                {
                    powershell.Streams.Error.DataAdded += (sender, e) => PsError_DataAdded(sender, e, powershell);
                    //var command = new PSCommand();
                    powershell.AddScript(PSCommand);
                    powershell.Runspace = rs;
                    psOutput = powershell.Invoke();
                    powershell.Commands.Clear();

                }
                rs.Close();
            }
            return psOutput;
        }


        //Test runspace pools        
        //public Collection<PSObject> ExecuteSynchronously(string PSCommand, string RemoteMachineFqdn, int RemoteMachinePort, string RemoteMachinePath,
        //   bool SslEnabled, string Username, SecureString Password)
        //{
        //    Collection<PSObject> psResult;

        //    if (rrp == null)
        //    {
        //        string shellUri = @"http://schemas.microsoft.com/powershell/Microsoft.PowerShell";
        //        PSCredential remoteCredential = new PSCredential(Username, Password);
        //        WSManConnectionInfo connectionInfo = new WSManConnectionInfo(SslEnabled, RemoteMachineFqdn,
        //            RemoteMachinePort, RemoteMachinePath, shellUri, remoteCredential);

        //        connectionInfo.SkipRevocationCheck = true;
        //        connectionInfo.SkipCACheck = true;
        //        connectionInfo.SkipCNCheck = true;

        //        rrp = RunspaceFactory.CreateRunspacePool(1, 10, connectionInfo);
        //        rrp.Open();
        //    }

        //    using (PowerShell powershell = PowerShell.Create())
        //    {
        //        powershell.RunspacePool = rrp;
        //        powershell.AddScript(PSCommand);
        //        psResult = powershell.Invoke();
        //    }
        //    return psResult;

        //}



        /// <summary>
        /// Event handler for when Data is added to the PS Error stream.
        /// </summary>
        /// <param name="sender">Contains the complete PSDataCollection of all error output items.</param>
        /// <param name="e">Contains the index ID of the added collection item and the ID of the PowerShell instance this event belongs to.</param>
        void PsError_DataAdded(object sender, DataAddedEventArgs e, PowerShell powershell)
        {
            if (powershell.Streams.Error.Count > 0)
            {
                var error = powershell.Streams.Error.ReadAll() as Collection<ErrorRecord>;
                if (error != null)
                {
                    foreach (ErrorRecord item in error)
                    {
                        Console.WriteLine("ERROR: " + item.Exception.Message);

                  

                    }
                }
            }
        }

        /// <summary>
        /// Event handler for when Data is added to the PS Warning stream.
        /// </summary>
        /// <param name="sender">Contains the complete PSDataCollection of all PS Warning output items.</param>
        /// <param name="e">Contains the index ID of the added collection item and the ID of the PowerShell instance this event belongs to.</param>
        void PsWarning_DataAdded(object sender, DataAddedEventArgs e, PowerShell powershell)
        {
            if (powershell.Streams.Warning.Count > 0)
            {
                var warning = powershell.Streams.Warning.ReadAll() as Collection<WarningRecord>;
                if (warning != null)
                {
                    foreach (WarningRecord item in warning)
                    {
                        Console.WriteLine("WARNING: " + item.Message);

                    }
                }
            }
        }

        /// <summary>
        /// Event handler for when Data is added to the PS Debug stream.
        /// </summary>
        /// <param name="sender">Contains the complete PSDataCollection of all PS Debug output items.</param>
        /// <param name="e">Contains the index ID of the added collection item and the ID of the PowerShell instance this event belongs to.</param>
        void PsDebug_DataAdded(object sender, DataAddedEventArgs e, PowerShell powershell)
        {
            if (powershell.Streams.Debug.Count > 0)
            {
                var debug = powershell.Streams.Debug.ReadAll() as Collection<DebugRecord>;
                if (debug != null)
                {
                    foreach (DebugRecord item in debug)
                    {
                        Console.WriteLine("DEBUG: " + item.Message);

                    }
                }
            }
        }


        /// <summary>
        /// Event handler for when Data is added to the PS Progress stream.
        /// </summary>
        /// <param name="sender">Contains the complete PSDataCollection of all PS Process output items.</param>
        /// <param name="e">Contains the index ID of the added collection item and the ID of the PowerShell instance this event belongs to.</param>
        void PsProgress_DataAdded(object sender, DataAddedEventArgs e, PowerShell powershell)
        {
            if (powershell.Streams.Progress.Count > 0)
            {
                var progress = powershell.Streams.Progress.ReadAll() as Collection<ProgressRecord>;
                if (progress != null)
                {
                    foreach (ProgressRecord item in progress)
                    {
                        Console.WriteLine("PROGRESS: " + item.CurrentOperation);

                    }
                }
            }
        }


        /// <summary>
        /// Event handler for when Data is added to the PS Verbose stream.
        /// </summary>
        /// <param name="sender">Contains the complete PSDataCollection of all PS Verbose output items.</param>
        /// <param name="e">Contains the index ID of the added collection item and the ID of the PowerShell instance this event belongs to.</param>
        void PsVerbose_DataAdded(object sender, DataAddedEventArgs e, PowerShell powershell)
        {
            if (powershell.Streams.Verbose.Count > 0)
            {
                var verbose = powershell.Streams.Verbose.ReadAll() as Collection<VerboseRecord>;
                if (verbose != null)
                {
                    foreach (VerboseRecord item in verbose)
                    {
                        Console.WriteLine("VERBOSE: " + item.Message);

                    }
                }
            }
        }

        public virtual void Dispose()
        {
            // Dispose of unmanaged resources.
            //Dispose(true);
            // Suppress finalization.
            //GC.SuppressFinalize(this);
        }
    }
}
