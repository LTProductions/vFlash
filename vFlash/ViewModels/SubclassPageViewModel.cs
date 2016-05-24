using Microsoft.WindowsAzure.MobileServices;
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

        private SubclassData _selectedItem;
        public SubclassData SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    RaisePropertyChanged();
                    // Navigate.
                    passedItem.SubclassID = value.Id;
                    passedItem.SubclassName = value.Name;
                    this.NavigationService.Navigate(typeof(Views.FCStackPage), passedItem);
                }
            }
        }

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

        // Item passed when navigating from ClassPage.
        private NamesAndIDs passedItem;

        #endregion

        #region Commands

        private DelegateCommand _addSubclassNavCommand;
        public DelegateCommand AddSubclassNavCommand
        {
            get { return _addSubclassNavCommand; }
        }

        #endregion

        #region Constructor

        public SubclassPageViewModel()
        {
            _addSubclassNavCommand = new DelegateCommand(delegate ()
            {
                this.NavigationService.Navigate(typeof(Views.SubclassAddPage), passedItem);
            });
        }

        #endregion

        #region Methods

        public async Task LoadData()
        {
            ClassName = passedItem.ClassName;

            //var scd = new SubclassData();
            //var list = await scd.GetList<SubclassData>();
            //var queriedList = list.Where(p => p.Class_ID == passedItem.Id);
            //SubclassList = new ObservableCollection<SubclassData>(queriedList);

            var scd = new SubclassData();
            var query = App.MobileService.GetTable<SubclassData>().CreateQuery();
            var list = await scd.GetQueriedList<SubclassData>(query.Where(p => p.Class_ID == passedItem.ClassID));
            SubclassList = new ObservableCollection<SubclassData>(list);
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
