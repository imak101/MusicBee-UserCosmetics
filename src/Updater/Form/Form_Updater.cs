using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using MusicBeePlugin.Form.Popup;

namespace MusicBeePlugin.Updater.Form
{
    public partial class Form_Updater : System.Windows.Forms.Form
    {
        private const string _GHRepo = "https://github.com/imak101/MusicBee-UserCosmetics";

        private GitHubRelease _GHRelease;

        private bool _releaseFailed;
        private bool _versionSame;
           
        public Form_Updater()
        {
            InitializeComponent();
            CenterToParent();
        }

        private async void Form_Updater_Load(object sender, EventArgs e)
        {
            button_update.Enabled = false;
            
            label_localVer.Text = $"Local Version:\nv{Plugin.About.VersionToString()}";
            
           _GHRelease = await new GitHubRelease().MakeGitHubReleaseAsync();
          
            VersionFiller(ref _GHRelease);
            
            VersionCompare(ref _GHRelease);

            if (!_releaseFailed) link_GHCurrent.Text = $"Patch Notes for {_GHRelease.TagName}";

            if (!_versionSame && !_releaseFailed) button_update.Enabled = true;
        }

        private void VersionFiller(ref GitHubRelease release)
        {
            if (Regex.IsMatch(release.TagName, "error"))
            {
                label_GHCurrentVer.Text = release.TagName;
                _releaseFailed = true;
                return;
            }
            
            label_GHCurrentVer.Text = $"Most current version:\n{_GHRelease.TagName}";
            _releaseFailed = false;
        }

        private void VersionCompare(ref GitHubRelease release)
        {
            if (Regex.IsMatch($"v{Plugin.About.VersionToString()}", release.TagName))
            {
                label_verCompare.Text = $"Plugin '{Plugin.About.Name}' is up-to-date with the GitHub repo!";
                _versionSame = true;
                return;
            }

            if (_releaseFailed)
            {
                label_verCompare.Text = "Unable to fetch current version.";
                return;
            }

            label_verCompare.Text = "This plugin is not current with the GitHub repo. Please press the 'update' button to automatically update.";
            _versionSame = false;
        }

        public async Task<string> ConfigureFormLabelHandler()
        {
            GitHubRelease configRelease = await new GitHubRelease().MakeGitHubReleaseAsync();
            
            if (Regex.IsMatch($"v{Plugin.About.VersionToString()}", configRelease.TagName))
            {
                return "Up-to-date!";
            }

            if (Regex.IsMatch(configRelease.TagName, "error"))
            {
                return "Failed to fetch data!";
            }

            return $"New version ({configRelease.TagName}) available!";
        }

        private void link_GHRepo_MouseHover(object sender, EventArgs e)
        { 
            toolTip_GHLink.SetToolTip(link_GHRepo.Controls.Owner, _GHRepo);
        }

        private void link_GHRepo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            TopMost = false;
            System.Diagnostics.Process.Start(_GHRepo);
        }

        private void link_GHCurrent_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                TopMost = false;

                System.Diagnostics.Process.Start(_GHRelease.HtmlUrl);
            }
            catch (Exception)    
            {
                new Form_Popup("Unable to open link. You may be offline or rate limited, please try again in 1 hour.", "Error");
            }
        }

        private void link_GHCurrent_MouseHover(object sender, EventArgs e)
        {
            try
            {
                toolTip_GHLink.SetToolTip(link_GHCurrent.Controls.Owner, _GHRelease.HtmlUrl);
            }
            catch (Exception)
            {
                toolTip_GHLink.SetToolTip(link_GHCurrent.Controls.Owner, "An error occurred.");
            }
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button_refresh_Click(object sender, EventArgs e)
        {
            label_GHCurrentVer.Text = "Fetching most recent version...";
            label_verCompare.Text = "Awaiting data...";
            Form_Updater_Load(sender, e);
        }

        private async void button_update_Click(object sender, EventArgs e)
        {
            button_update.Enabled = false;
            
            await _GHRelease.DownloadLatest();
        }
    }
}