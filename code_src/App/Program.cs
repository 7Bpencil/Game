using System;
using System.Windows.Forms;
using App.Engine;
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
                Logger.Log(e.ToString(), MessageClass.ERROR);
            }

            Logger.SaveLog();
        }
    }
}
