using System.Windows.Forms;
using App.View;

namespace App
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var app = new PlaygroundDeprecated();
            Application.Run(app);
        }
    }
}