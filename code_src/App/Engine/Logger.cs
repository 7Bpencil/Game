using System.Collections.Generic;
using System.IO;

namespace App.Engine
{
    public enum MessageClass
    {
        INFO,
        WARNING,
        ERROR
    }

    public static class Logger
    {
        private static readonly List<string> Messages = new List<string> {Capacity = 100};

        public static void Log(string message, MessageClass messageClass)
        {
            string messageAttrib;
            switch (messageClass)
            {
                case MessageClass.INFO:
                    messageAttrib = "";
                    break;
                case MessageClass.WARNING:
                    messageAttrib = "WARNING: ";
                    break;
                case MessageClass.ERROR:
                    messageAttrib = "ERROR: ";
                    break;
                default:
                    messageAttrib = "";
                    break;
            }

            Messages.Add(messageAttrib + message);
        }

        public static void SaveLog()
        {
            File.WriteAllLines("session.log", Messages);
        }

    }
}
