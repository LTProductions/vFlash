using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using vFlash.Models;
using vFlash.Utils;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace vFlash.ViewModels
{
    /// <summary>
    /// Base page used for adding data to the database. Several ViewModels inherit from this class because they share a lot of functionality.
    /// </summary>
    public abstract class BaseAddPageViewModel : Template10.Mvvm.ViewModelBase
    {

        #region Fields/Properties

        /// <summary>
        /// Holds the passed item from previous ViewModels.
        /// </summary>
        public NamesAndIDs passedItem;

        private ObservableCollection<TextBoxStrings> _textBoxList;
        /// <summary>
        /// Collection of TextBoxStrings used for holding the data that will be inserted into the DB.
        /// </summary>
        public ObservableCollection<TextBoxStrings> TextBoxList
        {
            get { return _textBoxList; }
            set
            {
                if (_textBoxList != value)
                {
                    _textBoxList = value;
                    RaisePropertyChanged();
                }
            }
        }

        // Most pages use 7 as the max, so set that here.
        private int _maxBoxes = 7;
        /// <summary>
        /// Maximum number of TextBoxes a user can load.
        /// </summary>
        public int MaxBoxes
        {
            get { return _maxBoxes; }
            set { _maxBoxes = value; }
        }

        private int _caretPosition;
        /// <summary>
        /// Holds the caret position; this is necessary because, when updating textbox text to remove unwanted characters via code, the caret goes back to the beginning
        /// of the textbox.
        /// </summary>
        public int CaretPosition
        {
            get { return _caretPosition; }
            set
            {
                if (_caretPosition != value)
                {
                    _caretPosition = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Commands

        private DelegateCommand _addTextBoxCommand;
        /// <summary>
        /// Command for adding a TextBoxString item to TextBoxList.
        /// </summary>
        public DelegateCommand AddTextBoxCommand
        {
            set { _addTextBoxCommand = value; }
            get { return _addTextBoxCommand; }
        }

        private DelegateCommand _saveItemsCommand;
        /// <summary>
        /// Command for saving items to the Azure Database.
        /// </summary>
        public DelegateCommand SaveItemsCommand
        {
            set { _saveItemsCommand = value; }
            get { return _saveItemsCommand; }
        }

        private DelegateCommand<TextBoxStrings> _deleteTBoxCommand;
        /// <summary>
        /// Command for deleting a TextBoxString item from the observable collection TextBoxList.
        /// </summary>
        public DelegateCommand<TextBoxStrings> DeleteTBoxCommand
        {
            set { _deleteTBoxCommand = value; }
            get { return _deleteTBoxCommand; }
        }

        private DelegateCommand<object> _textChangedCommand;
        /// <summary>
        /// Command raised whewever text from a TextBoxString item inside of TextBoxList changes.
        /// Uses object rather than TextBoxString due to command parameter errors.
        /// </summary>
        public DelegateCommand<object> TextChangedCommand
        {
            set { _textChangedCommand = value; }
            get { return _textChangedCommand; }
        }

        private DelegateCommand<object> _textChanged2Command;
        /// <summary>
        /// Command raised whewever text from a TextBoxString item inside of TextBoxList changes.
        /// Uses object rather than TextBoxString due to command parameter errors.
        /// </summary>
        public DelegateCommand<object> TextChanged2Command
        {
            set { _textChanged2Command = value; }
            get { return _textChanged2Command; }
        }

        #endregion

        #region Constructor

        public BaseAddPageViewModel()
        {
            // Initialize the commands that won't be initialized in the inheritors.
            TextChangedCommand = new DelegateCommand<object>(BoxTextChanged);
            TextChanged2Command = new DelegateCommand<object>(Box2TextChanged);
        }

        #endregion


        #region Methods

        /// <summary>
        /// Adds an initial item of type TextBoxString to the observable collection TextBoxList with placeholder text ph.
        /// </summary>
        /// <param name="ph"></param>
        public void LoadInitialTBox(string ph)
        {
            var tboxValues = new TextBoxStrings() { PlaceHolder = ph};
            TextBoxList.Add(tboxValues);
        }

        /// <summary>
        /// Adds an initial item of type TextBoxString to the obseravable collection TextBoxList with placeholder text ph and ph2.
        /// </summary>
        /// <param name="ph"></param>
        /// <param name="ph2"></param>
        public void LoadInitialTBoxes(string ph, string ph2)
        {
            var tboxValues = new TextBoxStrings() { PlaceHolder = ph, PlaceHolder2 = ph2 };
            TextBoxList.Add(tboxValues);
        }

        /// <summary>
        /// Add an item of type TextBoxString to the observable collection TextBoxList with no placeholder text.
        /// </summary>
        public void AddNewTextBox()
        {
            var tboxValues = new TextBoxStrings();
            TextBoxList.Add(tboxValues);
            AddTextBoxCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Determines whether or not another item can be added to TextBoxList.
        /// </summary>
        /// <returns>bool</returns>
        public bool CanAddTextBox()
        {
            return TextBoxList.Count() < MaxBoxes;
        }

        /// <summary>
        /// Removes an item from TextBoxList.
        /// </summary>
        /// <param name="tb"></param>
        public void DeleteTBox(TextBoxStrings tb)
        {
            TextBoxList.Remove(tb);
            AddTextBoxCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Determines whether or not an item can be deleted from TextBoxList.
        /// </summary>
        /// <param name="tb"></param>
        /// <returns></returns>
        public bool CanDeleteTextBox(TextBoxStrings tb)
        {
            return tb != TextBoxList.ElementAt(0);
        }

        /// <summary>
        /// Determines whether or not data can be saved in the Azure database.
        /// </summary>
        /// <returns></returns>
        public virtual bool CanSave()
        {
            // Start with true, data can be saved.
            bool canSave = true;

            // Make sure the list isn't null.
            if (TextBoxList != null)
            {
                // 
                foreach (var item in TextBoxList)
                {
                    // If the string is null / empty, it can't be saved; show error text.
                    if (string.IsNullOrWhiteSpace(item.BoxText))
                    {
                        // Set canSave to false because we don't want to save empty text.
                        canSave = false;
                        item.Error = "Cannot save empty text.";
                    }
                }
            }

            // If the list IS null, can't save.
            else
                canSave = false;

            return canSave;
        }

        /// <summary>
        /// Fires when the text of a TextBoxString item.BoxText in the observable collection TextBoxList is changed.
        /// </summary>
        /// <param name="itemPassed"></param>
        public void BoxTextChanged(object itemPassed)
        {
            // Create a temp item.
            TextBoxStrings item;
            try
            {
                // Try to cast the object to the type TextBoxStrings.
                item = (TextBoxStrings)itemPassed;
            }
            catch(InvalidCastException)
            {
                // If the casting doesn't work, set the item to null.
                item = null;
            }

            // If TextBoxList isn't null and the item isn't null...
            if (TextBoxList != null && item != null)
            {
                // Find the index of the item.
                int i = TextBoxList.IndexOf(item);
                // Check the text for unwanted characters.
                var temp = CheckBoxText(item.BoxText);
                // If the text contains unwanted characters, update it.
                if (temp != item.BoxText)
                {
                    // If the caret isn't at the end already...
                    if (CaretPosition != item.BoxText.Count())
                    { 
                        // Save position of the caret. If it's greater than 0, subtract one so it goes into the right spot after deleting a character. Else, keep the spot.
                        int index = CaretPosition > 0 ? CaretPosition - 1 : CaretPosition;
                        // Update the item's text.
                        TextBoxList.ElementAt(i).BoxText = temp;
                        // Set the caret posiiton.
                        CaretPosition = index;
                    }
                    else
                    {
                        // the caret is at the end; keep it at the end.
                        TextBoxList.ElementAt(i).BoxText = temp;
                        CaretPosition = temp.Count() >= 0 ? temp.Count() : 0;
                    }
                }
            }
            
        }

        /// <summary>
        /// Fires when the text of a TextBoxString item.Box2Text in the observable collection TextBoxList is changed.
        /// </summary>
        /// <param name="itemPassed"></param>
        public void Box2TextChanged(object itemPassed)
        {
            TextBoxStrings item;

            try
            {
                item = (TextBoxStrings)itemPassed;
            }
            catch(InvalidCastException)
            {
                item = null;
            }

            if (TextBoxList != null && item != null)
            {
                int i = TextBoxList.IndexOf(item);
                var temp = CheckBoxText(item.Box2Text);
                if (temp != item.Box2Text)
                {

                    if (CaretPosition != item.Box2Text.Count())
                    {
                        int index = CaretPosition > 0 ? CaretPosition - 1 : CaretPosition;
                        TextBoxList.ElementAt(i).Box2Text = temp;
                        CaretPosition = index;
                    }
                    else
                    {
                        TextBoxList.ElementAt(i).Box2Text = temp;
                        CaretPosition = temp.Count() >= 0 ? temp.Count() : 0;
                    }
                }
            }

        }

        /// <summary>
        /// Returns a string without specific unwanted characters.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public string CheckBoxText(string val)
        {
            var r = new Regex("[@_]");
            return r.Replace(val, "");
        }

        /// <summary>
        /// Returns a string without specific unwanted characters.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public string CheckBox2Text(string val)
        {
            var r = new Regex("[@_]");
            return r.Replace(val, "");
        }

        /// <summary>
        /// Each inheritor needs a SaveItem task, but each will implement it differently.
        /// </summary>
        /// <returns></returns>
        public abstract Task SaveItem();

        #endregion


    }

}
