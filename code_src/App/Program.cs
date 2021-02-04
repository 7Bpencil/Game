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
            try
            {
                Application.Run(new ViewForm());
            }
            catch (SystemException e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
