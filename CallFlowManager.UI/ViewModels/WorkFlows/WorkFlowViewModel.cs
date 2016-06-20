using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CallFlowManager.UI.Business;
using CallFlowManager.UI.Common;
using CallFlowManager.UI.ViewModels.BusinessHours;
using CallFlowManager.UI.ViewModels.Holidays;
using CallFlowManager.UI.ViewModels.Queues;
using CallFlowManager.UI.Views;

namespace CallFlowManager.UI.ViewModels.WorkFlows
{
    public class WorkFlowViewModel : PropertyChangedBase, ICloneable
    {      
        //public WorkFlowViewModel(DesignerViewModel designerViewModel)
        public WorkFlowViewModel()
        {
            Queue = new QueueViewModel();
            IvrOptions = new ObservableCollection<IvrViewModel>(new List<IvrViewModel>());
            AddIvrRootCommand = new RelayCommand(_ => AddIvr());            
            //_designerViewModel = designerViewModel;
        }

        
        private void AddIvr()
        {
            IvrOptions.Add(new IvrViewModel(this) { Name = "Voice Response (required)", Parent = null, Number = 1 });
        }

        public void RemoveParentIvr(IvrViewModel ivr)
        {
            _designerViewModel.CurrentWorkFlow.IvrOptions.Remove(ivr);
        }        


        //Instance of the DesignerViewModel is required so that RemoveParentIvr method has reference to its parent
        public DesignerViewModel DesignerViewModel
        {
            get { return _designerViewModel; }
            set
            {
                if (_designerViewModel != value)
                {
                    _designerViewModel = value;
                    OnPropertyChanged("DesignerViewModel");
                }
            }
        }

        public QueueViewModel Queue
        {
            get { return _queue; }
            set
            {
                if (_queue != value)
                {
                    _queue = value;
                    OnPropertyChanged("Queue");
                }
            }
        }

        public BusinessHourGroupViewModel BusinessHoursGroup
        {
            get { return _businessHoursGroup; }
            set
            {
                if (_businessHoursGroup != value)
                {
                    _businessHoursGroup = value;
                    OnPropertyChanged("BusinessHoursGroup");
                }
            }
        }

        private void UpdateFilters()
        {
            if (DesignerViewModel != null)
            {
                BusinessHoursGroupsFiltered =
                    DesignerViewModel.BusinessHoursGroups.Where(w => w.OwnerPool.Equals(OwnerPool));

                HolidayGroupsFiltered =
                    DesignerViewModel.HolidaysGroups.Where(w => w.OwnerPool.Equals(OwnerPool));

                QueuesFiltered =
                    DesignerViewModel.Queues.Where(w => w.OwnerPool.Equals(OwnerPool));
            }

        }
        

        public IEnumerable<BusinessHourGroupViewModel> BusinessHoursGroupsFiltered
        {
            get { return _businessHoursGroupsFiltered; }
            set
            {
                if (_businessHoursGroupsFiltered != value)
                {
                    _businessHoursGroupsFiltered = value;
                    OnPropertyChanged("BusinessHoursGroupsFiltered");
                }
            }
        }

        public IEnumerable<HolidayGroupViewModel> HolidayGroupsFiltered
        {
            get { return _holidayGroupsFiltered; }
            set
            {
                if (_holidayGroupsFiltered != value)
                {
                    _holidayGroupsFiltered = value;
                    OnPropertyChanged("HolidayGroupsFiltered");
                }
            }
        }

        public IEnumerable<QueueViewModel> QueuesFiltered
        {
            get { return _queuesFiltered; }
            set
            {
                if (_queuesFiltered != value)
                {
                    _queuesFiltered = value;
                    OnPropertyChanged("QueuesFiltered");
                }
            }
        }
        
        public HolidayGroupViewModel AfterHoursGroup
        {
            get { return _afterHoursGroup; }
            set
            {
                if (_afterHoursGroup != value)
                {
                    _afterHoursGroup = value;
                    OnPropertyChanged("AfterHoursGroup");
                }
            }
        }

        public HolidayGroupViewModel HolidayGroup
        {
            get { return _holidayGroup; }
            set
            {
                if (_holidayGroup != value)
                {
                    _holidayGroup = value;
                    OnPropertyChanged("HolidayGroup");
                }
            }
        }

