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
        private string _id;

        [JsonProperty(PropertyName = "id")]
        public string Id
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

        private DateTime _createdAt;

        [JsonProperty(PropertyName = "createdAt")]
        public DateTime CreatedAt
        {
            get { return _createdAt; }
            set
            {
                if (_createdAt != value)
                {
                    _createdAt = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private DateTime _updatedAt;

        [JsonProperty(PropertyName = "updatedAt")]
        public DateTime UpdatedAt
        {
            get { return _updatedAt; }
            set
            {
                if (_updatedAt != value)
                {
                    _updatedAt = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Version _version;

        [JsonProperty(PropertyName = "version")]
        public Version Version
        {
            get { return _version; }
            set
            {
                if (_version != value)
                {
                    _version = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Boolean _deleted;

        [JsonProperty(PropertyName = "deleted")]
        public Boolean Deleted
        {
            get { return _deleted; }
            set
            {
                if (_deleted != value)
                {
                    _deleted = value;
                    NotifyPropertyChanged();
                }
            }
        }

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
