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
    /// <summary>
    /// ViewModel used for loading ClassData from the Azure database.
    /// Corresponding View: ClassPage.xaml
    /// </summary>
    public class ClassPageViewModel : ViewModelBase
    {

        #region Properties and Fields

        private ObservableCollection<ClassData> _classList;
        /// <summary>
        /// List of ClassData.
        /// </summary>
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
        /// <summary>
        /// Item selected by the user.
        /// </summary>
        public ClassData SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    RaisePropertyChanged();
                    // Store data that needs to be passed to the next ViewModel.
                    NamesAndIDs item = new NamesAndIDs { ClassName = SelectedItem.Name, ClassID = SelectedItem.Id };
                    // Navigate.
                    this.NavigationService.Navigate(typeof(Views.SubclassPage), item);
                }
            }
        }

        #endregion

        #region Commands

        private DelegateCommand _addClassNavCommand;
        /// <summary>
        /// Command used for adding navigating to the View ClassAddPage.xaml
        /// </summary>
        public DelegateCommand AddClassNavCommand
        {
            get { return _addClassNavCommand; }
        }

        #endregion

        #region Constructor

        public ClassPageViewModel()
        {
            // Load the data. ConfigureAwait since we can't await from the constructor.
            LoadData().ConfigureAwait(false);

            #region Command Initializers

            _addClassNavCommand = new DelegateCommand(delegate ()
            {
                // Navigate.
                this.NavigationService.Navigate(typeof(Views.ClassAddPage));

            });

            #endregion
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load the initial data.
        /// </summary>
        /// <returns></returns>
        private async Task LoadData()
        {
            var cd = new ClassData();
            ClassList = new ObservableCollection<ClassData>(await cd.GetList<ClassData>());
        }

        #endregion

    }
}
