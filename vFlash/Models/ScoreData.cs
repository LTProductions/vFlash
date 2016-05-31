using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vFlash.Models
{
    public class ScoreData : BaseAzureModel
    {
        
        private bool _correct;

        [JsonProperty(PropertyName = "correct")]
        public bool Correct
        {
            get { return _correct; }
            set
            {
                if (_correct != value)
                {
                    _correct = value;
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
