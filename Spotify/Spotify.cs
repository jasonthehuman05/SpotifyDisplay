using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpotifyAPI.Web;

namespace SpotifyDisplay.Spotify
{
    internal class Spotify
    {
        private string accessToken;
        public SpotifyClient client;
        
        public Spotify()
        {
            AuthHandler auth = new AuthHandler();
            while(auth.token == "") { } //Wait until token generated
            accessToken = auth.token;
            client = new SpotifyClient(accessToken);
            client.Player.SkipNext();
        }
    }
}
