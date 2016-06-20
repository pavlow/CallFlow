using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using MetroFramework.Controls;
using MetroFramework.Forms;

namespace CallFlowManagerv2
{
    public partial class Form1 : MetroForm
    {
        #region VARIABLES
        //App log
        private LoggingOld log;

        //Bool to hold tab loaded state
        private bool workflowsLoaded, queuesLoaded, groupsLoaded, busHoursLoaded, holidaysLoaded, sharedLoaded;

        //Create instance of CsQueries class
        private CsQueries CsQ = new CsQueries();

        //IvrMenu class
        private List<IvrMenu> ivrMenus;

        //GridView BindingSources
        //Queues
        private List<CsQAgentGroups> queueGroups;
        private BindingSource queueGroupsBindingSource;
        //Groups
        private List<CsGrpAgents> groupAgents;
        private BindingSource groupAgentsBindingSource;
        //Bus Hours Groups
        private List<CsBusHoursGroup> busHoursGroup;
        private BindingSource busHoursGroupBindingSource;
        //Bus Hours
        private List<CsBusHours> busHours;
        private BindingSource busHoursBindingSource;
        //Holiday Groups
        private BindingList<CsHolidayGroup> holGroup;
        private BindingSource holGroupBindingSource;
        //Holidays
        private BindingList<CsHoliday> holidays;
        private BindingSource holBindingSource;
        
        //Texted Change Timers
        private System.Threading.Timer timerQTimeout;
        private System.Threading.Timer timerQOverflowNum;
        private System.Threading.Timer timerGrpRoutingTime;
        
        /// <summary>
        /// IVR Counters - Keeps track of the number of options added
        /// </summary>
        private int countIvrRoot = -1;
        private int countIvrSubRoot0 = -1;
        private int countIvrSubRoot1 = -1;
        private int countIvrSubRoot2 = -1;
        private int countIvrSubRoot3 = -1;
        private int countIvrSubRoot4 = -1;
        private int countIvrSubRoot5 = -1;
        private int countIvrSubRoot6 = -1;
        private int countIvrSubRoot7 = -1;
        private int countIvrSubRoot8 = -1;
        private int countIvrSubRoot9 = -1;
        
        //Max number of menu options
        private int maxOptions = 10;

        //Backgroups workers
        private BackgroundWorker wfWorker;
        private BackgroundWorker queueWorker;
        private BackgroundWorker groupWorker;
        private BackgroundWorker busHoursWorker;
        private BackgroundWorker holWorker;
        private BackgroundWorker sharedWorker;

        #endregion VARIABLES

        #region INITIALISE
        public Form1()
        {
            InitializeComponent();

            //Event for unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);

            //Logging - Start the application log
            try
            {
                log = LoggingOld.Instance;
                log.WriteToLog("Application started!");
            }
            catch (Exception ex)
            {
                ErrorHandler.ShowMessage("There was an error starting the event application log in Form1.cs:", ex.ToString(), "Application Error");
            }

            //Initialise variables
            try
            {
                InitialiseVariables();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("There was an error initialising variables in Form1.cs", ex);
            }
            
            //Load form
            try
            {
                FormSetup();
                LoadFormConstants();
                LoadFormDynamics();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("There was an error loading the form", ex);
            }
        }

        /// <summary>
        /// Tests for valid log instance then writes to the log
        /// </summary>
        /// <param name="text"></param>
        private void Log(string text)
        {
            if (log != null)
            {
                log.WriteToLog(text);
            }
        }

        /// <summary>
        /// Handles events that are raised as a result of an unhandled exception
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unhandled exception occured in the application. For more information, refer to the application log.", "Error - Unhandled Exception");
            Log(e.ExceptionObject.ToString());
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Initialises variables
        /// </summary>
        private void InitialiseVariables()
        {
            //Create instance of IvrMenu menu class
            ivrMenus = new List<IvrMenu>(110);

            //Timers used to delay the time between a user input in to a text box, and the resulting value being applied to the slider and label value
            timerQTimeout = new System.Threading.Timer((c) => UpdateQTimeout(), null, Timeout.Infinite, Timeout.Infinite);
            timerQOverflowNum = new System.Threading.Timer((c) => UpdateQOverflow(), null, Timeout.Infinite, Timeout.Infinite);
            timerGrpRoutingTime = new System.Threading.Timer((c) => UpdateGrpRoutingTime(), null, Timeout.Infinite, Timeout.Infinite);


            #region TAB LOAD STATE
            //Hold tab load state in bool
            queuesLoaded = false;
            groupsLoaded = false;
            busHoursLoaded = false;
            holidaysLoaded = false;
            sharedLoaded = false;
            #endregion TAB LOAD STATE

            #region BACKGROUND WORKERS - Initialise
            //BACKGROUND WORKERS - Initialise https://visualstudiomagazine.com/articles/2010/11/18/multithreading-in-winforms.aspx
            //Workflows
            wfWorker = new BackgroundWorker();
            wfWorker.WorkerReportsProgress = true;
            wfWorker.DoWork += WfDoWork;
            wfWorker.ProgressChanged += worker_WfProgressChanged;
            wfWorker.RunWorkerCompleted += worker_WfRunWorkerCompleted;

            //Queues
            queueWorker = new BackgroundWorker();
            queueWorker.WorkerReportsProgress = true;
            queueWorker.DoWork += QueueDoWork;
            queueWorker.ProgressChanged += worker_QueueProgressChanged;
            queueWorker.RunWorkerCompleted += worker_QueueRunWorkerCompleted;

            //Groups
            groupWorker = new BackgroundWorker();
            groupWorker.WorkerReportsProgress = true;
            groupWorker.DoWork += GroupDoWork;
            groupWorker.ProgressChanged += worker_GroupProgressChanged;
            groupWorker.RunWorkerCompleted += worker_GroupRunWorkerCompleted;

            //Business Hours
            busHoursWorker = new BackgroundWorker();
            busHoursWorker.WorkerReportsProgress = true;
            busHoursWorker.DoWork += BusHoursDoWork;
            busHoursWorker.ProgressChanged += worker_busHoursProgressChanged;
            busHoursWorker.RunWorkerCompleted += worker_busHoursRunWorkerCompleted;

            //Holidays
            holWorker = new BackgroundWorker();
            holWorker.WorkerReportsProgress = true;
            holWorker.DoWork += HolDoWork;
            holWorker.ProgressChanged += worker_holProgressChanged;
            holWorker.RunWorkerCompleted += worker_holRunWorkerCompleted;

            //Holidays
            sharedWorker = new BackgroundWorker();
            sharedWorker.WorkerReportsProgress = true;
            sharedWorker.DoWork += SharedDoWork;
            sharedWorker.ProgressChanged += worker_sharedProgressChanged;
            sharedWorker.RunWorkerCompleted += worker_sharedRunWorkerCompleted;

            #endregion BACKGROUND WORKERS - Initialise
        }

        #endregion INITIALISE   

        #region FORM LOAD
       
        /// <summary>
        /// Sets up form defaults
        /// </summary>
        private void FormSetup()
        {
            //Grid Borders
            metroGridBusHoursGroups.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            metroGridBusHoursOpenClose.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            metroGridHolGroups.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            metroGridHolList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            metroGridGAgentList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            metroGridQAgentCallGroups.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            //Grid Binding Sources
            //Queues
            queueGroups = new List<CsQAgentGroups>();
            queueGroupsBindingSource = new BindingSource(queueGroups, null);
            metroGridQAgentCallGroups.Columns.Clear();
            metroGridQAgentCallGroups.DataSource = queueGroupsBindingSource;
            //Groups
            groupAgents = new List<CsGrpAgents>();
            groupAgentsBindingSource = new BindingSource(groupAgents, null);
            metroGridGAgentList.Columns.Clear();
            metroGridGAgentList.DataSource = groupAgentsBindingSource;
            //Bus Hours Groups
            busHoursGroup = new List<CsBusHoursGroup>();
            busHoursGroupBindingSource = new BindingSource(busHoursGroup, null);
            metroGridBusHoursGroups.Columns.Clear();
            metroGridBusHoursGroups.DataSource = busHoursGroupBindingSource;
            //Bus Hours
            busHours = new List<CsBusHours>();
            busHoursBindingSource = new BindingSource(busHours, null);
            metroGridBusHoursOpenClose.Columns.Clear();
            metroGridBusHoursOpenClose.DataSource = busHoursBindingSource;
            //Holiday Groups
            holGroup = new BindingList<CsHolidayGroup>();
            holGroupBindingSource = new BindingSource(holGroup, null);
            metroGridHolGroups.Columns.Clear();
            metroGridHolGroups.DataSource = holGroupBindingSource;
            //Holidays
            holidays = new BindingList<CsHoliday>();
            holBindingSource = new BindingSource(holidays, null);
            metroGridHolList.Columns.Clear();
            metroGridHolList.DataSource = holBindingSource;

        }

        /// <summary>
        /// Loads constant data to form
        /// </summary>
        private void LoadFormConstants()
        {
            //Load static form controls
            //BUSINESS HOURS
            //Days of Week
            metroComboBoxBusHoursDay.DataSource = new BindingSource(Globals.DaysOfWeek, null);
            metroComboBoxBusHoursDay.DisplayMember = "Key";
            metroComboBoxBusHoursDay.ValueMember = "Value";
            //Time
            metroComboBoxBusHoursOpenHour1.DataSource = new BindingSource(Globals.TimeHours, null);
            metroComboBoxBusHoursOpenHour1.DisplayMember = "Key";
            metroComboBoxBusHoursOpenHour1.ValueMember = "Value";
            metroComboBoxBusHoursOpenMin1.DataSource = new BindingSource(Globals.TimeMinutes, null);
            metroComboBoxBusHoursOpenMin1.DisplayMember = "Key";
            metroComboBoxBusHoursOpenMin1.ValueMember = "Value";

            metroComboBoxBusHoursCloseHour1.DataSource = new BindingSource(Globals.TimeHours, null);
            metroComboBoxBusHoursCloseHour1.DisplayMember = "Key";
            metroComboBoxBusHoursCloseHour1.ValueMember = "Value";
            metroComboBoxBusHoursCloseMin1.DataSource = new BindingSource(Globals.TimeMinutes, null);
            metroComboBoxBusHoursCloseMin1.DisplayMember = "Key";
            metroComboBoxBusHoursCloseMin1.ValueMember = "Value";

            metroComboBoxBusHoursOpenHour2.DataSource = new BindingSource(Globals.TimeHours, null);
            metroComboBoxBusHoursOpenHour2.DisplayMember = "Key";
            metroComboBoxBusHoursOpenHour2.ValueMember = "Value";
            metroComboBoxBusHoursOpenMin2.DataSource = new BindingSource(Globals.TimeMinutes, null);
            metroComboBoxBusHoursOpenMin2.DisplayMember = "Key";
            metroComboBoxBusHoursOpenMin2.ValueMember = "Value";

            metroComboBoxBusHoursCloseHour2.DataSource = new BindingSource(Globals.TimeHours, null);
            metroComboBoxBusHoursCloseHour2.DisplayMember = "Key";
            metroComboBoxBusHoursCloseHour2.ValueMember = "Value";
            metroComboBoxBusHoursCloseMin2.DataSource = new BindingSource(Globals.TimeMinutes, null);
            metroComboBoxBusHoursCloseMin2.DisplayMember = "Key";
            metroComboBoxBusHoursCloseMin2.ValueMember = "Value";
            //Holidays
            metroComboBoxHolListStartHour.DataSource = new BindingSource(Globals.TimeHours, null);
            metroComboBoxHolListStartHour.DisplayMember = "Key";
            metroComboBoxHolListStartHour.ValueMember = "Value";
            metroComboBoxHolListStartMinute.DataSource = new BindingSource(Globals.TimeMinutes, null);
            metroComboBoxHolListStartMinute.DisplayMember = "Key";
            metroComboBoxHolListStartMinute.ValueMember = "Value";

            metroComboBoxHolListEndHour.DataSource = new BindingSource(Globals.TimeHours, null);
            metroComboBoxHolListEndHour.DisplayMember = "Key";
            metroComboBoxHolListEndHour.ValueMember = "Value";
            metroComboBoxHolListEndMinute.DataSource = new BindingSource(Globals.TimeMinutes, null);
            metroComboBoxHolListEndMinute.DisplayMember = "Key";
            metroComboBoxHolListEndMinute.ValueMember = "Value";
        }

        /// <summary>
        /// Loads dynamic data to form
        /// </summary>
        private void LoadFormDynamics()
        {
            //Get data from SfB and populate form
            LoadWf();
            LoadQueue();
            LoadGroup();
            LoadBusHours();
            LoadHolidays();

            //Update Ivr Mode (on/off)
            IvrModeToggle();
            
        }

        #endregion FORM LOAD

        #region TAB: WORKFLOWS
        //BACKGROUND WORKER:Workflows - start the worker
        private void LoadWf()
        {
            //Start background worker
            if (!wfWorker.IsBusy)
            {
                wfWorker.RunWorkerAsync();
                //Disable the button that starts the process so multi threads cant be started
                metroButtonWfLoad.Enabled = false;
                //Start progress spinner
                metroProgressSpinnerWfLoading.Visible = true;
                metroProgressSpinnerWfLoading.Spinning = true;
                metroLabelWfLoading.Text = "Loading Workflows...";
            }
        }

        //BACKGROUND WORKER:Workflows - Do Work
        private void WfDoWork(object sender, DoWorkEventArgs e)
        {
            //Update shared
            LoadShared();
            //Update CsQueries
            CsQ.GetCsWorkflows();
            //CsQ.GetCsQueues();
            //CsQ.GetCsGroups();
            //CsQ.GetCsUsers();
            //CsQ.UpdateUsersList();
            //CsQ.GetCsBusHours();
            //CsQ.UpdateBusinessHoursGroups();
            //CsQ.GetCsHolidays();
            //CsQ.GetSipDomains();
            //CsQ.UpdateSipDomainsList();
            //CsQ.GetRegistrarPools();

            //SHOULD MOVE THESE GENERICS OUT thenpopulate add combo boxs throught that rely on it
            //CsQ.GetSipDomains();
            //CsQ.UpdateSipDomainsList();

            #region e.g.

            //Filter the returned PSObject for required purpuse
            //var wFFilter = CsQ.Workflows.Where(x => x.Members["Name"].Value.ToString() == metroComboBoxWfSelector.SelectedItem.ToString()).ToList();

            //PROVIDED EXAMPLE
            //BackgroundWorker bgWorker = (BackgroundWorker)sender;
            //for (var i = 0; i < 10; i++)
            //{
            //    bgWorker.ReportProgress(i);
            //    Thread.Sleep(1000);
            //}

            #endregion e.g.
        }

        //BACKGROUND WORKER:Workflows - Update Progress
        private void worker_WfProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //metroTextBoxQDisplayName.Text = e.ProgressPercentage.ToString();
        }

        //BACKGROUND WORKER:Workflows - Completed
        void worker_WfRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Check if there was an error during DoWork processing
            if (e.Error != null)
                throw new ApplicationException("An error occured while executing the Workflow background worker.", e.Error);

            if (CsQ.Workflows != null)
            {
                ////Populate Workflows selector
                foreach (dynamic wf in CsQ.Workflows)
                {
                    if (wf != null)
                    {
                        metroComboBoxWfSelector.Items.Add(wf.Name);
                    }
                }

                workflowsLoaded = true;

                //re-enable button
                metroButtonWfLoad.Enabled = true;
                metroProgressSpinnerWfLoading.Visible = false;
                metroProgressSpinnerWfLoading.Spinning = false;
                metroLabelWfLoading.Text = "";
                
            }
            else
            {
                metroLabelWfLoading.Text = "Error loading workflows, please try again.";
            }

