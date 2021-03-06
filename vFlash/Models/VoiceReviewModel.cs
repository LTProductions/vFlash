﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vFlash.Models
{
    /// <summary>
    /// Model used for holding FlashCard data; used when reviewing to show which answers were right / wrong.
    /// </summary>
    public class VoiceReviewModel : BaseModel
    {
        private string _word;
        /// <summary>
        /// Holds the Word value from a FlashcardData object.
        /// </summary>
        public string Word
        {
            get { return _word; }
            set
            {
                if (_word != value)
                {
                    _word = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _definition;
        /// <summary>
        /// Holds the Definitio value from a FlashcardData object.
        /// </summary>
        public string Definition
        {
            get { return _word; }
            set
            {
                if (_definition != value)
                {
                    _definition = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _color;
        /// <summary>
        /// Holds the color value used when reviewing a voice session; based on a correct or incorrect answer.
        /// </summary>
        public string Color
        {
            get { return _color; }
            set
            {
                if (_color != value)
                {
                    _color = value;
                    NotifyPropertyChanged();
                }
            }
        }

    }
}
