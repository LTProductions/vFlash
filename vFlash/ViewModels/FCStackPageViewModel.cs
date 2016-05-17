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
        private SubclassData passedItem;

        #endregion

        #region Constructor

        public FCStackPageViewModel()
        {
            LoadData().ConfigureAwait(false);
        }

        #endregion

        #region Methods

        public async Task LoadData()
        {
            var fcsd = new FCStackData();
            var query = App.MobileService.GetTable<FCStackData>().CreateQuery();
            var list = await fcsd.GetQueriedList<FCStackData>(query.Where(p => p.Subclass_ID == passedItem.Id));
            StackList = new ObservableCollection<FCStackData>(list);
        }

        #endregion

        #region Navigation

        private string _Value = "Default";
        public string Value { get { return _Value; } set { Set(ref _Value, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            passedItem = (SubclassData)parameter;
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
