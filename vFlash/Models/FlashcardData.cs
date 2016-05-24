using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vFlash.Models
{
    public class FlashcardData : BaseAzureModel
    {

        private string _word_side1;

        [JsonProperty(PropertyName = "word_side1")]
        public string Word_Side1
        {
            get { return _word_side1; }
            set
            {
                if (_word_side1 != value)
                {
                    _word_side1 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _definition_side2;

        [JsonProperty(PropertyName = "definition_side2")]
        public string Definition_Side2
        {
            get { return _definition_side2; }
            set
            {
                if (_definition_side2 != value)
                {
                    _definition_side2 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _fcstack_id;

        [JsonProperty(PropertyName = "fcstack_id")]
        public string FCStack_ID
        {
            get { return _fcstack_id; }
            set
            {
                if (_fcstack_id != value)
                {
                    _fcstack_id = value;
                    NotifyPropertyChanged();
                }
            }
        }


    }
}
