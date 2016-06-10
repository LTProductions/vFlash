using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using vFlash.Utils;

namespace vFlash.Models
{

    /// <summary>
    /// Holds the data for the textboxes used when entering data that will be stored into the Azure database.
    /// </summary>
    public class TextBoxStrings : BaseModel
    {
        
        private string _placeholder;
        /// <summary>
        /// Placeholder text for the text box.
        /// </summary>
        public string PlaceHolder
        {
            get { return _placeholder; }
            set
            {
                if (_placeholder != value)
                {
                    _placeholder = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _placeholder2;
        /// <summary>
        /// Placeholder text for a second textbox using the same class.
        /// </summary>
        public string PlaceHolder2
        {
            get { return _placeholder2; }
            set
            {
                if (_placeholder2 != value)
                {
                    _placeholder2 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _boxText;
        /// <summary>
        /// The text of the text box.
        /// </summary>
        public string BoxText
        {
            get { return _boxText; }
            set
            {
                if (_boxText != value)
                {
                    _boxText = value;
                    NotifyPropertyChanged();

                    // When the data in this textbox changes, the Error text should be reset. It will be checked again before submitting.
                    if (Error != "")
                        Error = "";
                }
            } 
        }

        private string _box2Text;
        /// <summary>
        /// Text of the second textbox.
        /// </summary>
        public string Box2Text
        {
            get { return _box2Text; }
            set
            {
                if (_box2Text != value)
                {
                    _box2Text = value;
                    NotifyPropertyChanged();

                    // When the data in this textbox changes, the Error2 text should be reset. It will be checked again before submitting.
                    if (Error2 != "")
                        Error2 = "";
                }
            }
        }


        private string _error;
        /// <summary>
        /// Error message.
        /// </summary>
        public string Error
        {
            get { return _error; }
            set
            {
                if (_error != value)
                {
                    _error = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _error2;
        /// <summary>
        /// Second error message.
        /// </summary>
        public string Error2
        {
            get { return _error2; }
            set
            {
                if (_error2 != value)
                {
                    _error2 = value;
                    NotifyPropertyChanged();
                }
            }
        }

    }
}
