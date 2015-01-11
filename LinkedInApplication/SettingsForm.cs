using System;
using System.Configuration;
using System.Windows.Forms;

namespace LinkedInApplication
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            tbxAccessToken.Text = ConfigurationManager.AppSettings["AccessToken"];
            tbxApiKey.Text = ConfigurationManager.AppSettings["ApiKey"];
            tbxSecretKey.Text = ConfigurationManager.AppSettings["SecretKey"];
            tbxRedirectUri.Text = ConfigurationManager.AppSettings["RedirectUri"];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove("AccessToken");
            config.AppSettings.Settings.Remove("ApiKey");
            config.AppSettings.Settings.Remove("SecretKey");
            config.AppSettings.Settings.Remove("RedirectUri");
            config.AppSettings.Settings.Add("AccessToken", tbxAccessToken.Text);
            config.AppSettings.Settings.Add("ApiKey", tbxApiKey.Text);
            config.AppSettings.Settings.Add("SecretKey", tbxSecretKey.Text);
            config.AppSettings.Settings.Add("RedirectUri", tbxRedirectUri.Text);
            config.Save(ConfigurationSaveMode.Full);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void btnGenerate_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbxApiKey.Text))
            {
                MessageBox.Show("You should enter API KEY", "Error");
                return;
            }

            if (string.IsNullOrEmpty(tbxSecretKey.Text))
            {
                MessageBox.Show("You should enter Secret Key", "Error");
                return;
            }

            if (string.IsNullOrEmpty(tbxRedirectUri.Text))
            {
                MessageBox.Show("You should enter redirect Uri", "Error");
                return;
            }

            var authForm = new GetTokenForm(tbxApiKey.Text, tbxSecretKey.Text, tbxRedirectUri.Text);
            authForm.ShowDialog();
            tbxAccessToken.Text = authForm.AccessToken;
        }
    }
}
