using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using vFlash.Models;
using Windows.UI.Xaml.Navigation;

namespace vFlash.ViewModels
{
    /// <summary>
    /// ViewModel that adds SubclassData to the Azure database.
    /// Corresponding View: SubclassAddPage.xaml
    /// </summary>
    public class SubclassAddPageViewModel : BaseAddPageViewModel
    {

        private string _className;
        /// <summary>
        /// Holds the name of the ClassData that the added Subclasses will live in.
        /// </summary>
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

        #region Methods

        public SubclassAddPageViewModel()
        {
            // Load the initial data.
            TextBoxList = new ObservableCollection<TextBoxStrings>();
            LoadInitialTBox("Chapter 1, Chapter 2, etc.");
            MaxBoxes = 7;

            #region Command Initializers

            AddTextBoxCommand = new DelegateCommand(AddNewTextBox, CanAddTextBox);
            SaveItemsCommand = new DelegateCommand(async delegate ()
            {
                await SaveItem();
            });

            DeleteTBoxCommand = new DelegateCommand<TextBoxStrings>(DeleteTBox, CanDeleteTextBox);

            #endregion
        }

        /// <summary>
        /// Save the items into the Database.
        /// </summary>
        /// <returns></returns>
        public override async Task SaveItem()
        {
            bool isBusy = false;

            // Create a new ClassData item to be used for inserting.
            SubclassData Item;

            // Make sure saving is possible.
            if (CanSave())
            {
                // If the BusyModal isn't active...
                if (isBusy != true)
                {
                    Views.Busy.SetBusy(true, "Saving...");
                    isBusy = true;
                }

                // Save each item into the DB.
                foreach (var item in TextBoxList)
                {
                    try
                    {
                        // Set the properties of the classitem.
                        Item = new SubclassData() { Name = item.BoxText, Class_ID = passedItem.ClassID };

                        await Item.InsertItem(Item);
                    }

                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                        break;
                    }
                }

                // Reset/reload the data.
                TextBoxList = new ObservableCollection<TextBoxStrings>();
                LoadInitialTBox("Chapter 1, Chapter 2, etc.");
                AddTextBoxCommand.RaiseCanExecuteChanged();
            }

            if (isBusy)
                Views.Busy.SetBusy(false);
        }


        #endregion


        #region Navigation

        private string _Value = "Default";
        public string Value { get { return _Value; } set { Set(ref _Value, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            passedItem = (NamesAndIDs)parameter;
            ClassName = passedItem.ClassName;
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

