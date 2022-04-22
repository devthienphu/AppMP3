using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Clone_app_Zing_Mp3
{
    public class Song
    {
        private string songName;
        private string songUrl;
        private string songImageUrl;
        private string imageName;
        private string lyric;
        private string downloadUrl;
        private string pathLink;
        private double duration;
        private double position;
      

        public string SongName { get => songName; set => songName = value; }
        public string SongUrl { get => songUrl; set => songUrl = value; }
        public string SongImageUrl { get => songImageUrl; set => songImageUrl = value; }
        public string ImageName { get => imageName; set => imageName = value; }
        public string Lyric { get => lyric; set => lyric = value; }
        public string DownloadUrl { get => downloadUrl; set => downloadUrl = value; }
        public string PathLink { get => pathLink; set => pathLink = value; }
        public double Duration { get => duration; set => duration = value; }
        public double Position { get => position; set => position = value; }
    
    }
}
