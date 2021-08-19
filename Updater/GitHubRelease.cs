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
                
                // client.DownloadFile(new Uri ("https://github.com/imak101/MusicBeeSomething1/releases/download/v0.2.0/mb_Something1.dll"), pluginFolderPath + wwpp);
                
                client.DownloadFile(new Uri(await ParseResponseStringForValueFromKey("browser_download_url", true)), pluginFolderPath + urlDllName);

                if (urlDllName != Plugin.About.ProjectName + ".dll")
                {
                    XmlDocument mbIni = new XmlDocument(); mbIni.Load(mbSettingsIniPath);


                    try
                    {
                        XmlNodeList xmlEmu = mbIni.GetElementsByTagName("SystemPlugin").Item(0).ChildNodes;
                        
                        foreach (XmlNode systemPluginChild in xmlEmu)
                        {
                            foreach (XmlNode stateChild in systemPluginChild.ChildNodes)
                            {
                                if (stateChild.Name == "Id" && Regex.Match(stateChild.InnerText, $"{Plugin.About.ProjectName}.dll").Success)
                                {
                                    stateChild.InnerText = Path.Combine(pluginFolderPath, urlDllName);
                                    
                                    mbIni.Save(Path.Combine(Plugin.About.PersistentStoragePath, "MusicBee3Settings.ini"));

                                    await Task.Delay(1000);
                                    
                                    //Program restarts and maybe edits the ini file as it restarts?? file not saving properly
                                }
                            }
                        }
                    }
                    
                    catch (Exception e)
                    {
                        new MusicBeePlugin.Form.Popup.Form_Popup("An error occured while installing the new update. Reinstalling MusicBee may fix the problem.", "Error");
                    }
                }

                //FileStream fileLock = new FileStream(mbSettingsIniPath, FileMode.Open, FileAccess.Read, FileShare.None);

                Application.Restart();
            }
        } 
    }
}