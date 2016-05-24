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
    public class SubclassAddPageViewModel : BaseAddPageViewModel
    {

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

        #region Methods

        public SubclassAddPageViewModel()
        {

            TextBoxList = new ObservableCollection<TextBoxStrings>();
            LoadInitialTBox("Chapter 1, Chapter 2, etc.");
            MaxBoxes = 7;

            AddTextBoxCommand = new DelegateCommand(AddNewTextBox, CanAddTextBox);
            SaveItemsCommand = new DelegateCommand(async delegate ()
            {
                Views.Busy.SetBusy(true, "Saving...");
                await SaveItem();
            });

            DeleteTBoxCommand = new DelegateCommand<TextBoxStrings>(DeleteTBox, CanDeleteTextBox);
        }

        public override async Task SaveItem()
        {
            bool isBusy = false;

            // Create a new ClassData item to be used for inserting.
            SubclassData Item;

            foreach (var item in TextBoxList)
            {
                if (!string.IsNullOrWhiteSpace(item.BoxText))
                {
                    if (isBusy != true)
                    {
                        Views.Busy.SetBusy(true, "Saving...");
                    }

                    try
                    {
                        // Set the properties of the classitem.
                        Item = new SubclassData() { Name = item.BoxText, Class_ID = passedItem.ClassID};

                        await Item.InsertItem(Item);
                        item.BoxText = string.Empty;
                    }

                    catch (Exception e)
                    {
                        break;
                    }
                }

                else
                {
                    item.Error = "Item not saved: Can't save empty text box.";
                }
            }

            TextBoxList = new ObservableCollection<TextBoxStrings>();
            LoadInitialTBox("Chapter 1, Chapter 2, etc.");
            AddTextBoxCommand.RaiseCanExecuteChanged();


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

