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



    }
}
