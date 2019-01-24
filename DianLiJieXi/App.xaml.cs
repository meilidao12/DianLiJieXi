using Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DianLiJieXi
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }
        void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            HandleException(e.Exception);
            e.SetObserved();
        }

        void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
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

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
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

        public static void HandleException(Exception ex)
        {
            string version_Text = "V" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            SimpleLogHelper.Instance.WriteLog(LogType.Error, ex, version_Text);
        }
    }
}
