using System;                   
using System.Drawing;       
using System.Net.Http;
using System.Threading.Tasks;

namespace PalringoInformationScraper.Core.Models
{
    public class AccountInformation
    {
        private readonly HttpClient _client;

        public string Balance { get; }

        public string AvatarLink { get; }

        public Image Avatar { get; private set; }

        public string Username { get; }

        public string Email { get; }

        public string UserId { get; }

        public string Reputation { get; }    

        public DateTime Joined { get; }

        public async Task<Image> GetAvatarAsync()
        {
            if (Avatar != null)
                return Avatar;

            using (var avatarStream = await _client.GetStreamAsync(AvatarLink))      
                return Avatar = Image.FromStream(avatarStream); 
        }

        public AccountInformation(HttpClient client, string balance, string avatarLink, string username, string email, string userId, string reputation, DateTime joined)
        {
            _client = client;
            Balance = balance; 
            AvatarLink = avatarLink;   
            Username = username;
            Email = email;
            UserId = userId;
            Reputation = reputation; 
            Joined = joined;
        }
    }
}                            