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
using vFlash.Views;
using Windows.UI.Xaml.Navigation;

namespace vFlash.ViewModels
{
    public class ClassPageViewModel : ViewModelBase
    {

        #region Properties and Fields

        private ObservableCollection<ClassData> _classList;
        public ObservableCollection<ClassData> ClassList
        {
            get { return _classList; }
            set
            {
                if (_classList != value)
                {
                    _classList = value;
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
                    NamesAndIDs item = new NamesAndIDs { ClassName = SelectedItem.Name, ClassID = SelectedItem.Id };
                    this.NavigationService.Navigate(typeof(Views.SubclassPage), item);
                }
            }
        }

        #endregion

        #region Commands

        private DelegateCommand _addClassNavCommand;
        public DelegateCommand AddClassNavCommand
        {
            get { return _addClassNavCommand; }
        }

        #endregion

        #region Constructor

        public ClassPageViewModel()
        {
            LoadData().ConfigureAwait(false);

            #region Command Initializers

            _addClassNavCommand = new DelegateCommand(delegate ()
            {
                this.NavigationService.Navigate(typeof(Views.ClassAddPage));

            });

            #endregion
        }

        #endregion

        #region Methods

        public async Task LoadData()
        {
            var cd = new ClassData();
            ClassList = new ObservableCollection<ClassData>(await cd.GetList<ClassData>());
        }

        #endregion

        #region Navigation

        private string _Value = "Default";
        public string Value { get { return _Value; } set { Set(ref _Value, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
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
