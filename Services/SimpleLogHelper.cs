using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    //单例模式
    public sealed class SimpleLogHelper
    {
        private string errorFilePath;
        private string infoFilePath;
        private static object locker = new object();

        private static object fileLocker = new object();
        private static  SimpleLogHelper instance;

        private SimpleLogHelper()
        {
            errorFilePath = System.AppDomain.CurrentDomain.BaseDirectory + "\\ErrorLog";
            if (!System.IO.Directory.Exists(System.IO.Path.GetFullPath(errorFilePath)))
                System.IO.Directory.CreateDirectory(System.IO.Path.GetFullPath(errorFilePath));
            infoFilePath = System.AppDomain.CurrentDomain.BaseDirectory + "\\InfoLog";
            if (!System.IO.Directory.Exists(System.IO.Path.GetFullPath(infoFilePath)))
                System.IO.Directory.CreateDirectory(System.IO.Path.GetFullPath(infoFilePath));
        }

        public static SimpleLogHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            instance = new SimpleLogHelper();
                        }
                    }
                }
                return instance;
            }
        }

        public void WriteLog(LogType logtype, object o, string category = null)
        {
            lock (fileLocker)
            {
                DateTime now = DateTime.Now;
                string filename = string.Format(@"{0}{1}{2}.log", now.Year, now.Month, now.Day);
                switch (logtype)
                {
                    case LogType.Error:
                        filename = System.IO.Path.Combine(errorFilePath, filename);
                        break;
                    case LogType.Info:
                        filename = System.IO.Path.Combine(infoFilePath, filename);
                        break;
                    default:
                        filename = System.IO.Path.Combine(infoFilePath, filename);
                        break;
                }
                string meg = "";
                if (!string.IsNullOrEmpty(category))
                {
                    meg = category + ":";
                }

                if (o is Exception)
                {
                    Exception ex = (Exception)o;
                    meg += ex.Message + Environment.NewLine;
                    meg += ex.StackTrace;
                }
                else if (o != null)
                {
                    meg += o.ToString();
                }
                File.AppendAllText(filename, string.Format("\r\n----------------------{0}--------------------------\r\n", now.ToString("yyyy-MM-dd HH:mm:ss")));
                File.AppendAllText(filename, meg);
                File.AppendAllText(filename, "\r\n----------------------footer--------------------------\r\n");
                ClearOldLog();
            }
        }

        public void ClearOldLog()
        {
            try
            {
                List<string> paths = new List<string>();
                paths.Add(this.infoFilePath);
                paths.Add(this.errorFilePath);
                DirectoryInfo root;
                FileInfo[] files;
                foreach (var path in paths)
                {
                    root = new DirectoryInfo(path);
                    files = root.GetFiles();
                    if (files.Length != 0)
                    {
                        foreach (var item in files)
                        {
                            if (item.LastWriteTime <= DateTime.Now.AddDays(-30))
                            {
                                File.Delete(item.FullName);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }
    }

    public enum LogType
    {
        Error,
        Info
    }
}
