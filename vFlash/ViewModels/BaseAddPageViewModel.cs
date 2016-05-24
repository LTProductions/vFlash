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
        #endregion

        #region Constructor

        public BaseAddPageViewModel()
        {
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

        public abstract Task SaveItem();

        #endregion


    }

}
