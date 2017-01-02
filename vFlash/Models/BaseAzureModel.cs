using Microsoft.WindowsAzure.MobileServices;
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

        /// <summary>
        /// Returns a list of Azure Items of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<IList<T>> GetList<T>()
        {
            try
            {
                return await App.MobileService.GetTable<T>().ToListAsync();
            }

            catch(Exception e)
            {
                //error
                return null;
            }
        }

        /// <summary>
        /// Returns a quieried list of Azure data items of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns>List(T)"/></returns>
        public async Task<IList<T>> GetQueriedList<T>(IMobileServiceTableQuery<T> query)
        {
            try
            {
                return await query.ToListAsync();
            }

            catch
            {
                // error
                return null;
            }
        }


        /// <summary>
        /// Inserts the passed item from the linked Azure database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Deletes the passed item from the linked Azure database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
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
