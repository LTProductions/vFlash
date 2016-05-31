using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vFlash.Models
{
    public class QuizModel : BaseModel
    {

        private int _id;
        public int ID
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _question;
        public string Question
        {
            get { return _question; }
            set
            {
                if (_question != value)
                {
                    _question = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _a;
        public string A
        {
            get { return _a; }
            set
            {
                if (_a != value)
                {
                    _a = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _b;
        public string B
        {
            get { return _b; }
            set
            {
                if (_b != value)
                {
                    _b = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _c;
        public string C
        {
            get { return _c; }
            set
            {
                if (_c != value)
                {
                    _c = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _d;
        public string D
        {
            get { return _d; }
            set
            {
                if (_d != value)
                {
                    _d = value;
                    NotifyPropertyChanged();
                }
            }
        }








    }
}
