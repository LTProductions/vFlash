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
    public class FCStackAddPageViewModel : BaseAddPageViewModel
    {

        #region Fields/Properties

        

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

        private string _fcStackName;
        public string FCStackName
        {
            get { return _fcStackName; }
            set
            {
                if (_fcStackName != value)
                {
                    var temp = CheckBoxText(value);
                    _fcStackName = temp;
                    RaisePropertyChanged();

                    if (StackNameError != "")
                        StackNameError = "";
                }
            }
        }

        private bool _fcStackCanSave = true;
        public bool FCStackCanSave
        {
            get { return _fcStackCanSave; }
            set
            {
                if (_fcStackCanSave != value)
                {
                    _fcStackCanSave = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _stackNameError;
        public string StackNameError
        {
            get { return _stackNameError; }
            set
            {
                if (_stackNameError != value)
                {
                    _stackNameError = value;
                    RaisePropertyChanged();
                }
            }
        }

        private FCStackData fcItemForID;

        #endregion

        #region Methods

        public FCStackAddPageViewModel()
        {

            TextBoxList = new ObservableCollection<TextBoxStrings>();
            LoadInitialTBoxes("Word, Name, etc.", "Definition, Description, etc.");
            MaxBoxes = 50;

            AddTextBoxCommand = new DelegateCommand(AddNewTextBox, CanAddTextBox);
            SaveItemsCommand = new DelegateCommand(async delegate ()
            {
                await SaveItem();
            });

            DeleteTBoxCommand = new DelegateCommand<TextBoxStrings>(DeleteTBox, CanDeleteTextBox);
        }

        public bool CanSaveStackName()
        {
            bool canSave = false;

            if (FCStackCanSave && string.IsNullOrWhiteSpace(FCStackName))
            {
                canSave = false;
                StackNameError = "Cannot save empty text.";
            }

            else if (FCStackCanSave && !string.IsNullOrWhiteSpace(FCStackName))
            {
                canSave = true;
            }

            else
            {
                canSave = false;
            }

            return canSave;
        }

        public override bool CanSave()
        {
            bool canSave = true;

            if (TextBoxList != null)
            {
                foreach (var item in TextBoxList)
                {
                    if (string.IsNullOrWhiteSpace(item.BoxText))
                    {
                        item.Error = "Cannot save empty text.";
                        canSave = false;
                    }

                    if (string.IsNullOrWhiteSpace(item.Box2Text))
                    {
                        item.Error2 = "Cannot save empty text.";
                        canSave = false;
                    }
                }
            }

            else
                canSave = false;

            return canSave;
        }

        public override async Task SaveItem()
        {
            bool isBusy = false;

            // Create a new FCStackData item to be used for inserting.
            FCStackData StackItem;
            FlashcardData FCItem;

            if (CanSaveStackName())
            {
                StackItem = new FCStackData() { Name = FCStackName, Subclass_ID = passedItem.SubclassID };
                if (!isBusy)
                {
                    Views.Busy.SetBusy(true, "Saving...");
                    isBusy = true;
                }

                await StackItem.InsertItem(StackItem);
                FCStackCanSave = false;
                fcItemForID = StackItem;

            }

            if (CanSave())
            {
                if (isBusy != true)
                {
                    Views.Busy.SetBusy(true, "Saving...");
                    isBusy = true;
                }

                foreach (var item in TextBoxList)
                {

                    try
                    {
                        // Set the properties of the FCStackData item.
                        FCItem = new FlashcardData() { FCStack_ID = fcItemForID.Id, Word_Side1 = item.BoxText, Definition_Side2 = item.Box2Text };

                        await FCItem.InsertItem(FCItem);
                        FCStackCanSave = false;
                    }

                    catch (Exception e)
                    {
                        break;
                    }
                }

                TextBoxList = new ObservableCollection<TextBoxStrings>();
                LoadInitialTBoxes("Word, Name, etc.", "Definition, Description, etc.");
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
            SubclassName = passedItem.SubclassName;
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

