using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StrawhatNet.Study.BTRFCommDevTest.ViewModel
{
    public class DevConnectionPageViewModel : ViewModelBase
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
                RaisePropertyChangedEvent("ConnectButtonIsVisible");
                RaisePropertyChangedEvent("DisconnectButtonIsVisible");
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
                RaisePropertyChangedEvent("ListItem");
            }
        }
    }
}
