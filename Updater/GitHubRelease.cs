using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace MusicBeePlugin.Updater
{
    public class GitHubRelease : ApiRequests
    {
        public string TagName { get; }
        public string HtmlUrl { get; }
        
        /// <summary>
        /// Must call MakeGitHubReleaseAsync() to populate class data
        /// </summary>
        public GitHubRelease(){}

        private GitHubRelease(string tagName, string htmlUrl)
        {
            TagName = tagName;
            HtmlUrl = htmlUrl;
            //TODO: implement datetime
        }
        
        public async Task<GitHubRelease> MakeGitHubReleaseAsync()
        {
            return new GitHubRelease(await ParseResponseStringForValueFromKey("tag_name"), await ParseResponseStringForValueFromKey("html_url", true));
        }

        public async Task DownloadLatest()
        {
            using (WebClient client = new WebClient())
            {
                string pluginPath = Path.Combine(Application.StartupPath, $"Plugins\\{Plugin.About.ProjectName}.dll");
                string pluginFolderPath = Path.Combine(Application.StartupPath, "Plugins\\");
                string mbSettingsIniPath = Path.Combine(Plugin.About.PersistentStoragePath, "MusicBee3Settings.ini");
                
                File.Move(pluginPath, pluginFolderPath + "old.dll");

                string downloadUrl = await ParseResponseStringForValueFromKey("browser_download_url", true);
                string urlDllName = Regex.Match(downloadUrl, "mb_[\\S]*").Value;
                
                client.DownloadFile(new Uri(await ParseResponseStringForValueFromKey("browser_download_url", true)), pluginFolderPath + urlDllName);

                if (urlDllName != Plugin.About.ProjectName + ".dll")
                {
                    var form = new MusicBeePlugin.Form.Popup.Form_Popup("A name-change has been detected for this release. You may need to re-add the plugin in the \"Arrange Panels\" setting.", "Warning");

                    form.Disposed += (sender, args) => Application.Restart();

                    await Task.Delay(50000);
                }
                
                //TODO: gray out update button when update not needed, change setting file's name
                
                Application.Restart();
            }
        } 
    }
}