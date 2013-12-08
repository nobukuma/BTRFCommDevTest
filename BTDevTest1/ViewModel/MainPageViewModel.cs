using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StrawhatNet.Study.BTRFCommDevTest.ViewModel
{
    public class MainPageViewModel : ViewModelBase
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
                RaisePropertyChangedEvent("IsConnected");
                RaisePropertyChangedEvent("ConnectButtonIsVisible");
                RaisePropertyChangedEvent("ConnectButtonIsEnabled");
                RaisePropertyChangedEvent("DisconnectButtonIsVisible");
                RaisePropertyChangedEvent("DisconnectButtonIsEnabled");
                RaisePropertyChangedEvent("SendButtonIsEnabled");
            }
        }
        
        public Visibility ConnectButtonIsVisible
        {
            get
            {
                return IsConnected ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public bool ConnectButtonIsEnabled
        {
            get
            {
                return IsConnected ? false : true;
            }
        }

        public Visibility DisconnectButtonIsVisible
        {
            get
            {
                return IsConnected ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public bool DisconnectButtonIsEnabled
        {
            get
            {
                return IsConnected ? true : false;
            }
        }

        public bool SendButtonIsEnabled
        {
            get
            {
                return IsConnected ? true : false;
            }
        }

        private BluetoothDeviceInfo bluetoothDeviceInfo;
        public BluetoothDeviceInfo BluetoothDeviceInfo
        {
            get
            {
                return this.bluetoothDeviceInfo;
            }
            set
            {
                this.bluetoothDeviceInfo = value;
                RaisePropertyChangedEvent("BluetoothDeviceInfo");

                this.IsConnected = this.bluetoothDeviceInfo.IsConnected;
            }
        }
    }
}
