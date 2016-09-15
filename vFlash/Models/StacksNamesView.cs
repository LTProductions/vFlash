using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vFlash.Models
{
    public class StacksNamesView : BaseAzureModel
    {

        private string _className;

        [JsonProperty(PropertyName = "ClassName")]
        public string ClassName
        {
            get { return _className; }
            set
            {
                if (_className != value)
                {
                    _className = value;
                    NotifyPropertyChanged();
                }
            }

        }

        private string _subclassName;

        [JsonProperty(PropertyName = "SubclassName")]
        public string SubclassName
        {
            get { return _subclassName; }
            set
            {
                if (_subclassName != value)
                {
                    _subclassName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _stackName;

        [JsonProperty(PropertyName = "StackName")]
        public string StackName
        {
            get { return _stackName; }
            set
            {
                if (_stackName != value)
                {
                    _stackName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _stackID;

        [JsonProperty(PropertyName = "StackID")]
        public string StackID
        {
            get { return _stackID; }
            set
            {
                if (_stackID != value)
                {
                    _stackID = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //private DateTime _createdAt;

        //[JsonProperty(PropertyName = "CreatedAt")]
        //public DateTime CreatedAt
        //{
        //    get { return _createdAt; }
        //    set
        //    {
        //        if (_createdAt != value)
        //        {
        //            _createdAt = value;
        //            NotifyPropertyChanged();
        //        }
        //    }
        //}


        #region Methods

        //public async Task<IList<StackViewController>> GetList()
        //{
        //    try
        //    {
        //        return await App.MobileService.GetTable<StackViewController>().ToListAsync();
        //    }

        //    catch
        //    {
        //        //error
        //        return null;
        //    }
        //}

        //public async Task<IList<StackViewController>> GetQueriedList(IMobileServiceTableQuery<StackViewController> query)
        //{
        //    try
        //    {
        //        return await query.ToListAsync();
        //    }

        //    catch
        //    {
        //        // error
        //        return null;
        //    }
        //}

        #endregion


    } // end class



} // end namespace
