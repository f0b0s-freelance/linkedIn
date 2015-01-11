using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var url = new Uri("https://www.linkedin.com/uas/oauth2/authorization?response_type=code" +
                               "&client_id=npq2v0qgrl9y" +
                               "&scope=r_fullprofile%20r_emailaddress%20r_network" +
                               "&state=DCEEFWF45453sdffef424" +
                               "&redirect_uri=http://www.google.com");
            webBrowser1.Navigated += WebBrowser1OnNavigated;
            webBrowser1.Navigate(url);
        }

        private void WebBrowser1OnNavigated(object sender, WebBrowserNavigatedEventArgs webBrowserNavigatedEventArgs)
        {
            Debug.WriteLine(webBrowserNavigatedEventArgs.Url);
            var queryParams = webBrowserNavigatedEventArgs.Url.Query;
            var qs = HttpUtility.ParseQueryString(queryParams);

            if (qs["code"] != null && qs["state"] != null)
            {
                var code = qs["code"];

                var url = new Uri("https://www.linkedin.com/uas/oauth2/accessToken?grant_type=authorization_code" +
                                  "&code=" + code +
                                  "&redirect_uri=http://www.google.com" +
                                  "&client_id=npq2v0qgrl9y" +
                                  "&client_secret=50DUgVK6Cl6RwMRy");
                webBrowser1.Navigate(url);
            }
        }
    }
}
