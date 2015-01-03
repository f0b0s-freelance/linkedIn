using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Forms;
using LinkedInApplication.Core;

namespace LinkedInApplication
{
    public partial class Form1 : Form
    {
        private const string Key =
            "AQVfV_0s3MadzEi57qg6Qx7iftX5t-LOEVlzYssooo7-VMqsLpDOHlnQ9vLQaXkvOT_I5Y-ZPsLCOABnsdDggF9FEQ8hQD8dWvNm0Wfl9zdPw1Y-nRozmjjR5yelkmOsNuPzpI5K_SqTOKqLVWVAhoQnrSslsaClONKV8k-svtRRq-JdTCI";
        
        public Form1()
        {
            InitializeComponent();
        }

        protected override async void OnActivated(EventArgs e)
        {
            var locations = await LoadLocations(new Uri("https://api.linkedin.com/v1/people-search:(facets:(code,buckets:(code,name)))?facets=location"));

            cbxLocations.Items.AddRange(locations.ToArray());
            cbxCategory.Items.AddRange(IndustryFacetStaticInfo.IndustryFacets);
            
            base.OnActivated(e);
        }

        private static async Task<IEnumerable<FacetItemInfo>> LoadLocations(Uri url)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Key);
            var result = await httpClient.GetAsync(url);
            var content = await result.Content.ReadAsStringAsync();
            return FacetItemInfoParser.Parse(content);
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            var url = ConstractRequestUri();
            var persons = await PersonInfoDownloader.Download(url);
            var personsToExport = PersonInfoToCsvConverter.Convert(persons);
            richTextBox1.ResetText();
            richTextBox1.AppendText(personsToExport);
            File.WriteAllText("Test.csv", personsToExport);
        }

        private Uri ConstractRequestUri()
        {
            var position = tbxPosition.Text;

            var categoryFacet = (FacetItemInfo)cbxCategory.SelectedItem;

            return
                new Uri(
                    string.Format(
                        "https://api.linkedin.com/v1/people-search:(people:(first-name,last-name,headline,positions))?facets={0}&facet={1}&facet={2}&title={3}&current-title=true",
                        Uri.EscapeDataString("location,industry"),
                        //Uri.EscapeDataString("location,ru:7487,ru:7481"),
                        Uri.EscapeDataString("location"),
                        Uri.EscapeDataString("industry," + categoryFacet.Id),
                        position));
        }
    }
}
