using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trader.GUI;

namespace Trader.Utils
{
    public static class Loger
    {
        static public void Log(string message, string name)
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            path += "\\logs\\";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            path += name + ".log";
            File.AppendAllText(path, "[" + DateTime.Now.ToString() + "][MSG] " + message + "\n");
            SendToControl(new GUI.LogItem { Time = DateTime.Now, Status = "[MSG]", Message = message });
        }

        static public void Log(string message)
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            path += "\\logs\\";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            path += "main.log";
            File.AppendAllText(path, "[" + DateTime.Now.ToString() + "][MSG] " + message + "\n");
            SendToControl(new GUI.LogItem { Time = DateTime.Now, Status = "[MSG]", Message = message });
        }

        static public void Warning(string message, string name)
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            path += "\\logs\\";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            path += name + ".log";
            File.AppendAllText(path, "[" + DateTime.Now.ToString() + "][WRN] " + message + "\n");
            SendToControl(new GUI.LogItem { Time = DateTime.Now, Status = "[WRN]", Message = message });
        }

        static public void Warning(string message)
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            path += "\\logs\\";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            path += "main.log";
            File.AppendAllText(path, "[" + DateTime.Now.ToString() + "][WRN] " + message + "\n");
            SendToControl(new GUI.LogItem { Time = DateTime.Now, Status = "[WRN]", Message = message });
        }

        static public void Error(string message, string name)
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            path += "\\logs\\";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            path += name + ".log";
            File.AppendAllText(path, "[" + DateTime.Now.ToString() + "][ERR] " + message + "\n");
            SendToControl(new GUI.LogItem { Time = DateTime.Now, Status = "[ERR]", Message = message });
        }

        static public void Error(string message)
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            path += "\\logs\\";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            path += "main.log";
            File.AppendAllText(path, "[" + DateTime.Now.ToString() + "][ERR] " + message + "\n");
            SendToControl(new GUI.LogItem { Time = DateTime.Now, Status = "[ERR]", Message = message });
        }

        static private void SendToControl(GUI.LogItem message)
        {
            if (LogControl.Instatnce != null) LogControl.Instatnce.AddMessage(message);
        }
    }
}
