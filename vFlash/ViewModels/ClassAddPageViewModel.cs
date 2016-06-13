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
    /// ViewModel used for adding ClassData to the Azure database.
    /// Corresponding View: ClassAddPage.xaml
    /// </summary>
    public class ClassAddPageViewModel : BaseAddPageViewModel
    {

        #region Constructor

        public ClassAddPageViewModel()
        {
            // Initialize the ObservableCollection.
            TextBoxList = new ObservableCollection<TextBoxStrings>();
            // Load the initial text box with placeholder text.
            LoadInitialTBox("Biology, Calculus, etc.");
            // Set the max number of allowed boxes.
            MaxBoxes = 7;

            #region Command initializers

            AddTextBoxCommand = new DelegateCommand(AddNewTextBox, CanAddTextBox);
            SaveItemsCommand = new DelegateCommand(async delegate ()
            {
                await SaveItem();
            });

            DeleteTBoxCommand = new DelegateCommand<TextBoxStrings>(DeleteTBox, CanDeleteTextBox);

            #endregion

        }

        #endregion

        /// <summary>
        /// Override the base abstract method in BaseAddPageViewModel. Saves data into the database.
        /// </summary>
        /// <returns></returns>
        public override async Task SaveItem()
        {
            bool isBusy = false;

            // Create a new ClassData item to be used for inserting.
            ClassData classItem;

            // Check to see if you're allowed to save with the inputted data.
            if (CanSave())
            {
                // Check to see if the IsBusy modal is active. If not, make it active.
                if (!isBusy)
                {
                    Views.Busy.SetBusy(true, "Saving...");
                    isBusy = true;
                }

                // Save each item.
                foreach (var item in TextBoxList)
                {

                    try
                    {
                        // Set the properties of the classitem.
                        classItem = new ClassData() { Name = item.BoxText };

                        await classItem.InsertItem(classItem);
                    }

                    // Maybe this try-catch is unnecessary?
                    catch (Exception e)
                    {
                        //   break;
                        Debug.WriteLine(e.ToString());
                    }
                }

                // Reload the initial data.
                TextBoxList = new ObservableCollection<TextBoxStrings>();
                LoadInitialTBox("Biology, Calculus, etc.");
                AddTextBoxCommand.RaiseCanExecuteChanged();
            }

            // If BusyModal is active, set it back to false.
            if (isBusy)
                Views.Busy.SetBusy(false);
        }


    }

}
