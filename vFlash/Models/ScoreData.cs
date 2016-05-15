using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vFlash.Models
{
    public class ScoreData : BaseModel
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

        private string _fcdata_id;

        [JsonProperty(PropertyName = "fcdata_id")]
        public string FCData_ID
        {
            get { return _fcdata_id; }
            set
            {
                if (_fcdata_id != value)
                {
                    _fcdata_id = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _sessiondata_id;

        [JsonProperty(PropertyName = "sessiondata_id")]
        public string SessionData_ID
        {
            get { return _sessiondata_id; }
            set
            {
                if (_sessiondata_id != value)
                {
                    _sessiondata_id = value;
                    NotifyPropertyChanged();
                }
            }
        }

    }
}
