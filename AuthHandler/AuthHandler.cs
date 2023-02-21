using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpotifyDisplay.AuthHandler
{
    internal class AuthHandler
    {
        string callbackURL { get; }

        public AuthHandler()
        {
            //Create callback URL
            callbackURL = "http://localhost:4040/callback";

            string authURL = SpotifyAuthLink(); //Generate the auth link for user authentication
            string fp = GenerateLinkOpener(authURL);
            Process.Start("explorer", $"\"{fp}\""); //Open generated link
            
            string code = AuthoriseAccess(); //Get auth
            string token = GetAuthToken(code);
        }

        private string SpotifyAuthLink()
        {
            //Generate the link to use
            string url = "https://accounts.spotify.com/authorize";
            url += "?response_type=code";
            url += $"&client_id={System.IO.File.ReadAllText("clientid.txt")}";
            url += $"&redirect_uri={callbackURL}";
            url += "&scope=ugc-image-upload user-read-playback-state user-modify-playback-state user-read-currently-playing app-remote-control streaming playlist-read-private playlist-read-collaborative playlist-modify-private playlist-modify-public user-follow-modify user-follow-read user-read-playback-position user-top-read user-read-recently-played user-library-modify user-library-read user-read-email user-read-private";
            
            //Save URL to file
            System.IO.File.WriteAllText("url.txt", url);
            return url;
        }

        private string GenerateLinkOpener(string url)
        {
            string template = System.IO.File.ReadAllText("AuthHandler/redirect.html");
            template = template.Replace("#####", url);

            string fileName = $"{new Random().Next(1000).ToString()}.html";
            System.IO.File.WriteAllText(fileName, template);

            return fileName;
        }

        private string AuthoriseAccess()
        {
            HttpListener listener = new HttpListener(); //Create listener
            listener.Prefixes.Add("http://*:4040/"); //Add a binding for the listener
            listener.Start();

            HttpListenerContext context = listener.GetContext(); //Wait for a request

            //Wait for Spotify to send callback
            HttpListenerRequest request = context.Request;

            //Get code from callback
            string code = request.QueryString["code"];
            
            //Build response for the user
            string res = $"<body>Token received. You may close this window.</body>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(res);

            HttpListenerResponse response = context.Response;
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
            listener.Stop();
            
            return code;
        }

        private string GetAuthToken(string code)
        {
            ///<summary>Gets the access token and a refresh token from the spotify API</summary>
            string url = "https://accounts.spotify.com/api/token"; //URL to get token from
            //Attach code, redirect url and grant type

            HttpClient httpClient = new HttpClient(); //Create http client to use for request

            //build body of request
            List<KeyValuePair<string, string>> kp = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("grant_type",  "authorization_code"),
                new KeyValuePair<string, string>("redirect_uri", callbackURL)
            };
            FormUrlEncodedContent content = new FormUrlEncodedContent(kp);

            //Add Headers
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

            //-----Generate auth string
            string cid = System.IO.File.ReadAllText("clientid.txt");
            string csc = System.IO.File.ReadAllText("clientsecret.txt");

            string idsk = $"{cid}:{csc}";
            byte[] stringBytes = System.Text.Encoding.UTF8.GetBytes(idsk);
            //Encode as b64
            idsk = Convert.ToBase64String(stringBytes);

            //content.Headers.Add("Authorization", idsk);
            content.Headers.TryAddWithoutValidation("Authorization", idsk);


            //Send request
            HttpResponseMessage response = httpClient.PostAsync(url, content).Result;
            string res = response.Content.ReadAsStringAsync().Result;
            Debug.WriteLine("Hello there!");
            Debug.WriteLine(res);

            return res;
        }
    }
}
