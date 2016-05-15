using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vFlash.Models
{
    public class BaseAzureModel : BaseModel
    {
        // Inherits PropertyChangedEvent from BaseModel.

        // Represents all typical Azure columns.

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

        //private Version _version;

        //[JsonProperty(PropertyName = "version")]
        //public Version Version
        //{
        //    get { return _version; }
        //    set
        //    {
        //        if (_version != value)
        //        {
        //            _version = value;
        //            NotifyPropertyChanged();
        //        }
        //    }
        //}

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

        #region Methods

        // Generic methods for querying Azure data.

        public async Task<List<T>> GetList<T>()
        {
            return await App.MobileService.GetTable<T>().ToListAsync();
        }

        public async Task<Boolean> InsertItem<T>(T item)
        {
            try
            {
                await App.MobileService.GetTable<T>().InsertAsync(item);
                return true;
            }

            catch
            {
                // error
                return false;
            }
        }

        public async Task<Boolean> DeleteItem<T>(T item)
        {
            try
            {
                await App.MobileService.GetTable<T>().DeleteAsync(item);
                return true;
            }
            catch
            {
                // error
                return false;
            }
        }

        #endregion

    }
}
