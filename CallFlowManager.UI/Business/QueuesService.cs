using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using CallFlowManager.UI;
using CallFlowManager.UI.ViewModels.Groups;
using CallFlowManager.UI.ViewModels.Queues;

namespace CallFlowManager.UI.Business
{
    public class QueuesService
    {
        //Local copy of passed in Ps data object
        private ICollection<PSObject> _csQueues { get; set; }
        //private ICollection<PSObject> _csUsers { get; set; }

        public IList<QueueViewModel> LoadedQueues; 

        public QueuesService()
        {
            //LoadedQueues = new List<QueueViewModel>();
        }

        public IEnumerable<QueueViewModel> ProcessPsQueues(ICollection<PSObject> csQueues, ICollection<PSObject> csGroups)
        {
            _csQueues = csQueues;
            //_csUsers = csUsers;

            //var qFilter = _dataService.Queues;

            //var loadedQueues = new List<QueueViewModel>();
            LoadedQueues = new List<QueueViewModel>();

            if (_csQueues == null)
            {
                return LoadedQueues;
            }

            foreach (dynamic queue in _csQueues)
            {
                var newQueueViewModel = new QueueViewModel();
                if (queue != null)
                {
                    //Using only the instance Id now
                    //newQueueViewModel.Id = queue.Identity.ServiceId.FullName.ToString() + "/" +
                    //                queue.Identity.InstanceId.ToString();

                    newQueueViewModel.Id = queue.Identity.InstanceId.ToString();

                    if (queue.Name != null)
                    {
                        newQueueViewModel.Name = queue.Name;
                    }

                    if (queue.OwnerPool != null)
                    {
                        newQueueViewModel.OwnerPool = queue.OwnerPool.ToString();
                    }

                    if (queue.Description != null)
                    {
                        newQueueViewModel.Description = queue.Description;
                    }
                    
                    //Queue timeout
                    if (queue.TimeoutThreshold != null)
                    {
                        newQueueViewModel.TimeoutOn = true;
                        newQueueViewModel.Timeout = queue.TimeoutThreshold;
                    }
                    else
                    {
                        newQueueViewModel.TimeoutOn = false;
                    }

                    if (queue.TimeoutAction.Action != null)
                    {
                        newQueueViewModel.TimeoutDestination = queue.TimeoutAction.Action.ToString();
                    }

                    if (queue.TimeoutAction != null && queue.TimeoutAction.Uri != null)
                    {
                        var qUri = queue.TimeoutAction.Uri.ToString().Split('@');
                        var qUriLeft = qUri[0].Split(':');
                        newQueueViewModel.TimeoutUri = qUriLeft[1];
                        newQueueViewModel.TimeoutSipDomain = qUri[1];
                    }

                    //Is action queue
                    if (queue.TimeoutAction.QueueID != null)
                    {
                        //UI needs work
                        newQueueViewModel.TimeoutQueueId = queue.TimeoutAction.QueueID.InstanceId.ToString();
                    }


                    //Queue overflow
                    if (queue.OverflowThreshold != null)
                    {
                        newQueueViewModel.OverflowOn = true;

                        newQueueViewModel.OverFlow = queue.OverflowThreshold;

                        //Is action Uri
                        if (queue.OverflowAction.Uri != null)
                        {
                            var qOverflowUri = queue.OverflowAction.Uri.ToString().Split('@');
                            var qOverflowUriLeft = qOverflowUri[0].Split(':');
                            newQueueViewModel.OverFlowUri = qOverflowUriLeft[1];
                            newQueueViewModel.OverFlowSipDomain = qOverflowUri[1];
                        }
                        //Is action queue
                        if (queue.OverflowAction.QueueID != null)
                        {
                            //UI needs work
                            newQueueViewModel.OverFlowQueueId = queue.OverflowAction.QueueID.InstanceId.ToString();
                        }

                        newQueueViewModel.OverFlowDestination = queue.OverflowAction.Action.ToString();
                        newQueueViewModel.OverFlowCandidate = queue.OverflowCandidate.ToString();
                    }
                    else
                    {
                        newQueueViewModel.OverflowOn = false;
                    }

                    //Queue agent list
                    if (queue.AgentGroupIDList != null)
                    {
                        //int i = 0;
                        foreach (var queueGroup in queue.AgentGroupIDList)
                        {
                            //Filter the Queue object in memory to the selected queue
                            //if (csGroups == null)
                            //{
                            //    _dataService.LoadGroups();
                            //}

                            if (csGroups != null)
                            {
                                dynamic groupFilter =
                                    csGroups.Where(
                                        x => x.Members["Identity"].Value.ToString() == queueGroup.ToString()).ToList();

                                foreach (var group in groupFilter)
                                {
                                    if (group != null)
                                    {
                                        if (group.Name != null && group.AgentAlertTime != null &&
                                            group.ParticipationPolicy != null && group.RoutingMethod != null)
                                        {

                                            if (newQueueViewModel.Groups.Any(p => p.Name.Contains(group.Name.ToString())))
                                            {
                                                throw new InvalidOperationException("Already exists");
                                            }

                                            newQueueViewModel.Groups.Add(new QueueGroupViewModel(group.Name.ToString(), group.AgentAlertTime.ToString(),
                                                group.ParticipationPolicy.ToString(), group.RoutingMethod.ToString()));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                LoadedQueues.Add(newQueueViewModel);
            }

            //After all queues are loaded, process the queue values for Timeout and Overflow
            if (LoadedQueues != null)
            {
                foreach (QueueViewModel loadedQueue in LoadedQueues)
                {
                    if (loadedQueue.OverFlowQueueId != null && loadedQueue.Id != null)
                    {
                        loadedQueue.OverFlowQueue = LoadedQueues.FirstOrDefault(p => p.Id != null && loadedQueue.OverFlowQueueId == p.Id.ToString());
                    }

                    if (loadedQueue.TimeoutQueueId != null && loadedQueue.Id != null)
                    {
                        loadedQueue.TimeoutQueue = LoadedQueues.FirstOrDefault(p => p.Id != null && loadedQueue.TimeoutQueueId == p.Id.ToString());
                    }
                }
            }

            return LoadedQueues;
        }
    }
}
