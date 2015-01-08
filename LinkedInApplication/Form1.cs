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
using System.Diagnostics;
using System.Text;

namespace LinkedInApplication
{
  public partial class Form1 : Form
  {
      private Dictionary<string, FacetItemInfo> locatonsDictionary;

    public Form1()
    {
      InitializeComponent();
      locatonsDictionary = new Dictionary<string, FacetItemInfo>();
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
        foreach (var item in locations)
        {
          cbxLocations.Items.Add(item);
          if (!locatonsDictionary.ContainsKey(item.Name))
          {
              locatonsDictionary.Add(item.Name, item);
          }
        }
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

      var locations = new StringBuilder("location");
      foreach (var locationItem in cbxLocations.CheckedItems)
      {
          locations.Append("," + locatonsDictionary[locationItem.ToString()].Id);
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
      if (result == System.Windows.Forms.DialogResult.OK)
      {
        foreach (var item in form.LocationInfos)
        {
          if (locatonsDictionary.ContainsKey(item.Name))
            continue;

          cbxLocations.Items.Add(new FacetItemInfo
          {
            Id = item.Id,
            Name = item.Name
          });
          locatonsDictionary.Add(item.Name, new FacetItemInfo()
          {
            Id = item.Id,
            Name = item.Name
          });
        }
      }
    }
  }
}
