using System;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Windows.Forms;
using LinkedInApplication.Core;

namespace LinkedInApplication
{
    public partial class GetTokenForm : Form
    {
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly string _redirectUri;

        public string AccessToken { get; private set; }

        public GetTokenForm(string apiKey, string apiSecret, string redirectUri)
        {
            _apiKey = apiKey;
            _apiSecret = apiSecret;
            _redirectUri = redirectUri;

            InitializeComponent();

            var url = new Uri("https://www.linkedin.com/uas/oauth2/authorization?response_type=code" +
                               "&client_id=" + apiKey +
                               "&scope=r_fullprofile%20r_emailaddress%20r_network" +
                               "&state=" + Guid.NewGuid() +
                               "&redirect_uri=" + _redirectUri);
            webBrowser1.Navigate(url);
        }

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            var queryParams = e.Url.Query;
            var qs = HttpUtility.ParseQueryString(queryParams);

            if (qs["code"] != null && qs["state"] != null)
            {
                var code = qs["code"];

                var url = new Uri("https://www.linkedin.com/uas/oauth2/accessToken?grant_type=authorization_code" +
                                  "&code=" + code +
                                  "&redirect_uri=" + _redirectUri +
                                  "&client_id=" + _apiKey + 
                                  "&client_secret=" + _apiSecret);
                
                using (var httpClient = new HttpClient())
                {
                    var result = httpClient.GetAsync(url).Result;
                    var content = result.Content.ReadAsStringAsync().Result;
                    try
                    {
                        var byteArray = Encoding.UTF8.GetBytes(content);
                        var stream = new MemoryStream(byteArray);
                        var serializer = new DataContractJsonSerializer(typeof(AuthInfo));
                        var authInfo = (AuthInfo)serializer.ReadObject(stream);
                        AccessToken = authInfo.AccessToken;
                        DialogResult = DialogResult.OK;
                    }
                    catch (Exception exception)
                    {
                        throw new XmlParsingException("Can't parse server response, message: " + exception.Message);
                    }
                }
            }
        }
    }
}
