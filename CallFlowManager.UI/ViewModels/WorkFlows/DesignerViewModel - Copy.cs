using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CallFlowManager.UI.Business;
using CallFlowManager.UI.Common;
using CallFlowManager.UI.ViewModels.BusinessHours;
using CallFlowManager.UI.ViewModels.Holidays;
using CallFlowManager.UI.ViewModels.Queues;

namespace CallFlowManager.UI.ViewModels.WorkFlows
{
    public class DesignerViewModelBeforePsTidyUp : PropertyChangedBase
    {
        private WorkFlowViewModel _selectedWorkFlow;
        private WorkFlowViewModel _currentWorkFlow;

        private readonly IDataService _dataService = DataServiceFactory.Get();

        private readonly BackgroundWorker _backgroundWorker;
        private bool _isLoading;

        public DesignerViewModelBeforePsTidyUp()
        {
            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.DoWork += LoadWorkFlows;
            _backgroundWorker.RunWorkerCompleted += ChangeStatus;
            _workFlowsLoaded = new List<WorkFlowViewModel>();
            Languages = Globals.Languages;
            TimeZones = Globals.TimeZones;
            Destinations = Globals.WfDestinations;
            //Uncomment to revert:
            //CurrentWorkFlow = new WorkFlowViewModel(this);
            //SelectedWorkFlow = new WorkFlowViewModel(this);

            LoadCommand = new RelayCommand(_ => { if (!_backgroundWorker.IsBusy) _backgroundWorker.RunWorkerAsync(); });
            CreateCommand = new RelayCommand(_ => CreateWorkFlow());
            WorkFlows = new ObservableCollection<WorkFlowViewModel>();
            //SipDomains = _dataService.SipDomains;
            Queues = new ObservableCollection<QueueViewModel>();

            _dataService.OnQueuesUpdated += delegate(EventArgs args)
            {
                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    Queues.Clear();
                    foreach (var queue in _dataService.QueueData.ToList().OrderBy(a => a.Name))
                    {
                        Queues.Add(queue);
                    };

                    OnPropertyChanged("CurrentWorkFlow");

                    ////foreach (var workFlow in WorkFlows) {
                    ////    foreach (var ivr in workFlow.IvrOptions) { 
                    ////        foreach (var childIvr in ivr.ChildIvrNodes) { 
                    ////            OnPropertyChanged()
                    ////    } }
                    ////}
                });

                //  LoadWorkFlows();
                //foreach (var workFlow in WorkFlows)
                //{
                //    if (workFlow.IvrOptions != null)
                //        foreach (var ivr in workFlow.IvrOptions)
                //        {
                //            ivr.InvoiceQueue = Queues.FirstOrDefault(p => p.Name.Equals(ivr.InvoiceQueue.Name));
                //            if (ivr.ChildIvrNodes != null) foreach (var childIve in ivr.ChildIvrNodes)
                //                {
                //                    childIve.InvoiceQueue = Queues.FirstOrDefault(p => p.Name.Equals(childIve.InvoiceQueue.Name));
                //                }
                //        }
                //}
            };


