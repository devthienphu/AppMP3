using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using xNet;


namespace Clone_app_Zing_Mp3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Song> listSongPerson;
        private ObservableCollection<Song> listSongDiscover;
        private ObservableCollection<Song> listSongSearch;
        private ObservableCollection<Song> listSongAsian;
        private ObservableCollection<Song> listSongBXH;
        
        private Song currentSong;
        public ObservableCollection<Song> ListSongPerson { get => listSongPerson; set => listSongPerson = value; }
        public ObservableCollection<Song> ListSongDiscover { get => listSongDiscover; set => listSongDiscover = value; }
        public ObservableCollection<Song> ListSongSearch { get => listSongSearch; set => listSongSearch = value; }
        public ObservableCollection<Song> ListSongAsian { get => listSongAsian; set => listSongAsian = value; }
        public ObservableCollection<Song> ListSongBXH { get => listSongBXH; set => listSongBXH = value; }
        

        public Song CurrentSong { get => currentSong; set => currentSong = value; }
        

        public MainWindow()
        {
            InitializeComponent();
         
            ListSongPerson = new ObservableCollection<Song>();
            ListSongDiscover = new ObservableCollection<Song>();
            ListSongSearch = new ObservableCollection<Song>();
            ListSongAsian = new ObservableCollection<Song>();
            ListSongBXH = new ObservableCollection<Song>();
      
            ucPlayBack.BackToMain += UcPlayBack_BackToMain;

            

            Thread thrdPerson = new Thread(CrawlMusicPerson);
            thrdPerson.IsBackground = true;
            thrdPerson.Start();

            Thread thrdDiscover = new Thread(CrawlMusicDiscover);
            thrdDiscover.IsBackground = true;
            thrdDiscover.Start();

            Thread thrdAsian = new Thread(CrawlMusicAsian);
            thrdAsian.IsBackground = true;
            thrdAsian.Start();

            Thread thrdBXH = new Thread(CrawlMusicBXH);
            thrdBXH.IsBackground = true;
            thrdBXH.Start();

        }

        private void UcPlayBack_BackToMain(object sender, EventArgs e)
        {
            MainControl.Visibility = Visibility.Visible;
            ucPlayBack.Visibility = Visibility.Hidden;
        }

        private void PlayMusic_Click(object sender, RoutedEventArgs e)
        {
            Song song = (sender as Button).DataContext as Song;
            CurrentSong = song;
            MainControl.Visibility = Visibility.Hidden;
            ucPlayBack.Visibility = Visibility.Visible;

            ucPlayBack.SongInfo = song;
        }

        //0-14
        void CrawlMusicPerson()
        {
            HttpRequest http = new HttpRequest();
            string htmlMusic = "";
            try
            {
               htmlMusic = http.Get(@"https://nhacvietnam.mobi/").ToString();
            }
            catch
            {
                htmlMusic = http.Get(@"https://nhacvietnam.mobi/").ToString();
            }
            string musicPattern = @"<div class=""menu"">(.*?)</div>";
            var listMusic = Regex.Matches(htmlMusic, musicPattern, RegexOptions.Singleline);
            
            for (int i = 0; i < 15; i++)
            {
                string tempSong = listMusic[i].ToString();
                string songPattern = @"<a title=""(.*?)""";
                var SongName = Regex.Matches(tempSong, songPattern, RegexOptions.Singleline);
                string nameSong = SongName[0].ToString().Replace("<a title=\"Tải bài hát ", "").Replace("\"", "").Replace("?", "");

                songPattern = @"href=""(.*?)""";
                var UrlSong = Regex.Matches(tempSong, songPattern, RegexOptions.Singleline);
                string songUrl = UrlSong[0].ToString().Replace(@"href=""", "").Replace("\"", "");

                songPattern = @"src=""(.*?)""";
                var UrlImage = Regex.Matches(tempSong, songPattern, RegexOptions.Singleline);
                string imageUrl = UrlImage[0].ToString().Replace(@"src=""", "").Replace("\"", "");


                string imageName = AppDomain.CurrentDomain.BaseDirectory + "ImageSource\\" + nameSong + ".jpg";

                if (!File.Exists(imageName)) { File.WriteAllBytes((imageName), http.Get(imageUrl).ToMemoryStream().ToArray()); }

                songPattern = @"<div id=""fulllyric"">(.*?)</div>";
                string htmlSong = http.Get(songUrl).ToString();
                var lyric = Regex.Matches(htmlSong, songPattern, RegexOptions.Singleline);
                string templyric = "Chưa có lyric";
                if (lyric.Count > 0)
                {
                   templyric = lyric[0].ToString().Replace(@"<div id=""fulllyric"">", "").Replace(@"<br />", "").Replace(@"</div>", "").Replace("                                                                                                    ","");
                }

                //songPattern = @"<source src=(.*?)>";
                //var loadUrl = Regex.Matches(htmlSong, songPattern, RegexOptions.Singleline) ;
                //string downloadUrl = loadUrl[0].ToString().Replace(@"<source src=""", "").Replace(@""" type=""audio/mpeg"" />", "");
                string downloadUrl = songUrl.Replace("nhacmp3", "listen");

                string pathLink= AppDomain.CurrentDomain.BaseDirectory + "Song\\" + nameSong + ".mp3";

                Application.Current.Dispatcher.Invoke(new Action(() => {

                    ListSongPerson.Add(new Song() { SongName = nameSong, SongUrl = songUrl, SongImageUrl = imageUrl, ImageName = imageName, Lyric = templyric, DownloadUrl = downloadUrl, PathLink = pathLink });

                }));

                
            }

            Application.Current.Dispatcher.Invoke(new Action(() => {

                if(ListViewMusic.ItemsSource == null )
                ListViewMusic.ItemsSource = ListSongPerson;
            }));
                  
        }
         
        //15 - 30
        void CrawlMusicDiscover()
        {
            HttpRequest http = new HttpRequest();
            string htmlMusic = "";
            try
            {
                htmlMusic = http.Get(@"http://imuzik.com.vn/bang-xep-hang").ToString();
            }
            catch
            {
                htmlMusic = http.Get(@"http://imuzik.com.vn/bang-xep-hang").ToString();
            }
            
            string songPattern = @"<div class=""media"">(.*?)</div>";
            string songPatternTitle = @"<div class=""media-body"">(.*?)</div>";
            var listImage = Regex.Matches(htmlMusic, songPattern, RegexOptions.Singleline);
            var listTitle = Regex.Matches(htmlMusic, songPatternTitle, RegexOptions.Singleline);
            string UrlBase = @"http://imuzik.com.vn";

            for(int i = 15; i < 31; i++)
            {
                string tempImage = listImage[i].ToString();
                string tempTitle = listTitle[i].ToString();

                songPattern = @"href=""(.*?)""";
                var UrlSong = Regex.Matches(tempImage, songPattern, RegexOptions.Singleline);
                string songUrl = UrlBase + UrlSong[0].ToString().Replace(@"href=""", "").Replace("\"", "");

                songPatternTitle = @"title=""(.*?)""";
                var SongName= Regex.Matches(tempTitle, songPatternTitle, RegexOptions.Singleline);
                string nameSong = SongName[0].ToString().Replace(@"title=""", "").Replace("\"", "").Replace("?", "");
                nameSong += " - " + SongName[1].ToString().Replace(@"title=""", "").Replace("\"", "");

                songPattern = @"src=""(.*?)""";
                var UrlImage = Regex.Matches(tempImage, songPattern, RegexOptions.Singleline);
                string imageUrl = UrlImage[0].ToString().Replace(@"src=""", "").Replace("\"", "");

                string imageName = AppDomain.CurrentDomain.BaseDirectory+ "ImageSource\\" + nameSong  + ".jpg";
                if (!File.Exists(imageName)) { File.WriteAllBytes((imageName), http.Get(imageUrl).ToMemoryStream().ToArray()); }
                

                string templyric = "Chưa có lyric";

                songPattern = @"<input type=""hidden"" id=""audio_file_path"" value=""(.*?)""";
                string htmlUrl = http.Get(songUrl).ToString();
                var loadUrl = Regex.Matches(htmlUrl, songPattern, RegexOptions.Singleline);
                string downloadUrl = loadUrl[0].ToString().Replace(@"<input type=""hidden"" id=""audio_file_path"" value=""", "").Replace(@"""","");
                
                string pathLink= AppDomain.CurrentDomain.BaseDirectory + "Song\\" + nameSong + ".mp3";


                Application.Current.Dispatcher.Invoke(new Action(() => {

                    ListSongDiscover.Add(new Song() { SongName = nameSong, SongUrl = songUrl, SongImageUrl = imageUrl, ImageName = imageName, Lyric = templyric, DownloadUrl = downloadUrl, PathLink = pathLink });

                }));


            }
            //  ListViewMusic.ItemsSource = ListSongDiscover; 
        }

        void CrawlMusicSearch()
        {

            Application.Current.Dispatcher.Invoke(new Action(() => {

                if (SearchBar.Text != "")
                {
                    while (ListSongSearch.Count != 0)
                    {
                        ListSongSearch.RemoveAt(0);
                    }
                    string searchText = SearchBar.Text;
                    string baseLinkSearch = @"https://www.nhaccuatui.com/tim-kiem/bai-hat?q=" + WebUtility.UrlEncode(searchText) + @"&b=keyword&l=tat-ca&s=default";

                    HttpRequest http = new HttpRequest();
                    string html = "";
                    try
                    {
                        html = http.Get(baseLinkSearch).ToString();
                    }
                    catch
                    {
                        html = http.Get(baseLinkSearch).ToString();
                    }
                    
                    string Pattern = @"<li class=""sn_search_single_song"">(.*?)</li>";
                    var listDataSong = Regex.Matches(html, Pattern, RegexOptions.Singleline);
                    int count = listDataSong.Count;
                    if (count > 6) count = 6;
                    if (count == 0) MessageBox.Show("Không tìm thấy thông tin");
                    for (int i = 0; i < count; i++)
                    {
                        string tempSong = listDataSong[i].ToString();
                        if (tempSong == "") break;

                        //Pattern =@"blank"">(.*?)</a>";
                        //var Singer= Regex.Matches(tempSong, Pattern, RegexOptions.Singleline);
                        //string singer = Singer[0].ToString().Replace(@"blank"">", "").Replace(@"</a>", "");

                        Pattern = @"=""Nghe bài hát(.*?)ở ";
                        var songTitle = Regex.Matches(tempSong, Pattern, RegexOptions.Singleline);
                        string songName = songTitle[0].ToString().Replace(@"=""Nghe bài hát", "").Replace("ở", "").Replace("?", "");




                        Pattern = @"src=""(.*?)""";
                        var ImageUrl = Regex.Matches(tempSong, Pattern, RegexOptions.Singleline);
                        string imageUrl = ImageUrl[1].ToString().Replace(@"src=""", "").Replace("\"", "");
                        if (imageUrl == "")
                        {
                            imageUrl = ImageUrl[0].ToString().Replace(@"src=""", "").Replace("\"", "");
                        }
                        string imageName = AppDomain.CurrentDomain.BaseDirectory + "ImageSource\\" + songName + ".jpg";
                        if (!File.Exists(imageName)) { File.WriteAllBytes((imageName), http.Get(imageUrl).ToMemoryStream().ToArray()); }

                        string baseUrlSearch = @"https://nhacvietnam.mobi/tim-kiem.php?key=" + WebUtility.UrlEncode(songName) + @"&act=timkiem";
                        string htmlUrl = http.Get(baseUrlSearch).ToString();
                        Pattern = @"<a title=""Tai nhac mp3 "" href=""(.*?)""";
                        var listUrl = Regex.Matches(htmlUrl, Pattern, RegexOptions.Singleline);
                        string songUrl = listUrl[0].ToString().Replace(@"<a title=""Tai nhac mp3 "" href=""", "").Replace("\"", "");


                        Pattern = @"<div id=""fulllyric"">(.*?)</div>";
                        string htmlSong = http.Get(songUrl).ToString();
                        var lyric = Regex.Matches(htmlSong, Pattern, RegexOptions.Singleline);
                        string templyric = "Chưa có lyric";
                        if (lyric.Count > 0)
                        {
                            templyric = lyric[0].ToString().Replace(@"<div id=""fulllyric"">", "").Replace(@"<br />", "").Replace(@"</div>", "").Replace("                                                                                                    ", "");
                        }

                        string downloadUrl = songUrl.Replace("nhacmp3", "listen");

                        string pathLink = AppDomain.CurrentDomain.BaseDirectory + "Song\\" + songName + ".mp3";

                        Application.Current.Dispatcher.Invoke(new Action(() => {

                            ListSongSearch.Add(new Song() { SongName = songName, SongUrl = songUrl, SongImageUrl = imageUrl, ImageName = imageName, DownloadUrl = downloadUrl, Lyric = templyric, PathLink = pathLink });

                        }));
                    }

                    Application.Current.Dispatcher.Invoke(new Action(() => {
                        ListViewMusic.ItemsSource = null;
                        ListViewMusic.ItemsSource = ListSongSearch;
                    }));

                }

            }));
            
        }

        void CrawlMusicAsian()
        {
            HttpRequest http = new HttpRequest();
            string htmlMusic = "";
            try {
                htmlMusic = http.Get(@"http://imuzik.com.vn/bang-xep-hang").ToString();
            }
            catch
            {
                htmlMusic = http.Get(@"http://imuzik.com.vn/bang-xep-hang").ToString();
            }

            string songPattern = @"<div class=""media"">(.*?)</div>";
            string songPatternTitle = @"<div class=""media-body"">(.*?)</div>";
            var listImage = Regex.Matches(htmlMusic, songPattern, RegexOptions.Singleline);
            var listTitle = Regex.Matches(htmlMusic, songPatternTitle, RegexOptions.Singleline);
            string UrlBase = @"http://imuzik.com.vn";

            for (int i = 30; i < 40; i++)
            {
                string tempImage = listImage[i].ToString();
                string tempTitle = listTitle[i].ToString();

                songPattern = @"href=""(.*?)""";
                var UrlSong = Regex.Matches(tempImage, songPattern, RegexOptions.Singleline);
                string songUrl = UrlBase + UrlSong[0].ToString().Replace(@"href=""", "").Replace("\"", "");

                songPatternTitle = @"title=""(.*?)""";
                var SongName = Regex.Matches(tempTitle, songPatternTitle, RegexOptions.Singleline);
                string nameSong = SongName[0].ToString().Replace(@"title=""", "").Replace("\"", "").Replace("?", "");
                nameSong += " - " + SongName[1].ToString().Replace(@"title=""", "").Replace("\"", "");

                songPattern = @"src=""(.*?)""";
                var UrlImage = Regex.Matches(tempImage, songPattern, RegexOptions.Singleline);
                string imageUrl = UrlImage[0].ToString().Replace(@"src=""", "").Replace("\"", "");

                string imageName = AppDomain.CurrentDomain.BaseDirectory + "ImageSource\\" + nameSong + ".jpg";
                if (!File.Exists(imageName)) { File.WriteAllBytes((imageName), http.Get(imageUrl).ToMemoryStream().ToArray()); }


                string templyric = "Chưa có lyric";

                songPattern = @"<input type=""hidden"" id=""audio_file_path"" value=""(.*?)""";
                string htmlUrl = http.Get(songUrl).ToString();
                var loadUrl = Regex.Matches(htmlUrl, songPattern, RegexOptions.Singleline);
                string downloadUrl = loadUrl[0].ToString().Replace(@"<input type=""hidden"" id=""audio_file_path"" value=""", "").Replace(@"""", "");

                string pathLink = AppDomain.CurrentDomain.BaseDirectory + "Song\\" + nameSong + ".mp3";


                Application.Current.Dispatcher.Invoke(new Action(() => {

                    ListSongAsian.Add(new Song() { SongName = nameSong, SongUrl = songUrl, SongImageUrl = imageUrl, ImageName = imageName, Lyric = templyric, DownloadUrl = downloadUrl, PathLink = pathLink });

                }));

               
            }
            //  ListViewMusic.ItemsSource = ListSongDiscover; 
        }

        void CrawlMusicBXH()
        {
            HttpRequest http = new HttpRequest();
            string html = "";
            try
            {
                html = http.Get(@"https://www.nhaccuatui.com/bai-hat/top-20.nhac-viet.html").ToString();
            }
            catch
            {
                html = http.Get(@"https://www.nhaccuatui.com/bai-hat/top-20.nhac-viet.html").ToString();
            }
            
            string Pattern = @"<ul class=""list_show_chart"">(.*?)</ul>";
            var tempList = Regex.Matches(html, Pattern, RegexOptions.Singleline);
            string listHtml= tempList[0].ToString();
            Pattern = @"<li>(.*?)</li>";
            var listDataSong = Regex.Matches(listHtml, Pattern, RegexOptions.Singleline);

            for (int i = 0; i < 20; i++)
            {
                string tempSong = listDataSong[i].ToString();

                Pattern = @"title=""(.*?)""";
                var songTitle = Regex.Matches(tempSong, Pattern, RegexOptions.Singleline);
                string songName = songTitle[0].ToString().Replace(@"title=""", "").Replace(@"""", "").Replace("?", "");

                Pattern = @"src=""(.*?)""";
                var tempImage= Regex.Matches(tempSong, Pattern, RegexOptions.Singleline);
                string imageUrl = tempImage[0].ToString().Replace(@"src=""", "").Replace(@"""", "");

                string imageName = AppDomain.CurrentDomain.BaseDirectory + "ImageSource\\" + songName + ".jpg";
                if (!File.Exists(imageName)) {
                    try {
                        File.WriteAllBytes(imageName, http.Get(imageUrl).ToMemoryStream().ToArray());
                    }
                    catch { }
                    }

                string baseUrlSearch = @"https://nhacvietnam.mobi/tim-kiem.php?key=" + WebUtility.UrlEncode(songName) + @"&act=timkiem";
                string htmlUrl = "";
                try
                {
                    htmlUrl = http.Get(baseUrlSearch).ToString();
                }
                catch
                {
                    htmlUrl = http.Get(baseUrlSearch).ToString();
                }
                
                Pattern = @"<a title=""Tai nhac mp3 "" href=""(.*?)""";
                var listUrl = Regex.Matches(htmlUrl, Pattern, RegexOptions.Singleline);
                string songUrl = listUrl[0].ToString().Replace(@"<a title=""Tai nhac mp3 "" href=""", "").Replace("\"", "");


                Pattern = @"<div id=""fulllyric"">(.*?)</div>";
                string htmlSong = http.Get(songUrl).ToString();
                var lyric = Regex.Matches(htmlSong, Pattern, RegexOptions.Singleline);
                string templyric = "Chưa có lyric";
                if (lyric.Count > 0)
                {
                    templyric = lyric[0].ToString().Replace(@"<div id=""fulllyric"">", "").Replace(@"<br />", "").Replace(@"</div>", "").Replace("                                                                                                    ", "");
                }

                string downloadUrl = songUrl.Replace("nhacmp3", "listen");

                string pathLink = AppDomain.CurrentDomain.BaseDirectory + "Song\\" + songName + ".mp3";


                Application.Current.Dispatcher.Invoke(new Action(() => {

                    ListSongBXH.Add(new Song() { SongName = songName, SongUrl = songUrl, SongImageUrl = imageUrl, ImageName = imageName, DownloadUrl = downloadUrl, Lyric = templyric, PathLink = pathLink });

                }));

               
            }

        }

        private void Person_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListViewMusic.ItemsSource = null;
            ListViewMusic.ItemsSource = ListSongPerson;
        }

        private void Discover_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            ListViewMusic.ItemsSource = null;
            ListViewMusic.ItemsSource = ListSongDiscover;
        }

        private void PackIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Thread thrd1 = new Thread(CrawlMusicSearch);
            //thrd1.IsBackground = true;
            //thrd1.Start();

            Thread thrdSearch = new Thread(CrawlMusicSearch);
            thrdSearch.IsBackground = true;
            thrdSearch.Start();

            
        }

        private void Asian_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListViewMusic.ItemsSource = null;
            ListViewMusic.ItemsSource = ListSongAsian;
        }

        private void BXH_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListViewMusic.ItemsSource = null;
            ListViewMusic.ItemsSource = ListSongBXH;
        }

        // đang bí 2 cái này...
        private void UcPlayBack_PreClicked(object sender, EventArgs e)
        {

        }

        private void UcPlayBack_NextClicked(object sender, EventArgs e)
        {

        }

       
    }
}
