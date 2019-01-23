using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Services
{
    public class ExceptionCatch : Application
    {
        public void StartExceptionCatch()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }
        void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            HandleException(e.Exception);
            e.SetObserved();
        }

        void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                HandleException(e.Exception);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                if (e.ExceptionObject is System.Exception)
                {
                    HandleException((System.Exception)e.ExceptionObject);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {

            try
            {
                HandleException(e.Exception);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        public  void HandleException(Exception ex)
        {
            DateTime now = DateTime.Now;

            string path = System.AppDomain.CurrentDomain.BaseDirectory + "\\log";
            if (!System.IO.Directory.Exists(System.IO.Path.GetFullPath(path))) System.IO.Directory.CreateDirectory(System.IO.Path.GetFullPath(path));
            string logpath = string.Format(@"c:\fatal_{0}{1}{2}.log", now.Year, now.Month, now.Day);
            string filename = System.IO.Path.Combine(path, logpath);
            string version_Text = "V" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            System.IO.File.AppendAllText(logpath, "版本号：" + version_Text);
            System.IO.File.AppendAllText(logpath, "\r\n");
            System.IO.File.AppendAllText(logpath, string.Format("Date：" + now.ToString()));
            System.IO.File.AppendAllText(logpath, "\r\n");
            System.IO.File.AppendAllText(logpath, ex.Message);
            System.IO.File.AppendAllText(logpath, "\r\n");
            System.IO.File.AppendAllText(logpath, ex.StackTrace);
            System.IO.File.AppendAllText(logpath, "\r\n");
            System.IO.File.AppendAllText(logpath, "\r\n----------------------footer--------------------------\r\n");

        }
    }
}
