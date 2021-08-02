using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using MusicBeePlugin.Form.Popup;


namespace MusicBeePlugin.Updater
{
    public abstract class ApiRequests
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly Uri _uriReleases = new Uri("https://api.github.com/repos/imak101/MusicBeeSomething1/releases");

        protected ApiRequests() {}
        
        private async Task<HttpResponseMessage> ReleasesResponseRaw()
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, _uriReleases))
            {
                request.Headers.UserAgent.Add(ProductInfoHeaderValue.Parse("request"));

                return await _client.SendAsync(request);
            }
        }

        private async Task<string> ReleasesResponseString()
        {
            HttpResponseMessage response = await ReleasesResponseRaw();

            return await response.Content.ReadAsStringAsync();
        }

        protected async Task<string> ParseResponseStringForValueFromKey(string key)
        {
            string response = await ReleasesResponseString();

            try
            {
                string match = Regex.Match(response, $"\"{key}[^,]*").Value;

                match = match.Replace("\"", " ").Trim();
                string[] splitS = match.Split(new []{':'}, 2);

                return splitS[1].Trim();
            }
            catch (Exception e)
            {
                return $"An exception ({e.Message}) occured while parsing ResponseString from a key.";
            }
        }
        
        public async void sss()
        {
            Debug.WriteLine(await ReleasesResponseString());
            Debug.WriteLine(await ParseResponseStringForValueFromKey("html_url"));
            Debug.WriteLine(await ParseResponseStringForValueFromKey("published_at"));
            Debug.WriteLine(await ParseResponseStringForValueFromKey("tag_name"));
            //GitHubRelease ghRelease = await MakeGitHubRelease();
            //new Form_Popup($"{ghRelease.HtmlUrl}...\n..{ghRelease.TagName}", "TITLE");
        }
    }
}