        public string Language
        {
            get { return _language; }
            set
            {
                if (_language != value)
                {
                    _language = value;
                    OnPropertyChanged("Language");
                }
            }
        }

        public string TimeZone
        {
            get { return _timeZone; }
            set
            {
                if (_timeZone != value)
                {
                    _timeZone = value;
                    OnPropertyChanged("TimeZone");
                }
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public string Uri
        {
            get { return _uri; }
            set
            {
                if (_uri != value)
                {
                    _uri = value;
                    _sipAddress = String.Concat(_uri + "@" + _sipDomain);
                    OnPropertyChanged("Uri");
                }
            }
        }

        public string AfterHoursUri
        {
            get { return _afterHoursUri; }
            set
            {
                if (_afterHoursUri != value)
                {
                    _afterHoursUri = value;
                    OnPropertyChanged("AfterHoursUri");
                }
            }
        }

        public string HolidayUri
        {
            get { return _holidayUri; }
            set
            {
                if (_holidayUri != value)
                {
                    _holidayUri = value;
                    OnPropertyChanged("HolidayUri");
                }
            }
        }

        public string AfterHoursSipDomain
        {
            get { return _afterHoursSipDomain; }
            set
            {
                if (_afterHoursSipDomain != value)
                {
                    _afterHoursSipDomain = value;
                    OnPropertyChanged("AfterHoursSipDomain");
                }
            }
        }

        public string HolidaySipDomain
        {
            get { return _holidaySipDomain; }
            set
            {
                if (_holidaySipDomain != value)
                {
                    _holidaySipDomain = value;
                    OnPropertyChanged("HolidaySipDomain");
                }
            }
        }

        public string AfterHoursQueue
        {
            get { return _afterHoursQueue; }
            set
            {
                if (_afterHoursQueue != value)
                {
                    _afterHoursQueue = value;
                    OnPropertyChanged("AfterHoursQueue");
                }
            }
        }

        public string HolidayQueue
        {
            get { return _holidayQueue; }
            set
            {
                if (_holidayQueue != value)
                {
                    _holidayQueue = value;
                    OnPropertyChanged("HolidayQueue");
                }
            }
        }

        public string Number
        {
            get { return _number; }
            set
            {
                if (_number != value)
                {
                    _number = value;
                    OnPropertyChanged("Number");
                }
            }
        }

        public string DisplayNumber
        {
            get { return _displayNumber; }
            set
            {
                if (_displayNumber != value)
                {
                    _displayNumber = value;
                    OnPropertyChanged("DisplayNumber");
                }
            }
        }

        public string BusinessHourDestination
        {
            get { return _businessHourDestination; }
            set
            {
                if (_businessHourDestination != value)
                {
                    _businessHourDestination = value;
                    OnPropertyChanged("BusinessHourDestination");
                }
            }
        }

        public string AfterHoursDestination
        {
            get { return _afterHoursDestination; }
            set
            {
                if (_afterHoursDestination != value)
                {
                    _afterHoursDestination = value;
                    OnPropertyChanged("AfterHoursDestination");
                }
            }
        }

        public string HolidayDestination
        {
            get { return _holidayDestination; }
            set
            {
                if (_holidayDestination != value)
                {
                    _holidayDestination = value;
                    OnPropertyChanged("HolidayDestination");
                }
            }
        }

        public string HolidayMessage
        {
            get { return _holidayMessage; }
            set
            {
                if (_holidayMessage != value)
                {
                    _holidayMessage = value;
                    OnPropertyChanged("HolidayMessage");
                }
            }
        }

        public string AfterHoursMessage
        {
            get { return _afterHoursMessage; }
            set
            {
                if (_afterHoursMessage != value)
                {
                    _afterHoursMessage = value;
                    OnPropertyChanged("AfterHoursMessage");
                }
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged("Description");
                }
            }
        }

        public string WelcomeMessage
        {
            get { return _welcomeMessage; }
            set
            {
                if (_welcomeMessage != value)
                {
                    _welcomeMessage = value;
                    OnPropertyChanged("WelcomeMessage");
                }
            }
        }  
        
        public string SipDomain
        {
            get { return _sipDomain; }
            set
            {
                if (_sipDomain != value)
                {
                    _sipDomain = value;
                    _sipAddress = String.Concat(_uri + "@" + _sipDomain);
                    OnPropertyChanged("SipDomain");
                }
            }
        }

        public string SipAddress
        {
            //TODO: this should return the prefix of the SIP address
            get { return _sipAddress; }
            set
            {
                if (_sipAddress != value)
                {
                    _sipAddress = value;
                    OnPropertyChanged("SipAddress");
                }
            }
        }


        public bool EnableWorkflow
        {
            get { return _enableWorkflow; }
            set
            {
                if (_enableWorkflow != value)
                {
                    _enableWorkflow = value;
                    OnPropertyChanged("EnableWorkflow");
                }
            }
        }

        public bool EnableForFederation
        {
            get { return _enableForFederation; }
            set
            {
                if (_enableForFederation != value)
                {
                    _enableForFederation = value;
                    OnPropertyChanged("EnableForFederation");
                }
            }
        }

        public bool EnableAgentAnonymity
        {
            get { return _enableAgentAnonymity; }
            set
            {
                if (_enableAgentAnonymity != value)
                {
                    _enableAgentAnonymity = value;
                    OnPropertyChanged("EnableAgentAnonymity");
                }
            }
        }

        public bool EnableIVRMode
        {
            get { return _enableIVRMode; }
            set
            {
                if (_enableIVRMode != value)
                {
                    _enableIVRMode = value;
                    OnPropertyChanged("EnableIVRMode");
                }
            }
        }

        public bool EnableBusinessHours
        {
            get { return _enableBusinessHours; }
            set
            {
                if (_enableBusinessHours != value)
                {
                    _enableBusinessHours = value;
                    OnPropertyChanged("EnableBusinessHours");
                }
            }
        }
        
        public bool EnableHolidays
        {
            get { return _enableHolidays; }
            set
            {
                if (_enableHolidays != value)
                {
                    _enableHolidays = value;
                    OnPropertyChanged("EnableHolidays");
                }
            }
        }

        public string OwnerPool
        {
            get { return _ownerPool; }
            set
            {
                if (_ownerPool != value)
                {
                    _ownerPool = value;
                    UpdateFilters();
                    OnPropertyChanged("OwnerPool");
                }
            }
        }

        public string IvrMessage
        {
            get { return _ivrMessage; }
            set
            {
                if (_ivrMessage != value)
                {
                    _ivrMessage = value;
                    OnPropertyChanged("IvrMessage");
                }
            }
        }


        public string AudioWelcome
        {
            get { return _audioWelcome; }
            set
            {
                if (_audioWelcome != value)
                {
                    _audioWelcome = value;
                    OnPropertyChanged("AudioWelcome");
                }
            }
        }

        public string AudioWelcomeFilePath
        {
            get { return _audioWelcomeFilePath; }
            set
            {
                if (_audioWelcomeFilePath != value)
                {
                    _audioWelcomeFilePath = value;
                    OnPropertyChanged("AudioWelcomeFilePath");
                }
            }
        }

        public string AudioAfterHours
        {
            get { return _audioAfterHours; }
            set
            {
                if (_audioAfterHours != value)
                {
                    _audioAfterHours = value;
                    OnPropertyChanged("AudioAfterHours");
                }
            }
        }

        public string AudioAfterHoursFilePath
        {
            get { return _audioAfterHoursFilePath; }
            set
            {
                if (_audioAfterHoursFilePath != value)
                {
                    _audioAfterHoursFilePath = value;
                    OnPropertyChanged("AudioAfterHoursFilePath");
                }
            }
        }         

        public string AudioHolidays
        {
            get { return _audioHolidays; }
            set
            {
                if (_audioHolidays != value)
                {
                    _audioHolidays = value;
                    OnPropertyChanged("AudioHolidays");
                }
            }
        }

        public string AudioHolidaysFilePath
        {
            get { return _audioHolidaysFilePath; }
            set
            {
                if (_audioHolidaysFilePath != value)
                {
                    _audioHolidaysFilePath = value;
                    OnPropertyChanged("AudioHolidaysFilePath");
                }
            }
        }

        public string AudioIvr
        {
            get { return _audioIvr; }
            set
            {
                if (_audioIvr != value)
                {
                    _audioIvr = value;
                    OnPropertyChanged("AudioIvr");
                }
            }
        }

        public string AudioIvrFilePath
        {
            get { return _audioIvrFilePath; }
            set
            {
                if (_audioIvrFilePath != value)
                {
                    _audioIvrFilePath = value;
                    OnPropertyChanged("AudioIvrFilePath");
                }
            }
        }

        public string AudioHoldMusic
        {
            get { return _audioHoldMusic; }
            set
            {
                if (_audioHoldMusic != value)
                {
                    _audioHoldMusic = value;
                    OnPropertyChanged("AudioHoldMusic");
                }
            }
        }        

        public string AudioHoldMusicFilePath
        {
            get { return _audioHoldMusicFilePath; }
            set
            {
                if (_audioHoldMusicFilePath != value)
                {
                    _audioHoldMusicFilePath = value;
                    OnPropertyChanged("AudioHoldMusicFilePath");
                }
            }
        }        
        
        public object Clone()
        {
            //var newWorkFlow =  new WorkFlowViewModel(_designerViewModel)
            var newWorkFlow =  new WorkFlowViewModel()
            {
                DesignerViewModel = DesignerViewModel,
                Name = Name,
                DisplayNumber= DisplayNumber,
                Description= Description,
                Number= Number,
                Language= Language,
                TimeZone = TimeZone,
                AfterHoursMessage = AfterHoursMessage,
                AfterHoursUri = AfterHoursUri,
                AfterHoursSipDomain = AfterHoursSipDomain,
                AfterHoursDestination = AfterHoursDestination,
                AfterHoursQueue = AfterHoursQueue,  //not used, doesnt work in Lync
                HolidayMessage = HolidayMessage,
                HolidayUri = HolidayUri,
                HolidaySipDomain = HolidaySipDomain,
                HolidayQueue = HolidayQueue,
                HolidayDestination = HolidayDestination, 
                HolidayGroup = HolidayGroup,
                EnableWorkflow = EnableWorkflow,
                EnableAgentAnonymity = EnableAgentAnonymity,
                EnableForFederation = EnableForFederation,
                EnableIVRMode = EnableIVRMode,
                EnableBusinessHours = EnableBusinessHours,
                EnableHolidays = EnableHolidays,
                SipAddress = SipAddress,
                SipDomain = SipDomain,
                WelcomeMessage = WelcomeMessage,
                BusinessHourDestination = BusinessHourDestination,
                BusinessHoursGroup = BusinessHoursGroup,
                Uri = Uri,
               
                IvrOptions = new ObservableCollection<IvrViewModel>(IvrOptions),
                OwnerPool = OwnerPool,
                IvrMessage = IvrMessage,
                AudioWelcome = AudioWelcome,
                AudioWelcomeFilePath = AudioWelcomeFilePath,
                AudioAfterHours = AudioAfterHours,
                AudioAfterHoursFilePath = AudioAfterHoursFilePath,
                AudioHoldMusic = AudioHoldMusic,
                AudioHoldMusicFilePath = AudioHoldMusicFilePath,
                AudioHolidays = AudioHolidays,
                AudioHolidaysFilePath = AudioHolidaysFilePath,
                AudioIvr = AudioIvr,
                AudioIvrFilePath = AudioIvrFilePath,
                Queue = string.IsNullOrEmpty(Queue.Id) ? null : Queue,
            };

            return newWorkFlow;
        }

        public void Copy(WorkFlowViewModel src)
        {            
            {
                DesignerViewModel = src.DesignerViewModel;
                Name = src.Name;
                DisplayNumber = src.DisplayNumber;
                Description = src.Description;
                Number = src.Number;
                Language = src.Language;
                TimeZone = src.TimeZone;
                AfterHoursMessage = src.AfterHoursMessage;
                AfterHoursUri = src.AfterHoursUri;
                AfterHoursSipDomain = src.AfterHoursSipDomain;
                AfterHoursDestination = src.AfterHoursDestination;
                AfterHoursQueue = src.AfterHoursQueue;
                HolidayMessage = src.HolidayMessage;
                HolidayUri = src.HolidayUri;
                HolidaySipDomain = src.HolidaySipDomain;
                HolidayQueue = src.HolidayQueue;
                HolidayDestination = src.HolidayDestination;
                HolidayGroup = src.HolidayGroup;
                EnableWorkflow = src.EnableWorkflow;
                EnableAgentAnonymity = src.EnableAgentAnonymity;
                EnableForFederation = src.EnableForFederation;
                EnableIVRMode = src.EnableIVRMode;
                EnableBusinessHours = src.EnableBusinessHours;
                EnableHolidays = src.EnableHolidays;
                SipAddress = src.SipAddress;
                SipDomain = src.SipDomain;
                WelcomeMessage = src.WelcomeMessage;
                BusinessHourDestination = src.BusinessHourDestination;
                BusinessHoursGroup = src.BusinessHoursGroup;
                Uri = src.Uri;
                Queue = src.Queue;
                IvrOptions = src.IvrOptions;
                OwnerPool = src.OwnerPool;
                IvrMessage = src.IvrMessage;
                AudioWelcome = src.AudioWelcome;
                AudioWelcomeFilePath = src.AudioWelcomeFilePath;
                AudioAfterHours = src.AudioAfterHours;
                AudioAfterHoursFilePath = src.AudioAfterHoursFilePath;
                AudioHoldMusic = src.AudioHoldMusic;
                AudioHoldMusicFilePath = src.AudioHoldMusicFilePath;
                AudioHolidays = src.AudioHolidays;
                AudioHolidaysFilePath = src.AudioHolidaysFilePath;
                AudioIvr = src.AudioIvr;
                AudioIvrFilePath = src.AudioIvrFilePath;
            };
        }

        //Class wide variables
        private string _name;
        private string _number;
        private string _displayNumber;
        private string _description = "Required";
        private string _welcomeMessage;
        private string _language;
        private string _timeZone;
        private string _uri;
        private string _sipDomain;
        private string _sipAddress;

        private bool _enableWorkflow = true;
        private bool _enableForFederation = true;
        private bool _enableAgentAnonymity;
        private bool _enableIVRMode;
        private bool _enableHolidays;
        private bool _enableBusinessHours;

        private IEnumerable<BusinessHourGroupViewModel> _businessHoursGroupsFiltered;
        private IEnumerable<HolidayGroupViewModel> _holidayGroupsFiltered;
        private IEnumerable<QueueViewModel> _queuesFiltered;

        private BusinessHourGroupViewModel _businessHoursGroup;
        private string _businessHourDestination;
        private string _afterHoursDestination;
        private string _holidayDestination;

        private string _afterHoursMessage;
        private string _afterHoursUri;
        private string _afterHoursSipDomain;
        private string _afterHoursQueue;
        private HolidayGroupViewModel _holidayGroup;
        private HolidayGroupViewModel _afterHoursGroup;
        private string _holidayMessage;
        private string _holidayUri;
        private string _holidaySipDomain;
        private string _holidayQueue;
        private QueueViewModel _queue;
        private DesignerViewModel _designerViewModel;
        private string _ownerPool;

        private string _ivrMessage;
        private string _audioWelcome;
        private string _audioWelcomeFilePath;
        private string _audioAfterHours;
        private string _audioAfterHoursFilePath;
        private string _audioHolidays;
        private string _audioHolidaysFilePath;
        private string _audioIvr;
        private string _audioIvrFilePath;
        private string _audioHoldMusic;
        private string _audioHoldMusicFilePath;

        public ICommand AddIvrRootCommand { get; private set; }
        public ObservableCollection<QueueViewModel> Queues { get; set; }
        public ObservableCollection<IvrViewModel> IvrOptions { get; set; }
    }
}


 ////Not reference types 
 //       int? nullableInt = 25; 
 //       double? nullableDouble = 3.14; 
 //       char? nullableChar = 'm'; 
 //       int?[] arrayOfNullableInts = new int?[19]; 
 //       bool? myNulableInt = null; 
 //       DateTime? myNullableDate = null; 
        
 //       // Reference types 
 //       string myString = null; 
 //       class DatabaseReader 
 //       { 
 //           // Nullable data field 
 //           public int? numValue = null; 
 //           public bool? boolValue = true; 
            
 //           // Return nullable 
 //           public int? GetIntFromDatabase()
 //           {
 //               return numericValue;
 //           }

 //           public bool? GetBoolFromDatabase()
 //           {
 //               return boolValue;
 //           } 
 //       }
 //       private bool? createMissing = null;

 //       public bool CreateMissing
 //       {
 //           get { return createMissing.HasValue ? createMissing.Value : false; } 
 //           set { createMissing = value; }
 //       }
        

