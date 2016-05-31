using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vFlash.Models
{
    public class StudySessionData : BaseAzureModel
    {

        private string _sessionname_id;

        [JsonProperty(PropertyName = "sessionname_id")]
        public string SessionName_ID
        {
            get { return _sessionname_id; }
            set
            {
                if (_sessionname_id != value)
                {
                    _sessionname_id = value;
                    NotifyPropertyChanged();
                }
            }
        }

    }
}
