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
    public class ClassAddPageViewModel : Template10.Mvvm.ViewModelBase
    {


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

        private List<ClassData> _classList;
        public List<ClassData> ClassList
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


        #region Commands

        private DelegateCommand _addTextBoxCommand;
        public DelegateCommand AddTextBoxCommand
        {
            get { return _addTextBoxCommand; }
        }

        private DelegateCommand _saveClassesCommand;
        public DelegateCommand SaveClassesCommand
        {
            get { return _saveClassesCommand; }
        }

        private DelegateCommand<TextBoxStrings> _deleteTBoxCommand;
        public DelegateCommand<TextBoxStrings> DeleteTBoxCommand
        {
            get { return _deleteTBoxCommand; }
        }
        #endregion

        #region Constructor

        public ClassAddPageViewModel()
        {
            TextBoxList = new ObservableCollection<TextBoxStrings>();
            LoadInitialTBox();

            _addTextBoxCommand = new DelegateCommand(AddNewTextBox, CanAddTextBox);
            _saveClassesCommand = new DelegateCommand(async delegate ()
           {
               Views.Busy.SetBusy(true, "Saving...");
               await SaveClasses();
           });

            _deleteTBoxCommand = new DelegateCommand<TextBoxStrings>(DeleteTBox, CanDeleteTextBox);
        }

        #endregion


        #region Methods

        public void LoadInitialTBox()
        {
            var tboxValues = new TextBoxStrings() { PlaceHolder = "Biology, Calculus, etc." };
            TextBoxList.Add(tboxValues);
        }

        public void AddNewTextBox()
        {
            var tboxValues = new TextBoxStrings();
            TextBoxList.Add(tboxValues);
            AddTextBoxCommand.RaiseCanExecuteChanged();
        }

        private bool CanAddTextBox()
        {
            return TextBoxList.Count() < 7;
        }

        private void DeleteTBox(TextBoxStrings tb)
        {
            TextBoxList.Remove(tb);
            AddTextBoxCommand.RaiseCanExecuteChanged();
        }

        private bool CanDeleteTextBox(TextBoxStrings tb)
        {
            return tb != TextBoxList.ElementAt(0);
        }

        private async Task SaveClasses()
        {
            bool isBusy = false;
            
            // Create a new ClassData item to be used for inserting.
            ClassData classItem;

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
                        classItem = new ClassData() { Name = item.BoxText };

                        await classItem.InsertItem(classItem);
                        item.BoxText = string.Empty;
                    }

                    catch (Exception e)
                    {
                        //   break;
                    }
                }

                else
                {
                    item.Error = "Item not saved: Can't save empty text box.";
                }
            }

            TextBoxList = new ObservableCollection<TextBoxStrings>();
            LoadInitialTBox();
            AddTextBoxCommand.RaiseCanExecuteChanged();


            Views.Busy.SetBusy(false);
        }

        #endregion


    }

}
