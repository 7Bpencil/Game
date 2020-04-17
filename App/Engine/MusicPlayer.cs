using System.Collections.Generic;
using System.Media;
using System.Threading;
using System.Xml;

namespace App.Engine
{
    public class MusicPlayer
    {
        private Dictionary<string, SoundPlayer> playList;
        private Dictionary<string, int> musicDuration;
        
        public MusicPlayer()
        {
            musicDuration = LoadPlayList();
            playList = new Dictionary<string, SoundPlayer>();
            foreach (var musicFileName in musicDuration.Keys)
                playList.Add(musicFileName, new SoundPlayer {SoundLocation = musicFileName});
        }

        private Dictionary<string, int> LoadPlayList()
        {
            var playList = new Dictionary<string, int>();
            
            var doc = new XmlDocument();
            doc.Load(@"Assets\Music\PlayList.xml");
            var root = doc.DocumentElement;

            foreach (XmlNode node in root)
            {
                playList.Add(
                    node.Attributes.GetNamedItem("title").Value,
                    int.Parse(node.Attributes.GetNamedItem("duration").Value));    
            }

            return playList;
        }

        public void PlayPlaylist()
        {
            foreach (var theme in playList.Keys)
            {
                playList[theme].Play();
                Thread.Sleep(musicDuration[theme] * 1000);
            }
        }
    }
}