            ////TO BE MOVED when put in to seperate worker
            ////BusHours
            ////Pools
            //metroComboBoxBusHoursPool.DataSource = new BindingSource(CsQ.RegistrarPoolsList, null);
            ////Holidays
            ////Pools
            //metroComboBoxHolPool.DataSource = new BindingSource(CsQ.RegistrarPoolsList, null);


            ////Bind list to Sip Domain comboboxes as required
            //metroComboBoxWfSipDomain.DataSource = new BindingSource(CsQ.SipDomainsList, null);
            //metroComboBoxWfHolSipDomain.DataSource = new BindingSource(CsQ.SipDomainsList, null);
            //metroComboBoxWfAhSipDomain.DataSource = new BindingSource(CsQ.SipDomainsList, null);
            //metroComboBoxQTimeoutSipDomain.DataSource = new BindingSource(CsQ.SipDomainsList, null);
            //metroComboBoxQOverflowSipDomain.DataSource = new BindingSource(CsQ.SipDomainsList, null);

            //Populate Time Zones
            metroComboBoxWfTimeZone.DataSource = new BindingSource(Globals.TimeZones, null);
            metroComboBoxWfTimeZone.DisplayMember = "Key";
            metroComboBoxWfTimeZone.ValueMember = "Value";

            //Populate Languages
            metroComboBoxWfLanguage.DataSource = new BindingSource(Globals.Languages, null);
            metroComboBoxWfLanguage.DisplayMember = "Key";
            metroComboBoxWfLanguage.ValueMember = "Value";

            //Populate Destination Types:
            //After Hours
            metroComboBoxWfAhType.DataSource = new BindingSource(Globals.WfDestinations, null);
            metroComboBoxWfAhType.DisplayMember = "Key";
            metroComboBoxWfAhType.ValueMember = "Value";
            //Holidays
            metroComboBoxWfHolType.DataSource = new BindingSource(Globals.WfDestinations, null);
            metroComboBoxWfHolType.DisplayMember = "Key";
            metroComboBoxWfHolType.ValueMember = "Value";

            #region e.g.
            //LEARNING - Get the Workflows
            //var pSMembers = workflowName.Members;
            //metroTextBoxLogs.Text += pSMembers["Name"].Value + Environment.NewLine;
            //WorkflowNames = pS.ExecuteSynchronously(@"Get-CsRgsWorkflow | Select Name");
            //Collection<PSObject> WorkflowNames1 = pS.ExecuteSynchronously(@"Get-CsRgsWorkflow");
            //dynamic xd = WorkflowNames1.Where(WorkflowNames1.Name == "Andrew Test");
            //var xd = WorkflowNames1.Where(WorkflowNames1.Members["Name"]);
            //var filter = WorkflowNames1.Where(x => x.Members["Name"].Value.ToString() == "Andrew Test").ToList();

