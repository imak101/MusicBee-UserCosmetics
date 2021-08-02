using System;
using System.Threading.Tasks;

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
            return new GitHubRelease(await ParseResponseStringForValueFromKey("tag_name"), await ParseResponseStringForValueFromKey("html_url"));
        }
    }
}