using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrawhatNet.Study.BTDevTest1
{
    public class PageViewModel : INotifyPropertyChanged
    {
        private IEnumerable<ListItem> _listItem;
        public IEnumerable<ListItem> ListItem
        {
            get
            {
                return this._listItem;
            }

            set
            {
                this._listItem = value;
                if (PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ListItem"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