            //Filter the return PSObject
            //var filter = CsQ.Workflows.Where(x => x.Members["Name"].Value.ToString() == "Andrew Test").ToList();
            //foreach (PSObject item in filter)
            //{
            //    //MessageBox.Show(item.Members["Name"].Value.ToString());
            //}
            #endregion e.g.

        }

        /// <summary>
        /// Load Workflows
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void metroButtonWfLoad_Click(object sender, System.EventArgs e)
        {
            LoadWf();
        }

        /// <summary>
        /// Updates the form information
        /// </summary>
        private void RefreshFormWorkflows()
        {
            //Clear form
            string[] exclusions = "metroComboBoxWfSelector".Split(',');
            ClearPanelControls(metroPanelWf, exclusions);
            IvrReset();

            //If queue not in memory load
            if (CsQ.Workflows == null)
            {
                LoadWf();
            }

            //Filter the returned PSObject for required purpuse
            var wFFilter = CsQ.Workflows.Where(x => x.Members["Name"].Value.ToString() == metroComboBoxWfSelector.SelectedItem.ToString()).ToList();

            //foreach (PSObject workflowName in workflowNames)
            foreach (dynamic workflowName in wFFilter)
            {
                if (workflowName != null)
                {
                    //GetDisplay Name
                    metroTextBoxWfDisplayName.Text = workflowName.Name;

                    if (workflowName.Description != null)
                    {
                        metroTextBoxWfDescription.Text = workflowName.Description;
                    }

                    //Get Line Uri
                    metroTextBoxWfNumber.Text = workflowName.LineUri;

                    //Get Display Number
                    metroTextBoxWfDisplayNumber.Text = workflowName.DisplayNumber;

                    //Get Sip Address
                    var primaryUri = workflowName.PrimaryUri.Split('@');
                    metroTextBoxWfUri.Text = primaryUri[0];
                    metroComboBoxWfSipDomain.Text = primaryUri[1];

                    //Get Time Zones
                    metroComboBoxWfTimeZone.SelectedValue = workflowName.TimeZone;

                    //Get Languages
                    metroComboBoxWfLanguage.SelectedValue = workflowName.Language;
                    
                    //Get Toggle State
                    //Federation
                    if (workflowName.EnabledForFederation == true)
                    {
                        metroToggleWfFederation.Checked = true;
                    }
                    else metroToggleWfFederation.Checked = true;
                    
                    //Anonymous
                    if(workflowName.Anonymous == true)
                    {
                        metroToggleWfAnonymity.Checked = true;
                    }
                    else metroToggleWfAnonymity.Checked = false;
                    
                    //Ivr Mode
                    if (workflowName.DefaultAction.Action.ToString() == "TransferToQuestion")
                    {
                        //Set IVR mode to on
                        metroToggleWfIvrMode.Checked = true;

                        //Populate IVR options
                        foreach (var root in workflowName.DefaultAction.Question.AnswerList)
                        {
                            if (root.Action.QueueID != null)
                            {
                                //var AnswerQueueName = Get-CsRgsQueue -Identity AnswerList.Action.QueueID).Name
                            }

                            if (root.Action.Action.ToString() == "TransferToQuestion")
                            {
                                var root1 = IvrAddRoot(0);
                                root1.comboboxIvrDtmf1.Text = root.DtmfResponse;
                                root1.textboxIvrVoiceResponse1.Text = string.Join(",", root.VoiceResponseList);
                                root1.comboboxIvrQueue1.Enabled = false;
                                
                                
                                foreach (dynamic sub1 in root.Action.Question.AnswerList)
                                {
                                    if (sub1.Action.Action.ToString() == "TransferToQueue")
                                    {
                                        var sub = IvrAddSub1(Convert.ToInt32(root1.OptionPosition));
                                        sub.comboboxIvrDtmf1.Text = sub1.DtmfResponse;
                                        sub.textboxIvrVoiceResponse1.Text = string.Join(",", sub1.VoiceResponseList);
                                        sub.comboboxIvrQueue1.SelectedValue = sub1.Action.QueueID.ToString();
                                        //MessageBox.Show(sub1.Action.QueueID.ToString());
                                    }
                                    else
                                    {
                                        MessageBox.Show("Error!", "IVR Menus are only supported to 2 levels of depth");
                                        //Abort processing and grey out IVR controls
                                    }
                                }
                            }

                            if (root.Action.Action.ToString() == "TransferToQueue")
                            {
                                var root2 = IvrAddRoot(0);
                                root2.comboboxIvrDtmf1.Text = root.DtmfResponse;
                                root2.textboxIvrVoiceResponse1.Text = string.Join(",", root.VoiceResponseList);
                                root2.comboboxIvrQueue1.SelectedValue = root.Action.QueueID.ToString();
                                
                               //x.comboboxIvrQueue1.Enabled = false;

                                //var qFilter =
                                //CsQ.Queues.Where(
                                //    x => x.Members["Name"].Value.ToString() == metroComboBoxQSelector.SelectedItem.ToString())
                                //    .ToList(); 
                                //            LoadQueue() 
                                //            queuesLoaded

                                //item.Name
                                //item.Action.Question;
                                //item.Action.Prompt
                                //item.Action.Action
                                //item.Action.Uri
                            }
                        }

                        UpdateIvrControls();
                    }
                    else if (workflowName.DefaultAction.Action.ToString() == "TransferToQueue")
                    {
                        //IVR toggle off
                        metroToggleWfIvrMode.Checked = false;
                    }
                    
                    if (workflowName.DefaultAction.Prompt != null)
                    {
                        //Get Workflow Message
                        if (workflowName.DefaultAction.Prompt.TextToSpeechPrompt != null)
                        {
                            metroTextBoxWfWelcome.Text = workflowName.DefaultAction.Prompt.TextToSpeechPrompt.ToString();
                        }

                        //Get Workflow Audio
                        if (workflowName.DefaultAction.Prompt.AudioFilePrompt != null)
                        {
                            metroLabelWfWelcomeAudio.Text = workflowName.DefaultAction.Prompt.AudioFilePrompt.ToString();
                        }
                        else metroLabelWfWelcomeAudio.Text = "None";
                    }

                    //Get Queue


                    //Get Queue Music
                    if (workflowName.CustomMusicOnHoldFile != null)
                    {
                        metroLabelWfQueueMusic.Text = workflowName.CustomMusicOnHoldFile.ToString();
                    }
                    else metroLabelWfQueueMusic.Text = "None";

                    //Get Business Hours Group



                    //Get Afterhours Message
                    if (workflowName.NonBusinessHoursAction.Prompt != null)
                    {
                        if (workflowName.NonBusinessHoursAction.Prompt.TextToSpeechPrompt != null)
                        {
                            metroTextBoxWfAhMessage.Text =
                                workflowName.NonBusinessHoursAction.Prompt.TextToSpeechPrompt.ToString();
                        }
                    }

                    //Get Afterhours Audio
                    if (workflowName.NonBusinessHoursAction.Prompt != null)
                    {
                        if (workflowName.NonBusinessHoursAction.Prompt.AudioFilePrompt != null)
                        {
                            metroLabelWfAfterhoursAudio.Text =
                                workflowName.NonBusinessHoursAction.Prompt.AudioFilePrompt.ToString();
                        }
                        else metroLabelWfAfterhoursAudio.Text = "None";
                    }

                    //Get Afterhours Sip Address
                    if (workflowName.NonBusinessHoursAction.Uri != null)
                    {
                        var afterhoursUri = workflowName.NonBusinessHoursAction.Uri.Split('@');
                        metroTextBoxWfAhUri.Text = afterhoursUri[0];
                        metroComboBoxWfAhSipDomain.Text = afterhoursUri[1];
                    }

                    //Get Afterhours Destination Type
                    if (workflowName.NonBusinessHoursAction.Action != null)
                    {
                        metroComboBoxWfAhType.Text = workflowName.NonBusinessHoursAction.Action.ToString();
                    }


                    //Get Holidays Group

                    //Get Holidays Message
                    if (workflowName.HolidayAction.Prompt != null)
                    {
                        if (workflowName.HolidayAction.Prompt.TextToSpeechPrompt != null)
                        {
                            metroTextBoxWfHolMessage.Text =
                                workflowName.HolidayAction.Prompt.TextToSpeechPrompt.ToString();
                        }
                    }

                    //Get Holidays Audio
                    if (workflowName.HolidayAction.Prompt != null)
                    {
                        if (workflowName.HolidayAction.Prompt.AudioFilePrompt != null)
                        {
                            metroLabelWfHolAudio.Text = workflowName.HolidayAction.Prompt.AudioFilePrompt.ToString();
                        }
                        else metroLabelWfHolAudio.Text = "None";
                    }

                    //Get Holidays Sip Address
                    if (workflowName.HolidayAction.Uri != null)
                    {
                        var holidayUri = workflowName.HolidayAction.Uri.Split('@');
                        metroTextBoxWfHolUri.Text = holidayUri[0];
                        metroComboBoxWfHolSipDomain.Text = holidayUri[1];
                    }

                    //Get Holidays Destination Type
                    if (workflowName.HolidayAction.Action != null)
                    {
                        metroComboBoxWfHolType.Text = workflowName.HolidayAction.Action.ToString();
                    }
                }
            }
        }

        /// <summary>
        /// Updates the Workflow display on the form based on the user Workflow selected in the drop down menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void metroComboBoxWfSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshFormWorkflows();
        }

        /// <summary>
        /// IVR Mode - Enabled/Disable event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void metroToggleWfIvrMode_CheckedChanged(object sender, EventArgs e)
        {
            IvrModeToggle();
        }

        /// <summary>
        /// IVR Mode - Enables/Disables IVR controls based on toggle status
        /// </summary>
        private void IvrModeToggle()
        {
            if (metroToggleWfIvrMode.Checked)
            {
                UpdateIvrControls();
                metroButtonWfIvrAddOption.Enabled = true;
                metroButtonWfIvrAudio.Enabled = true;
                metroLabelWfIvrMessage.Enabled = true;

                //metroPanelWfIvrOptions.Visible = true;
                //metroLabelWfIvrMessage.Visible = true;
                //metroButtonWfIvrAddOption.Visible = true;
                //metroButtonWfIvrAudio.Visible = true;
            }
            else if (!metroToggleWfIvrMode.Checked)
            {
                metroPanelWfIvrOptions.Controls.Clear();
                metroButtonWfIvrAddOption.Enabled = false;
                metroButtonWfIvrAudio.Enabled = false;
                metroLabelWfIvrMessage.Enabled = false;

                //metroPanelWfIvrOptions.Visible = false;
                //metroLabelWfIvrMessage.Visible = false;
                //metroButtonWfIvrAddOption.Visible = false;
                //metroButtonWfIvrAudio.Visible = false;
            }
        }

        #endregion TAB: WORKFLOWS

        #region TAB: QUEUES
        //BACKGROUND WORKER - start the worker
        private void LoadQueue()
        {
            //Start background worker
            if (!queueWorker.IsBusy)
            {
                queueWorker.RunWorkerAsync();
                //Disable the button that starts the process so multi threads cant be started
                metroButtonQLoad.Enabled = false;
                //Start progress spinner
                metroProgressSpinnerQLoading.Visible = true;
                metroProgressSpinnerQLoading.Spinning = true;
                metroLabelQLoading.Text = "Loading Queues...";
            }
        }

        //BACKGROUND WORKER - Do Work
		private void QueueDoWork(object sender, DoWorkEventArgs e)
		{
            //Update shared
            LoadShared();

            //BackgroundWorker bgWorker = (BackgroundWorker) sender;
            CsQ.GetCsQueues();
		}

        //BACKGROUND WORKER - Update Progress
		private void worker_QueueProgressChanged(object sender, ProgressChangedEventArgs e)
		{
            //metroTextBoxQDisplayName.Text = e.ProgressPercentage.ToString();
		}

        //BACKGROUND WORKER - Completed
		void worker_QueueRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
            //Check if there was an error during DoWork processing
            if (e.Error != null)
                throw new ApplicationException("An error occured while executing the Queue background worker.", e.Error);

		    if (CsQ.Queues != null)
		    {
		        //Populate Queue selector
		        foreach (dynamic queueName in CsQ.Queues)
		        {
		            if (queueName != null)
		            {
		                metroComboBoxQSelector.Items.Add(queueName.Name);
		            }
		        }

                queuesLoaded = true;

		        //re-enable button
		        metroButtonQLoad.Enabled = true;
		        metroProgressSpinnerQLoading.Visible = false;
		        metroProgressSpinnerQLoading.Spinning = false;
		        metroLabelQLoading.Text = "";
		    }
		    else
		    {
		        metroLabelQLoading.Text = "Error loading queues, please try again.";
		    }

		    //Bind list to type comboboxes as required
            metroComboBoxQTimeoutType.DataSource = new BindingSource(Globals.WfDestinations, null);
		    metroComboBoxQTimeoutType.DisplayMember = "Key";
            metroComboBoxQTimeoutType.ValueMember = "Value";
            metroComboBoxQOverflowType.DataSource = new BindingSource(Globals.WfDestinations, null);
            metroComboBoxQOverflowType.DisplayMember = "Key";
            metroComboBoxQOverflowType.ValueMember = "Value";
		}

        /// <summary>
        /// Load Queues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void metroButtonQLoad_Click(object sender, System.EventArgs e)
        {
            LoadQueue();
        }
      
        /// <summary>
        /// Loads the default queue control values
        /// </summary>
        private void QueueLoadDefaults()
        {
            metroTrackBarQTimeoutSec.Value = 10;
            //metroTextBoxQTimeoutSec;
            metroTrackBarQOverflowNum.Value = 0;
            //metroTextBoxQOverflow

        }

        /// <summary>
        /// Updates the form information
        /// </summary>
        private void RefreshFormQueues()
        {
            string[] exclusions = "metroComboBoxQSelector".Split(',');
            ClearPanelControls(metroPanelQ, exclusions);
            QueueLoadDefaults();
            
            //If queue not in memory load
            if (CsQ.Queues == null)
            {
                LoadQueue();
            }

            //Filter the Queue object in memory to the selected queue
            var qFilter =
                    CsQ.Queues.Where(
                        x => x.Members["Name"].Value.ToString() == metroComboBoxQSelector.SelectedItem.ToString())
                        .ToList(); 
        
            if (qFilter != null)
            {
                foreach (dynamic queue in qFilter)
                {
                    if (queue != null)
                    {
                        if (queue.Name != null)
                        {
                            metroTextBoxQDisplayName.Text = queue.Name;
                        }
                        if (queue.Name != null)
                        {
                            metroTextBoxQDescription.Text = queue.Description;
                        }
                        //Queue timeout
                        if (queue.TimeoutThreshold != null)
                        {
                            metroToggleQTimeoutOn.Checked = true;

                            metroTrackBarQTimeoutSec.Value = queue.TimeoutThreshold;

                            var qUri = queue.TimeoutAction.Uri.ToString().Split('@');
                            var qUriLeft = qUri[0].Split(':');
                            metroTextBoxQTimeoutUri.Text = qUriLeft[1];
                            metroComboBoxQTimeoutSipDomain.Text = qUri[1];
                            metroComboBoxQTimeoutType.SelectedValue = queue.TimeoutAction.Action.ToString();
                        }
                        else metroToggleQTimeoutOn.Checked = false;
                       
                        //Queue overflow
                        if (queue.OverflowThreshold != null)
                        {
                            metroToggleQOverflowOn.Checked = true;

                            metroTrackBarQOverflowNum.Value = queue.OverflowThreshold;

                            var qOverflowUri = queue.OverflowAction.Uri.ToString().Split('@');
                            var qOverflowUriLeft = qOverflowUri[0].Split(':');
                            metroTextBoxQOverflowUri.Text = qOverflowUriLeft[1];
                            metroComboBoxQOverflowSipDomain.Text = qOverflowUri[1];
                            metroComboBoxQOverflowType.SelectedValue = queue.OverflowAction.Action.ToString();
                        }
                        else metroToggleQOverflowOn.Checked = false;

                        //Queue agent list
                        if (queue.AgentGroupIDList != null)
                        {
                            //Clear current agent groups
                            queueGroups.Clear();

                            //int i = 0;

                            foreach (var queueAgent in queue.AgentGroupIDList)
                            {
                                //Filter the Queue object in memory to the selected queue
                                if (CsQ.Groups == null)
                                {
                                    LoadGroup();
                                }
                                if (CsQ.Groups != null)
                                {
                                    dynamic agentFilter =
                                        CsQ.Groups.Where(
                                            x => x.Members["Identity"].Value.ToString() == queueAgent.ToString())
                                            .ToList();

                                    foreach (var agent in agentFilter)
                                    {
                                        //MessageBox.Show(agent.Name);

                                        if (agent != null)
                                        {
                                            if (agent.Name != null && agent.AgentAlertTime != null && agent.ParticipationPolicy != null && agent.RoutingMethod != null)
                                            {
                                                //metroGridQAgentCallGroups.Rows.Add();
                                                //metroGridQAgentCallGroups.Rows[i].Cells["metroGridQGrid_Name"].Value = agent.Name.ToString();
                                                //metroGridQAgentCallGroups.Rows[i].Cells["metroGridQGrid_AlertTime"].Value = agent.AgentAlertTime.ToString();
                                                //metroGridQAgentCallGroups.Rows[i].Cells["metroGridQGrid_ParticipationPolicy"].Value = agent.ParticipationPolicy.ToString();
                                                //metroGridQAgentCallGroups.Rows[i].Cells["metroGridQGrid_RoutingMethod"].Value = agent.RoutingMethod.ToString();
                                                //i++;
                                                QueueAddAgentGroup(agent.Name.ToString(), agent.AgentAlertTime.ToString(), agent.ParticipationPolicy.ToString(), agent.RoutingMethod.ToString());
                                            }
                                        }
                                    }
                                }
                                else MessageBox.Show("Error retrieving group information");

                            }
                        }


                        //Prompt
                        //Question
                        //Action   : TransferToVoicemailUri
                        //QueueID  : 
                        //Uri      : sip:andrew.test@lexel.co.nz//OwnerPool
                        //AgentGroupIDList
                        // {service:ApplicationServer:LXLLS2013FE.lexel.local/5b2fa39c-0f1d-4748-b185-68ce01f6d93d, service:ApplicationServer:LXLLS2013FE.lexel.local/0411615c-60fb-4932-a5a2-af6c90dc2b4f, 
                    }
                }
            }

        }

        /// <summary>
        /// Updates the form based on the queue selected from the drop down menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void metroComboBoxQSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshFormQueues();
        }


        /// <summary>
        /// Updates the queue timeout slider and text
        /// </summary>
        private void UpdateQTimeout()
        {
            int min = 10;
            int max = 7200;
            //Thread Safe
            if (InvokeRequired)
            {
               Invoke(new Action(UpdateQTimeout));
            }
            else
            {
                int num;
                if (int.TryParse(metroTextBoxQTimeoutSec.Text, out num))
                {
                    if (num >= min && num <= max)
                    {
                        metroTrackBarQTimeoutSec.Value = num;
                        metroLabelQTimeoutSendTo.Text = "After " + num + " seconds send to:";
                    }
                    else if (num < min)
                    {
                        metroTrackBarQTimeoutSec.Value = min;
                        metroLabelQTimeoutSendTo.Text = "After " + min + " seconds send to:";
                    }
                    else if (num > max)
                    {
                        metroTrackBarQTimeoutSec.Value = max;
                        metroLabelQTimeoutSendTo.Text = "After " + max + " seconds send to:";
                    }
                }
            } 
        }

        /// <summary>
        /// Queue timeout silder value change event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void metroTrackBarQTimeoutSec_ValueChanged(object sender, EventArgs e)
        {
            metroTextBoxQTimeoutSec.Text = metroTrackBarQTimeoutSec.Value.ToString();
        }

        /// <summary>
        /// Queue timeout textbox change event
        /// http://johannblais.blogspot.co.nz/2010/09/add-delay-before-processing-on-textbox.html
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void metroTextBoxQTimeoutSec_TextChanged(object sender, EventArgs e)
        {
            //textbox text change, start input wait timer
            timerQTimeout.Change(1000, Timeout.Infinite);
        }


        /// <summary>
        /// Updates the queue overflow slider and text
        /// </summary>
        private void UpdateQOverflow()
        {
            int min = 0;
            int max = 100;
            //Thread Safe
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateQOverflow));
            }
            else
            {
                int num;
                if (int.TryParse(metroTextBoxQOverflow.Text, out num))
                {
                    if (num >= min && num <= max)
                    {
                        metroTrackBarQOverflowNum.Value = num;
                        metroLabelQOverflowSendTo.Text = "After " + num + " concurrent calls send to:";
                    }
                    else if (num < min)
                    {
                        metroTrackBarQOverflowNum.Value = min;
                        metroLabelQOverflowSendTo.Text = "After " + min + " concurrent calls send to:";
                    }
                    else if (num > max)
                    {
                        metroTrackBarQOverflowNum.Value = max;
                        metroLabelQOverflowSendTo.Text = "After " + max + " concurrent calls send to:";
                    }
                }
            }
        }

        /// <summary>
        /// Queue overflow silder value change event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void metroTrackBarQOverflow_ValueChanged(object sender, EventArgs e)
        {
            metroTextBoxQOverflow.Text = metroTrackBarQOverflowNum.Value.ToString();
        }

        /// <summary>
        /// Queue overflow textbox change event
        /// http://johannblais.blogspot.co.nz/2010/09/add-delay-before-processing-on-textbox.html
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void metroTextBoxQOverflow_TextChanged(object sender, EventArgs e)
        {
            //textbox text change, start input wait timer
            timerQOverflowNum.Change(1000, Timeout.Infinite);
        }


        /// <summary>
        /// Adds CsQAgentGroups to List 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="alertTime"></param>
        /// <param name="participationPolicy"></param>
        /// <param name="routingMethod"></param>
        private void QueueAddAgentGroup(string name, string alertTime, string participationPolicy, string routingMethod)
        {
            var match = queueGroups.Where(p => p.Name == name);
            var matchCount = match.Count();

            if (matchCount == 0)
            {
                queueGroups.Add(new CsQAgentGroups(name, alertTime, participationPolicy, routingMethod));
                queueGroupsBindingSource.ResetBindings(false);
            }
            else if (matchCount > 1)
            {
                MessageBox.Show("Error determining if item already exisits - more than 1 result returned");
            }
            else if (matchCount == 1)
            {
                MessageBox.Show("Already exists");
            }
        }

        private void metroButtonQAgentCallGroupsAdd_Click(object sender, EventArgs e)
        {
            if (!groupsLoaded)
                LoadGroup();

            var qFilter =
                    CsQ.Groups.Where(
                        x => x.Members["Name"].Value.ToString() == metroComboBoxQAgentCallGroups.SelectedItem.ToString())
                        .ToList();
            foreach (dynamic group in qFilter)
            {
                //MessageBox.Show(group.name.ToString());
//                QueueAddAgentGroup(group.name.ToString(), group.AgentAlertTime.ToString(), group.ParticipationPolicy.ToString(), group.RoutingMethod.ToString());
            }
            //MessageBox.Show(metroComboBoxQAgentCallGroups.SelectedItem.ToString());
            //for (int i = 0; i < 10; i++)
            //{
            //    QueueAddAgentGroup("Name: " + i, "AlertTime: " + i, "ParticipationPolicy: " + i,"RoutingMethod: " + i);
            //}
        }

        private void metroButtonQAgentCallGroupsRemove_Click(object sender, EventArgs e)
        {
            //Delete selected rows
            //GridControl.Delete(metroGridQAgentCallGroups);
            //queueGroupsBindingSource.RemoveCurrent();
            BsControl.RemoveSelected(queueGroupsBindingSource);
        }

        private void metroButtonQAgentCallGroupsUp_Click(object sender, EventArgs e)
        {
            //GridControl.MoveRowUp(metroGridQAgentCallGroups);
            BsControl.Up(queueGroupsBindingSource);
        }

        private void metroButtonQAgentCallGroupsDn_Click(object sender, EventArgs e)
        {
            //GridControl.MoveRowDown(metroGridQAgentCallGroups);
            BsControl.Down(queueGroupsBindingSource);
        }

        #endregion TAB: QUEUES

        #region TAB: GROUPS
        //BACKGROUND WORKER - start the worker
        private void LoadGroup()
        {
            //Start background worker
            if (!groupWorker.IsBusy)
            {
                groupWorker.RunWorkerAsync();
                //Disable the button that starts the process so multi threads cant be started
                metroButtonGrpLoad.Enabled = false;
                //Start progress spinner
                metroProgressSpinnerGrpLoading.Visible = true;
                metroProgressSpinnerGrpLoading.Spinning = true;
                metroLabelGrpLoading.Text = "Loading Groups...";
            }
        }

        //BACKGROUND WORKER - Do Work
        private void GroupDoWork(object sender, DoWorkEventArgs e)
        {
            //Update shared
            LoadShared();

            //BackgroundWorker bgWorker = (BackgroundWorker)sender;
            CsQ.GetCsGroups();
            CsQ.GetCsUsers();
            CsQ.UpdateUsersList();
        }

        //BACKGROUND WORKER - Update Progress
        private void worker_GroupProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //metroTextBoxGrpDisplayName.Text = e.ProgressPercentage.ToString();
        }

        //BACKGROUND WORKER - Completed
        void worker_GroupRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Check if there was an error during DoWork processing
            if (e.Error != null)
                throw new ApplicationException("An error occured while executing the Group background worker.", e.Error);

            if (CsQ.Groups != null)
            {
                //Populate Queue selector
                foreach (dynamic grp in CsQ.Groups)
                {
                    if (grp != null)
                    {
                        metroComboBoxGrpSelector.Items.Add(grp.Name);
                        metroComboBoxQAgentCallGroups.Items.Add(grp.Name);
                    }
                }

                //Bind list to type comboboxes as required
                metroComboBoxGrpRoutingMethod.DataSource = new BindingSource(Globals.GrpRoutingMethods, null);
                metroComboBoxGrpRoutingMethod.DisplayMember = "Key";
                metroComboBoxGrpRoutingMethod.ValueMember = "Value";

                //Users
                if (CsQ.UsersList.Count > 0)
                {
                    metroComboBoxGrpAgents.DataSource = new BindingSource(CsQ.UsersList, null);
                    metroComboBoxGrpAgents.DisplayMember = "Key";
                    metroComboBoxGrpAgents.ValueMember = "Value";
                }

                groupsLoaded = true;

                //re-enable button
                metroButtonGrpLoad.Enabled = true;
                metroProgressSpinnerGrpLoading.Visible = false;
                metroProgressSpinnerGrpLoading.Spinning = false;
                metroLabelGrpLoading.Text = "";
            }
            else
            {
                metroLabelGrpLoading.Text = "Error loading groups, please try again.";
            }
        }

        /// <summary>
        /// Loads Groups
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void metroButtonGrpLoad_Click(object sender, System.EventArgs e)
        {
            LoadGroup();
        }

        /// <summary>
        /// Updates form based on the group selected from the drop down menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void metroComboBoxGrpSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshFormGroups();
        }

        private void GroupsLoadDefaults()
        {
            metroTrackBarGrpRoutingTime.Value = 10;
        }

        /// <summary>
        /// Updates the form information
        /// </summary>
        private void RefreshFormGroups()
        {
            string[] exclusions = "metroComboBoxGrpSelector".Split(',');
            ClearPanelControls(metroPanelGrp, exclusions);
            GroupsLoadDefaults();

            //If group not in memory load
            if (CsQ.Groups == null)
            {
                LoadGroup();
            }

            //Filter the Group object in memory to the selected queue
            var grpFilter =
                    CsQ.Groups.Where(
                        x => x.Members["Name"].Value.ToString() == metroComboBoxGrpSelector.SelectedItem.ToString())
                        .ToList();

            if (grpFilter != null)
            {
                foreach (dynamic grp in grpFilter)
                {
                    if (grp != null)
                    {
                        //Name
                        if (grp.Name != null)
                        metroTextBoxGrpDisplayName.Text = grp.Name;
                        //Description
                        if (grp.Description != null)
                            metroTextBoxGrpDescription.Text = grp.Description;
                        //Enable Distribution Group
                        if (grp.DistributionGroupAddress != null)
                        {
                            metroToggleGrpDistyGroup.Checked = true;
                            metroTextBoxGrpDistyGroup.Text = grp.DistributionGroupAddress;
                        }
                        //Enable Agent Group
                        else if (grp.DistributionGroupAddress == null)
                        {
                            metroToggleGrpAgents.Checked = true;
                             
                        }

                        //Routing Methods
                        if (grp.RoutingMethod != null)
                            metroComboBoxGrpRoutingMethod.SelectedValue = grp.RoutingMethod.ToString();

                        //Routing Time
                        if (grp.AgentAlertTime != null)
                            metroTrackBarGrpRoutingTime.Value = grp.AgentAlertTime;

                        //Agent Sign-In
                        if (grp.ParticipationPolicy != null)
                        {
                            if (grp.ParticipationPolicy.ToString() == "Formal")
                            {
                                metroToggleGrpAgentSignIn.Checked = true;
                            }
                            else if (grp.ParticipationPolicy.ToString() == "Informal")
                            {
                                metroToggleGrpAgentSignIn.Checked = false;
                            }
                        }
                        
                        //metroComboBoxGrpAgents
                        //Agent Grid
                        //metroGridGAgentList;

                        //Group Agents
                        if (grp.AgentsByUri.Count > 0)
                        {
                            //Clear Agents
                            groupAgents.Clear();

                            foreach (var agentUri in grp.AgentsByUri)
                            {
                                if (agentUri != null)
                                {
                                    var userFilter =
                                        CsQ.Users.Where(
                                            x => x.Members["SipAddress"].Value.ToString() == agentUri.ToString())
                                            .ToList();

                                    //USER GROUP MEMBERSHIP _ COME BACK TO LATER
                                    //CsQ.GetCsUserRgsGrpMembership(agentUri.ToString());
                                    //if (CsQ.UserRgsGrpMembership != null)
                                    //{
                                    //    var test = string.Join(",", CsQ.UserRgsGrpMembership);
                                    //    MessageBox.Show(string.Join(",", CsQ.UserRgsGrpMembership.ToString()));
                                    //}

                                    //var uri =
                                    //from g in CsQ.Groups
                                    //from dynamic member in group.Members
                                    //where
                                    //    (from agent in (IEnumerable<dynamic>)member.AgentsByUri
                                    //     where agent.Name = "AgentsByUri"  // this line may be redundant
                                    //     from x in (IEnumerable<dynamic>)agent.Value
                                    //     select x.AbsoluteUri).Contains("SomeValue")
                                    //select member.Value;

                                    if (userFilter != null)
                                    {
                                        foreach (dynamic user in userFilter)
                                        {
                                            GroupAddAgent(user.Name, agentUri.ToString());
                                            //MessageBox.Show(agentUri.ToString() + user.Name);
                                        }
                                    }
                                }
                            }
                        }

                        //Owning Pool
                        if (grp.OwnerPool != null)
                        metroLabelGrpOwnerPool.Text = "Owner: " + grp.OwnerPool;

                        //Identity
                        if (grp.Identity != null)
                        metroLabelGrpIdentity.Text = "Identity: " + grp.Identity;
                    }
                }
            }

        }
        
        private void GroupAddAgent(string name, string sipAddress)
        {
            var match = groupAgents.Where(p => p.Name == name);
            var matchCount = match.Count();

            if (matchCount == 0)
            {
                groupAgents.Add(new CsGrpAgents(name, sipAddress));
                groupAgentsBindingSource.ResetBindings(false);
            }
            else if (matchCount > 1)
            {
                MessageBox.Show("Error determining if item already exisits - more than 1 result returned");
            }
            else if (matchCount == 1)
            {
                MessageBox.Show("Already exists");
            }
        }

        private void metroButtonGrpAgentsAdd_Click(object sender, EventArgs e)
        {
            if (!groupsLoaded)
                LoadGroup();

            var userFilter =
                    CsQ.Users.Where(
                        x => x.Members["SipAddress"].Value.ToString() == metroComboBoxGrpAgents.SelectedValue.ToString())
                        .ToList();
            
            foreach (dynamic user in userFilter)
            {
                //MessageBox.Show(group.name.ToString());
                GroupAddAgent(user.name.ToString(), user.SipAddress);
            }
        }

        private void metroButtonGrpAgentsRemove_Click(object sender, EventArgs e)
        {
            BsControl.RemoveSelected(groupAgentsBindingSource);
        }

        private void metroButtonGrpAgentsUp_Click(object sender, EventArgs e)
        {
            BsControl.Up(groupAgentsBindingSource);
        }

        private void metroButtonGrpAgentsDn_Click(object sender, EventArgs e)
        {
            BsControl.Down(groupAgentsBindingSource);
        }

        //Distribution Group Toggle
        private void metroToggleGrpDistyGroup_CheckedChanged(object sender, EventArgs e)
        {
            if (metroToggleGrpDistyGroup.Checked)
            {
                metroToggleGrpAgents.Checked = false;
            }
            else if (!metroToggleGrpDistyGroup.Checked)
            {
                metroToggleGrpAgents.Checked = true;
            }
        }

        //Agent Group Toggle
        private void metroToggleGrpAgents_CheckedChanged(object sender, EventArgs e)
        {
            if (metroToggleGrpAgents.Checked)
            {
                metroToggleGrpDistyGroup.Checked = false;
            }
            else if (!metroToggleGrpAgents.Checked)
            {
                metroToggleGrpDistyGroup.Checked = true;
            }
        }

        private void metroComboBoxGrpAgents_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void metroComboBoxGrpRoutingMethod_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Updates the routing time slider and text
        /// </summary>
        private void UpdateGrpRoutingTime()
        {
            int min = 10;
            int max = 7200;
            //Thread Safe
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateGrpRoutingTime));
            }
            else
            {
                int num;
                if (int.TryParse(metroTextBoxGrpRoutingTime.Text, out num))
                {
                    if (num >= min && num <= max)
                    {
                        metroTrackBarGrpRoutingTime.Value = num;
                        metroLabelGrpAgentAlertInfo.Text = "Alert agents for " + num + " seconds in parallel:";
                    }
                    else if (num < min)
                    {
                        metroTrackBarGrpRoutingTime.Value = min;
                        metroLabelGrpAgentAlertInfo.Text = "Alert agents for " + metroLabelGrpAgentAlertInfo + " seconds in parallel:";
                    }
                    else if (num > max)
                    {
                        metroTrackBarGrpRoutingTime.Value = max;
                        metroLabelGrpAgentAlertInfo.Text = "Alert agents for " + max + " seconds in parallel:";
                    }
                }
            }
        }


        //Routing Time Slider Changed
        private void metroTrackBarGrpRoutingTime_ValueChanged(object sender, EventArgs e)
        {
            metroTextBoxGrpRoutingTime.Text = metroTrackBarGrpRoutingTime.Value.ToString();
        }

        //Routing Time Text Changed
        private void metroTextBoxGrpRoutingTime_TextChanged(object sender, EventArgs e)
        {
            //textbox text change, start input wait timer
            timerGrpRoutingTime.Change(1000, Timeout.Infinite);
        }

        private void metroToggleGrpAgentSignIn_CheckedChanged(object sender, EventArgs e)
        {

        }
        
        #endregion TAB: GROUPS

        #region TAB: BUSINESS HOURS
        //BACKGROUND WORKER - start the worker
        private void LoadBusHours()
        {
            //Start background worker
            if (!busHoursWorker.IsBusy)
            {
                busHoursWorker.RunWorkerAsync();
                //Disable the button that starts the process so multi threads cant be started
                metroButtonBusHoursLoad.Enabled = false;
                //Start progress spinner
                metroProgressSpinnerBusHoursLoading.Visible = true;
                metroProgressSpinnerBusHoursLoading.Spinning = true;
                metroLabelBusHoursLoading.Text = "Loading Business Hours...";
            }
        }

        //BACKGROUND WORKER - Do Work
        private void BusHoursDoWork(object sender, DoWorkEventArgs e)
        {
            //Update shared
            LoadShared();

            //BackgroundWorker bgWorker = (BackgroundWorker)sender;
            CsQ.GetCsBusHours();
            CsQ.UpdateBusinessHoursGroups();
            
        }

        //BACKGROUND WORKER - Update Progress
        private void worker_busHoursProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //metroTextBoxGrpDisplayName.Text = e.ProgressPercentage.ToString();
        }

        //BACKGROUND WORKER - Completed
        void worker_busHoursRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string regexBusHoursGroupNames = @"_\w{8}-\w{4}-\w{4}-\w{4}-\w{12}";
            //int i = 0;

            if (CsQ.BusHours != null && CsQ.BusinessHoursGroups != null)
            {
                foreach (dynamic bhour in CsQ.BusHours)
                {
                    if (bhour != null)
                    {
                        //Filter out Business Hours that contain GUID - these are system created for customised business hours on a queue by queue basis
                        Regex regex = new Regex(regexBusHoursGroupNames);
                        Match match = regex.Match(bhour.Name);

                        //Those that dont contain a GUID are deemed as user created
                        if (!match.Success)
                        {
                            BusHoursGroupAdd(bhour.Name, bhour.OwnerPool.ToString(), bhour.Identity.ToString());
                            BusHoursAddFromSfB(bhour.Name);
                            //MessageBox.Show(bhoursName.Name);
                            //metroGridBusHoursGroups.Rows.Add();
                            //metroGridBusHoursGroups.Rows[i].Cells["dataGridViewBusHoursGroups_Name"].Value = bhour.Name;
                            //i++;
                        }

                    }
                }

                //Update BusHoursGroups grid
                busHoursGroupBindingSource.ResetBindings(false);

                //Populate pools drop down
                //NOW DONE IN WORKFLOWS AFTER QUERY TO PS COMPLETED
                //if (CsQ.RegistrarPools != null)
                //{
                //    metroComboBoxBusHoursPool.Items.Clear();
                //    foreach (dynamic registrarPool in CsQ.RegistrarPools)
                //    {
                //        metroComboBoxBusHoursPool.Items.Add(registrarPool.Identity);
                //    }
                //}

                busHoursLoaded = true;

                //re-enable button
                metroButtonBusHoursLoad.Enabled = true;
                metroProgressSpinnerBusHoursLoading.Visible = false;
                metroProgressSpinnerBusHoursLoading.Spinning = false;
                metroLabelBusHoursLoading.Text = "";
            }
            else
            {
                metroLabelBusHoursLoading.Text = "Error loading business hours, please try again.";
            }
        }

        
        private void BusHoursAddFromSfB(string groupName)
        {
            var busHoursFilter =
                        CsQ.BusHours.Where(
                            x => x.Members["Name"].Value.ToString() == groupName)
                            .ToList();

                if (busHoursFilter != null)
                {
                    foreach (dynamic bhour in busHoursFilter)
                    {
                        if (bhour != null)
                        {
                            //Monday 1 only
                            if (bhour.MondayHours1 != null && bhour.MondayHours2 == null)
                            { 
                                if (bhour.MondayHours1.OpenTime != null && bhour.MondayHours1.CloseTime != null)
                                {
                                    BusHoursAdd(groupName, "Monday", bhour.MondayHours1.OpenTime.ToString(), bhour.MondayHours1.CloseTime.ToString(), "", "");
                                }
                            }
                            //Monday 2 only
                            else if (bhour.MondayHours1 == null && bhour.MondayHours2 != null)
                            { 
                                if (bhour.MondayHours2.OpenTime != null && bhour.MondayHours2.CloseTime != null)
                                {
                                    BusHoursAdd(groupName, "Monday", "", "", bhour.MondayHours2.OpenTime.ToString(), bhour.MondayHours2.CloseTime.ToString());
                                }
                            }
                            //Monday 1 and 2
                            else if (bhour.MondayHours1 != null && bhour.MondayHours2 != null)
                            {
                                if (bhour.MondayHours1.OpenTime != null && bhour.MondayHours1.CloseTime != null && bhour.MondayHours2.OpenTime != null && bhour.MondayHours2.CloseTime != null)
                                {
                                    BusHoursAdd(groupName, "Monday", bhour.MondayHours1.OpenTime.ToString(), bhour.MondayHours1.CloseTime.ToString(), bhour.MondayHours2.OpenTime.ToString(), bhour.MondayHours2.CloseTime.ToString());
                                }
                            }


                            //Tuesday 1 only
                            if (bhour.TuesdayHours1 != null && bhour.TuesdayHours2 == null)
                            {
                                if (bhour.TuesdayHours1.OpenTime != null && bhour.TuesdayHours1.CloseTime != null)
                                {
                                    BusHoursAdd(groupName, "Tuesday", bhour.TuesdayHours1.OpenTime.ToString(), bhour.TuesdayHours1.CloseTime.ToString(), "", "");
                                }
                            }
                            //Tuesday 2 only
                            else if (bhour.TuesdayHours1 == null && bhour.TuesdayHours2 != null)
                            {
                                if (bhour.TuesdayHours2.OpenTime != null && bhour.TuesdayHours2.CloseTime != null)
                                {
                                    BusHoursAdd(groupName, "Tuesday", "", "", bhour.TuesdayHours2.OpenTime.ToString(), bhour.TuesdayHours2.CloseTime.ToString());
                                }
                            }
                            //Tuesday 1 and 2
                            else if (bhour.TuesdayHours1 != null && bhour.TuesdayHours2 != null)
                            {
                                if (bhour.TuesdayHours1.OpenTime != null && bhour.TuesdayHours1.CloseTime != null && bhour.TuesdayHours2.OpenTime != null && bhour.TuesdayHours2.CloseTime != null)
                                {
                                    BusHoursAdd(groupName, "Tuesday", bhour.TuesdayHours1.OpenTime.ToString(), bhour.TuesdayHours1.CloseTime.ToString(), bhour.TuesdayHours2.OpenTime.ToString(), bhour.TuesdayHours2.CloseTime.ToString());
                                }
                            }




                            //Wednesday 1 only
                            if (bhour.WednesdayHours1 != null && bhour.WednesdayHours2 == null)
                            {
                                if (bhour.WednesdayHours1.OpenTime != null && bhour.WednesdayHours1.CloseTime != null)
                                {
                                    BusHoursAdd(groupName, "Wednesday", bhour.WednesdayHours1.OpenTime.ToString(), bhour.WednesdayHours1.CloseTime.ToString(), "", "");
                                }
                            }
                            //Wednesday 2 only
                            else if (bhour.WednesdayHours1 == null && bhour.WednesdayHours2 != null)
                            {
                                if (bhour.WednesdayHours2.OpenTime != null && bhour.WednesdayHours2.CloseTime != null)
                                {
                                    BusHoursAdd(groupName, "Wednesday", "", "", bhour.WednesdayHours2.OpenTime.ToString(), bhour.WednesdayHours2.CloseTime.ToString());
                                }
                            }
                            //Wednesday 1 and 2
                            else if (bhour.WednesdayHours1 != null && bhour.WednesdayHours2 != null)
                            {
                                if (bhour.WednesdayHours1.OpenTime != null && bhour.WednesdayHours1.CloseTime != null && bhour.WednesdayHours2.OpenTime != null && bhour.WednesdayHours2.CloseTime != null)
                                {
                                    BusHoursAdd(groupName, "Wednesday", bhour.WednesdayHours1.OpenTime.ToString(), bhour.WednesdayHours1.CloseTime.ToString(), bhour.WednesdayHours2.OpenTime.ToString(), bhour.WednesdayHours2.CloseTime.ToString());
                                }
                            }




                            //Thursday 1 only
                            if (bhour.ThursdayHours1 != null && bhour.ThursdayHours2 == null)
                            {
                                if (bhour.ThursdayHours1.OpenTime != null && bhour.ThursdayHours1.CloseTime != null)
                                {
                                    BusHoursAdd(groupName, "Thursday", bhour.ThursdayHours1.OpenTime.ToString(), bhour.ThursdayHours1.CloseTime.ToString(), "", "");
                                }
                            }
                            //Thursday 2 only
                            else if (bhour.ThursdayHours1 == null && bhour.ThursdayHours2 != null)
                            {
                                if (bhour.ThursdayHours2.OpenTime != null && bhour.ThursdayHours2.CloseTime != null)
                                {
                                    BusHoursAdd(groupName, "Thursday", "", "", bhour.ThursdayHours2.OpenTime.ToString(), bhour.ThursdayHours2.CloseTime.ToString());
                                }
                            }
                            //Thursday 1 and 2
                            else if (bhour.ThursdayHours1 != null && bhour.ThursdayHours2 != null)
                            {
                                if (bhour.ThursdayHours1.OpenTime != null && bhour.ThursdayHours1.CloseTime != null && bhour.ThursdayHours2.OpenTime != null && bhour.ThursdayHours2.CloseTime != null)
                                {
                                    BusHoursAdd(groupName, "Thursday", bhour.ThursdayHours1.OpenTime.ToString(), bhour.ThursdayHours1.CloseTime.ToString(), bhour.ThursdayHours2.OpenTime.ToString(), bhour.ThursdayHours2.CloseTime.ToString());
                                }
                            }



                            //Friday 1 only
                            if (bhour.FridayHours1 != null && bhour.FridayHours2 == null)
                            {
                                if (bhour.FridayHours1.OpenTime != null && bhour.FridayHours1.CloseTime != null)
                                {
                                    BusHoursAdd(groupName, "Friday", bhour.FridayHours1.OpenTime.ToString(), bhour.FridayHours1.CloseTime.ToString(), "", "");
                                }
                            }
                            //Friday 2 only
                            else if (bhour.FridayHours1 == null && bhour.FridayHours2 != null)
                            {
                                if (bhour.FridayHours2.OpenTime != null && bhour.FridayHours2.CloseTime != null)
                                {
                                    BusHoursAdd(groupName, "Friday", "", "", bhour.FridayHours2.OpenTime.ToString(), bhour.FridayHours2.CloseTime.ToString());
                                }
                            }
                            //Friday 1 and 2
                            else if (bhour.FridayHours1 != null && bhour.FridayHours2 != null)
                            {
                                if (bhour.FridayHours1.OpenTime != null && bhour.FridayHours1.CloseTime != null && bhour.FridayHours2.OpenTime != null && bhour.FridayHours2.CloseTime != null)
                                {
                                    BusHoursAdd(groupName, "Friday", bhour.FridayHours1.OpenTime.ToString(), bhour.FridayHours1.CloseTime.ToString(), bhour.FridayHours2.OpenTime.ToString(), bhour.FridayHours2.CloseTime.ToString());
                                }
                            }



                            //Saturday 1 only
                            if (bhour.SaturdayHours1 != null && bhour.SaturdayHours2 == null)
                            {
                                if (bhour.SaturdayHours1.OpenTime != null && bhour.SaturdayHours1.CloseTime != null)
                                {
                                    BusHoursAdd(groupName, "Saturday", bhour.SaturdayHours1.OpenTime.ToString(), bhour.SaturdayHours1.CloseTime.ToString(), "", "");
                                }
                            }
                            //Saturday 2 only
                            else if (bhour.SaturdayHours1 == null && bhour.SaturdayHours2 != null)
                            {
                                if (bhour.SaturdayHours2.OpenTime != null && bhour.SaturdayHours2.CloseTime != null)
                                {
                                    BusHoursAdd(groupName, "Saturday", "", "", bhour.SaturdayHours2.OpenTime.ToString(), bhour.SaturdayHours2.CloseTime.ToString());
                                }
                            }
                            //Saturday 1 and 2
                            else if (bhour.SaturdayHours1 != null && bhour.SaturdayHours2 != null)
                            {
                                if (bhour.SaturdayHours1.OpenTime != null && bhour.SaturdayHours1.CloseTime != null && bhour.SaturdayHours2.OpenTime != null && bhour.SaturdayHours2.CloseTime != null)
                                {
                                    BusHoursAdd(groupName, "Saturday", bhour.SaturdayHours1.OpenTime.ToString(), bhour.SaturdayHours1.CloseTime.ToString(), bhour.SaturdayHours2.OpenTime.ToString(), bhour.SaturdayHours2.CloseTime.ToString());
                                }
                            }



                            //Sunday 1 only
                            if (bhour.SundayHours1 != null && bhour.SundayHours2 == null)
                            {
                                if (bhour.SundayHours1.OpenTime != null && bhour.SundayHours1.CloseTime != null)
                                {
                                    BusHoursAdd(groupName, "Sunday", bhour.SundayHours1.OpenTime.ToString(), bhour.SundayHours1.CloseTime.ToString(), "", "");
                                }
                            }
                            //Sunday 2 only
                            else if (bhour.SundayHours1 == null && bhour.SundayHours2 != null)
                            {
                                if (bhour.SundayHours2.OpenTime != null && bhour.SundayHours2.CloseTime != null)
                                {
                                    BusHoursAdd(groupName, "Sunday", "", "", bhour.SundayHours2.OpenTime.ToString(), bhour.SundayHours2.CloseTime.ToString());
                                }
                            }
                            //Sunday 1 and 2
                            else if (bhour.SundayHours1 != null && bhour.SundayHours2 != null)
                            {
                                if (bhour.SundayHours1.OpenTime != null && bhour.SundayHours1.CloseTime != null && bhour.SundayHours2.OpenTime != null && bhour.SundayHours2.CloseTime != null)
                                {
                                    BusHoursAdd(groupName, "Sunday", bhour.SundayHours1.OpenTime.ToString(), bhour.SundayHours1.CloseTime.ToString(), bhour.SundayHours2.OpenTime.ToString(), bhour.SundayHours2.CloseTime.ToString());
                                }
                            }
                        }
                    }
                }
        }

        private void metroGridBusHoursGroups_SelectionChanged(object sender, EventArgs e)
        {
            if (metroGridBusHoursGroups.CurrentCell.Value != null)
            {
                var busHoursFilter =
                    busHours.Where(
                        b => b.Parent == metroGridBusHoursGroups.CurrentCell.Value.ToString());
                    //.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());

                busHoursBindingSource = new BindingSource(busHoursFilter, null);
                metroGridBusHoursOpenClose.DataSource = busHoursBindingSource;

                var busHoursGroupFilter =
                    busHoursGroup.Where(
                        b => b.Name == metroGridBusHoursGroups.CurrentCell.Value.ToString()).Single();

                metroTextBoxBusHoursGroupName.Text = metroGridBusHoursGroups.CurrentCell.Value.ToString();
                metroComboBoxBusHoursPool.SelectedText = busHoursGroupFilter.OwnerPool;
            }

            #region OLD
            //busHoursBindingSource.ResetBindings(false);

            //MessageBox.Show(metroGridBusHoursGroups.CurrentCell.Value.ToString());

            //foreach (var item in busHoursFilter)
            //{
            //    MessageBox.Show(metroGridBusHoursGroups.CurrentCell.Value.ToString(), item.Day);
            //}
            //DataGridViewRow row = metroGridBusHoursGroups.SelectedRows[0];
            //row.Cells["ColumnName"].Value;

                 //metroGridBusHoursGroups.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            //busHoursBindingSource.ResetBindings(false);
            //busHoursBindingSource.Filter = "parent = 'AndrewTest'"; //.ResetBindings(false);
#endregion OLD

        }


        private void metroGridBusHoursGroups_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            #region OLD
            //    if (metroGridBusHoursGroups.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
        //    {
        //        metroTextBoxBusHoursGroupName.Text =
        //            metroGridBusHoursGroups.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                
        //        //clear grids
        //        //busHoursGroup.Clear();
        //        busHours.Clear();

        //        //metroGridBusHoursOpenClose.Rows.Clear();
        //       // int i = 0;

        //        //Filter the business hours object in memory to the selected Group
        //    //    var busHoursFilter =
        //    //            CsQ.BusHours.Where(
        //    //                x => x.Members["Name"].Value.ToString() == metroGridBusHoursGroups.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString())
        //    //                .ToList();

        //    //    if (busHoursFilter != null)
        //    //    {
        //    //        foreach (dynamic bhour in busHoursFilter)
        //    //        {
        //    //            if (bhour != null)
        //    //            {
        //    //                //Monday 1 only
        //    //                if (bhour.MondayHours1 != null && bhour.MondayHours2 == null)
        //    //                { 
        //    //                    if (bhour.MondayHours1.OpenTime != null && bhour.MondayHours1.CloseTime != null)
        //    //                    {
        //    //                        BusHoursAdd("Monday", bhour.MondayHours1.OpenTime.ToString(), bhour.MondayHours1.CloseTime.ToString(), "", "");
        //    //                    }
        //    //                }
        //    //                //Monday 2 only
        //    //                else if (bhour.MondayHours1 == null && bhour.MondayHours2 != null)
        //    //                { 
        //    //                    if (bhour.MondayHours2.OpenTime != null && bhour.MondayHours2.CloseTime != null)
        //    //                    {
        //    //                        BusHoursAdd("Monday", "", "", bhour.MondayHours2.OpenTime.ToString(), bhour.MondayHours2.CloseTime.ToString());
        //    //                    }
        //    //                }
        //    //                //Monday 1 and 2
        //    //                else if (bhour.MondayHours1 != null && bhour.MondayHours2 != null)
        //    //                {
        //    //                    if (bhour.MondayHours1.OpenTime != null && bhour.MondayHours1.CloseTime != null && bhour.MondayHours2.OpenTime != null && bhour.MondayHours2.CloseTime != null)
        //    //                    {
        //    //                        BusHoursAdd("Monday", bhour.MondayHours1.OpenTime.ToString(), bhour.MondayHours1.CloseTime.ToString(), bhour.MondayHours2.OpenTime.ToString(), bhour.MondayHours2.CloseTime.ToString());
        //    //                    }
        //    //                }


        //    //                //Tuesday 1 only
        //    //                if (bhour.TuesdayHours1 != null && bhour.TuesdayHours2 == null)
        //    //                {
        //    //                    if (bhour.TuesdayHours1.OpenTime != null && bhour.TuesdayHours1.CloseTime != null)
        //    //                    {
        //    //                        BusHoursAdd("Tuesday", bhour.TuesdayHours1.OpenTime.ToString(), bhour.TuesdayHours1.CloseTime.ToString(), "", "");
        //    //                    }
        //    //                }
        //    //                //Tuesday 2 only
        //    //                else if (bhour.TuesdayHours1 == null && bhour.TuesdayHours2 != null)
        //    //                {
        //    //                    if (bhour.TuesdayHours2.OpenTime != null && bhour.TuesdayHours2.CloseTime != null)
        //    //                    {
        //    //                        BusHoursAdd("Tuesday", "", "", bhour.TuesdayHours2.OpenTime.ToString(), bhour.TuesdayHours2.CloseTime.ToString());
        //    //                    }
        //    //                }
        //    //                //Tuesday 1 and 2
        //    //                else if (bhour.TuesdayHours1 != null && bhour.TuesdayHours2 != null)
        //    //                {
        //    //                    if (bhour.TuesdayHours1.OpenTime != null && bhour.TuesdayHours1.CloseTime != null && bhour.TuesdayHours2.OpenTime != null && bhour.TuesdayHours2.CloseTime != null)
        //    //                    {
        //    //                        BusHoursAdd("Tuesday", bhour.TuesdayHours1.OpenTime.ToString(), bhour.TuesdayHours1.CloseTime.ToString(), bhour.TuesdayHours2.OpenTime.ToString(), bhour.TuesdayHours2.CloseTime.ToString());
        //    //                    }
        //    //                }




        //    //                //Wednesday 1 only
        //    //                if (bhour.WednesdayHours1 != null && bhour.WednesdayHours2 == null)
        //    //                {
        //    //                    if (bhour.WednesdayHours1.OpenTime != null && bhour.WednesdayHours1.CloseTime != null)
        //    //                    {
        //    //                        BusHoursAdd("Wednesday", bhour.WednesdayHours1.OpenTime.ToString(), bhour.WednesdayHours1.CloseTime.ToString(), "", "");
        //    //                    }
        //    //                }
        //    //                //Wednesday 2 only
        //    //                else if (bhour.WednesdayHours1 == null && bhour.WednesdayHours2 != null)
        //    //                {
        //    //                    if (bhour.WednesdayHours2.OpenTime != null && bhour.WednesdayHours2.CloseTime != null)
        //    //                    {
        //    //                        BusHoursAdd("Wednesday", "", "", bhour.WednesdayHours2.OpenTime.ToString(), bhour.WednesdayHours2.CloseTime.ToString());
        //    //                    }
        //    //                }
        //    //                //Wednesday 1 and 2
        //    //                else if (bhour.WednesdayHours1 != null && bhour.WednesdayHours2 != null)
        //    //                {
        //    //                    if (bhour.WednesdayHours1.OpenTime != null && bhour.WednesdayHours1.CloseTime != null && bhour.WednesdayHours2.OpenTime != null && bhour.WednesdayHours2.CloseTime != null)
        //    //                    {
        //    //                        BusHoursAdd("Wednesday", bhour.WednesdayHours1.OpenTime.ToString(), bhour.WednesdayHours1.CloseTime.ToString(), bhour.WednesdayHours2.OpenTime.ToString(), bhour.WednesdayHours2.CloseTime.ToString());
        //    //                    }
        //    //                }




        //    //                //Thursday 1 only
        //    //                if (bhour.ThursdayHours1 != null && bhour.ThursdayHours2 == null)
        //    //                {
        //    //                    if (bhour.ThursdayHours1.OpenTime != null && bhour.ThursdayHours1.CloseTime != null)
        //    //                    {
        //    //                        BusHoursAdd("Thursday", bhour.ThursdayHours1.OpenTime.ToString(), bhour.ThursdayHours1.CloseTime.ToString(), "", "");
        //    //                    }
        //    //                }
        //    //                //Thursday 2 only
        //    //                else if (bhour.ThursdayHours1 == null && bhour.ThursdayHours2 != null)
        //    //                {
        //    //                    if (bhour.ThursdayHours2.OpenTime != null && bhour.ThursdayHours2.CloseTime != null)
        //    //                    {
        //    //                        BusHoursAdd("Thursday", "", "", bhour.ThursdayHours2.OpenTime.ToString(), bhour.ThursdayHours2.CloseTime.ToString());
        //    //                    }
        //    //                }
        //    //                //Thursday 1 and 2
        //    //                else if (bhour.ThursdayHours1 != null && bhour.ThursdayHours2 != null)
        //    //                {
        //    //                    if (bhour.ThursdayHours1.OpenTime != null && bhour.ThursdayHours1.CloseTime != null && bhour.ThursdayHours2.OpenTime != null && bhour.ThursdayHours2.CloseTime != null)
        //    //                    {
        //    //                        BusHoursAdd("Thursday", bhour.ThursdayHours1.OpenTime.ToString(), bhour.ThursdayHours1.CloseTime.ToString(), bhour.ThursdayHours2.OpenTime.ToString(), bhour.ThursdayHours2.CloseTime.ToString());
        //    //                    }
        //    //                }



        //    //                //Friday 1 only
        //    //                if (bhour.FridayHours1 != null && bhour.FridayHours2 == null)
        //    //                {
        //    //                    if (bhour.FridayHours1.OpenTime != null && bhour.FridayHours1.CloseTime != null)
        //    //                    {
        //    //                        BusHoursAdd("Friday", bhour.FridayHours1.OpenTime.ToString(), bhour.FridayHours1.CloseTime.ToString(), "", "");
        //    //                    }
        //    //                }
        //    //                //Friday 2 only
        //    //                else if (bhour.FridayHours1 == null && bhour.FridayHours2 != null)
        //    //                {
        //    //                    if (bhour.FridayHours2.OpenTime != null && bhour.FridayHours2.CloseTime != null)
        //    //                    {
        //    //                        BusHoursAdd("Friday", "", "", bhour.FridayHours2.OpenTime.ToString(), bhour.FridayHours2.CloseTime.ToString());
        //    //                    }
        //    //                }
        //    //                //Friday 1 and 2
        //    //                else if (bhour.FridayHours1 != null && bhour.FridayHours2 != null)
        //    //                {
        //    //                    if (bhour.FridayHours1.OpenTime != null && bhour.FridayHours1.CloseTime != null && bhour.FridayHours2.OpenTime != null && bhour.FridayHours2.CloseTime != null)
        //    //                    {
        //    //                        BusHoursAdd("Friday", bhour.FridayHours1.OpenTime.ToString(), bhour.FridayHours1.CloseTime.ToString(), bhour.FridayHours2.OpenTime.ToString(), bhour.FridayHours2.CloseTime.ToString());
        //    //                    }
        //    //                }



        //    //                //Saturday 1 only
        //    //                if (bhour.SaturdayHours1 != null && bhour.SaturdayHours2 == null)
        //    //                {
        //    //                    if (bhour.SaturdayHours1.OpenTime != null && bhour.SaturdayHours1.CloseTime != null)
        //    //                    {
        //    //                        BusHoursAdd("Saturday", bhour.SaturdayHours1.OpenTime.ToString(), bhour.SaturdayHours1.CloseTime.ToString(), "", "");
        //    //                    }
        //    //                }
        //    //                //Saturday 2 only
        //    //                else if (bhour.SaturdayHours1 == null && bhour.SaturdayHours2 != null)
        //    //                {
        //    //                    if (bhour.SaturdayHours2.OpenTime != null && bhour.SaturdayHours2.CloseTime != null)
        //    //                    {
        //    //                        BusHoursAdd("Saturday", "", "", bhour.SaturdayHours2.OpenTime.ToString(), bhour.SaturdayHours2.CloseTime.ToString());
        //    //                    }
        //    //                }
        //    //                //Saturday 1 and 2
        //    //                else if (bhour.SaturdayHours1 != null && bhour.SaturdayHours2 != null)
        //    //                {
        //    //                    if (bhour.SaturdayHours1.OpenTime != null && bhour.SaturdayHours1.CloseTime != null && bhour.SaturdayHours2.OpenTime != null && bhour.SaturdayHours2.CloseTime != null)
        //    //                    {
        //    //                        BusHoursAdd("Saturday", bhour.SaturdayHours1.OpenTime.ToString(), bhour.SaturdayHours1.CloseTime.ToString(), bhour.SaturdayHours2.OpenTime.ToString(), bhour.SaturdayHours2.CloseTime.ToString());
        //    //                    }
        //    //                }



        //    //                //Sunday 1 only
        //    //                if (bhour.SundayHours1 != null && bhour.SundayHours2 == null)
        //    //                {
        //    //                    if (bhour.SundayHours1.OpenTime != null && bhour.SundayHours1.CloseTime != null)
        //    //                    {
        //    //                        BusHoursAdd("Sunday", bhour.SundayHours1.OpenTime.ToString(), bhour.SundayHours1.CloseTime.ToString(), "", "");
        //    //                    }
        //    //                }
        //    //                //Sunday 2 only
        //    //                else if (bhour.SundayHours1 == null && bhour.SundayHours2 != null)
        //    //                {
        //    //                    if (bhour.SundayHours2.OpenTime != null && bhour.SundayHours2.CloseTime != null)
        //    //                    {
        //    //                        BusHoursAdd("Sunday", "", "", bhour.SundayHours2.OpenTime.ToString(), bhour.SundayHours2.CloseTime.ToString());
        //    //                    }
        //    //                }
        //    //                //Sunday 1 and 2
        //    //                else if (bhour.SundayHours1 != null && bhour.SundayHours2 != null)
        //    //                {
        //    //                    if (bhour.SundayHours1.OpenTime != null && bhour.SundayHours1.CloseTime != null && bhour.SundayHours2.OpenTime != null && bhour.SundayHours2.CloseTime != null)
        //    //                    {
        //    //                        BusHoursAdd("Sunday", bhour.SundayHours1.OpenTime.ToString(), bhour.SundayHours1.CloseTime.ToString(), bhour.SundayHours2.OpenTime.ToString(), bhour.SundayHours2.CloseTime.ToString());
        //    //                    }
        //    //                }
        //    //            }
        //    //        }
        //    //    }

        //    }
