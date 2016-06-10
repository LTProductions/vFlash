using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vFlash.Models
{

    /// <summary>
    /// Represents a table from the Azure database.
    /// </summary>
    public class SubclassData : BaseAzureModel
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

        private string _class_id;

        [JsonProperty(PropertyName = "class_id")]
        public string Class_ID
        {
            get { return _class_id; }
            set
            {
                if (_class_id != value)
                {
                    _class_id = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
