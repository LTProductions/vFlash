using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vFlash.Utils;

namespace vFlash.Models
{
    public class ClassData : BaseDataModel
    {
        // Inherits all typical Azure columns from BaseDataModel.


        // public IMobileServiceTable<ClassData> classDataTable;

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


        #region Methods

        public async Task<List<ClassData>> GetClassList()
        {
            try
            {
                return await App.MobileService.GetTable<ClassData>().ToListAsync();
            }

            catch (Exception e)
            {
                // error
                return null;
            }
        }

        public async Task<Boolean> CreateClass(ClassData classItem)
        {
            try
            {
                await App.MobileService.GetTable<ClassData>().InsertAsync(classItem);
                return true;
            }

            catch
            {
                // error
                return false;
            }
        }

        public async Task<Boolean> DeleteClass(ClassData classItem)
        {
            try
            {
                await App.MobileService.GetTable<ClassData>().DeleteAsync(classItem);
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