#endregion OLD
        }

        private void BusHoursAdd(string parent, string day, string openTime1, string closeTime1, string openTime2, string closeTime2)
        {
            var match = busHours.Where(p => (p.OpenTime1 == openTime1 && p.Day == day && p.Parent == parent)
                || (p.CloseTime1 == closeTime1 && p.Day == day && p.Parent == parent)
                || (p.OpenTime2 == openTime2 && p.Day == day && p.Parent == parent)
                || (p.CloseTime2 == closeTime2 && p.Day == day && p.Parent == parent));
            
            var matchCount = match.Count();

            if (matchCount == 0)
            {
                busHours.Add(new CsBusHours(parent, day, openTime1, closeTime1, openTime2, closeTime2));
                busHoursBindingSource.ResetBindings(false);
            }
            else if (matchCount > 1)
            {
                MessageBox.Show("Error determining if item already exisits - more than 1 result returned");
            }
            else if (matchCount == 1)
            {
                MessageBox.Show("Already exists");
            }
        }

        private void BusHoursGroupAdd(string name, string ownerPool, string identity)
        {
            var match = busHoursGroup.Where(p => (p.Name == name));
            var matchCount = match.Count();

            if (matchCount == 0)
            {
                busHoursGroup.Add(new CsBusHoursGroup(name, ownerPool, identity));

                //busHoursGroupBindingSource.ResetBindings(false);
            }
            else if (matchCount > 1)
            {
                MessageBox.Show("Error determining if item already exisits - more than 1 result returned");
            }
            else if (matchCount == 1)
            {
                MessageBox.Show("Already exists");
            }
        }

        
        private void metroButtonBusHoursLoad_Click(object sender, System.EventArgs e)
        {
            LoadBusHours();
        }

        private void metroButtonBusHoursGroupsRemove_Click(object sender, EventArgs e)
        {
            BsControl.RemoveSelected(busHoursGroupBindingSource);
        }

        private void metroButtonBusHoursOpenCloseRemove_Click(object sender, EventArgs e)
        {
            BsControl.RemoveSelected(busHoursBindingSource);
        }

        private void metroButtonBusHoursGroupAdd_Click(object sender, EventArgs e)
        {
            if (metroTextBoxBusHoursGroupName.Text != null)
            {
               //PLACE HOLDER to display update, need to actually execute PS, then upate from PS
                BusHoursGroupAdd(metroTextBoxBusHoursGroupName.Text, "TEMP", "TEMP");
                busHoursGroupBindingSource.ResetBindings(false);
            }
        }

        private void metroButtonBusHoursAdd_Click_1(object sender, EventArgs e)
        {
            if (metroToggleBusHours1.Checked && !metroToggleBusHours2.Checked)
            {
                if (metroComboBoxBusHoursDay.SelectedValue != null)
                {
                    var days = BusHoursDays(metroComboBoxBusHoursDay.SelectedValue.ToString());

                    foreach (var day in days)
                    {
                        //MessageBox.Show(day);
                        BusHoursAdd(metroTextBoxBusHoursGroupName.Text, day, metroComboBoxBusHoursOpenHour1.Text + ":" + metroComboBoxBusHoursOpenMin1.Text + ":00", metroComboBoxBusHoursCloseHour1.Text + ":" + metroComboBoxBusHoursCloseMin1.Text + ":00", "", "");
                        
                    }
                    busHoursGroupBindingSource.ResetBindings(false);
                    //busHoursBindingSource.ResetBindings(false);
                    //busHoursBindingSource = new BindingSource(busHoursFilter, null);
                    //metroGridBusHoursOpenClose.DataSource = busHoursBindingSource;
                 }
            }
            else if (metroToggleBusHours2.Checked && !metroToggleBusHours1.Checked)
            {
                if (metroComboBoxBusHoursDay.SelectedValue != null)
                {
                    var days = BusHoursDays(metroComboBoxBusHoursDay.SelectedValue.ToString());

                    foreach (var day in days)
                    {
                        //MessageBox.Show(day);
                        BusHoursAdd(metroTextBoxBusHoursGroupName.Text, day, "", "", metroComboBoxBusHoursOpenHour2.Text + ":" + metroComboBoxBusHoursOpenMin2.Text + ":00", metroComboBoxBusHoursCloseHour2.Text + ":" + metroComboBoxBusHoursCloseMin2.Text + ":00");

                    }
                    busHoursGroupBindingSource.ResetBindings(false);
                }
            }
            else if (metroToggleBusHours1.Checked && metroToggleBusHours2.Checked)
            {
                if (metroComboBoxBusHoursDay.SelectedValue != null)
                {
                    var days = BusHoursDays(metroComboBoxBusHoursDay.SelectedValue.ToString());

                    foreach (var day in days)
                    {
                        //MessageBox.Show(day);
                        BusHoursAdd(metroTextBoxBusHoursGroupName.Text, day,
                            metroComboBoxBusHoursOpenHour1.Text + ":" + metroComboBoxBusHoursOpenMin1.Text + ":00",
                            metroComboBoxBusHoursCloseHour1.Text + ":" + metroComboBoxBusHoursCloseMin1.Text + ":00",
                            metroComboBoxBusHoursOpenHour2.Text + ":" + metroComboBoxBusHoursOpenMin2.Text + ":00",
                            metroComboBoxBusHoursCloseHour2.Text + ":" + metroComboBoxBusHoursCloseMin2.Text + ":00");

                    }
                    busHoursGroupBindingSource.ResetBindings(false);
                }
            }
        }

        static List<string> BusHoursDays(string days)
        {
            List<string> arrayOfDays = new List<string>();
 
            switch (days)
            {
                case "MonFri":
                    arrayOfDays.Add("Monday");
                    arrayOfDays.Add("Tuesday");
                    arrayOfDays.Add("Wednesday");
                    arrayOfDays.Add("Thursday");
                    arrayOfDays.Add("Frinday");
                    return arrayOfDays;
                case "MonSat":
                    arrayOfDays.Add("Monday");
                    arrayOfDays.Add("Tuesday");
                    arrayOfDays.Add("Wednesday");
                    arrayOfDays.Add("Thursday");
                    arrayOfDays.Add("Friday");
                    arrayOfDays.Add("Saturday");
                    return arrayOfDays;
                case "MonSun":
                    arrayOfDays.Add("Monday");
                    arrayOfDays.Add("Tuesday");
                    arrayOfDays.Add("Wednesday");
                    arrayOfDays.Add("Thursday");
                    arrayOfDays.Add("Friday");
                    arrayOfDays.Add("Saturday");
                    arrayOfDays.Add("Sunday");
                    return arrayOfDays;
                case "Monday":
                    arrayOfDays.Add("Monday");
                    return arrayOfDays;
                case "Tuesday":
                    arrayOfDays.Add("Tuesday");
                    return arrayOfDays;
                case "Wednesday":
                    arrayOfDays.Add("Wednesday");
                    return arrayOfDays;
                case "Thursday":
                    arrayOfDays.Add("Thursday");
                    return arrayOfDays;
                case "Friday":
                    arrayOfDays.Add("Friday");
                    return arrayOfDays;
                case "Saturday":
                    arrayOfDays.Add("Saturday");
                    return arrayOfDays;
                case "Sunday":
                    arrayOfDays.Add("Sunday");
                    return arrayOfDays;
                default:
                    return null;
            }
        }

        private void metroButtonBusHoursBulkAdd_Click(object sender, EventArgs e)
        {

        }


        #endregion TAB: BUSINESS HOURS

        #region TAB: HOLIDAYS
        //BACKGROUND WORKER - start the worker
        private void LoadHolidays()
        {
            //Start background worker
            if (!holWorker.IsBusy)
            {
                holWorker.RunWorkerAsync();
                //Disable the button that starts the process so multi threads cant be started
                metroButtonHolLoad.Enabled = false;
                //Start progress spinner
                metroProgressSpinnerHolLoading.Visible = true;
                metroProgressSpinnerHolLoading.Spinning = true;
                metroLabelHolLoading.Text = "Loading Holidays...";
            }
        }

        //BACKGROUND WORKER - Do Work
        private void HolDoWork(object sender, DoWorkEventArgs e)
        {
            //Update shared
            LoadShared();

            //BackgroundWorker bgWorker = (BackgroundWorker)sender;
            CsQ.GetCsHolidays();
        }

        //BACKGROUND WORKER - Update Progress
        private void worker_holProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //metroTextBoxGrpDisplayName.Text = e.ProgressPercentage.ToString();
        }

        //BACKGROUND WORKER - Completed
        void worker_holRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Check if there was an error during DoWork processing
            if (e.Error != null)
                throw new ApplicationException("An error occured while executing the Holiday background worker.", e.Error);
            
            //string regexBusHoursGroupNames = @"_\w{8}-\w{4}-\w{4}-\w{4}-\w{12}";
            //int i = 0;

            if (CsQ.Holidays != null)
            {
                foreach (dynamic hol in CsQ.Holidays)
                {
                    if (hol != null)
                    {
                        //Filter out Business Hours that contain GUID - these are system created for customised business hours on a queue by queue basis
                        //Regex regex = new Regex(regexBusHoursGroupNames);
                        //Match match = regex.Match(bhour.Name);

                        //Those that dont contain a GUID are deemed as user created
                        //if (!match.Success)
                        //{
                        HolGroupAdd(hol.Name, hol.OwnerPool.ToString(), hol.Identity.ToString());
                        HolAddFromSfB(hol.Name);
                        //MessageBox.Show(bhoursName.Name);
                        //metroGridBusHoursGroups.Rows.Add();
                        //metroGridBusHoursGroups.Rows[i].Cells["dataGridViewBusHoursGroups_Name"].Value = bhour.Name;
                        //i++;
                        //}

                    }
                }

                //Update Holidays grid
                holGroupBindingSource.ResetBindings(false);

                //Populate pools drop down
                //NOW DONE IN WORKFLOWS AFTER QUERY TO PS COMPLETED
                //if (CsQ.RegistrarPools != null)
                //{
                //    metroComboBoxBusHoursPool.Items.Clear();
                //    foreach (dynamic registrarPool in CsQ.RegistrarPools)
                //    {
                //        metroComboBoxBusHoursPool.Items.Add(registrarPool.Identity);
                //    }
                //}

                holidaysLoaded = true;

                //re-enable button
                metroButtonHolLoad.Enabled = true;
                metroProgressSpinnerHolLoading.Visible = false;
                metroProgressSpinnerHolLoading.Spinning = false;
                metroLabelHolLoading.Text = "";
            }
            else
            {
                metroLabelHolLoading.Text = "Error loading holidays, please try again.";
            }
        }

        private void HolAddFromSfB(string groupName)
        {
            var holFilter =
                        CsQ.Holidays.Where(
                            x => x.Members["Name"].Value.ToString() == groupName)
                            .ToList();

            if (holFilter != null)
            {
                foreach (dynamic hol in holFilter)
                {
                    if (hol != null)
                    {
                        //TO BE COMPLETED
                        if (hol.HolidayList != null)
                        {
                            foreach (var holList in hol.HolidayList)
                            {
                                if (holList.Name != null && holList.StartDate != null &&
                                    holList.EndDate != null)
                                {
                                    HolAdd(groupName, holList.Name.ToString(), holList.StartDate.ToString(),
                                        holList.EndDate.ToString());
                                }
                            }
                        }
                    }
                }
            }
        }


        private void HolAdd(string parent, string name, string startDate, string endDate)
        {
            var match = holidays.Where(p => p.Parent == parent && p.Name == name);

            var matchCount = match.Count();

            if (matchCount == 0)
            {
                holidays.Add(new CsHoliday(parent, name, startDate, endDate));
                holBindingSource.ResetBindings(false);
            }
            else if (matchCount > 1)
            {
                MessageBox.Show("Error determining if item already exisits - more than 1 result returned");
            }
            else if (matchCount == 1)
            {
                //ISSUE WITH LOGIC HERE - MESSAGE BOX KEEPS POPPING!
                //MessageBox.Show("Already exists");
            }
        }

        private void HolGroupAdd(string name, string ownerPool, string identity)
        {
            var match = holGroup.Where(p => (p.Name == name));
            var matchCount = match.Count();

            if (matchCount == 0)
            {
                holGroup.Add(new CsHolidayGroup(name, ownerPool, identity));
            }
            else if (matchCount > 1)
            {
                MessageBox.Show("Error determining if item already exisits - more than 1 result returned");
            }
            else if (matchCount == 1)
            {
                MessageBox.Show("Already exists");
            }
        }

        private void metroGridHolGroups_SelectionChanged(object sender, EventArgs e)
        {
            if (metroGridHolGroups.CurrentCell != null)
            {
                var holFilter =
                    holidays.Where(
                        b => b.Parent == metroGridHolGroups.CurrentCell.Value.ToString());
                //.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());

                //holBindingSource.ResetBindings(true);
                holBindingSource = new BindingSource(holFilter, null);
                metroGridHolList.DataSource = holBindingSource;

                var holGroupFilter =
                    holGroup.Where(
                        b => b.Name == metroGridHolGroups.CurrentCell.Value.ToString()).Single();

                metroTextBoxHolGroupName.Text = metroGridHolGroups.CurrentCell.Value.ToString();
                metroComboBoxHolPool.SelectedText = holGroupFilter.OwnerPool;
            }
        }

        /// <summary>
        /// HolidaysList selection changed - updates the holiday name text box
        /// NOTE: Fires multiple times, need to look at this later
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void metroGridHolList_SelectionChanged(object sender, EventArgs e)
        {
            if (metroGridHolList.CurrentRow != null)
            {
                if (metroGridHolList.CurrentRow.Cells[0].Value != null)
                {
                    //MessageBox.Show(metroGridHolList.CurrentRow.Cells[0].Value.ToString());
                    metroTextBoxHolListName.Text = metroGridHolList.CurrentRow.Cells[0].Value.ToString();
                }
            }

            //if (metroGridHolList.CurrentCell.Value != null)
            //{
            //    MessageBox.Show(metroGridHolList.CurrentCell.Value.ToString());
            //}
        }
      
        private void metroButtonHolLoad_Click(object sender, EventArgs e)
        {
            LoadHolidays();
        }

        private void metroButtonHolGroupAdd_Click_1(object sender, EventArgs e)
        {
            if (metroTextBoxHolGroupName.Text != null)
            {
                //PLACE HOLDER to display update, need to actually execute PS, then upate from PS
                HolGroupAdd(metroTextBoxHolGroupName.Text, "TEMP", "TEMP");
                holGroupBindingSource.ResetBindings(false);
            }
        }

        private void metroButtonHolGroupRemove_Click(object sender, EventArgs e)
        {
            BsControl.RemoveSelected(holGroupBindingSource);
        }

        private void metroButtonHolGroupBulkAdd_Click(object sender, EventArgs e)
        {
            //TO BE COMPLETED
        }

        private void metroButtonHolListAdd_Click(object sender, EventArgs e)
        {
            if (metroTextBoxHolListName.Text != null)
            {
                HolAdd(metroTextBoxHolGroupName.Text, metroTextBoxHolListName.Text,
                    metroDateTimeHolListStartDate.Text + " " + metroComboBoxHolListStartHour.Text + ":" +
                    metroComboBoxHolListStartMinute.Text + ":00",
                    metroDateTimeHolListEndDate.Text + " " + metroComboBoxHolListEndHour.Text + ":" +
                    metroComboBoxHolListEndMinute.Text + ":00");

                //MessageBox.Show(metroDateTimeHolListStartDate.Text + " " + metroComboBoxHolListStartHour.Text + ":" + metroComboBoxHolListStartMinute.Text + ":00");
                //MessageBox.Show(metroDateTimeHolListEndDate.Text + " " + metroComboBoxHolListEndHour.Text + ":" + metroComboBoxHolListEndMinute.Text + ":00");
                holGroupBindingSource.ResetBindings(true);
            }
        }

        private void metroButtonHolListRemove_Click(object sender, EventArgs e)
        {
            BsControl.RemoveSelected(holBindingSource);
        }


        #endregion TAB: HOLIDAYS

        
        #region TAB: SHARED
        //BACKGROUND WORKER - start the worker
        private void LoadShared()
        {
            //Start background worker
            if (!sharedWorker.IsBusy)
            {
                sharedWorker.RunWorkerAsync();

                ////Disable the button that starts the process so multi threads cant be started
                //metroButtonWfLoad.Enabled = false;
                //metroButtonQLoad.Enabled = false;
                //metroButtonGrpLoad.Enabled = false;
                //metroButtonBusHoursLoad.Enabled = false;
                //metroButtonHolLoad.Enabled = false;
                
                ////Start progress spinner
                //metroProgressSpinnerWfLoading.Visible = true;
                //metroProgressSpinnerQLoading.Visible = true;
                //metroProgressSpinnerGrpLoading.Visible = true;
                //metroProgressSpinnerBusHoursLoading.Visible = true;
                //metroProgressSpinnerHolLoading.Visible = true;

                //metroProgressSpinnerWfLoading.Spinning = true;
                //metroProgressSpinnerQLoading.Spinning = true;
                //metroProgressSpinnerGrpLoading.Spinning = true;
                //metroProgressSpinnerBusHoursLoading.Spinning = true;
                //metroProgressSpinnerHolLoading.Spinning = true;

                //metroLabelWfLoading.Text = "Loading shared resources...";
                //metroLabelQLoading.Text = "Loading shared resources...";
                //metroLabelGrpLoading.Text = "Loading shared resources...";
                //metroLabelBusHoursLoading.Text = "Loading shared resources...";
                //metroLabelHolLoading.Text = "Loading shared resources...";
                //metroLabelHolLoading.Text = "Loading shared resources...";
            }
        }

        //BACKGROUND WORKER - Do Work
        private void SharedDoWork(object sender, DoWorkEventArgs e)
        {
            CsQ.GetSipDomains();
            CsQ.UpdateSipDomainsList();
            CsQ.GetRegistrarPools();
        }

        //BACKGROUND WORKER - Update Progress
        private void worker_sharedProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            
        }

        //BACKGROUND WORKER - Completed
        void worker_sharedRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Check if there was an error during DoWork processing
            if (e.Error != null)
                throw new ApplicationException("An error occured while executing the Shared background worker.", e.Error);

            if (CsQ.SipDomainsList != null)
            {
                //WORKFLOWS
                //Bind list to Sip Domain comboboxes as required
                metroComboBoxWfSipDomain.DataSource = new BindingSource(CsQ.SipDomainsList, null);
                metroComboBoxWfHolSipDomain.DataSource = new BindingSource(CsQ.SipDomainsList, null);
                metroComboBoxWfAhSipDomain.DataSource = new BindingSource(CsQ.SipDomainsList, null);
                metroComboBoxQTimeoutSipDomain.DataSource = new BindingSource(CsQ.SipDomainsList, null);
                metroComboBoxQOverflowSipDomain.DataSource = new BindingSource(CsQ.SipDomainsList, null);

                //QUEUES
                //Bind list to Sip Domain comboboxes as required
                metroComboBoxQTimeoutSipDomain.DataSource = new BindingSource(CsQ.SipDomainsList, null);
                metroComboBoxQOverflowSipDomain.DataSource = new BindingSource(CsQ.SipDomainsList, null);

                //GROUPS

                
                //HOLIDAYS

                
            }
            else
            {
                metroLabelHolLoading.Text = "Error loading shared resources (SipDomainsList), please try again.";
            }

            if (CsQ.RegistrarPoolsList != null)
            {
                //BUSINESS HOURS
                //Pools
                metroComboBoxBusHoursPool.DataSource = new BindingSource(CsQ.RegistrarPoolsList, null);
                //Holidays
                //Pools
                metroComboBoxHolPool.DataSource = new BindingSource(CsQ.RegistrarPoolsList, null);
            }


            if (CsQ.SipDomains != null && CsQ.SipDomains != null && CsQ.RegistrarPools != null && CsQ.RegistrarPoolsList != null)
                sharedLoaded = true;

            //NOT SURE WHAT TO DO HERE AS IT CONFLICTS WITH OTHER WORKERS
            ////re-enable button
            //metroButtonWfLoad.Enabled = true;
            //metroButtonQLoad.Enabled = true;
            //metroButtonGrpLoad.Enabled = true;
            //metroButtonBusHoursLoad.Enabled = true;
            //metroButtonHolLoad.Enabled = true;

            ////Start progress spinner
            //metroProgressSpinnerWfLoading.Visible = false;
            //metroProgressSpinnerQLoading.Visible = false;
            //metroProgressSpinnerGrpLoading.Visible = false;
            //metroProgressSpinnerBusHoursLoading.Visible = false;
            //metroProgressSpinnerHolLoading.Visible = false;

            //metroProgressSpinnerWfLoading.Spinning = false;
            //metroProgressSpinnerQLoading.Spinning = false;
            //metroProgressSpinnerGrpLoading.Spinning = false;
            //metroProgressSpinnerBusHoursLoading.Spinning = false;
            //metroProgressSpinnerHolLoading.Spinning = false;

            //metroLabelWfLoading.Text = "Loading shared resources...";
            //metroLabelQLoading.Text = "Loading shared resources...";
            //metroLabelGrpLoading.Text = "Loading shared resources...";
            //metroLabelBusHoursLoading.Text = "Loading shared resources...";
            //metroLabelHolLoading.Text = "Loading shared resources...";
            //metroLabelHolLoading.Text = "Loading shared resources...";
        }

        #endregion TAB: SHARED

        
        #region IVR OPTIONS CONTROLS

        private void IvrReset()
        {
            //reset counters
            countIvrRoot = -1;
            countIvrSubRoot0 = -1;
            countIvrSubRoot1 = -1;
            countIvrSubRoot2 = -1;
            countIvrSubRoot3 = -1;
            countIvrSubRoot4 = -1;
            countIvrSubRoot5 = -1;
            countIvrSubRoot6 = -1;
            countIvrSubRoot7 = -1;
            countIvrSubRoot8 = -1;
            countIvrSubRoot9 = -1;

            ivrMenus.Clear();
        }

       
        /// <summary>
        /// Dynamically adds IVR options at level 1
        /// http://www.aspsnippets.com/Articles/Add-Dynamic-TextBox-Label-and-Button-controls-with-TextChanged-and-Click-event-handlers-in-Windows-Forms-WinForms-Application.aspx
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void metroButtonWfIvrAddOption_Click(object sender, EventArgs e)
        {
            IvrAddRoot(0);
        }


        private IvrMenu IvrAddRoot(int optionLevel)
        {
            if (countIvrRoot == maxOptions - 1)
            {
                MessageBox.Show("Maximum of 10 rows can be added");
                return null;
            }
            else
            {
                countIvrRoot++;

                //Create IVR Menu
                var root = new IvrMenu();
                //ivrMenus[countIvrRoot] = new IvrMenu();

                //Setup IVR menu
                //root.Name = countIvrRoot.ToString();
                root.OptionLevel = optionLevel;
                //root.OptionNumberParent = countIvr1;
                root.OptionPosition = countIvrRoot;
                root.SubAdd1 += buttonIvrAddSubOption1_Click;
                root.RootRemove += buttonIvrRemoveRootOption1_Click;
                
                //Create menu option
                root.CreateOption();

                //Configure menu option
                //Queue selector
                //NEED TO ENSURE QUEUES ARE LOADED!
                root.comboboxIvrQueue1.DataSource = new BindingSource(CsQ.CsQueuesList, null);
                root.comboboxIvrQueue1.DisplayMember = "Value";
                root.comboboxIvrQueue1.ValueMember = "Key";
                
                ivrMenus.Add(root);

                UpdateIvrControls();
                return root;
            }

            #region OLD

                //testButton[countIvr1].Parent = null;

                //labelIvr1[countIvr1] = new MetroLabel();
                //labelIvr1[countIvr1].Name = "labelIvr_" + countIvr1;
                //labelIvr1[countIvr1].Text = "Option " + countIvr1;
                //labelIvr1[countIvr1].Location = new Point(5, (25 * (countIvr1 + countIvr2)) + 2);

                //MessageBox.Show(testButton[countIvr1].labelIvr1.Text);
                //UpdateIvrControls(testButton[countIvr1].labelIvr1);

                //metroPanelWfIvrOptions.Controls.Add(labelIvr1[countIvr1]);
                ////comboboxIvrDtmf1[countIvr1] = new MetroComboBox();
                ////comboboxIvrDtmf1[countIvr1].Name = "comboboxIvrDtmf_" + countIvr1;
                ////comboboxIvrDtmf1[countIvr1].Size = new Size(25, 25);
                ////comboboxIvrDtmf1[countIvr1].Location = new Point(120, 25 * (countIvr1 + countIvr2));

                ////textboxIvrVoiceResponse1[countIvr1] = new MetroTextBox();
                ////textboxIvrVoiceResponse1[countIvr1].Name = "textboxIvrVoiceResponse_" + countIvr1;
                ////textboxIvrVoiceResponse1[countIvr1].Size = new Size(300, 20);
                ////textboxIvrVoiceResponse1[countIvr1].Location = new Point(200, 25 * (countIvr1 + countIvr2));

                ////comboboxIvrQueue1[countIvr1] = new MetroComboBox();
                ////comboboxIvrQueue1[countIvr1].Name = "comboboxIvrQueue_" + countIvr1;
                ////comboboxIvrQueue1[countIvr1].Location = new Point(550, 25 * (countIvr1 + countIvr2));

                ////buttonIvrAddSubOption1[countIvr1] = new MetroButton();
                ////buttonIvrAddSubOption1[countIvr1].Name = "buttonIvrAddSubOption_" + countIvr1;
                ////buttonIvrAddSubOption1[countIvr1].Text = "+";
                ////buttonIvrAddSubOption1[countIvr1].Size = new Size(25, 25);
                ////buttonIvrAddSubOption1[countIvr1].Location = new Point(800, 25 * (countIvr1 + countIvr2));
                //////buttonIvrAddSubOption1[countIvr1].Click += buttonIvrAddSubOption1_Click;
                ////buttonIvrAddSubOption1[countIvr1].Click += (sender1, e1) => buttonIvrAddSubOption1_Click(buttonIvrAddSubOption1[countIvr1].Name, sender, e);

                ////buttonIvrRemoveSubOption1[countIvr1] = new MetroButton();
                ////buttonIvrRemoveSubOption1[countIvr1].Name = "buttonIvrRemoveSubOption_" + countIvr1;
                ////buttonIvrRemoveSubOption1[countIvr1].Text = "-";
                ////buttonIvrRemoveSubOption1[countIvr1].Size = new Size(25, 25);
                ////buttonIvrRemoveSubOption1[countIvr1].Location = new Point(830, 25 * (countIvr1 + countIvr2));



                //var filter0 = labelIvr1.Where(x => x != null && x.Name.Contains("labelIvr_0"));
                //foreach (MetroLabel item in filter0)
                //{
                //metroPanelWfIvrOptions.Controls.Add(item);
                //metroPanelWfIvrOptions.Controls.Add(labelIvr1[countIvr1]);
                //metroPanelWfIvrOptions.Controls.Add(comboboxIvrDtmf1[countIvr1]);
                //metroPanelWfIvrOptions.Controls.Add(textboxIvrVoiceResponse1[countIvr1]);
                //metroPanelWfIvrOptions.Controls.Add(comboboxIvrQueue1[countIvr1]);
                //metroPanelWfIvrOptions.Controls.Add(buttonIvrAddSubOption1[countIvr1]);
                //metroPanelWfIvrOptions.Controls.Add(buttonIvrRemoveSubOption1[countIvr1]);
                //}

                //var filter1 = labelIvr1.Where(x => x != null && x.Name.Contains("labelIvr_1"));
                //foreach (MetroLabel item in filter1)
                //{
                //    metroPanelWfIvrOptions.Controls.Add(item);
                //}

                #endregion OLD
        }

        public void buttonIvrRemoveRootOption1_Click(int optionNumberLevel, int optionNumber)
        {
            DialogResult dialogResult = MessageBox.Show("Removing the root option will also remove all child options. Do you want to continue?", "Warning", MessageBoxButtons.YesNo);
            if(dialogResult == DialogResult.Yes)
            {
                //Remove root menu option
                ivrMenus.RemoveAll(x => x.OptionLevel == optionNumberLevel && x.OptionPosition == optionNumber);
                countIvrRoot--;

                //Remove all children
                ivrMenus.RemoveAll(x => x.OptionParent == optionNumber);
                countIvrSubRoot0--;
                
                UpdateIvrControls();
            }
            
        }

        /// <summary>
        /// Handles add sub option click event for dynamically generated IVR controls
        /// </summary>
        /// <param name="optionNumberLevel"></param>
        /// <param name="optionNumber"></param>
        /// <param name="optionNumberParent"></param>
        public void buttonIvrAddSubOption1_Click(int optionNumberLevel, int optionNumber, int optionNumberParent)
        {
            IvrAddSub1(optionNumber);
        }


        private IvrMenu IvrAddSub1(int optionNumberParent)
        {
            metroLabelTestOutput.Text = "SUB CLICK - optionNumberLevel:" + 1 + " optionNumber:" + optionNumberParent +
                                        " optionNumberParent" + optionNumberParent + " countIvr1:" + countIvrRoot +
                                        " countIvr2:" + countIvrSubRoot0;
            int sub1Counter = -1;
            if (optionNumberParent == 0) { sub1Counter = countIvrSubRoot0; }
            else if (optionNumberParent == 1) { sub1Counter = countIvrSubRoot1; }
            else if (optionNumberParent == 2) { sub1Counter = countIvrSubRoot2; }
            else if (optionNumberParent == 3) { sub1Counter = countIvrSubRoot3; }
            else if (optionNumberParent == 4) { sub1Counter = countIvrSubRoot4; }
            else if (optionNumberParent == 5) { sub1Counter = countIvrSubRoot5; }
            else if (optionNumberParent == 6) { sub1Counter = countIvrSubRoot6; }
            else if (optionNumberParent == 7) { sub1Counter = countIvrSubRoot7; }
            else if (optionNumberParent == 8) { sub1Counter = countIvrSubRoot8; }
            else if (optionNumberParent == 9) { sub1Counter = countIvrSubRoot9; }

            if (sub1Counter == maxOptions - 1)
            {
                MessageBox.Show("Maximum of 10 rows can be added");
                return null;
            }
            else
            {
                if (optionNumberParent == 0) { countIvrSubRoot0++; sub1Counter = countIvrSubRoot0; }
                else if (optionNumberParent == 1) { countIvrSubRoot1++; sub1Counter = countIvrSubRoot1; }
                else if (optionNumberParent == 2) { countIvrSubRoot2++; sub1Counter = countIvrSubRoot2; }
                else if (optionNumberParent == 3) { countIvrSubRoot3++; sub1Counter = countIvrSubRoot3; }
                else if (optionNumberParent == 4) { countIvrSubRoot4++; sub1Counter = countIvrSubRoot4; }
                else if (optionNumberParent == 5) { countIvrSubRoot5++; sub1Counter = countIvrSubRoot5; }
                else if (optionNumberParent == 6) { countIvrSubRoot6++; sub1Counter = countIvrSubRoot6; }
                else if (optionNumberParent == 7) { countIvrSubRoot7++; sub1Counter = countIvrSubRoot7; }
                else if (optionNumberParent == 8) { countIvrSubRoot7++; sub1Counter = countIvrSubRoot8; }
                else if (optionNumberParent == 9) { countIvrSubRoot9++; sub1Counter = countIvrSubRoot9; }
                
                var sub = new IvrMenu();
                //sub.Name = countIvrRoot.ToString();
                sub.OptionLevel = 1;
                sub.OptionPosition = sub1Counter;
                sub.OptionParent = optionNumberParent;
                //sub.SubAdd1 += buttonIvrAddSubOption1_Click;
                sub.SubRemove1 += buttonIvrRemoveSubOption1_Click;
                sub.CreateOption();

                //NEED TO ENSURE QUEUES ARE LOADED!
                sub.comboboxIvrQueue1.DataSource = new BindingSource(CsQ.CsQueuesList, null);
                sub.comboboxIvrQueue1.DisplayMember = "Value";
                sub.comboboxIvrQueue1.ValueMember = "Key";

                ivrMenus.Add(sub);

                UpdateIvrControls();
                return sub;
            }
        }

        public void buttonIvrRemoveSubOption1_Click(int optionNumberLevel, int optionNumber, int optionNumberParent)
        {
            int sub1Counter = -1;
            if (optionNumberParent == 0) { sub1Counter = countIvrSubRoot0; }
            else if (optionNumberParent == 1) { sub1Counter = countIvrSubRoot1; }
            else if (optionNumberParent == 2) { sub1Counter = countIvrSubRoot2; }
            else if (optionNumberParent == 3) { sub1Counter = countIvrSubRoot3; }
            else if (optionNumberParent == 4) { sub1Counter = countIvrSubRoot4; }
            else if (optionNumberParent == 5) { sub1Counter = countIvrSubRoot5; }
            else if (optionNumberParent == 6) { sub1Counter = countIvrSubRoot6; }
            else if (optionNumberParent == 7) { sub1Counter = countIvrSubRoot7; }
            else if (optionNumberParent == 8) { sub1Counter = countIvrSubRoot8; }
            else if (optionNumberParent == 9) { sub1Counter = countIvrSubRoot9; }

            if (sub1Counter > -1)
            {
                ivrMenus.RemoveAll(x => x.OptionLevel == optionNumberLevel && x.OptionPosition == optionNumber && x.OptionParent == optionNumberParent);

                if (optionNumberParent == 0) { countIvrSubRoot0--; }
                else if (optionNumberParent == 1) { countIvrSubRoot1--; }
                else if (optionNumberParent == 2) { countIvrSubRoot2--; }
                else if (optionNumberParent == 3) { countIvrSubRoot3--; }
                else if (optionNumberParent == 4) { countIvrSubRoot4--; }
                else if (optionNumberParent == 5) { countIvrSubRoot5--; }
                else if (optionNumberParent == 6) { countIvrSubRoot6--; }
                else if (optionNumberParent == 7) { countIvrSubRoot7--; }
                else if (optionNumberParent == 8) { countIvrSubRoot7--; }
                else if (optionNumberParent == 9) { countIvrSubRoot9--; }
                
                UpdateIvrControls();
            }
        }

        private void UpdateIvrControls()
        {
            //var filter0 = testButton.Where(x => x != null && x.OptionLevel == 1 && x.OptionNumber == 1);
            metroPanelWfIvrOptions.Controls.Clear();
            metroPanelWfIvrOptions.Size = new Size(948, 200);
            int pos = 0;
            
            //Level 1
            for (int i1 = 0; i1 <= 9; i1++)
            {
                var filterL1 = (from t in ivrMenus
                    where t != null && t.OptionLevel == 0 && t.OptionPosition == i1
                    select t).ToList();

                if (filterL1.Count >= 1 && filterL1[0] != null)
                {                   
                    metroPanelWfIvrOptions.Size = new Size(metroPanelWfIvrOptions.Size.Width, metroPanelWfIvrOptions.Size.Height + 35);
                    
                    //1-1
                    //filterL1[0].labelIvr1.Location = new Point(5, 40 * pos + 2);
                    filterL1[0].comboboxIvrDtmf1.Location = new Point(0, 40 * pos);
                    filterL1[0].textboxIvrVoiceResponse1.Location = new Point(55, 40 * pos);
                    filterL1[0].comboboxIvrQueue1.Location = new Point(360, 40 * pos);
                    filterL1[0].buttonIvrAddSubOption1.Location = new Point(665, 40 * pos);
                    filterL1[0].buttonIvrRemoveSubOption1.Location = new Point(695, 40 * pos);

                    //metroPanelWfIvrOptions.Controls.Add(filterL1[0].labelIvr1);
                    metroPanelWfIvrOptions.Controls.Add(filterL1[0].comboboxIvrDtmf1);
                    metroPanelWfIvrOptions.Controls.Add(filterL1[0].textboxIvrVoiceResponse1);
                    metroPanelWfIvrOptions.Controls.Add(filterL1[0].comboboxIvrQueue1);
                    metroPanelWfIvrOptions.Controls.Add(filterL1[0].buttonIvrAddSubOption1);
                    metroPanelWfIvrOptions.Controls.Add(filterL1[0].buttonIvrRemoveSubOption1);
                    pos++;
                }

                //Level 2
                for (int i2 = 0; i2 <= 9; i2++)
                {
                    var filterL2 = (from t in ivrMenus
                                    where t != null && t.OptionLevel == 1 && t.OptionPosition == i2 && t.OptionParent == i1
                                    select t).ToList();


                    if (filterL2.Count >= 1 && filterL2[0] != null)
                    {
                        metroPanelWfIvrOptions.Size = new Size(metroPanelWfIvrOptions.Size.Width, metroPanelWfIvrOptions.Size.Height + 35);

                        //1-1
                        //filterL2[0].labelIvr1.Location = new Point(20, 40 * pos + 2);
                        filterL2[0].comboboxIvrDtmf1.Location = new Point(55, 40 * pos);
                        filterL2[0].textboxIvrVoiceResponse1.Location = new Point(110, 40 * pos);
                        filterL2[0].comboboxIvrQueue1.Location = new Point(415, 40 * pos);
                        //filterL2[0].buttonIvrAddSubOption1.Location = new Point(720, 40 * pos);
                        filterL2[0].buttonIvrRemoveSubOption1.Location = new Point(720, 40 * pos);

                        //metroPanelWfIvrOptions.Controls.Add(filterL2[0].labelIvr1);
                        metroPanelWfIvrOptions.Controls.Add(filterL2[0].comboboxIvrDtmf1);
                        metroPanelWfIvrOptions.Controls.Add(filterL2[0].textboxIvrVoiceResponse1);
                        metroPanelWfIvrOptions.Controls.Add(filterL2[0].comboboxIvrQueue1);
                        //metroPanelWfIvrOptions.Controls.Add(filterL2[0].buttonIvrAddSubOption1);
                        metroPanelWfIvrOptions.Controls.Add(filterL2[0].buttonIvrRemoveSubOption1);
                        pos++;
                    }
                }
            }
        }
       
        #endregion IVR OPTIONS CONTROLS

        #region LOAD ACTIVE TAB
        /// <summary>
        /// Loads main form active tab
        /// </summary>
        private void LoadTab()
        {
            if (metroTabControlMain.SelectedTab.Text == "Call Flow Designer" && !workflowsLoaded)
            {
                //Load Queues tab
                //LoadWf();
            }
            else if (metroTabControlMain.SelectedTab.Text == "Queues" && !queuesLoaded)
            {
                //Load Queues tab
                //LoadQueue();
            }
            else if (metroTabControlMain.SelectedTab.Text == "Groups" && !groupsLoaded)
            {
                //Load Groups tab
                //LoadGroup();
            }
            else if (metroTabControlMain.SelectedTab.Text == "Business Hours" && !busHoursLoaded)
            {
                //Load Business Hours tab
                //LoadBusHours();
            }
            else if (metroTabControlMain.SelectedTab.Text == "Holidays" && !holidaysLoaded)
            {
                //Load holidays tab
            }
        }
        
        /// <summary>
        /// Event handler for tab changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void metroTabControlMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadTab();
        }

        #endregion LOAD ACTIVE TAB

   

        //HELPER METHODS
        /// <summary>
        /// Clears most (as defined by the method) controls inside a defined panel
        /// http://stackoverflow.com/questions/6177069/how-to-clear-the-textbox-controls-in-a-panel
        /// </summary>
        /// <param name="panelName"></param>
        /// <param name="exclusions"></param>
        private void ClearPanelControls(Control panelName, string[] exclusions)
        {
            foreach (Control c in panelName.Controls)
            {
                if (!exclusions.Contains(c.Name))
                {
                    if (c is MetroTextBox)
                    {
                        MetroTextBox textBox = c as MetroTextBox;
                        if (textBox != null)
                        {
                            textBox.Text = "";
                        }
                    }

                    if (c is MetroComboBox)
                    {
                        MetroComboBox combobox = c as MetroComboBox;
                        if (combobox != null)
                        {
                            combobox.SelectedIndex = -1;
                        }
                    }
                }

                //if(c.Controls.count > 0)
                //{
                //    cleartest(c)
                //}
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        
    }
}
