using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using PalringoInformationScraper.Core.Models;

namespace PalringoInformationScraper.Core.Providers
{
    public class PalringoProvider : IDisposable
    {
        private const string UserAgent 
            = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36";

        private readonly HttpClient _client;

        public PalringoProvider()
        {
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            };
            _client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://www.palringo.com")
            };
            _client.DefaultRequestHeaders.UserAgent.TryParseAdd(UserAgent);
        }

        public async Task<bool> Login(NetworkCredential credential)
        {
            var postData = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "email", credential.UserName },
                { "password", credential.Password }
            });
            var response = await _client.PostAsync("en/gb/signin", postData);
            var content = await response.Content.ReadAsStringAsync();
                                                                 
            return !content.Contains("Login incorrect");
        }

        public async Task<AccountInformation> GetInformationAsync()
        {
            var parser = new HtmlParser();    
            var content = await _client.GetStringAsync("en/gb/dashboard");
            using (var document = parser.Parse(content))
            {
                var dtTags = document.QuerySelectorAll("dt");
                var balance = dtTags.First(tag => tag.TextContent == "Credit Balance").NextElementSibling.TextContent.TrimStart();
                var username = document.QuerySelector(".username").TextContent;
                var email = document.QuerySelectorAll(".status")[1].TextContent;
                var userId = dtTags.First(tag => tag.TextContent == "Palringo User ID").NextElementSibling.TextContent;
                var avatarLink = "showavatar.php?id=" + userId;
                var reputation = dtTags.First(tag => tag.TextContent == "Reputation").NextElementSibling.TextContent.TrimStart();
                var joinedText = dtTags.First(tag => tag.TextContent == "Joined").NextElementSibling.TextContent;
                var joined = DateTime.ParseExact(joinedText, "dd MMM yyyy", CultureInfo.InvariantCulture);

                return new AccountInformation(_client, balance, avatarLink, username, email, userId, reputation, joined); 
            }  
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}