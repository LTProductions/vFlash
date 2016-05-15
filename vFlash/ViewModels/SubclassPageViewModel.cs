using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using vFlash.Models;
using Windows.UI.Xaml.Navigation;

namespace vFlash.ViewModels
{
    class SubclassPageViewModel : ViewModelBase
    {

        #region Properties and Fields

        private ObservableCollection<SubclassData> _subclassList;
        public ObservableCollection<SubclassData> SubclassList
        {
            get { return _subclassList; }
            set
            {
                if (_subclassList != value)
                {
                    _subclassList = value;
                    RaisePropertyChanged();
                }
            }
        }

        private ClassData _selectedItem;
        public ClassData SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    RaisePropertyChanged();
                    // Navigate.
                    // this.NavigationService.Navigate(typeof(Views.FCStackPage), SelectedItem);
                }
            }
        }

        private ClassData passedItem;

        #endregion

        #region Constructor

        public SubclassPageViewModel()
        {
            
        }

        #endregion

        #region Methods

        public async void LoadData()
        {
            #region GetList...not needed
            //var scd = new SubclassData();
            //var list = await scd.GetList<SubclassData>();
            //var queriedList = list.Where(p => p.Class_ID == passedItem.Id);
            //SubclassList = new ObservableCollection<SubclassData>(queriedList);
            #endregion

            // Create a SubclassData item and a query to get only the relevant data.
            var scd = new SubclassData();
            var query = from SubclassData in
                            App.MobileService.GetTable<SubclassData>()
                        where SubclassData.Class_ID == passedItem.Id
                        select new SubclassData();

            var list = await scd.GetEnum<SubclassData>(query);
            SubclassList = new ObservableCollection<SubclassData>(list);
        }

        #endregion

        #region Navigation

        private string _Value = "Default";
        public string Value { get { return _Value; } set { Set(ref _Value, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            passedItem = (ClassData)parameter;
            LoadData();
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
