using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace vFlash.Models
{
    public class BaseModel : INotifyPropertyChanged
    {

        #region NotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify that a property changed

        public void NotifyPropertyChanged([CallerMemberName] String propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion


    }
}
