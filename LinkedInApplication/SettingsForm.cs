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
      textBox1.Text = ConfigurationManager.AppSettings["AccessToken"];
      tbxApiKey.Text = ConfigurationManager.AppSettings["ApiKey"];
      tbxSecretKey.Text = ConfigurationManager.AppSettings["SecretKey"];
    }

    private void button1_Click(object sender, EventArgs e)
    {
      var accessToken = textBox1.Text;
      var apiKey = tbxApiKey.Text;
      var secretKey = tbxSecretKey.Text;
      var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      config.AppSettings.Settings.Remove("AccessToken");
      config.AppSettings.Settings.Remove("ApiKey");
      config.AppSettings.Settings.Remove("SecretKey");
      config.AppSettings.Settings.Add("AccessToken", accessToken);
      config.AppSettings.Settings.Add("ApiKey", apiKey);
      config.AppSettings.Settings.Add("SecretKey", secretKey);
      config.Save(ConfigurationSaveMode.Full);
      ConfigurationManager.RefreshSection("appSettings");
    }

    private void btnGenerate_Click(object sender, EventArgs e)
    {
        var text = string.Format("https://www.linkedin.com/uas/oauth2/accessToken?grant_type=authorization_code" +
                                "&code=AQTH4piKPjgAvf7JaDLSfe-Y58QyzoM44vO4DVTrMfSYtTbzFtgulyohFwhaLY-po7m3ZIOwl9mVEXBjOH6mSQPCqU1NT2chmaODVRUmh5NiNbpohis" +
                                "&redirect_uri=http://skilleo.herokuapp.com" +
                                "&client_id=npq2v0qgrl9y" +
                                "&client_secret=50DUgVK6Cl6RwMRy", tbxApiKey.Text, Guid.NewGuid());
      textBox2.Text = text;
    }
  }
}
