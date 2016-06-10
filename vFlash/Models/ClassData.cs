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
    /// <summary>
    /// Represents a table from the Azure database.
    /// </summary>
    public class ClassData : BaseAzureModel
    {
        // Inherits all typical Azure columns from BaseDataModel.

        
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

    }
}
