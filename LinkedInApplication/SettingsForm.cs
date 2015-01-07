using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LinkedInApplication
{
  public partial class SettingsForm : Form
  {
    public SettingsForm()
    {
      InitializeComponent();
      var accessToken = ConfigurationManager.AppSettings["AccessToken"];
      textBox1.Text = accessToken;
    }

    private void button1_Click(object sender, EventArgs e)
    {
      var accessToken = textBox1.Text;
      var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      config.AppSettings.Settings.Remove("AccessToken");
      config.AppSettings.Settings.Add("AccessToken", textBox1.Text);
      config.Save(ConfigurationSaveMode.Full);
      ConfigurationManager.RefreshSection("appSettings");
    }
  }
}
