using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vFlash.Models
{
    public class FlashcardData : BaseModel
    {

        private string _name;

        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
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
