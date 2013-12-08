using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using StrawhatNet.Study.BTDevTest1.Resources;
using System.Diagnostics;
using System.ComponentModel;
using System.Text;

using Windows.Networking.Proximity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Microsoft.Phone.Tasks;

namespace StrawhatNet.Study.BTDevTest1
{
    public class ListItem
    {
        public string DisplayName { get; set; }
        public string ServiceName { get; set; }
        public Windows.Networking.HostName HostName { get; set; }
    }

    public partial class MainPage : PhoneApplicationPage
    {
        private StreamSocket socket;

        // コンストラクター
        public MainPage()
        {
            InitializeComponent();

            // ApplicationBar をローカライズするためのサンプル コード
            //BuildLocalizedApplicationBar();
        }

        private void ApplicationBarMenuItem_About_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {

        }

         private const string SerialPortServiceClass_UUID = "{00001101-0000-1000-8000-00805F9B34FB}";

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            //PeerFinder.AlternateIdentities["Bluetooth:Paired"] = "";
            PeerFinder.AlternateIdentities["Bluetooth:SDP"] = SerialPortServiceClass_UUID;

            try
            {
                var pairedDevices = await PeerFinder.FindAllPeersAsync();

                if (pairedDevices.Count == 0)
                {
                    WriteLog("No paired devices were found.");
                }
                else
                {
                    var items = from item in pairedDevices
                                select new ListItem
                                {
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
                        ShowBluetoothcControlPanel();
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

        private void ShowBluetoothcControlPanel()
        {
            ConnectionSettingsTask connectionSettingsTask = new ConnectionSettingsTask();
            connectionSettingsTask.ConnectionSettingsType = ConnectionSettingsType.Bluetooth;
            connectionSettingsTask.Show();
        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.deviceListBox.SelectedItem == null)
            {
                WriteLog("選択されていません");
                return;
            }

            var selectedDevice = this.deviceListBox.SelectedItem as ListItem;
            WriteLog(String.Format("接続しています...: {0}:{1}",
                selectedDevice.DisplayName,
                selectedDevice.ServiceName));

            this.socket = new StreamSocket();
            try
            {
                await socket.ConnectAsync(selectedDevice.HostName, "1");

                this.ConnectButton.IsEnabled = false;
                this.ReadButton.IsEnabled = true;
                this.DisconnectButton.IsEnabled = true;
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

        private void PhoneApplicationPage_Loaded_1(object sender, RoutedEventArgs e)
        {
            this.ConnectButton.IsEnabled = false;
            this.ReadButton.IsEnabled = false;
            this.DisconnectButton.IsEnabled = false;
            return;

        }

        private void WriteLog(string text)
        {
            this.LogTextBox.Items.Add(DateTime.Now.ToString() + " " + text);
            this.LogTextBox.SelectedIndex = this.LogTextBox.Items.Count - 1;

            return;
        }

        private async void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.socket == null)
            {
                WriteLog("接続していません");
                return;
            }

            try
            {
                List<byte> data = new List<byte>();
                uint len = 1;
                DataReader dr = new DataReader(socket.InputStream);
                while (true)
                {
                    await dr.LoadAsync(len);
                    byte b = dr.ReadByte();
                    data.Add(b);
                    if (b == 0x0A)
                    {
                        break;
                    }
                }

                List<String> tmpList = new List<string>();
                List<String> hexList = new List<String>();
                foreach (byte b in data)
                {
                    hexList.Add(string.Format("{0:X2}", b));
                    tmpList.Add(Char.ToString((char)b));
                }

                WriteLog(String.Format("Hex: {0}", String.Join(" ", hexList)));
                WriteLog(String.Format("Chr: {0}", String.Join(" ", tmpList)));
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
                Disconnect();
            }
            return;
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            Disconnect();
            return;
        }

        private void Disconnect()
        {
            this.socket.Dispose();

            this.ConnectButton.IsEnabled = true;
            this.ReadButton.IsEnabled = false;
            this.DisconnectButton.IsEnabled = false;

            WriteLog("Disconnected");
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