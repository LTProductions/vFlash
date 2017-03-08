using Template10.Mvvm;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using vFlash.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;

namespace vFlash.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {

        #region Fields/Properties

        private ObservableCollection<StacksNamesView> _fcStacks = new ObservableCollection<StacksNamesView>();
        /// <summary>
        /// List of StacksNamesView (FCSTackData for now) Data; used to hold the 10 most-recently created stacks.
        /// </summary>
        public ObservableCollection<StacksNamesView> FCStacks
        {
            get { return _fcStacks; }
            set
            {
                if (_fcStacks != value)
                {
                    _fcStacks = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _noData;
        /// <summary>
        /// Boolean used to show "no data loaded" if FCStacks is empty.
        /// </summary>
        public bool NoData
        {
            get { return _noData; }
            set
            {
                if (_noData != value)
                {
                    _noData = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _dataLoaded;
        /// <summary>
        /// Boolean used to make the recent stacks visible.
        /// </summary>
        public bool DataLoaded
        {
            get { return _dataLoaded; }
            set
            {
                if (_dataLoaded != value)
                {
                    _dataLoaded = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Commands

        private DelegateCommand<StacksNamesView> _navToQuizCommand;
        public DelegateCommand<StacksNamesView> NavToQuizCommand
        {
            get { return _navToQuizCommand; }
        }

        private DelegateCommand<StacksNamesView> _navToIntVoiceCommand;
        public DelegateCommand<StacksNamesView> NavToIntVoiceCommand
        {
            get { return _navToIntVoiceCommand; }
        }

        private DelegateCommand _navToClassesCommand;
        public DelegateCommand NavToClassesCommand
        {
            get { return _navToClassesCommand; }
        }

        private DelegateCommand _navToQuickAddCommand;
        public DelegateCommand NavToQuickAddCommand
        {
            get { return _navToQuickAddCommand; }
        }

        #endregion

        #region Constructor

        public MainPageViewModel()
        {
            LoadRecentStacks().ConfigureAwait(false);

            #region Command Initializers

            _navToQuizCommand = new DelegateCommand<StacksNamesView>(delegate (StacksNamesView stackItem)
            {
                // QuizPageViewModel needs a NameAndIDs item, so pass that instead of the FCStackData item.
                NamesAndIDs item = new NamesAndIDs();
                item.FCStackID = stackItem.StackID;
                item.FCStackName = stackItem.StackName;
                item.ClassName = stackItem.ClassName;
                item.SubclassName = stackItem.SubclassName;
                this.NavigationService.Navigate(typeof(Views.QuizPage), item);
            });

            _navToIntVoiceCommand = new DelegateCommand<StacksNamesView>(delegate (StacksNamesView stackItem)
            {
                // InteractiveVoiceViewModel needs a NameAndIDs item, so pass that instead of the FCStackData item.
                NamesAndIDs item = new NamesAndIDs();
                item.FCStackID = stackItem.StackID;
                item.FCStackName = stackItem.StackName;
                item.ClassName = stackItem.ClassName;
                item.SubclassName = stackItem.SubclassName;
                this.NavigationService.Navigate(typeof(Views.InteractiveVoicePage), item);
            });

            _navToClassesCommand = new DelegateCommand(delegate ()
            {
                this.NavigationService.Navigate(typeof(Views.ClassPage));
            });

            _navToQuickAddCommand = new DelegateCommand(delegate ()
            {
                this.NavigationService.Navigate(typeof(Views.QuickAddPage));
            });

            #endregion
        }

        #endregion

        #region Methods

        /// <summary>
        /// Task to load the 10 most recently created FCStackData items.
        /// </summary>
        /// <returns></returns>
        private async Task LoadRecentStacks()
        {
            // Load the 10 most recently created Flashcard Stacks.
            var SNV = new StacksNamesView();
            var query = App.MobileService.GetTable<StacksNamesView>().CreateQuery();
            
            var list = await SNV.GetQueriedList(query.OrderBy(p => p.CreatedAt).Take(10));
            FCStacks = new ObservableCollection<StacksNamesView>(list);

            //// Load the 10 most recently created Flashcard Stacks.
            //var fcsd = new FCStackData();
            //var query = App.MobileService.GetTable<FCStackData>().CreateQuery();
            //var list = await fcsd.GetQueriedList<FCStackData>(query.OrderBy(p => p.CreatedAt).Take(10));
            //FCStacks = new ObservableCollection<FCStackData>(list);



            if (FCStacks.Count() == 0)
            {
                NoData = true;
            }
            else
            {
                DataLoaded = true;
            }

            try
            {
                var userInfo = await App.MobileService.InvokeApiAsync(
        "userInfo", HttpMethod.Get, null);
                Debug.WriteLine(userInfo.ToString() + "HELLO");
            }
            catch (Exception e)
            {

                Debug.WriteLine(e.ToString());
            }
        }

        #endregion


        #region Navigation

        string _Value = "";
        public string Value { get { return _Value; } set { Set(ref _Value, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            if (suspensionState.Any())
            {
                Value = suspensionState[nameof(Value)]?.ToString();
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                suspensionState[nameof(Value)] = Value;
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }


        #endregion
    }
}

