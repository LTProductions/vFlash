using Template10.Mvvm;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using vFlash.Models;

namespace vFlash.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {

        #region Fields/Properties

        private ObservableCollection<FCStackData> _fcStacks;
        /// <summary>
        /// List of FCStackData; used to hold the 10 most-recently created stacks.
        /// </summary>
        public ObservableCollection<FCStackData> FCStacks
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

        #endregion

        #region Commands

        private DelegateCommand<FCStackData> _navToQuizCommand;
        public DelegateCommand<FCStackData> NavToQuizCommand
        {
            get { return _navToQuizCommand; }
        }

        private DelegateCommand<FCStackData> _navToIntVoiceCommand;
        public DelegateCommand<FCStackData> NavToIntVoiceCommand
        {
            get { return _navToIntVoiceCommand; }
        }

        #endregion

        #region Constructor

        public MainPageViewModel()
        {
            LoadRecentStacks().ConfigureAwait(false);

            #region Command Initializers

            _navToQuizCommand = new DelegateCommand<FCStackData>(delegate (FCStackData stackItem)
            {
                // QuizPageViewModel needs a NameAndIDs item, so pass that instead of the FCStackData item.
                NamesAndIDs item = new NamesAndIDs();
                item.FCStackID = stackItem.Id;
                item.FCStackName = stackItem.Name;
                this.NavigationService.Navigate(typeof(Views.QuizPage), item);
            });

            _navToIntVoiceCommand = new DelegateCommand<FCStackData>(delegate (FCStackData stackItem)
            {
                // InteractiveVoiceViewModel needs a NameAndIDs item, so pass that instead of the FCStackData item.
                NamesAndIDs item = new NamesAndIDs();
                item.FCStackID = stackItem.Id;
                item.FCStackID = stackItem.Name;
                this.NavigationService.Navigate(typeof(Views.InteractiveVoicePage), item);
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
            var fcsd = new FCStackData();
            var query = App.MobileService.GetTable<FCStackData>().CreateQuery();
            var list = await fcsd.GetQueriedList<FCStackData>(query.OrderBy(p => p.CreatedAt).Take(10));
            FCStacks = new ObservableCollection<FCStackData>(list);

            if (FCStacks.Count() == 0)
                NoData = true;
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

