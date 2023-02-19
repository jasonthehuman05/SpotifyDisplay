using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SpotifyAPI.Web;

namespace SpotifyDisplay.SpotifyHandler
{
    internal class SpotifyHandler
    {
        public SpotifyHandler()
        {
            //Create spotifyapi object
            SpotifyClient spotify = new SpotifyClient(File.ReadAllText("clientid.txt"));
        }
    }
}
