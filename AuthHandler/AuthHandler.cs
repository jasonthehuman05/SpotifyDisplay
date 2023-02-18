using System;
using System.Net;

namespace SpotifyDisplay.AuthHandler
{
    internal class AuthHandler
    {
        public AuthHandler()
        {
            HttpListener listener = new HttpListener(); //Create listener
            listener.Prefixes.Add("http://*:4040/"); //Add a binding for the listener
            listener.Start();
            
            HttpListenerContext context = listener.GetContext(); //Wait for a request
            
            //Get Request
            HttpListenerRequest request = context.Request;
            Console.WriteLine(request.Headers);

            //Create Response content
            string res = $"<body>HELLO WORLD!</body>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(res);

            HttpListenerResponse response = context.Response;
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
            listener.Stop();
        }
    }
}
