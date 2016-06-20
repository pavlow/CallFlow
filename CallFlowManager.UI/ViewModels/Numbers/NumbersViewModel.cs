using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CallFlowManager.UI.Business;
using CallFlowManager.UI.Common;
using CallFlowManager.UI.ViewModels.BusinessHours;
using CallFlowManager.UI.Models;
using CallFlowManager.UI.ViewModels.Queues;
using CallFlowManager.UI.ViewModels.Users;
using CallFlowManager.UI.ViewModels.WorkFlows;
using Logging;
using System.Threading.Tasks;

namespace CallFlowManager.UI.ViewModels.Numbers
{
    public class NumbersViewModel : PropertyChangedBase
    {
        public NumbersViewModel()
        {
            Logger.Info("Started loading numbers");

            LoadCommand = new RelayCommand(_ => Load());
            RemoveSearchCommand = new RelayCommand(_ => RemoveSearch());

            _loadedNumbersInventory = new List<Number>();
            Numbers = new ObservableCollection<Number>();

            //Auto-complete: Init of NumbersFiltered and TestsFiltered
            NumbersFiltered = new ObservableCollection<Number>();
            UsersFiltered = new ObservableCollection<UserViewModel>();
            QueuesFiltered = new ObservableCollection<QueueViewModel>();
            WfFiltered = new ObservableCollection<WorkFlowViewModel>();
            //TestsFiltered = new ObservableCollection<TestItem>();
        }

        /// <summary>
        /// Value to display on the status bar
        /// </summary>
        public string StatusBar
        {
            get { return _statusBar; }
            set
            {
                if (_statusBar != value)
                {
                    _statusBar = value;
                    OnPropertyChanged("StatusBar");
                }
            }
        }

        /// <summary>
        /// True when the application is processing data
        /// </summary>
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;

                    if (value)
                        IsEnabled = false;
                    if (!value)
                        IsEnabled = true;

