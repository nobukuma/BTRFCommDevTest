using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using StrawhatNet.Study.BTRFCommDevTest.Resources;
using System.Diagnostics;
using System.ComponentModel;
using System.Text;

using Windows.Networking.Proximity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Microsoft.Phone.Tasks;

namespace StrawhatNet.Study.BTRFCommDevTest
{
    public partial class DevConnectionPage : PhoneApplicationPage
    {
        public static readonly string OUTARG_BLUETOOTHDEVINFO = "OUTARG_BLUETOOTHDEVINFO_KEY";
        public static readonly string OUTARG_SOCKET = "OUTARG_SOCKET_KEY";

        private StreamSocket streamSocket;
        private BluetoothDeviceInfo bluetoothDeviceInfo;

        // コンストラクター
        public DevConnectionPage()
        {
            InitializeComponent();
        }

        private const string SerialPortServiceClass_UUID = "{00001101-0000-1000-8000-00805F9B34FB}";

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            //PeerFinder.AlternateIdentities["Bluetooth:Paired"] = "";
            PeerFinder.AlternateIdentities["Bluetooth:SDP"] = SerialPortServiceClass_UUID;

            try
            {
                WriteLog("デバイスを探索しています");
                var pairedDevices = await PeerFinder.FindAllPeersAsync();

                if (pairedDevices.Count == 0)
                {
                    WriteLog("デバイスが見つかりません");
                }
                else
                {
                    var items = from item in pairedDevices
                                select new BluetoothDeviceInfo
                                {
                                    IsConnected = false,
                                    DisplayName = item.DisplayName,
                                    ServiceName = item.ServiceName,
                                    HostName = item.HostName,
                                };
                    this.deviceListBox.ItemsSource = items.ToArray();
                    WriteLog(String.Format("{0} devices found", pairedDevices.Count));

                    this.ConnectButton.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x8007048F)
                {
                    var result = MessageBox.Show("Bluetoothが有効になっていません。OKを押すと設定します", "Bluetoothオフ", MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                    {
                        BluetoothControlPanelUtil.ShowBluetoothControlPanel();
                    }
                }
                else if ((uint)ex.HResult == 0x80070005)
                {
                    MessageBox.Show("アプリにID_CAP_PROXIMITY権限が設定されていません");
                }
                else
                {
                    MessageBox.Show(ex.Message);
                    Debug.WriteLine(ex.StackTrace);
                    Debug.WriteLine(ex.HResult);
                }
            }
        }

        private readonly byte[] ConnAck = new byte[] { 0xbe, 0xef, 8, 0x43, 0x4F, 0x4E, 0x4E, 0x5F, 0x41, 0x43, 0x4B, 0x11 };

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.deviceListBox.SelectedItem == null)
            {
                WriteLog("選択されていません");
                return;
            }

            var selectedDevice = this.deviceListBox.SelectedItem as BluetoothDeviceInfo;
            WriteLog(String.Format("接続しています...: {0}:{1}",
                selectedDevice.DisplayName,
                selectedDevice.ServiceName));

            this.streamSocket = new StreamSocket();
            try
            {
                await streamSocket.ConnectAsync(selectedDevice.HostName, "1");

                while (true)
                {
                    DataReader dr = new DataReader(streamSocket.InputStream);

                    var loadSize = await dr.LoadAsync((uint)ConnAck.Length);
                    if (loadSize < sizeof(byte))
                    {
                        WriteLog("切断されました");
                        return;
                    }

                    var readByteData = new byte[loadSize];
                    dr.ReadBytes(readByteData);

                    if (readByteData.SequenceEqual(ConnAck))
                    {
                        break;
                    }
                }

                this.ConnectButton.IsEnabled = false;
                this.bluetoothDeviceInfo.IsConnected = true;
                this.bluetoothDeviceInfo.DisplayName = selectedDevice.DisplayName;
                this.bluetoothDeviceInfo.ServiceName = selectedDevice.ServiceName;
                this.bluetoothDeviceInfo.HostName = selectedDevice.HostName;
                WriteLog("接続しました");
            }
            catch (Exception ex)
            {
                WriteLog("接続失敗: " + ex.Message);
                Debug.WriteLine(ex.StackTrace);
                Debug.WriteLine(ex.HResult);
            }

            return;
        }

        private void WriteLog(string text)
        {
            this.LogTextBox.Items.Add(DateTime.Now.ToString() + " " + text);
            this.LogTextBox.SelectedIndex = this.LogTextBox.Items.Count - 1;

            return;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            //if (e.NavigationMode != NavigationMode.Back)
            //{
                this.SearchButton.IsEnabled = true;
                this.ConnectButton.IsEnabled = false;
                this.streamSocket = null;
                this.bluetoothDeviceInfo = new BluetoothDeviceInfo();
            //}
            //else
            //{
            //    // 他AP画面から戻ってきた場合：　接続が破棄されているはず
            //    // XXX: 初期化しなおす?
            //}
            return;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == NavigationMode.Back)
            {
                // 前画面に戻る場合
                PhoneApplicationService.Current.State[OUTARG_BLUETOOTHDEVINFO] = this.bluetoothDeviceInfo;
                PhoneApplicationService.Current.State[OUTARG_SOCKET] = this.streamSocket;
            }

            return;
        }

        // ローカライズされた ApplicationBar を作成するためのサンプル コード
        //private void BuildLocalizedApplicationBar()
        //{
        //    // ページの ApplicationBar を ApplicationBar の新しいインスタンスに設定します。
        //    ApplicationBar = new ApplicationBar();

        //    // 新しいボタンを作成し、テキスト値を AppResources のローカライズされた文字列に設定します。
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // AppResources のローカライズされた文字列で、新しいメニュー項目を作成します。
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}