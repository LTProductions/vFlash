using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vFlash.Models
{
    public class FCStackData : BaseAzureModel
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

        private string _subclass_id;

        [JsonProperty(PropertyName = "subclass_id")]
        public string Subclass_ID
        {
            get { return _subclass_id; }
            set
            {
                if (_subclass_id != value)
                {
                    _subclass_id = value;
                    NotifyPropertyChanged();
                }
            }
        }


    }
}
