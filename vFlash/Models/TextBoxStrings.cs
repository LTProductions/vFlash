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
    public class TextBoxStrings : BaseModel
    {

        private string _placeholder;
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
        public string BoxText
        {
            get { return _boxText; }
            set
            {
                if (_boxText != value)
                {
                    // var tmp = CheckBoxText(value);
                    // _boxText = tmp;
                    _boxText = value;
                    NotifyPropertyChanged();
                }
            } 
        }

        public string CheckBoxText(string val)
        {
            var r = new Regex("[^a-zA-Z0-9]+");
            return r.Replace(val, "");
        }

        private string _box2Text;
        public string Box2Text
        {
            get { return _box2Text; }
            set
            {
                if (_box2Text != value)
                {
                    _box2Text = value;
                    NotifyPropertyChanged();
                }
            }
        }


        private string _error;
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

    }
}
