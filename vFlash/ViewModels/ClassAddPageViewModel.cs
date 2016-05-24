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
    public class ClassAddPageViewModel : BaseAddPageViewModel
    {

        public ClassAddPageViewModel()
        {
            TextBoxList = new ObservableCollection<TextBoxStrings>();
            LoadInitialTBox("Biology, Calculus, etc.");
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
            LoadInitialTBox("Biology, Calculus, etc.");
            AddTextBoxCommand.RaiseCanExecuteChanged();


            Views.Busy.SetBusy(false);
        }


    }

}
