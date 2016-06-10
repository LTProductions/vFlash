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
    /// <summary>
    /// ViewModel used for adding FCStackData to the Azure database.
    /// Corresponding View: FCStackAddPage.xaml
    /// </summary>
    public class FCStackAddPageViewModel : BaseAddPageViewModel
    {

        #region Fields/Properties
        

        private string _className;
        /// <summary>
        /// String that will hold the name of the Class that this Stack will live in.
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

        private string _subclassName;
        /// <summary>
        /// String that will hold the name of the Subclass that this Stack will live in.
        /// </summary>
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
        /// <summary>
        /// The name of the Stack.
        /// </summary>
        public string FCStackName
        {
            get { return _fcStackName; }
            set
            {
                if (_fcStackName != value)
                {
                    // Make sure it doesn't contain any unwanted characters.
                    var temp = CheckBoxText(value);
                    _fcStackName = temp;
                    RaisePropertyChanged();

                    // Reset the error whenever text is changed.
                    if (StackNameError != "")
                        StackNameError = "";
                }
            }
        }

        private bool _fcStackCanSave = true;
        /// <summary>
        /// Holds the bool for whether or not the name of the Stack has been saved already.
        /// </summary>
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
        /// <summary>
        /// Holds the error string used when trying to save a Stack and the name has an invalid string.
        /// </summary>
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

        // Flashcard item used to hold the stack's ID.
        private FCStackData fcItemForID;

        #endregion

        #region Methods

        public FCStackAddPageViewModel()
        {
            // Initialize the collection and load the items with placeholder text.
            TextBoxList = new ObservableCollection<TextBoxStrings>();
            LoadInitialTBoxes("Word, Name, etc.", "Definition, Description, etc.");
            MaxBoxes = 50;

            #region Command initializers

            AddTextBoxCommand = new DelegateCommand(AddNewTextBox, CanAddTextBox);
            SaveItemsCommand = new DelegateCommand(async delegate ()
            {
                await SaveItem();
            });

            DeleteTBoxCommand = new DelegateCommand<TextBoxStrings>(DeleteTBox, CanDeleteTextBox);

            #endregion
        }

        /// <summary>
        /// Determines whether or not the Stack name can be saved based on if the text is empty or if the item has already been saved.
        /// </summary>
        /// <returns>bool</returns>
        private bool CanSaveStackName()
        {
            bool canSave = false;


            if (FCStackCanSave && string.IsNullOrWhiteSpace(FCStackName))
            {
                // If the text is empty, can't save and set the error.
                canSave = false;
                StackNameError = "Cannot save empty text.";
            }

            else if (FCStackCanSave && !string.IsNullOrWhiteSpace(FCStackName))
            {
                canSave = true;
            }

            // If the previous else is not true, can't save.
            else
            {
                canSave = false;
            }

            return canSave;
        }

        /// <summary>
        /// Determines whether or not all of the items in TextBoxList can be saved.
        /// </summary>
        /// <returns>bool</returns>
        public override bool CanSave()
        {
            bool canSave = true;

            if (TextBoxList != null)
            {
                // Save each item.
                foreach (var item in TextBoxList)
                {
                    if (string.IsNullOrWhiteSpace(item.BoxText))
                    {
                        // If an item is empty, it can't be saved. Show an error.
                        item.Error = "Cannot save empty text.";
                        canSave = false;
                    }

                    if (string.IsNullOrWhiteSpace(item.Box2Text))
                    {
                        // Stacks use two text boxes; one for Word and one for Definition. If any are empty, can't save. Show error.
                        item.Error2 = "Cannot save empty text.";
                        canSave = false;
                    }
                }
            }

            else
                canSave = false;

            return canSave;
        }


        /// <summary>
        /// Save items to the Azure database.
        /// </summary>
        /// <returns></returns>
        public override async Task SaveItem()
        {
            bool isBusy = false;

            // Create a new FCStackData item to be used for inserting.
            FCStackData StackItem;
            FlashcardData FCItem;

            // Check to see if the StackName can be saved.
            if (CanSaveStackName())
            {
                StackItem = new FCStackData() { Name = FCStackName, Subclass_ID = passedItem.SubclassID };
                if (!isBusy)
                {
                    // Set the ViewModal to busy.
                    Views.Busy.SetBusy(true, "Saving...");
                    isBusy = true;
                }

                // Insert the item.
                await StackItem.InsertItem(StackItem);
                // The StackName is saved, so set CanSave to false.
                FCStackCanSave = false;
                // Save the StackItem to fcItemForID to retain the ID of the item, used for inserting other data.
                fcItemForID = StackItem;

            }

            // Check to make sure all items can be saved.
            if (CanSave())
            {
                // If the BusyModal isn't already active
                if (isBusy != true)
                {
                    Views.Busy.SetBusy(true, "Saving...");
                    isBusy = true;
                }

                // Save each item
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

                // Reset the ObservableCollection.
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

