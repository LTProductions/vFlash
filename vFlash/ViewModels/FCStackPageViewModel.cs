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
    public class FCStackPageViewModel : Template10.Mvvm.ViewModelBase
    {
        #region Properties and Fields

        private ObservableCollection<FCStackData> _stackList;
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
        //}

        // Item passed when navigating from SubclassPage.
        private NamesAndIDs passedItem;

        private string _className;
        public string ClassName
        {
            get { return _className; }
            set
            {
                if (_className != value)
                {
                    _className = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _subclassName;
        public string SubclassName
        {
            get { return _subclassName; }
            set
            {
                if (_subclassName != value)
                {
                    _subclassName = value;
                    RaisePropertyChanged();
                }
            }
        }

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

        #endregion

        #region Constructor

        public FCStackPageViewModel()
        {
            _addFCStackCommand = new DelegateCommand(delegate ()
            {
                this.NavigationService.Navigate(typeof(Views.FCStackAddPage), passedItem);
            });

            _navQuizCommand = new DelegateCommand<FCStackData>(delegate (FCStackData item)
            {
                passedItem.FCStackID = item.Id;
                this.NavigationService.Navigate(typeof(Views.QuizPage), passedItem);
            });
        }

        #endregion

        #region Methods

        public async Task LoadData()
        {
            ClassName = passedItem.ClassName;
            SubclassName = passedItem.SubclassName;
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
