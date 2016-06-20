using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Web;
using Logging;
using System.Reflection;
using Microsoft.PowerShell.Commands;

//Performance
//http://powershell.org/wp/forums/topic/executing-powershell-from-c/

namespace Lync_WCF
{
    public class PsFactory
    {
        private readonly string[] _powerShellModules = new string[] { };
        //private readonly string _scriptFolder = string.Empty;
        private readonly string _remotingUri;
        private readonly string _shellUri;
        private readonly bool _isRemoting = false;
        private readonly Logging_old _log = Logging_old.Instance;
        private static readonly ILogger Logger = LoggerFactory.Instance.GetCurrentClassLogger();
        private readonly string[] resourceFiles = Assembly.GetExecutingAssembly().GetManifestResourceNames();

        /// <summary>
        /// Default constructor
        /// </summary>
        public PsFactory()
        {
            //_scriptFolder = GetValidPath(AppDomain.CurrentDomain.BaseDirectory + "Scripts\\");
        }

        /// <summary>
        /// Use this constructor to import PowerShell modules on the local machine
        /// </summary>
        /// <param name="powerShellModules"></param>
        public PsFactory(string[] powerShellModules) //, string scriptFolder - also referenced in PsLyncFactory.cs
        {
            Logger.Info("Importing PowerShell Modules {0}", powerShellModules.ToString().Split(','));

            if (powerShellModules != null)
            {
                _powerShellModules = powerShellModules;
            }
            //_scriptFolder = GetValidPath(scriptFolder);
        }
         
        
        /// <summary>
        /// Use this constructor to execute powershell remoting
        /// </summary>
        /// <param name="remotingUri"></param>
        /// <param name="shellUri"></param>
        public PsFactory(string remotingUri, string shellUri)
        {
            Logger.Info("Using PowerShell remoting");

            if (string.IsNullOrWhiteSpace(remotingUri)) throw new ArgumentNullException("remotingUri");
            if (string.IsNullOrWhiteSpace(shellUri)) throw new ArgumentNullException("shellUri");
            _remotingUri = remotingUri;
            _shellUri = shellUri;
            //_scriptFolder = GetValidPath(AppDomain.CurrentDomain.BaseDirectory + "Scripts\\");
            _isRemoting = true;
        }

       
        /// <summary>
        /// Takes a passed in script name then deterines if it exists as an embeded resource. If it doesnt it will construct a file path based on \Scripts
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string ScriptReader(string fileName)
        {
            string script;
            string resourcePath = "Lync_WCF.Scripts." + fileName;

            if (resourceFiles.Contains(resourcePath))
            {
                //Resource resource file as preference
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath))
                
                using (StreamReader reader = new StreamReader(stream))
                {
                    script = reader.ReadToEnd();
                }

