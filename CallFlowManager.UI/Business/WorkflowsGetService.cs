using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using CallFlowManager.UI;
using CallFlowManager.UI.ViewModels.BusinessHours;
using CallFlowManager.UI.ViewModels.Groups;
using CallFlowManager.UI.ViewModels.Holidays;
using CallFlowManager.UI.ViewModels.Queues;
using CallFlowManager.UI.ViewModels.WorkFlows;
using CallFlowManager.UI.Views;

namespace CallFlowManager.UI.Business
{
    public class WorkflowsGetService
    {
        //Local copy of passed in Ps data object
        private ICollection<PSObject> _csWorkflows { get; set; }
        //private ICollection<PSObject> _csUsers { get; set; }

        public IList<WorkFlowViewModel> LoadedWorkflows;

        //public DesignerViewModel DesignerViewModel;
        //public ObservableCollection<QueueViewModel> Queues { get; set; }
        //public ObservableCollection<BusinessHourGroupViewModel> BusinessHoursGroups { get; private set; }
        //public ObservableCollection<HolidaysGroupViewModel> HolidaysGroups { get; private set; }

        public WorkflowsGetService()
        {
            //DesignerViewModel = new DesignerViewModel();
        }

        public IEnumerable<WorkFlowViewModel> ProcessPsWorkflows(ICollection<PSObject> csWorkflows, DesignerViewModel designerViewModel)
        //public IEnumerable<WorkFlowViewModel> ProcessPsWorkflows(ICollection<PSObject> csWorkflows)
        {
            _csWorkflows = csWorkflows;

            LoadedWorkflows = new List<WorkFlowViewModel>();
            //Queues = new ObservableCollection<QueueViewModel>();
            //BusinessHoursGroups = new ObservableCollection<BusinessHourGroupViewModel>();
            //HolidaysGroups = new ObservableCollection<HolidaysGroupViewModel>();


            //Filter the returned PSObject for required purpuse
            //var wFFilter = _dataService.WorkFlows.ToList();
            //var designerViewModel = new DesignerViewModel();

            //foreach (PSObject workflowName in workflowNames)
            foreach (dynamic workflowName in _csWorkflows)
            {
                //var newWorkflow = new WorkFlowViewModel(designerViewModel);
                var newWorkflow = new WorkFlowViewModel();
                //newWorkflow.DesignerViewModel = designerViewModel;
                if (workflowName != null)
                {
                    //GetDisplay Name
                    if (workflowName.Name != null)
                        newWorkflow.Name = workflowName.Name;

                    //OwnerPool
                    if (workflowName.OwnerPool != null)
                        newWorkflow.OwnerPool = workflowName.OwnerPool.ToString();

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
                    //Active
                    if (workflowName.Active == true)
                    {
                        newWorkflow.EnableWorkflow = true;
                    }
                    else newWorkflow.EnableWorkflow = false;
                    
                    //Federation
                    if (workflowName.EnabledForFederation == true)
                    {
                        newWorkflow.EnableForFederation = true;
                    }
                    else newWorkflow.EnableForFederation = false;

                    //Anonymous
                    if (workflowName.Anonymous == true)
                    {
                        newWorkflow.EnableAgentAnonymity = true;
                    }
                    else newWorkflow.EnableAgentAnonymity = false;

                    //Ivr Mode ON
                    if (workflowName.DefaultAction != null)
                    {
                        if (workflowName.DefaultAction.Action.ToString() == "TransferToQuestion")
                        {
                            //Set IVR mode to on
                            newWorkflow.EnableIVRMode = true;

                            //Root IVR Message
                            if (workflowName.DefaultAction.Question != null)
                            {
                                //Text to speech
                                if (workflowName.DefaultAction.Question.Prompt.TextToSpeechPrompt != null)
                                {
                                    newWorkflow.IvrMessage =
                                        workflowName.DefaultAction.Question.Prompt.TextToSpeechPrompt.ToString();
                                }

                                //Audio file
                                if (workflowName.DefaultAction.Question.Prompt.AudioFilePrompt != null)
                                {
                                    newWorkflow.AudioIvr =
                                        workflowName.DefaultAction.Question.Prompt.AudioFilePrompt.ToString();
                                }
                            }

                            //Populate IVR options
                            foreach (var root in workflowName.DefaultAction.Question.AnswerList)
                            {
                                //if (root.Action.QueueID != null)
                                //{
                                //    //var AnswerQueueName = Get-CsRgsQueue -Identity AnswerList.Action.QueueID).Name
                                //}

                                var rootIvr = new IvrViewModel(newWorkflow)
                                {
                                    Name = string.Join(",", root.VoiceResponseList),
                                    Number = Int32.Parse(root.DtmfResponse)
                                };

                                if (root.Action.QueueID != null)
                                {
                                    rootIvr.InvoiceQueue =
                                        designerViewModel.Queues.FirstOrDefault(p => p.Id.Equals(root.Action.QueueID.InstanceId.ToString()));
                                }

                                //Sub Option TextToSpeech and Audio File
                                if (root.Action.Action.ToString() == "TransferToQuestion")
                                {
                                    //Get Ivr sub option message
                                    if (root.Action.Question.Prompt != null)
                                    {
                                        //Text to speech
                                        if (root.Action.Question.Prompt.TextToSpeechPrompt != null)
                                        {
                                            rootIvr.TextIvrMessage =
                                                root.Action.Question.Prompt.TextToSpeechPrompt.ToString();
                                        }

                                        //Audio file
                                        if (root.Action.Question.Prompt.AudioFilePrompt != null)
                                        {
                                            rootIvr.AudioIvrTree =
                                                root.Action.Question.Prompt.AudioFilePrompt.ToString();
                                        }
                                    }
                                    
                                    
                                    if (root.Action.Question.AnswerList != null)
                                    {
                                        foreach (dynamic sub1 in root.Action.Question.AnswerList)
                                        {
                                            if (sub1.Action.Action.ToString() == "TransferToQueue")
                                            {
                                                var childIvr = new IvrViewModel(newWorkflow);
                                                childIvr.Name = string.Join(",", sub1.VoiceResponseList);
                                                childIvr.Number = Int32.Parse(sub1.DtmfResponse);

                                                childIvr.Queues = designerViewModel.Queues;
                                                childIvr.Parent = rootIvr;

                                                rootIvr.ChildIvrNodes.Add(childIvr);

                                                childIvr.InvoiceQueue =
                                                designerViewModel.Queues.FirstOrDefault(p => p.Id.Equals(sub1.Action.QueueID.InstanceId.ToString()));

                                                //{Service:1-ApplicationServer-1/bec87a12-f898-4c4e-a8c2-9aa7540db9d2}
                                                //"Service:1-ApplicationServer-1/bec87a12-f898-4c4e-a8c2-9aa7540db9d2"
                                                //{bec87a12-f898-4c4e-a8c2-9aa7540db9d2}
                                                //"bec87a12-f898-4c4e-a8c2-9aa7540db9d2"

                                            }
                                        }
                                    }
                                }

                                newWorkflow.IvrOptions.Add(rootIvr);
                            }
                        }

                        //Ivr Mode OFF
                        if (workflowName.DefaultAction.Action != null)
                        {
                            if (workflowName.DefaultAction.Action.ToString() == "TransferToQueue" &&
                                workflowName.DefaultAction.QueueID != null)
                            {
                                newWorkflow.Queue =
                                    designerViewModel.Queues.FirstOrDefault(
                                        p => p.Id.Equals(workflowName.DefaultAction.QueueID.InstanceId.ToString()));
                            }

                            
                        }

                        //Get Workflow Message
                        if (workflowName.DefaultAction.Prompt != null)
                        {
                            //Text to speech
                            if (workflowName.DefaultAction.Prompt.TextToSpeechPrompt != null)
                            {
                                newWorkflow.WelcomeMessage =
                                    workflowName.DefaultAction.Prompt.TextToSpeechPrompt.ToString();
                            }

                            //Audio file
                            if (workflowName.DefaultAction.Prompt.AudioFilePrompt != null)
                            {
                                newWorkflow.AudioWelcome =
                                    workflowName.DefaultAction.Prompt.AudioFilePrompt.ToString();
                            }
                        }
                    }

                    //Get Afterhours Message
                    if (workflowName.NonBusinessHoursAction != null)
                    {
                        if (workflowName.NonBusinessHoursAction.Prompt != null)
                        {
                            newWorkflow.EnableBusinessHours = true;

                            //Text to speech
                            if (workflowName.NonBusinessHoursAction.Prompt.TextToSpeechPrompt != null)
                            {
                                newWorkflow.AfterHoursMessage =
                                    workflowName.NonBusinessHoursAction.Prompt.TextToSpeechPrompt.ToString();
                            }

                            //Audio file
                            if (workflowName.NonBusinessHoursAction.Prompt.AudioFilePrompt != null)
                            {
                                newWorkflow.AudioAfterHours =
                                    workflowName.NonBusinessHoursAction.Prompt.AudioFilePrompt.ToString();
                            }
                        }


                        //Get Afterhours Sip Address
                        if (workflowName.NonBusinessHoursAction != null)
                        {
                            newWorkflow.EnableBusinessHours = true;
                            if (workflowName.NonBusinessHoursAction.Uri != null)
                            {
                                var afterhoursUri = workflowName.NonBusinessHoursAction.Uri.Split('@');
                                newWorkflow.AfterHoursUri = afterhoursUri[0];
                                newWorkflow.AfterHoursSipDomain = afterhoursUri[1];
                            }

                            //Get Afterhours Destination Type
                            if (workflowName.NonBusinessHoursAction.Action != null)
                            {
                                newWorkflow.AfterHoursDestination =
                                    workflowName.NonBusinessHoursAction.Action.ToString();
                            }

                            ////Get Afterhours Queue
                            //if (workflowName.NonBusinessHoursAction.QueueID != null)
                            //{
                            //    newWorkflow.AfterHoursQueue =
                            //        workflowName.NonBusinessHoursAction.QueueID.InstanceId.ToString();
                            //}

                        }
                    }
                    else
                    {
                        newWorkflow.EnableBusinessHours = false;
                    }

                    //Get After Hours Group
                    if (workflowName.BusinessHoursID != null)
                    {
                        newWorkflow.BusinessHoursGroup =
                            designerViewModel.BusinessHoursGroups.FirstOrDefault(p => p.Id == workflowName.BusinessHoursID.InstanceId.ToString());
                    }


                    //Get Holidays Group
                    if (workflowName.HolidaySetIDList.Count >= 1)
                    {
                        newWorkflow.EnableHolidays = true;

                        foreach (dynamic holList in workflowName.HolidaySetIDList)
                        {
                            newWorkflow.HolidayGroup =
                                designerViewModel.HolidaysGroups.FirstOrDefault(
                                    p => p.Id == holList.InstanceId.ToString());
                        }
                    }
                    else
                    {
                        newWorkflow.EnableHolidays = false;
                    }
                    

                    //Get Holidays Message
                    if (workflowName.HolidayAction != null)
                    {
                        if (workflowName.HolidayAction.Prompt != null)
                        {
                            //Text to speech
                            if (workflowName.HolidayAction.Prompt.TextToSpeechPrompt != null)
                            {
                                newWorkflow.HolidayMessage =
                                    workflowName.HolidayAction.Prompt.TextToSpeechPrompt.ToString();
                            }

                            //Audio file
                            if (workflowName.HolidayAction.Prompt.AudioFilePrompt != null)
                            {
                                newWorkflow.AudioHolidays =
                                    workflowName.HolidayAction.Prompt.AudioFilePrompt.ToString();
                            }
                        }

                        if (workflowName.HolidayAction != null)
                        {
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

                            ////Get Holidays Queue
                            //if (workflowName.HolidayAction.QueueID != null)
                            //{
                            //    newWorkflow.HolidayQueue = workflowName.HolidayAction.QueueID.InstanceId.ToString();
                            //}
                        }
                    }

                    //Hold Music                    
                    if (workflowName.CustomMusicOnHoldFile != null)
                    {
                        newWorkflow.AudioHoldMusic =
                            workflowName.CustomMusicOnHoldFile.OriginalFileName.ToString();
                    }
                    
                }

                LoadedWorkflows.Add(newWorkflow);
            }
            return LoadedWorkflows;
        }
    }
}
