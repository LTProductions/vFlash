using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vFlash.Models;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace vFlash.ViewModels
{
    public class ClassAddPageViewModel : Template10.Mvvm.ViewModelBase
    {


        private ObservableCollection<TextBox> _textBoxList = new ObservableCollection<TextBox>();
        public ObservableCollection<TextBox> TextBoxList
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

        #endregion

        #region Constructor

        public ClassAddPageViewModel()
        {
            LoadInitialTBox();

            _addTextBoxCommand = new DelegateCommand(AddNewTextBox, CanAddTextBox);
            _saveClassesCommand = new DelegateCommand(async delegate ()
           {
               Views.Busy.SetBusy(true, "Saving...");
               await SaveClasses();
           });
        }

        #endregion


        #region Methods

        private void LoadInitialTBox()
        {
            // Create the initial text box with the header and placeholder text and add the event.
            var box = new TextBox { Header = "Class Name", PlaceholderText = "Biology, Calculus, etc." };
            box.Width = 200;
            TextBoxList.Add(box);
        }

        public void AddNewTextBox()
        {
            TextBox newTBox = new TextBox();
            newTBox.Width = 200;
            TextBoxList.Add(newTBox);
            AddTextBoxCommand.RaiseCanExecuteChanged();
        }

        private bool CanAddTextBox()
        {
            return TextBoxList.Count() < 7;
        }

        private bool CanSave()
        {
            foreach (var item in TextBoxList)
            {
                if (string.IsNullOrWhiteSpace(item.Text))
                { 
                    return false;
                }

                if (item.Text.Length > 30)
                {
                    //display popup with error.
                    return false;
                }
            }

            return true;
        }

        private async Task SaveClasses()
        {
            if (CanSave())
            {
                // Create a new ClassData item to be used for inserting.
                ClassData classItem;

                foreach (var item in TextBoxList)
                {
                    try
                    {
                        // Set the properties of the classitem.
                        classItem = new ClassData() { Name = item.Text };

                        await classItem.InsertItem(classItem);

                        // Remove that textbox.
                        item.Text = string.Empty;
                    }

                    catch (Exception e)
                    {
                        //   break;
                    }
                }

                TextBoxList = new ObservableCollection<TextBox>();
                AddTextBoxCommand.RaiseCanExecuteChanged();
                LoadInitialTBox();
            }

            Views.Busy.SetBusy(false);
        }

        #endregion

    }
}
