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

namespace LinkedInApplication
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
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
          var locations = await LoadLocations(new Uri("https://api.linkedin.com/v1/people-search:(facets:(code,buckets:(code,name)))?facets=location"));
          cbxLocations.Items.AddRange(locations.ToArray());
      }
      catch (Exception ex)
      {
        MessageBox.Show("Error", "Can't load locations, message: " + ex.Message);
      }
    }

    private static async Task<IEnumerable<FacetItemInfo>> LoadLocations(Uri url)
    {
      using (var httpClient = new HttpClient())
      {
          httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigurationManager.AppSettings["AccessToken"]);
        var result = await httpClient.GetAsync(url);
        var content = await result.Content.ReadAsStringAsync();
        return FacetItemInfoParser.Parse(content);
      }
    }

    private async void btnSearch_Click(object sender, EventArgs e)
    {
      var url = ConstractRequestUri();
      string personsToExport;
      
      try
      {
        var persons = await PersonInfoDownloader.Download(url);
        persons = await CompanyInfoDownloader.Download(persons);
        personsToExport = PersonInfoToCsvConverter.Convert(persons);
        richTextBox1.ResetText();
        richTextBox1.AppendText(personsToExport);
      }
      catch (Exception ex)
      {
          MessageBox.Show("Error", "Can't load search results, message: " + ex.Message);
        return;
      }

      try
      {
        File.WriteAllText(string.Format("{0}.csv", DateTime.Now.ToString("yyyyMMddhhmmss")), personsToExport);
      }
      catch (Exception ex)
      {
          MessageBox.Show("Error", "Can't save result file, message: " + ex.Message);
      }
    }

    private Uri ConstractRequestUri()
    {
      var position = tbxPosition.Text;

      var categoryFacet = (FacetItemInfo) cbxCategory.SelectedItem;

      return
        new Uri(
          string.Format(
            "https://api.linkedin.com/v1/people-search:(people:(first-name,last-name,headline,positions,location:(name)))?facets={0}&facet={1}&facet={2}&title={3}&current-title=true",
            Uri.EscapeDataString("location,industry"),
            //Uri.EscapeDataString("location,ru:7487,ru:7481"),
            Uri.EscapeDataString("location"),
            Uri.EscapeDataString("industry," + categoryFacet.Id),
            position));
    }

    private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      var settingsForm = new SettingsForm();
      settingsForm.ShowDialog();
    }
  }
}
