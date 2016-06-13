using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using vFlash.Models;
using Windows.UI.Xaml.Navigation;

namespace vFlash.ViewModels
{
    /// <summary>
    /// ViewModel used for viewing FCStackData from the Azure database.
    /// Corresponding View: FCStackPage.xaml
    /// </summary>
    public class FCStackPageViewModel : BaseDataPage
    {
        #region Properties and Fields

        private ObservableCollection<FCStackData> _stackList;
        /// <summary>
        /// List of FCStackData items.
        /// </summary>
        public ObservableCollection<FCStackData> StackList
        {
            get { return _stackList; }
            set
            {
                if (_stackList != value)
                {
                    _stackList = value;
                    RaisePropertyChanged();
                }
            }
        }

        //private FCStackData _selectedItem;
        //public FCStackData SelectedItem
        //{
        //    get { return _selectedItem; }
        //    set
        //    {
        //        if (_selectedItem != value)
        //        {
        //            _selectedItem = value;
        //            RaisePropertyChanged();
        //            // Navigate.
        //            this.NavigationService.Navigate(typeof(Views.SubclassPage), SelectedItem);
        //        }
        //    }
        //

        #endregion

        #region Commands

        private DelegateCommand _addFCStackCommand;
        public DelegateCommand AddFCStackCommand
        {
            get { return _addFCStackCommand; }
        }

        private DelegateCommand<FCStackData> _navQuizCommand;
        public DelegateCommand<FCStackData> NavQuizCommand
        {
            get { return _navQuizCommand; }
        }

        private DelegateCommand<FCStackData> _navInteractiveVoiceCommand;
        public DelegateCommand<FCStackData> NavInteractiveVoiceCommand
        {
            get { return _navInteractiveVoiceCommand; }
        }

        #endregion

        #region Constructor

        public FCStackPageViewModel()
        {

            #region Command Initializers

            _addFCStackCommand = new DelegateCommand(delegate ()
            {
                // Navigate and pass an item of type NamesAndIDs.
                this.NavigationService.Navigate(typeof(Views.FCStackAddPage), passedItem);
            });

            _navQuizCommand = new DelegateCommand<FCStackData>(delegate (FCStackData item)
            {
                // Navigate to a quiz of the selected FlashCardStack item.
                passedItem.FCStackID = item.Id;
                this.NavigationService.Navigate(typeof(Views.QuizPage), passedItem);
            });

            _navInteractiveVoiceCommand = new DelegateCommand<FCStackData>(delegate (FCStackData item)
            {
                // Navigate to an Interactive Voice session of the selected FlashCardStack item.
                passedItem.FCStackName = item.Name;
                passedItem.FCStackID = item.Id;
                this.NavigationService.Navigate(typeof(Views.InteractiveVoicePage), passedItem);
            });

            #endregion
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load the data from the Azure database.
        /// </summary>
        /// <returns></returns>
        private async Task LoadData()
        {
            // Set the (sub)class name based on the passed item from the previous ViewModel.
            ClassName = passedItem.ClassName;
            SubclassName = passedItem.SubclassName;
            // Load the correct data based on SubclassID.
            var fcsd = new FCStackData();
            var query = App.MobileService.GetTable<FCStackData>().CreateQuery();
            var list = await fcsd.GetQueriedList<FCStackData>(query.Where(p => p.Subclass_ID == passedItem.SubclassID));
            StackList = new ObservableCollection<FCStackData>(list);
        }

        #endregion

        #region Navigation

        private string _Value = "Default";
        public string Value { get { return _Value; } set { Set(ref _Value, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            passedItem = (NamesAndIDs)parameter;
            await LoadData();
            Value = (suspensionState.ContainsKey(nameof(Value))) ? suspensionState[nameof(Value)]?.ToString() : parameter?.ToString();
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
