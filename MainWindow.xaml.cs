using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SpotifyDisplay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Spotify.Spotify spotify;
        Timer updateDisplayTimer;
        FullTrack currentSong;

        public MainWindow()
        {
            //Set Up timer
            updateDisplayTimer = new Timer();
            updateDisplayTimer.Interval = 1000;
            updateDisplayTimer.Elapsed += RefreshDisplay;

            //Set up spotify
            spotify = new Spotify.Spotify();

            //Draw Window
            InitializeComponent();

            updateDisplayTimer.Start(); //Start timer
        }

        private void RefreshDisplay(object? sender, ElapsedEventArgs e)
        {
            ///<summary>Reloads the display content</summary>
            Console.WriteLine("hehehe");
            PlayerCurrentlyPlayingRequest pcpr = new PlayerCurrentlyPlayingRequest();
            CurrentlyPlaying cp = spotify.client.Player.GetCurrentlyPlaying(pcpr).Result;
            
            //If something is playing, keep going
            if (cp != null)
            {
                FullTrack song = (FullTrack)cp.Item;

                if (currentSong != song)
                //if(true)
                {
                    //The song is a different song, poll for details

                    //Get artists
                    List<SimpleArtist> artists = song.Artists;
                    string artistsString = "";
                    foreach(SimpleArtist a in artists)
                    {
                        artistsString += $"{a.Name}, ";
                    }
                    artistsString = artistsString.Remove(artistsString.Length - 2, 2);

                    //Get song title
                    string title = song.Name;

                    //Get Image
                    List<SpotifyAPI.Web.Image> albumArtList = song.Album.Images;
                    SpotifyAPI.Web.Image albumArt = albumArtList[0];
                    string imageFilePath = DownloadImage(albumArt.Url, song.Name);

                    UpdateSongDetails(artistsString, title, imageFilePath);
                }
            }
        }

        private void UpdateSongDetails(string artists, string title, string imageFP)
        {
            Dispatcher.BeginInvoke(new Action(() => {
                artistLabel.Content = artists;
                titleLabel.Content = title;
                albumArtImage.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + imageFP, UriKind.Absolute));
            }), DispatcherPriority.SystemIdle);
        }

        private string DownloadImage(string url, string name)
        {
            string filepath = $"images/{name}.png";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (var s = client.GetStreamAsync(url))
                    {
                        using (var fs = new FileStream(filepath, FileMode.OpenOrCreate))
                        {
                            s.Result.CopyTo(fs);
                        }
                    }
                }
            }
            catch { }
            return filepath;
        }
    }
}