                return script;
            }
            else
            {
                //otherwise use the script file path
                //var scriptFolder = GetValidPath(AppDomain.CurrentDomain.BaseDirectory + "Scripts\\");
                var scriptFolder = AppDomain.CurrentDomain.BaseDirectory + "Scripts\\";
                var filePath = Path.Combine(scriptFolder, fileName);
                // TODO Should add check to vaidate the path and script exisits, exit if not

                using (StreamReader reader = new StreamReader(filePath))
                {
                    script = reader.ReadToEnd();
                }
                
                return script;
            }
            
        }


        /// <summary>
        /// Run a powershell script from embedded resource or _scriptFolder with or without parameters. If the file name isnt found as a resource, the file path will be used
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ICollection<PSObject> RunPowerShellScriptFile(string fileName, Dictionary<string, object> parameters = null)
        {
            //var scriptPath = Path.Combine(_scriptFolder, fileName);
            var script = ScriptReader(fileName);

            if (_isRemoting)
            {
                return RunPowerShellRemotingScript(script, fileName, parameters);
            }
            else
            {
                return RunPowerShellScript(script, fileName, parameters);
            }
        }

        
        /// <summary>
        /// Execute PowerShell script without PS remoting
        /// </summary>
        /// <param name="script"></param>
        /// <param name="scriptParameters"></param>
        private ICollection<PSObject> RunPowerShellScript(string script, string scriptName, Dictionary<string, object> scriptParameters)
        {
            //TraceScript(script, scriptParameters);
            InitialSessionState iss = InitialSessionState.CreateDefault();
            iss.ImportPSModule(_powerShellModules);
            using (Runspace runSpace = RunspaceFactory.CreateRunspace(iss))
            {
                runSpace.Open();
                //using (Pipeline pipeLine = runSpace.CreatePipeline())
                using (PowerShell powershell = PowerShell.Create())
                {
                    //Subscribe to PowerShell events - PsError_DataAdded, PsWarning_DataAdded, PsDebug_DataAdded, PsProgress_DataAdded, PsVerbose_DataAdded
                    powershell.Streams.Debug.DataAdded += (sender, e) => PsDebug_DataAdded(sender, e, powershell, scriptName);
                    powershell.Streams.Verbose.DataAdded += (sender, e) => PsVerbose_DataAdded(sender, e, powershell, scriptName);
                    powershell.Streams.Warning.DataAdded += (sender, e) => PsWarning_DataAdded(sender, e, powershell, scriptName);
                    powershell.Streams.Error.DataAdded += (sender, e) => PsError_DataAdded(sender, e, powershell, scriptName);

                    Logger.Info("Executing PowerShell script " + scriptName);
                    
                    powershell.Runspace = runSpace;                    
                    
                    //OLD:
                    //Command command = new Command(scriptFullPath, false);
                    //PassParametersToCommand(command, scriptParameters);
                    ////pipeLine.Commands.Add(command);
                    //powershell.Commands.AddCommand(command);
                    //powershell.Commands.AddScript(script);

                    var ps = powershell.AddScript(script);
                    PassParametersToCommand(ps, scriptParameters);
                    
                    //Working on ps1's as a resource
                    //Command command = new Command(script, false);

                    try
                    {
                        //var results = pipeLine.Invoke();
                        var results = powershell.Invoke();

                        //Trace("\r\nRESULT:" + results.ToString() + "\r\n");
                        
                        //foreach (var item in results)
                        //{
                        //  //Console.WriteLine(item);
                        //    Trace(item.ToString());
                        //}

                        Logger.Info("Done executing PowerShell script " + scriptName);
                        StringBuilder sbPowerShellResult = new StringBuilder();
                        sbPowerShellResult.AppendLine("PowerShell Results for " + scriptName + ":");

                        foreach (dynamic result in results)
                        {
                            if (result.Name != null)
                            {
                                sbPowerShellResult.AppendLine("PS Result for " + scriptName + ": " + result.Name.ToString());
                            }
                            else if (result.Identity != null)
                            {
                                sbPowerShellResult.AppendLine("PS Result for " + scriptName + ": " + result.Identity.ToString());

                            }
                            else if (result.DDI != null)
                            {
                                sbPowerShellResult.AppendLine("PS Result for " + scriptName + ": " + result.DDI.ToString());
                            }
                            else if (result == null)
                            {
                                sbPowerShellResult.AppendLine("PS Result for " + scriptName + ":" + "No results returned");
                            }
                            else
                            {
                                sbPowerShellResult.AppendLine("PS Result for " + scriptName + ": " + result.ToString());
                            }
                        }

                        Logger.Debug(sbPowerShellResult.ToString());
                        //Logger.Warn("Warn test");
                        //Logger.Error("Error test");
                        //Logger.Trace("Trace test");
                        //Logger.Fatal("Fatal test");
                        //Logger.Debug("Debug test");
                        //Logger.Info("Info Test");
                        
                        //var errors = pipeLine.Error.NonBlockingRead();
                        //if (powershell.Streams.Error.Count > 0)
                        //{
                        //    Logger.Error(powershell.Streams.Error[0].ToString());
                        //  //Console.WriteLine("{0} errors", powershell.Streams.Error.Count);
                        //    Trace(powershell.Streams.Error[0].ToString());
                        //}
                        
                        //if (errors != null && errors.Count > 0)
                        //{
                        //    throw new PowershellResultException(errors[0].ToString());
                        //}
                        
                        return results;

                    }
                    catch (PowershellResultException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        var message = string.Format(@"Failed to run the ""{0}"" script. Error: {1}", scriptName, ex.Message);
                        //Trace(message);
                        Logger.Info("Something went wrong: \n" + message);
                        Logger.Error(ex, message);
                       // throw; //new ApplicationException(message, ex);
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// Run the powershell remoting from the scriptFullPath with paramters - THIS NEEDS UPDATING TO MATCH RunPowerShellScript METHOD WHICH NOW USES AddScript RATHER THAN Command
        /// </summary>
        /// <param name="script"></param>
        /// <param name="scriptParameters"></param>
        private ICollection<PSObject> RunPowerShellRemotingScript(string script, string scriptName, Dictionary<string, object> scriptParameters)
        {
            //TODO - This needs to be updated to run the same way as above with script stream
            //TraceScript(script, scriptParameters);
            
            PSCredential credentials = null; // Use the executing credentials           
            var connection = new WSManConnectionInfo(new Uri(_remotingUri), _shellUri, credentials);
                     
            
            //TODO existing - read script from file system - changing this to read scripts from embedded resources. Below stream will no longer be required once this is changed
            //var script = System.IO.File.ReadAllText(scriptFullPath);
            string scriptStream;
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(script))
            using (StreamReader reader = new StreamReader(stream))
            {
                scriptStream = reader.ReadToEnd();
            }
          
            using (Runspace runSpace = RunspaceFactory.CreateRunspace(connection))
            {
                runSpace.Open();
                using (Pipeline pipeLine = runSpace.CreatePipeline())
                {
                    Command command = new Command(scriptStream, true);
                    PassParametersToCommand(command, scriptParameters);
                    pipeLine.Commands.Add(command);
                    try
                    {
                        var results = pipeLine.Invoke();
                        Logger.Info(results.ToString());
                        //Trace(results.ToString());

                        var errors = pipeLine.Error.NonBlockingRead();
                        if (errors != null && errors.Count > 0)
                        {
                            throw new PowershellResultException(errors[0].ToString());
                        }
                        return results;
                    }
                    catch (PowershellResultException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        var message = string.Format(@"Failed to run the remoting script {0}. Error: {1}", scriptName, ex.Message);
                        //Trace(message);

                        Logger.Error(ex, message);
                        throw new ApplicationException(message, ex);
                    }
                }
            }
        }

        /// <summary>
        /// Fill the pipe line object with parameters using PowerShell AddScript
        /// </summary>
        /// <param name="powershellScript"></param>
        /// <param name="scriptParameters"></param>
        private void PassParametersToCommand(PowerShell powershellScript, Dictionary<string, object> scriptParameters)
        {
            if (scriptParameters == null)
            {
                return;
            }

            foreach (var param in scriptParameters)
            {
                powershellScript.AddParameter(param.Key, param.Value);
            }
        }

        /// <summary>
        /// Fill the pipe line object with parameters using PowerShell Command
        /// </summary>
        /// <param name="pipeline"></param>
        /// <param name="scriptParameters"></param>
        private void PassParametersToCommand(Command command, Dictionary<string, object> scriptParameters)
        {
            if (scriptParameters == null)
            {
                return;
            }

            foreach (var param in scriptParameters.Select(entry => new CommandParameter(entry.Key, entry.Value)))
            {
                command.Parameters.Add(param);
            }
        }

        
        private void TraceScript(string script, Dictionary<string, object> scriptParameters)
        {
            var sb = new System.Text.StringBuilder();
            //sb.AppendLine(string.Format("Script Path: {0}", scriptFullPath));
            //sb.AppendLine("");
            //sb.AppendLine("Parameters:");
            sb.AppendLine("Executing Lync/SfB commands");

            if (scriptParameters != null && scriptParameters.Count > 0)
            {
                foreach (var param in scriptParameters)
                {
                    var value = param.Value as string[];
                    if (value != null)
                    {
                        sb.AppendLine(string.Format("${0} = '{1}';", param.Key, string.Join(",", value)));
                    }
                    else
                    {
                        sb.AppendLine(string.Format("${0} = '{1}';", param.Key, param.Value));
                    }
                }
            }

            sb.AppendLine("");
            //sb.AppendLine("Script:");
            //sb.AppendLine(GetScript(scriptFullPath));
            //Trace(sb.ToString());
        }

        public void Trace(string message)
        {
            //File log
            _log.WriteToLog(message);

            if (HttpContext.Current == null)
            {
                return;
            }

            //Http trace
            HttpContext.Current.Trace.Write(message);
        }

        /// <summary>
        /// Event handler for when Data is added to the PS Error stream.
        /// </summary>
        /// <param name="sender">Contains the complete PSDataCollection of all error output items.</param>
        /// <param name="e">Contains the index ID of the added collection item and the ID of the PowerShell instance this event belongs to.</param>
        void PsError_DataAdded(object sender, DataAddedEventArgs e, PowerShell powershell, string scriptName)
        {
            if (powershell.Streams.Error.Count > 0)
            {
                var error = powershell.Streams.Error.ReadAll() as Collection<ErrorRecord>;
                if (error != null)
                {
                    foreach (ErrorRecord item in error)
                    {
                        //Console.WriteLine("ERROR: " + item.Exception.Message);
                        //Trace("ERROR::PS:" + scriptName + ":" + item.Exception.Message);
                        Logger.Error("ERROR::PS:" + scriptName + ":" + item.Exception.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Event handler for when Data is added to the PS Warning stream.
        /// </summary>
        /// <param name="sender">Contains the complete PSDataCollection of all PS Warning output items.</param>
        /// <param name="e">Contains the index ID of the added collection item and the ID of the PowerShell instance this event belongs to.</param>
        void PsWarning_DataAdded(object sender, DataAddedEventArgs e, PowerShell powershell, string scriptName)
        {
            if (powershell.Streams.Warning.Count > 0)
            {
                var warning = powershell.Streams.Warning.ReadAll() as Collection<WarningRecord>;
                if (warning != null)
                {
                    foreach (WarningRecord item in warning)
                    {
                        //Console.WriteLine("WARNING: " + item.Message);
                        Logger.Warn("WARNING::PS:" + scriptName + ":" + item.Message);

                    }
                }
            }
        }

        /// <summary>
        /// Event handler for when Data is added to the PS Debug stream.
        /// </summary>
        /// <param name="sender">Contains the complete PSDataCollection of all PS Debug output items.</param>
        /// <param name="e">Contains the index ID of the added collection item and the ID of the PowerShell instance this event belongs to.</param>
        void PsDebug_DataAdded(object sender, DataAddedEventArgs e, PowerShell powershell, string scriptName)
        {
            if (powershell.Streams.Debug.Count > 0)
            {
                var debug = powershell.Streams.Debug.ReadAll() as Collection<DebugRecord>;
                if (debug != null)
                {
                    foreach (DebugRecord item in debug)
                    {
                        //Console.WriteLine("DEBUG: " + item.Message);
                        Logger.Debug("DEBUG::PS:" + scriptName + ":" + item.Message);

                    }
                }
            }
        }

        /// <summary>
        /// Event handler for when Data is added to the PS Progress stream.
        /// </summary>
        /// <param name="sender">Contains the complete PSDataCollection of all PS Process output items.</param>
        /// <param name="e">Contains the index ID of the added collection item and the ID of the PowerShell instance this event belongs to.</param>
        void PsProgress_DataAdded(object sender, DataAddedEventArgs e, PowerShell powershell, string scriptName)
        {
            if (powershell.Streams.Progress.Count > 0)
            {
                var progress = powershell.Streams.Progress.ReadAll() as Collection<ProgressRecord>;
                if (progress != null)
                {
                    foreach (ProgressRecord item in progress)
                    {
                        Console.WriteLine("PROGRESS:" + scriptName + ":" + item.CurrentOperation);

                    }
                }
            }
        }
        
        /// <summary>
        /// Event handler for when Data is added to the PS Verbose stream.
        /// </summary>
        /// <param name="sender">Contains the complete PSDataCollection of all PS Verbose output items.</param>
        /// <param name="e">Contains the index ID of the added collection item and the ID of the PowerShell instance this event belongs to.</param>
        void PsVerbose_DataAdded(object sender, DataAddedEventArgs e, PowerShell powershell, string scriptName)
        {
            if (powershell.Streams.Verbose.Count > 0)
            {
                var verbose = powershell.Streams.Verbose.ReadAll() as Collection<VerboseRecord>;
                if (verbose != null)
                {
                    foreach (VerboseRecord item in verbose)
                    {
                        //Console.WriteLine("VERBOSE: " + item.Message);
                        Logger.Trace("VERBOSE::PS:" + scriptName + ":" + item.Message);

                    }
                }
            }
        }
    }
}


//OLD STUFF:

//private string GetScript(string scriptPath)
//{
//    try
//    {
//        using (StreamReader reader = File.OpenText(scriptPath))
//        {
//            return reader.ReadToEnd();
//        }
//    }
//    catch (Exception ex)
//    {
//        Logger.Error(ex, string.Format("Failed to read the script file: {0}", scriptPath));
//        throw new FileLoadException(string.Format("Failed to read the script file: {0}", scriptPath), ex);
//    }
//}

//private string GetValidPath(string path)
//{
//    if (path == null)
//    {
//        throw new ArgumentException("Script path can't be empty.");
//    }

//    if (path.Length < 3)
//    {
//        return path.Substring(0, 1) + ":\\";
//    }

//    if (!Directory.Exists(path))
//    {
//        throw new ArgumentException("Script path doesn't exist.");
//    }

//    return path;
//}


/// <summary>
/// Run a powershell script from embedded resource or _scriptFolder with no parameters. If the file name isnt found as a resource, the file path will be used
/// </summary>
/// <param name="fileName"></param>
/// <returns></returns>
//public ICollection<PSObject> RunPowerShellScriptFile(string fileName)
//{
//    return RunPowerShellScriptFile(fileName, null);
//}


/// <summary>
/// ???
/// </summary>
/// <param name="psObjects"></param>
/// <returns></returns>
//public IEnumerable<string> GetPsObjectNames(ICollection<PSObject> psObjects)
//{
//    List<string> names = new List<string>();
//    if (psObjects == null)
//    {
//        return names;
//    }
//    names.AddRange(psObjects.Select(psObject => psObject.ToString()));
//    return names;
//}

