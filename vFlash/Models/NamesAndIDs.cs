using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vFlash.Models
{
    public class NamesAndIDs : BaseModel
    {
        private string _className;
        public string ClassName
        {
            get { return _className; }
            set
            {
                if (_className != value)
                {
                    _className = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _classID;
        public string ClassID
        {
            get { return _classID; }
            set
            {
                if (_classID != value)
                {
                    _classID = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _subclassName;
        public string SubclassName
        {
            get { return _subclassName; }
            set
            {
                if (_subclassName != value)
                {
                    _subclassName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _subclassID;
        public string SubclassID
        {
            get { return _subclassID; }
            set
            {
                if (_subclassID != value)
                {
                    _subclassID = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _fcStackName;
        public string FCStackName
        {
            get { return _fcStackName; }
            set
            {
                if (_fcStackName != value)
                {
                    _fcStackName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _fcStackID;
        public string FCStackID
        {
            get { return _fcStackID; }
            set
            {
                if (_fcStackID != value)
                {
                    _fcStackID = value;
                    NotifyPropertyChanged();
                }
            }
        }



    }
}
