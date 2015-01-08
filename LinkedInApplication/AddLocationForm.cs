using LinkedInApplication.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LinkedInApplication
{
  public partial class AddLocationForm : Form
  {
    private Dictionary<string, LocationInfo> locatonsDictionary;

    public AddLocationForm()
    {
      InitializeComponent();
      locatonsDictionary = new Dictionary<string, LocationInfo>();
    }

    private async void textBox1_TextChanged(object sender, EventArgs e)
    {
      Debug.WriteLine(((TextBox) sender).Text);

      try
      {
        using (var httpClient = new HttpClient())
        {
          var locations = await LocationInfoDownloader.Download(((TextBox) sender).Text);
          checkedListBox1.Items.Clear();
          locatonsDictionary.Clear();
          checkedListBox1.Items.AddRange(locations.Select(x => x.Name).ToArray());
          foreach (var locationINfo in locations)
          {
            if (locatonsDictionary.ContainsKey(locationINfo.Name))
              continue;

            locatonsDictionary.Add(locationINfo.Name, locationINfo);
          }
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.Message);
      }
    }

    public IEnumerable<LocationInfo> LocationInfos
    {
      get
      {
        var infos = new List<LocationInfo>();
        foreach (var info in checkedListBox1.CheckedItems)
        {
          infos.Add(locatonsDictionary[info.ToString()]);
        }
        return infos;
      }
    }
  }
}
