using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Forms;
using LinkedInApplication.Core;
using System.Configuration;
using System.Text;
using NLog;

namespace LinkedInApplication
{
    public partial class Form1 : Form
    {
        private readonly Logger _log = LogManager.GetLogger("Main");
        private readonly Dictionary<string, FacetItemInfo> _locationsDictionary = new Dictionary<string, FacetItemInfo>();
        private IEnumerable<PersonInfo> _currentResult;

        public Form1()
        {
            _log.Info("Application starting");
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            InitializeComponent();
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            _log.Error(unhandledExceptionEventArgs.ExceptionObject);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            LoadLocations();
            cbxCategory.Items.AddRange(IndustryFacetStaticInfo.IndustryFacets);
            base.OnActivated(e);
        }

        private async void LoadLocations()
        {
            try
            {
                var url = new Uri("https://api.linkedin.com/v1/people-search:(facets:(code,buckets:(code,name)))?facets=location");
                var locations = await LoadLocations(url);

                foreach (var item in locations)
                {
                    cbxLocations.Items.Add(item);
                    if (!_locationsDictionary.ContainsKey(item.Name))
                    {
                        _locationsDictionary.Add(item.Name, item);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                MessageBox.Show("Can't load locations, message: " + ex.Message, "Error");
            }
        }

        private static async Task<IEnumerable<FacetItemInfo>> LoadLocations(Uri url)
        {
            using (var httpClient = new HttpClient())
            {
                var accessToken = ConfigurationManager.AppSettings["AccessToken"];
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var result = await httpClient.GetAsync(url);
                var content = await result.Content.ReadAsStringAsync();
                return FacetItemInfoParser.Parse(content);
            }
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            var accessToken = ConfigurationManager.AppSettings["AccessToken"];
            if (string.IsNullOrEmpty(accessToken))
            {
                MessageBox.Show("You should retrieve access token", "Error");
                return;
            }

            if (string.IsNullOrEmpty(tbxPosition.Text))
            {
                MessageBox.Show("You should enter position to search", "Error");
                return;
            }

            if (cbxCategory.SelectedItem == null)
            {
                MessageBox.Show("You should enter category to search in", "Error");
                return;
            }

            var url = ConstractRequestUri();
            _log.Info("Search uri: " + url);

            btnExport.Enabled = false;
            btnSearch.Enabled = false;
            btnAddLocation.Enabled = false;
            btnClearLocations.Enabled = false;
            listView1.Items.Clear();
            lblTotal.Text = "Total rows: 0";

            try
            {
                var persons = await PersonInfoDownloader.Download(url, ConfigurationManager.AppSettings["AccessToken"]);
                persons = await CompanyInfoDownloader.Download(persons, ConfigurationManager.AppSettings["AccessToken"]);
                PersonInfoDownloader.PostProcess(persons);
                _currentResult = persons;
                LoadResultTable(persons);

                var count = persons.Count();
                _log.Info("Search completed: " + count);
                lblTotal.Text = "Total rows: " + count;
                btnExport.Enabled = true;
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                MessageBox.Show("Can't load search results, message: " + ex.Message, "Error");
            }

            btnSearch.Enabled = true;
            btnAddLocation.Enabled = true;
            btnClearLocations.Enabled = true;
        }

        private void LoadResultTable(IEnumerable<PersonInfo> persons)
        {
            foreach (var person in persons)
            {
                var items = new[]
                            {
                                person.CompanyName,
                                person.FirstName,
                                person.LastName,
                                person.Headline,
                                person.CompanyWebSite,
                                person.CompanyLocation,
                                person.Email
                            };
                var listViewItem = new ListViewItem(items);
                listView1.Items.Add(listViewItem);
            }
        }

        private Uri ConstractRequestUri()
        {
            var position = tbxPosition.Text;
            var categoryFacet = (FacetItemInfo) cbxCategory.SelectedItem;

            var locations = new StringBuilder("location");
            foreach (var locationItem in cbxLocations.CheckedItems)
            {
                locations.Append("," + _locationsDictionary[locationItem.ToString()].Id);
            }

            return
                new Uri(
                    string.Format(
                        "https://api.linkedin.com/v1/people-search:(people:(first-name,last-name,headline,positions,location:(name)))?facets={0}&facet={1}&facet={2}&title={3}&current-title=true",
                        Uri.EscapeDataString("location,industry"),
                        //Uri.EscapeDataString("location,ru:7487,ru:7481"),
                        Uri.EscapeDataString(locations.ToString()),
                        Uri.EscapeDataString("industry," + categoryFacet.Id),
                        position));
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var settingsForm = new SettingsForm();
            settingsForm.ShowDialog();
        }

        private void btnAddLocation_Click(object sender, EventArgs e)
        {
            var form = new AddLocationForm();
            var result = form.ShowDialog();
            
            if (result != DialogResult.OK) 
                return;

            foreach (var item in form.LocationInfos)
            {
                if (_locationsDictionary.ContainsKey(item.Name))
                    continue;

                cbxLocations.Items.Add(new FacetItemInfo
                                       {
                                           Id = item.Id,
                                           Name = item.Name
                                       });
                _locationsDictionary.Add(item.Name, new FacetItemInfo
                                                    {
                                                        Id = item.Id,
                                                        Name = item.Name
                                                    });
            }
        }

        private void btnClearLocations_Click(object sender, EventArgs e)
        {
            _locationsDictionary.Clear();
            cbxLocations.Items.Clear();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
                                  {
                                      RestoreDirectory = true
                                  };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var personsToExport = PersonInfoToCsvConverter.Convert(_currentResult);
                    File.WriteAllText(string.Format("{0}.csv", DateTime.Now.ToString("yyyyMMddHHmmss")), personsToExport);
                    File.WriteAllText(saveFileDialog.FileName, personsToExport);
                }
                catch (Exception exception)
                {
                    _log.Error(exception);
                    MessageBox.Show("Can't save result file, error: " + exception.Message, "Error");
                }
            }
        }
    }
}
