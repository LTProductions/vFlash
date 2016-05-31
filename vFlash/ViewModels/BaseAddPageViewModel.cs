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
    public abstract class BaseAddPageViewModel : Template10.Mvvm.ViewModelBase
    {

        #region Fields/Properties

        public NamesAndIDs passedItem;

        private ObservableCollection<TextBoxStrings> _textBoxList;
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

        private int _maxBoxes = 7;
        public int MaxBoxes
        {
            get { return _maxBoxes; }
            set { _maxBoxes = value; }
        }

        private int _caretPosition;
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
        public DelegateCommand AddTextBoxCommand
        {
            set { _addTextBoxCommand = value; }
            get { return _addTextBoxCommand; }
        }

        private DelegateCommand _saveItemsCommand;
        public DelegateCommand SaveItemsCommand
        {
            set { _saveItemsCommand = value; }
            get { return _saveItemsCommand; }
        }

        private DelegateCommand<TextBoxStrings> _deleteTBoxCommand;
        public DelegateCommand<TextBoxStrings> DeleteTBoxCommand
        {
            set { _deleteTBoxCommand = value; }
            get { return _deleteTBoxCommand; }
        }

        private DelegateCommand<object> _textChangedCommand;
        public DelegateCommand<object> TextChangedCommand
        {
            set { _textChangedCommand = value; }
            get { return _textChangedCommand; }
        }

        private DelegateCommand<object> _textChanged2Command;
        public DelegateCommand<object> TextChanged2Command
        {
            set { _textChanged2Command = value; }
            get { return _textChanged2Command; }
        }

        #endregion

        #region Constructor

        public BaseAddPageViewModel()
        {
            TextChangedCommand = new DelegateCommand<object>(BoxTextChanged);
            TextChanged2Command = new DelegateCommand<object>(Box2TextChanged);
        }

        #endregion


        #region Methods

        public void LoadInitialTBox(string ph)
        {
            var tboxValues = new TextBoxStrings() { PlaceHolder = ph};
            TextBoxList.Add(tboxValues);
        }

        public void LoadInitialTBoxes(string ph, string ph2)
        {
            var tboxValues = new TextBoxStrings() { PlaceHolder = ph, PlaceHolder2 = ph2 };
            TextBoxList.Add(tboxValues);
        }

        public void AddNewTextBox()
        {
            var tboxValues = new TextBoxStrings();
            TextBoxList.Add(tboxValues);
            AddTextBoxCommand.RaiseCanExecuteChanged();
        }

        public bool CanAddTextBox()
        {
            return TextBoxList.Count() < MaxBoxes;
        }

        public void DeleteTBox(TextBoxStrings tb)
        {
            TextBoxList.Remove(tb);
            AddTextBoxCommand.RaiseCanExecuteChanged();
        }

        public bool CanDeleteTextBox(TextBoxStrings tb)
        {
            return tb != TextBoxList.ElementAt(0);
        }

        public virtual bool CanSave()
        {
            bool canSave = true;

            if (TextBoxList != null)
            {
                foreach (var item in TextBoxList)
                {
                    if (string.IsNullOrWhiteSpace(item.BoxText))
                    {
                        canSave = false;
                        item.Error = "Cannot save empty text.";
                    }
                }
            }

            else
                canSave = false;

            return canSave;
        }

        public void BoxTextChanged(object itemPassed)
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
                var temp = CheckBoxText(item.BoxText);
                if (temp != item.BoxText)
                {
                    
                    if (CaretPosition != item.BoxText.Count())
                    { 
                        int index = CaretPosition > 0 ? CaretPosition - 1 : CaretPosition;
                        TextBoxList.ElementAt(i).BoxText = temp;
                        CaretPosition = index;
                    }
                    else
                    {
                        TextBoxList.ElementAt(i).BoxText = temp;
                        CaretPosition = temp.Count() >= 0 ? temp.Count() : 0;
                    }
                }
            }
            
        }

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

        public string CheckBoxText(string val)
        {
            var r = new Regex("[^\\w #-]+");
            return r.Replace(val, "");
        }

        public string CheckBox2Text(string val)
        {
            var r = new Regex("[^\\w !@#$%^&*()-+={}\\|[];:\"\'?/.,<>]+");
            return r.Replace(val, "");
        }

        public abstract Task SaveItem();

        #endregion


    }

}
