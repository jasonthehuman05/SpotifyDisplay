using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;

namespace SpotifyDisplay.Spotify
{
    internal class AuthHandler
    {
        EmbedIOAuthServer _server;
        public string token = "";
        private string id;
        private string secret;

        public AuthHandler(){
            //Create authhandler server
            _server = new EmbedIOAuthServer(new Uri("http://localhost:4040/callback"), 4040);
            _server.Start();
            _server.AuthorizationCodeReceived += AuthCodeReceived;
            _server.ErrorReceived += ErrorThrown;

            //Create Login request
            id = File.ReadAllText("clientid.txt");
            secret = File.ReadAllText("clientsecret.txt");
            string scopes = File.ReadAllText("scopes.txt");
            List<string> scope = scopes.Split(" ").ToList();

            LoginRequest request = new LoginRequest(_server.BaseUri, id, LoginRequest.ResponseType.Code)
            {
                Scope = scope
            };

            BrowserUtil.Open(request.ToUri());
        }

        private Task ErrorThrown(object sender, string error, string? state)
        {
            _server.Stop();
            Debug.WriteLine(error);
            return Task.CompletedTask;
        }

        private Task AuthCodeReceived(object sender, AuthorizationCodeResponse response)
        {
            _server.Stop();

            var config = SpotifyClientConfig.CreateDefault();
            AuthorizationCodeTokenResponse tokenResponse = new OAuthClient(config).RequestToken(
              new AuthorizationCodeTokenRequest(
                id, secret, response.Code, new Uri("http://localhost:4040/callback")
              )
            ).Result;

            token = tokenResponse.AccessToken;

            return Task.CompletedTask;
        }
    }
}
