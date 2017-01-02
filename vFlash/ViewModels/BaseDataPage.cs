using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vFlash.Models;

namespace vFlash.ViewModels
{
    /// <summary>
    /// Base ViewModel that holds reusable fields/properties/methods for loading data from the Azure database.
    /// </summary>
    public class BaseDataPage : Template10.Mvvm.ViewModelBase
    {

        #region Fields/Properties

        // Item passed when navigating from SubclassPage.
        public NamesAndIDs passedItem;

        private string _className;
        /// <summary>
        /// Holds the name of the corresponding ClassName that the FCStacks live in.
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
        /// Holds the name of the corresponding SubclassName that the FCStacks live in.
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
        /// Holds the name of the corresponding FCStackData in relation to the selected item from the previous ViewModel/View.
        /// </summary>
        public string FCStackName
        {
            get { return _fcStackName; }
            set
            {
                if (_fcStackName != value)
                {
                    _fcStackName = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Commands

        private DelegateCommand<object> _deleteItemCommand;
        /// <summary>
        /// Used to delete an item from the database / list. Takes an object which must be casted to the proper type.
        /// </summary>
        public DelegateCommand<object> DeleteItemCommand
        {
            set { _deleteItemCommand = value; }
            get { return _deleteItemCommand; }
        }

        private DelegateCommand<object> _navEditItemCommand;
        /// <summary>
        /// Used to edit an item from the database / list. Takes an object which must be casted to the proper type.
        /// </summary>
        public DelegateCommand<object> NavEditItemCommand
        {
            set { _navEditItemCommand = value; }
            get { return _navEditItemCommand; }
        }
        #endregion


        #region Methods

        /// <summary>
        /// Deletes the passed item from the Azure database.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async void DeleteItem(object item)
        {
            var azureItem = new BaseAzureModel();
            await azureItem.DeleteItem(item);
        }

        #endregion


    }
}