            BusinessHoursGroups = new ObservableCollection<BusinessHourGroupViewModel>();
            _dataService.OnBusinessHoursUpdated += delegate(EventArgs args)
            {
                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    BusinessHoursGroups.Clear();
                    if (_dataService.BusinessHoursGroups != null)
                        foreach (var @group in _dataService.BusinessHoursGroups.OrderBy(a => a.Name))
                        {

                            BusinessHoursGroups.Add(new BusinessHourGroupViewModel(@group));
                        };

                });
            };

            HolidaysGroups = new ObservableCollection<HolidayGroupViewModel>();

            _dataService.OnHolidaysUpdated += delegate(EventArgs args)
            {
                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    HolidaysGroups.Clear();
                    if (_dataService.HolidayGroups != null)
                        foreach (var @group in _dataService.HolidayGroups.OrderBy(a => a.Name))
                        {

                            HolidaysGroups.Add(new HolidayGroupViewModel(@group));
                        };

                });
            };
        }

        public ICommand RemoveRootIvrCommand { get; set; }

        public ObservableCollection<QueueViewModel> Queues { get; set; }

        public ObservableCollection<BusinessHourGroupViewModel> BusinessHoursGroups { get; private set; }

        public ObservableCollection<HolidayGroupViewModel> HolidaysGroups { get; private set; }

        private void ChangeStatus(object sender, RunWorkerCompletedEventArgs e)
        {
            IsLoading = false;
            WorkFlows.Clear();
            foreach (var workFlow in _workFlowsLoaded.OrderBy(a => a.Name))
            {
                WorkFlowViewModel flow = workFlow;
                Application.Current.Dispatcher.Invoke((Action)(() => WorkFlows.Add(flow)));
            }

            SipDomains = _dataService.SipDomains.ToList();
        }

        private void LoadWorkFlows(object sender, DoWorkEventArgs e)
        {
            IsLoading = true;
            LoadWorkFlows();
        }

        private void CreateWorkFlow()
        {
            //do the business here

            var test = _selectedWorkFlow;

            foreach (var IvrOption in test.IvrOptions)
            {
                var thisOPtion = IvrOption;

            }
            
            if (!CurrentWorkFlow.Name.Equals(SelectedWorkFlow.Name))
            {
                WorkFlows.Add(CurrentWorkFlow);
            }
            else
            {
                SelectedWorkFlow = CurrentWorkFlow;
            }
        }

        private void LoadWorkFlows()
        {
            IsLoading = true;
            _workFlowsLoaded.Clear();

            _dataService.LoadWorkFlows(new DesignerViewModel());

            while (!Queues.Any())
            {
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(10));
            }


            if (_dataService.WorkFlows == null)
            {
                return;
            }

            //Filter the returned PSObject for required purpuse
            var wFFilter = _dataService.WorkFlows.ToList();

            //foreach (PSObject workflowName in workflowNames)
            foreach (dynamic workflowName in wFFilter)
            {
                //Changed, should be (this) to revert
                var newWorkflow = new WorkFlowViewModel();
                if (workflowName != null)
                {
                    //GetDisplay Name
                    newWorkflow.Name = workflowName.Name;

                    if (workflowName.Description != null)
                    {
                        newWorkflow.Description = workflowName.Description;
                    }

                    //Get Line Uri
                    newWorkflow.Number = workflowName.LineUri;

                    //Get Display Number
                    newWorkflow.DisplayNumber = workflowName.DisplayNumber;

                    //Get Sip Address
                    var primaryUri = workflowName.PrimaryUri.Split('@');
                    newWorkflow.Uri = primaryUri[0];
                    newWorkflow.SipDomain = primaryUri[1];

                    //Get Time Zones
                    newWorkflow.TimeZone = workflowName.TimeZone;

                    //Get Languages
                    newWorkflow.Language = workflowName.Language;

                    //Get Toggle State
                    //Federation
                    if (workflowName.EnabledForFederation == true)
                    {
                        newWorkflow.EnableForFederation = true;
                    }
                    else newWorkflow.EnableForFederation = true;

                    //Anonymous
                    if (workflowName.Anonymous == true)
                    {
                        newWorkflow.EnableAgentAnonymity = true;
                    }
                    else newWorkflow.EnableAgentAnonymity = false;

                    //Ivr Mode
                    if (workflowName.DefaultAction.Action.ToString() == "TransferToQuestion")
                    {
                        //Set IVR mode to on
                        newWorkflow.EnableIVRMode = true;

                        //Populate IVR options
                        foreach (var root in workflowName.DefaultAction.Question.AnswerList)
                        {
                            if (root.Action.QueueID != null)
                            {
                                //var AnswerQueueName = Get-CsRgsQueue -Identity AnswerList.Action.QueueID).Name
                            }

                            var rootIvr = new IvrViewModel(newWorkflow)
                            {
                                Name = string.Join(",", root.VoiceResponseList),
                                Number = Int32.Parse(root.DtmfResponse),
                                InvoiceQueue = Queues.Last()
                            };

                            if (root.Action.Action.ToString() == "TransferToQuestion")
                            {
                                ////IVR toggle off
                                //newWorkflow.EnableIVRMode = false;
                                if (root.Action.Question.AnswerList != null)
                                {
                                    foreach (dynamic sub1 in root.Action.Question.AnswerList)
                                    {

                                        if (sub1.Action.Action.ToString() == "TransferToQueue")
                                        {
                                            var childIvr = new IvrViewModel(newWorkflow);
                                            childIvr.Name = string.Join(",", sub1.VoiceResponseList);
                                            childIvr.Number = Int32.Parse(sub1.DtmfResponse);

                                            childIvr.Queues = Queues;
                                            childIvr.Parent = rootIvr;

                                            rootIvr.ChildIvrNodes.Add(childIvr);

                                            childIvr.InvoiceQueue = Queues.FirstOrDefault(p => p.Id.Equals(sub1.Action.QueueID.ToString()));
                                        }
                                    }
                                }
                            }

                            newWorkflow.IvrOptions.Add(rootIvr);

                            if (workflowName.DefaultAction.Prompt != null)
                            {
                                //Get Workflow Message
                                if (workflowName.DefaultAction.Prompt.TextToSpeechPrompt != null)
                                {
                                    newWorkflow.WelcomeMessage =
                                        workflowName.DefaultAction.Prompt.TextToSpeechPrompt.ToString();
                                }
                            }

                            //Get Afterhours Message
                            if (workflowName.NonBusinessHoursAction.Prompt != null)
                            {
                                if (workflowName.NonBusinessHoursAction.Prompt.TextToSpeechPrompt != null)
                                {
                                    newWorkflow.AfterHoursMessage =
                                        workflowName.NonBusinessHoursAction.Prompt.TextToSpeechPrompt.ToString();
                                }
                            }

                            //Get Afterhours Sip Address
                            if (workflowName.NonBusinessHoursAction.Uri != null)
                            {
                                var afterhoursUri = workflowName.NonBusinessHoursAction.Uri.Split('@');
                                newWorkflow.AfterHoursUri = afterhoursUri[0];
                                newWorkflow.AfterHoursSipDomain = afterhoursUri[1];
                            }

                            //Get Afterhours Destination Type
                            if (workflowName.NonBusinessHoursAction.Action != null)
                            {
                                newWorkflow.BusinessHourDestination =
                                    workflowName.NonBusinessHoursAction.Action.ToString();
                            }


                            //Get Holidays Group

                            //Get Holidays Message
                            if (workflowName.HolidayAction.Prompt != null)
                            {
                                if (workflowName.HolidayAction.Prompt.TextToSpeechPrompt != null)
                                {
                                    newWorkflow.HolidayMessage =
                                        workflowName.HolidayAction.Prompt.TextToSpeechPrompt.ToString();
                                }
                            }

                            //Get Holidays Sip Address
                            if (workflowName.HolidayAction.Uri != null)
                            {
                                var holidayUri = workflowName.HolidayAction.Uri.Split('@');
                                newWorkflow.HolidayUri = holidayUri[0];
                                newWorkflow.HolidaySipDomain = holidayUri[1];
                            }

                            //Get Holidays Destination Type
                            if (workflowName.HolidayAction.Action != null)
                            {
                                newWorkflow.HolidayDestination = workflowName.HolidayAction.Action.ToString();
                            }



                        }
                    }
                }

                _workFlowsLoaded.Add(newWorkflow);
            }
        }

        public ICommand LoadCommand { get; private set; }

        public ICommand CreateCommand { get; private set; }

        public Dictionary<string, string> Destinations { get; private set; }

        public Dictionary<string, string> Languages { get; private set; }

        public Dictionary<string, string> TimeZones { get; private set; }

        public List<string> SipDomains { get; set; }


        public ObservableCollection<WorkFlowViewModel> WorkFlows { get; private set; }
        private List<WorkFlowViewModel> _workFlowsLoaded { get; set; }


        public WorkFlowViewModel SelectedWorkFlow
        {
            get { return _selectedWorkFlow; }
            set
            {
                if (_selectedWorkFlow != value && value != null)
                {
                    _selectedWorkFlow = value;
                    CurrentWorkFlow = (WorkFlowViewModel)value.Clone();
                    OnPropertyChanged("SelectedWorkFlow");
                }
            }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged("IsLoading");
                }
            }
        }

        public WorkFlowViewModel CurrentWorkFlow
        {
            get { return _currentWorkFlow; }
            set
            {
                if (_currentWorkFlow != value && value != null)
                {
                    _currentWorkFlow = value;
                    OnPropertyChanged("CurrentWorkFlow");
                }
            }
        }
    }
}