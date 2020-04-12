using System;
using System.Windows.Forms;
using App.Model.LevelData;
using App.Model.Parser;
using App.View;

namespace App
{
    
    internal class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            /*
            var app = new ViewExperimental();
            Application.Run(app);
            */
            var a = TileSetParser.LoadTileSets();
            Console.WriteLine("BB");
        }
    }
}