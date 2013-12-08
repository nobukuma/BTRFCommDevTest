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
using StrawhatNet.Study.BTRFCommDevTest.ViewModel;

namespace StrawhatNet.Study.BTRFCommDevTest
{
    public partial class MainPage : PhoneApplicationPage
    {
        private StreamSocket streamSocket;

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
            BluetoothControlPanelUtil.ShowBluetoothControlPanel();
        }

        private void WriteLog(string text)
        {
            this.LogTextBox.Items.Add(DateTime.Now.ToString() + " " + text);
            this.LogTextBox.SelectedIndex = this.LogTextBox.Items.Count - 1;

            return;
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/DevConnectionPage.xaml", UriKind.Relative));
        }

        public static byte[] StringToAscii(string s)
        {
            byte[] retval = new byte[s.Length];
            for (int ix = 0; ix < s.Length; ++ix)
            {
                char ch = s[ix];
                if (ch <= 0x7f) retval[ix] = (byte)ch;
                else retval[ix] = (byte)'?';
            }
            return retval;
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.streamSocket == null)
            {
                WriteLog("接続していません");
                return;
            }

            try
            {
                // 出力
                DataWriter dw = new DataWriter(streamSocket.OutputStream);

                List<byte> tmp = new List<byte>();
                var writeData = this.MessageTextBox.Text;
                var writeByteData = StringToAscii(writeData);
                dw.WriteBytes(writeByteData);

                var writeBytesSize = await dw.StoreAsync();
                await dw.FlushAsync();

                // 結果を読み取る
                DataReader dr = new DataReader(streamSocket.InputStream);

                //var loadSize = await dr.LoadAsync(sizeof(byte));
                //if (loadSize < sizeof(byte))
                //{
                //    WriteLog("切断されました");
                //    return;
                //}

                //uint readSize = (uint)writeByteData.Length;
                uint readSize = writeBytesSize;
                var loadSize = await dr.LoadAsync(readSize);
                if (loadSize < readSize)
                {
                    WriteLog("切断されました");
                    return;
                }

                var readByteData = new byte[readSize];
                dr.ReadBytes(readByteData);

                List<String> tmpList = new List<string>();
                List<String> hexList = new List<String>();
                foreach (byte b in readByteData)
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

            var dataContext = DataContext as MainPageViewModel;
            dataContext.BluetoothDeviceInfo = new BluetoothDeviceInfo();

            return;
        }

        private void Disconnect()
        {
            this.streamSocket.Dispose();
            WriteLog("切断しました");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (e.NavigationMode == NavigationMode.Back)
            {
                if (PhoneApplicationService.Current.State.ContainsKey(DevConnectionPage.OUTARG_BLUETOOTHDEVINFO))
                {
                    var state = PhoneApplicationService.Current.State;

                    this.streamSocket = state[DevConnectionPage.OUTARG_SOCKET] as StreamSocket;

                    var dataContext = DataContext as MainPageViewModel;
                    dataContext.BluetoothDeviceInfo = state[DevConnectionPage.OUTARG_BLUETOOTHDEVINFO] as BluetoothDeviceInfo;
                }
            }
            else {
                this.streamSocket = null;

                var dataContext = DataContext as MainPageViewModel;
                dataContext.BluetoothDeviceInfo = new BluetoothDeviceInfo();
            }
            return;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
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