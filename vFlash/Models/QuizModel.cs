using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vFlash.Models
{

    /// <summary>
    /// Represents the model of the quiz.
    /// </summary>
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
        /// <summary>
        /// Holds the data for a question.
        /// </summary>
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
        /// <summary>
        /// Holds the data for answer A.
        /// </summary>
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
        /// <summary>
        /// Holds the data for answer B.
        /// </summary>
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
        /// <summary>
        /// Holds the answer for answer C.
        /// </summary>
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
        /// <summary>
        /// Holds the answer for answer D.
        /// </summary>
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

        private string _aBG;
        /// <summary>
        /// Holds the background color for answer A; used when reviewing the quiz. If the user selected this answer and it is the correct answer,
        /// the background should be green; else, red.
        /// </summary>
        public string ABG
        {
            get { return _aBG; }
            set
            {
                if (_aBG != value)
                {
                    _aBG = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _bBG;
        /// <summary>
        /// Holds the background color for answer A; used when reviewing the quiz. If the user selected this answer and it is the correct answer,
        /// the background should be green; else, red.
        /// </summary>
        public string BBG
        {
            get { return _bBG; }
            set
            {
                if (_bBG != value)
                {
                    _bBG = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _cBG;
        /// <summary>
        /// Holds the background color for answer A; used when reviewing the quiz. If the user selected this answer and it is the correct answer,
        /// the background should be green; else, red.
        /// </summary>
        public string CBG
        {
            get { return _cBG; }
            set
            {
                if (_cBG != value)
                {
                    _cBG = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _dBG;
        /// <summary>
        /// Holds the background color for answer A; used when reviewing the quiz. If the user selected this answer and it is the correct answer,
        /// the background should be green; else, red.
        /// </summary>
        public string DBG
        {
            get { return _dBG; }
            set
            {
                if (_dBG != value)
                {
                    _dBG = value;
                    NotifyPropertyChanged();
                }
            }
        }




    }
}