                    OnPropertyChanged("IsLoading");
                }
            }
        }

        /// <summary>
        /// Binding to tell WPF controls whether they should be disabled or not
        /// </summary>
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged("IsEnabled");
                }
            }
        }

        /// <summary>
        /// Runs the load task when the user selects the "Refresh" button. The load task will query SfB data.
        /// </summary>
        /// <returns></returns>
        public async Task Load()
        {
            await Task.Run(() =>
            {
                ////This used to live in the LoadBusinessHours method on its own
                IsLoading = true;
                StatusBar = "Loading numbers, please wait...";

                _loadedNumbersInventory.Clear();
                _dataService.GetNumberInventory(true, true);
                _loadedNumbersInventory = _dataService.NumbersData;
            });

            ////This used to live in the ChangeStatus method which was executed after the background worker completed
            ////Cannot run this within above thread!
            Numbers.Clear();
            foreach (var numbers in _loadedNumbersInventory)
            {
                Number flow = numbers;
                Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    Numbers.Add(flow);

                }));
            }

            StatusBar = "";
            IsLoading = false;
        }



        #region AUTO-COMPLETE METHODS
        /// <summary>
        /// Auto-complete: Tracks the state of the search results pop-up
        /// </summary>
        public bool PopupOpen
        {
            get
            {
                return _popupOpen;
            }

            set
            {
                if (_popupOpen != value)
                {
                    _popupOpen = value;
                    OnPropertyChanged("PopupOpen");
                }
            }
        }

        /// <summary>
        /// Auto-complete: Holds the search query
        /// </summary>
        public string SearchQuery
        {
            get { return _searchQuery; }
            set
            {
                if (_searchQuery != value)
                {
                    _searchQuery = value;

                    OnChangeSearchQuery();
                    OnPropertyChanged("SearchQuery");
                }
            }
        }

        /// <summary>
        /// Auto-complete: Holds the value of the selected search result
        /// </summary>
        public object AutoCompleteSelection
        {
            get { return _autoCompleteSelection; }
            set
            {
                if (_autoCompleteSelection != value)
                {
                    _autoCompleteSelection = value;
                    OnPropertyChanged("AutoCompleteSelection");

                    PopupOpen = false;

                    //Holds selected item
                    var number = _autoCompleteSelection as Number;
                    var user = _autoCompleteSelection as UserViewModel;
                    var queue = _autoCompleteSelection as QueueViewModel;
                    var wf = _autoCompleteSelection as WorkFlowViewModel;
                    //var test = _autoCompleteSelection as TestItem;

                    if (number != null)
                    {
                        //_searchQuery = String.Concat(number.AssignedTo, " \t DDI:  \t", number.DDI);
                        _searchQuery = String.Concat(number.DDI);
                    }
                    //else
                    //{
                    if (queue != null)
                    {
                        //_searchQuery = String.Concat(user.DisplayName, "\t Sip Address: \t", user.SipAddress);
                        _searchQuery = String.Concat(queue.Name);
                    }
                    if (wf != null)
                    {
                        //_searchQuery = String.Concat(user.DisplayName, "\t Sip Address: \t", user.SipAddress);
                        _searchQuery = String.Concat(wf.SipAddress);
                    }
                    if (user != null)
                    {
                        //_searchQuery = String.Concat(user.DisplayName, "\t Sip Address: \t", user.SipAddress);
                        _searchQuery = String.Concat(user.SipAddress);
                    }
                        //if (test != null)
                        //{
                        //    _searchQuery = String.Concat(test.TestName, "\t Description: \t", test.TestDescription);
                        //}
                    //}

                    OnPropertyChanged("SearchQuery");
                }
            }
        }

        /// <summary>
        /// Auto-complete: Handles a change in auto-complete search query
        /// </summary>
        private void OnChangeSearchQuery()
        {
            //If search query is empty return
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                PopupOpen = false;
                return;
            }

            //Search will be performed in upper case
            var searchUpperQuery = SearchQuery.ToUpper();
            
            //Search Source - Numbers
            NumbersFiltered.Clear();
            foreach (var number in Numbers.Where(number => number.AssignedTo.ToUpper().Contains(searchUpperQuery) ||
                number.DDI.ToUpper().Contains(searchUpperQuery) ||
                number.Ext.ToUpper().Contains(searchUpperQuery)))
            {
                NumbersFiltered.Add(number);
            }

            var Users = _dataService.GetUsers(false, false); //THIS IS VERY INEFFICIENT I THINK - Prob delegate to return value once loaded by users tab?
            //Search Source - Users
            UsersFiltered.Clear();
            foreach (var item in Users.Where(item => item.DisplayName.ToUpper().Contains(searchUpperQuery) ||
                item.SipAddress.ToUpper().Contains(searchUpperQuery)))
            {
                UsersFiltered.Add(item);
            }

            var Queues = _dataService.GetQueues(false, false); //THIS IS VERY INEFFICIENT I THINK - Prob delegate to return value once loaded by users tab?
            //Search Source - Users
            QueuesFiltered.Clear();
            foreach (var item in Queues.Where(item => item.Name.ToUpper().Contains(searchUpperQuery) ||
                item.Description.ToUpper().Contains(searchUpperQuery)))
            {
                QueuesFiltered.Add(item);
            }

            var Wf = _dataService.GetWorkflows(); //THIS IS VERY INEFFICIENT I THINK - Prob delegate to return value once loaded by users tab?
            //Search Source - Users
            WfFiltered.Clear();
            foreach (var item in Wf.Where(item => item.Name.ToUpper().Contains(searchUpperQuery) ||
                item.Uri.ToUpper().Contains(searchUpperQuery) || item.SipDomain.ToUpper().Contains(searchUpperQuery)))
            {
                WfFiltered.Add(item);
            }


            //Search Source - TestCollection
            //TestsFiltered.Clear();
            //foreach (var test in TestCollection.Where(test => test.TestId.ToUpper().Contains(searchUpperQuery) ||
            //    test.TestDescription.ToUpper().Contains(searchUpperQuery) ||
            //    test.TestName.ToUpper().Contains(searchUpperQuery)))
            //{
            //    TestsFiltered.Add(test);
            //}

            if (!PopupOpen)
            {
                PopupOpen = true;
            }
        }

        /// <summary>
        /// Auto-complete: Clears the current search query
        /// </summary>
        private void RemoveSearch()
        {
            _searchQuery = String.Empty;
            OnChangeSearchQuery();
            OnPropertyChanged("SearchQuery");
        }
        #endregion AUTO-COMPLETE METHODS^

        //CLASS WIDE
        private readonly IDataService _dataService = DataServiceFactory.Get();
        private IList<Number> _loadedNumbersInventory;
        private bool _isLoading;
        private bool _isEnabled;
        private string _statusBar;
        private static readonly ILogger Logger = LoggerFactory.Instance.GetCurrentClassLogger();
        public ICommand LoadCommand { get; private set; }
        public ObservableCollection<Number> Numbers { get; private set; }

        //Auto-compete
        private bool _popupOpen;
        private string _searchQuery;
        private object _autoCompleteSelection;
        public ICommand RemoveSearchCommand { get; private set; }
        public ObservableCollection<Number> NumbersFiltered { get; set; }
        public ObservableCollection<UserViewModel> UsersFiltered { get; set; }
        public ObservableCollection<QueueViewModel> QueuesFiltered { get; set; }
        public ObservableCollection<WorkFlowViewModel> WfFiltered { get; set; }
    }

    


}
