using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using vFlash.Models;
using Windows.UI.Xaml.Navigation;
using Prism.Commands;

namespace vFlash.ViewModels
{
    /// <summary>
    /// ViewModel used for viewing SubclassData from the Azure database.
    /// Corresponding View: SubclassPage.xaml
    /// </summary>
    class SubclassPageViewModel : BaseDataPage
    {

        #region Properties and Fields

        private ObservableCollection<SubclassData> _subclassList;
        /// <summary>
        /// Holds the list of SubclassData items.
        /// </summary>
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
        /// <summary>
        /// Holds the SubclassData item selected by the user.
        /// </summary>
        public SubclassData SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    RaisePropertyChanged();
                    // Set the values of the passedItem.
                    passedItem.SubclassID = value.Id;
                    passedItem.SubclassName = value.Name;
                    // Navigate and pass the item to the next ViewModel with the updated data.
                    this.NavigationService.Navigate(typeof(Views.FCStackPage), passedItem);
                }
            }
        }

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
            #region Command Initializers

            _addSubclassNavCommand = new DelegateCommand(delegate ()
            {
                // Navigate and pass passedItem.
                this.NavigationService.Navigate(typeof(Views.SubclassAddPage), passedItem);
            });

            DeleteItemCommand = new DelegateCommand<object>(DeleteItem);

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
            // Set the ClassName these Subclasses live in.
            ClassName = passedItem.ClassName;

            // Load the data based on the ClassID.
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
