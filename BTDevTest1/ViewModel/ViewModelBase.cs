using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrawhatNet.Study.BTRFCommDevTest.ViewModel
{
    abstract public class ViewModelBase : INotifyPropertyChanged
    {
        protected event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent(string argName)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(argName));
            }
        }
    }
}
