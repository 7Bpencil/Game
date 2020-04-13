using System;
using System.Windows.Forms;
using App.View;

namespace App
{
    
    internal class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var app = new ViewExperimental();
            Application.Run(app);
        }
    }
}