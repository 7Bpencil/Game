using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Threading;
using System.Xml;

namespace App.Engine
{
    public class MusicPlayer
    {
        private SoundPlayer[] music;
        private Dictionary<string, int> playList;
        
        public MusicPlayer()
        {
            var musicFileNames = Directory.GetFiles("Assets/Music");
            LoadPlayList();
            music = new SoundPlayer[musicFileNames.Length - 1];
            for (var i = 0; i < musicFileNames.Length - 1;)
            {
                if (musicFileNames[i][musicFileNames[i].Length - 1] == 'l') continue;
                music[i] = new SoundPlayer {SoundLocation = musicFileNames[i]};
                music[i].Load();
                i++;
            }
        }

        public void LoadPlayList()
        {
            playList = new Dictionary<string, int>();
            
            var doc = new XmlDocument();
            doc.Load(@"Assets\Music\PlayList.xml");
            var root = doc.DocumentElement;

            foreach (XmlNode node in root)
            {
                playList.Add(
                    node.Attributes.GetNamedItem("title").Value,
                    int.Parse(node.Attributes.GetNamedItem("duration").Value));    
            }
        }

        public void PlayPlaylist()
        {
            foreach (var theme in music)
            {
                theme.Play();
                Thread.Sleep(playList[theme.SoundLocation] * 1000);
            }
        }
    }
}