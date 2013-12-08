using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace StrawhatNet.Study.BTDevTest1
{
    public partial class AboutPage : PhoneApplicationPage
    {
        private const string NavigationKeyPageIndex = "Page";

        public AboutPage()
        {
            string ietfLanguageTag = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            this.Language = System.Windows.Markup.XmlLanguage.GetLanguage(ietfLanguageTag);
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            int pageIndex;

            PhoneApplicationService service = PhoneApplicationService.Current;
            if (NavigationContext.QueryString.ContainsKey(NavigationKeyPageIndex))
            {
                string indexString = NavigationContext.QueryString[NavigationKeyPageIndex];
                pageIndex = int.Parse(indexString);
                this.pivotControl.SelectedIndex = pageIndex;
            }

            base.OnNavigatedTo(e);
            return;
        }
    }
}