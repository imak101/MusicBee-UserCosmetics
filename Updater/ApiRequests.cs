using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using MusicBeePlugin.Form.Configure;
using MusicBeePlugin.Form.Popup;


namespace MusicBeePlugin.Updater
{
    public abstract class ApiRequests
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly Uri _uriReleases = new Uri("https://api.github.com/repos/imak101/MusicBee-UserCosmetics/releases");
        //private readonly Uri _uriReleases = new Uri("http://127.0.0.1:5000/releases");

        private string _currentResponse;
       

        private async Task<HttpResponseMessage> ReleasesResponseRaw()
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, _uriReleases))
            {
                request.Headers.UserAgent.Add(ProductInfoHeaderValue.Parse("request"));
                

                try
                {
                    return await _client.SendAsync(request);
                }
                catch (Exception)
                {
                    if (!Form_Configure.CheckOpened("Update Menu Connectivity Error"))
                    {
                        new Form_Popup("An error occured.\n You may be offline.\n The menu will have limited functionality.", "Update Menu Connectivity Error");
                    }

                    return new HttpResponseMessage {StatusCode = HttpStatusCode.BadRequest};
                }
                
            }
        }

        /// <returns>Null if response was not successful</returns>
        private async Task<string> ReleasesResponseString()
        {
            HttpResponseMessage response = await ReleasesResponseRaw();

            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadAsStringAsync();
        }
        

        /// <param name="rapidSuccession">If true, uses the last most API response to save API calls for method calls in rapid succession</param>
        protected async Task<string> ParseResponseStringForValueFromKey(string key, bool rapidSuccession = false)
        {
            if (rapidSuccession && _currentResponse != null) goto Matching;
            
            _currentResponse = await ReleasesResponseString();  

            if (_currentResponse == null) return "An error occured. You may be offline or rate limited, please try again later. (one hour)";

            Matching:
            try
            {
                string match = Regex.Match(_currentResponse, $"\"{key}[^,}}\n]*").Value;
                match = match.Replace("\"", " ").Trim();
                string[] splitS = match.Split(new []{':'}, 2);

                return splitS[1].Trim();
            }
            catch (Exception e)
            {
                return $"An exception ({e.Message}) occured while parsing ResponseString for a key.";
            }
        }
    }
}