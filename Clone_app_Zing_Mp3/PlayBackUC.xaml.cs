using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
using System.Windows.Threading;

namespace Clone_app_Zing_Mp3
{
    /// <summary>
    /// Interaction logic for PlayBackUC.xaml
    /// </summary>
    public partial class PlayBackUC : UserControl, INotifyPropertyChanged
    {
        public Song songInfo;
        public Song SongInfo { get { return songInfo; }
            set
            {
                this.DataContext = SongInfo;
                songInfo = value;
                OnPropertyChanged("SongInfo");
                DownLoadSong(songInfo);
                txbLyric.Text = SongInfo.Lyric;
                currentSong.Text = SongInfo.SongName;
                Uri tempUri = new Uri(SongInfo.PathLink);
                BitmapImage tempImage = new BitmapImage(new Uri(SongInfo.ImageName));
                if (MediaPlay.Source != tempUri && PlayBackImage.Source != tempImage)
                {
                    MediaPlay.Source = new Uri(SongInfo.PathLink);
                    PlayBackImage.Source = new BitmapImage(new Uri(SongInfo.ImageName));
                }
                
                }
        }

        private bool isPlaying;
        public bool IsPlaying { get { return isPlaying; }
            set {
                isPlaying = value;
                if (isPlaying)
                {
                    MediaPlay.Play();
                    timer.Start();
                }
                else
                {
                    MediaPlay.Pause();
                    timer.Stop();
                }
            }
        }

        DispatcherTimer timer;
        public PlayBackUC()
        {
            InitializeComponent();
            this.DataContext = SongInfo;
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;
            
        }
       
        private void Timer_Tick(object sender, EventArgs e)
        {
            SongInfo.Position++;
            sdDuration.Value = SongInfo.Position;
        }

        public event EventHandler backToMain;
        public event EventHandler BackToMain
        {
            add { backToMain += value; }
            remove { backToMain += value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string newName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(newName));
            }
        }

        void DownLoadSong(Song songInfo)
        {
            string songName =SongInfo.PathLink;
            if (!File.Exists(songName))
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Tls11
                       | SecurityProtocolType.Tls12
                       | SecurityProtocolType.Ssl3;


                WebClient wb = new WebClient();
                wb.DownloadFile(SongInfo.DownloadUrl, songName);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (backToMain != null)
                backToMain(this, new EventArgs());
        }

        private void MediaPlay_MediaOpened(object sender, RoutedEventArgs e)
        {
            IsPlaying = true;
            SongInfo.Duration = MediaPlay.NaturalDuration.TimeSpan.TotalSeconds;
            tbxDuration.Text = new TimeSpan(0, (int)(SongInfo.Duration /60), (int)(SongInfo.Duration % 60)).ToString(@"mm\:ss");    
            sdDuration.Maximum = SongInfo.Duration;
            SongInfo.Position = 0;
            //timer.Start();
            
        }

        bool isDraging = false;
        private void SdDuration_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isDraging)
            {
                SongInfo.Position = sdDuration.Value;
                MediaPlay.Position = new TimeSpan(0, 0, (int)SongInfo.Position);
            }
            tbxPosition.Text = new TimeSpan(0, (int)(SongInfo.Position / 60), (int)(SongInfo.Position % 60)).ToString(@"mm\:ss");
        }

        private void SdDuration_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isDraging = true;
        }

        private void SdDuration_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDraging = false;
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            IsPlaying = !IsPlaying;
        }

        private event EventHandler preClicked;
        public event EventHandler PreClicked
        {
            add { preClicked += value; }
            remove { preClicked -= value; }
        }

        private event EventHandler nextClicked;
        public event EventHandler NextClicked
        {
            add { nextClicked += value; }
            remove { nextClicked -= value; }
        }

        private void BtnPre_Click(object sender, RoutedEventArgs e)
        {
            if (preClicked != null)
            {
                preClicked(this, new EventArgs());
            }
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (nextClicked != null)
            {
                nextClicked(this, new EventArgs());
            }
        }
    }
}
