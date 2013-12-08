using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StrawhatNet.Study.BTRFCommDevTest.ViewModel
{
    public class DevConnectionPageViewModel : INotifyPropertyChanged
    {
        private bool isConnected;
        public bool IsConnected {
            get
            {
                return this.isConnected;
            }
            set
            {
                this.isConnected = value;
                if (PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ConnectButtonIsVisible"));
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("DisconnectButtonIsVisible"));
                }
            }
        }

        public Visibility ConnectButtonIsVisible
        {
            get
            {
                return IsConnected ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        public Visibility DisconnectButtonIsVisible
        {
            get
            {
                return IsConnected ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private IEnumerable<BluetoothDeviceInfo> _listItem;
        public IEnumerable<BluetoothDeviceInfo> ListItem
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
