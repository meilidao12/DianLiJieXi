using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Windows.Data;
using System.Reflection;
using System.Windows.Controls;

namespace WpfServers
{
    public delegate void WindowBackHome();
    public class WindowFactory
    {
        public static event WindowBackHome windowbackhome;
        public static Window CreateWindow(string WindowName)
        {
            string strDLL = "MesToPlc"; //程序集
            string strNamespace = "MesToPlc." + WindowName; //命名空间+类名
            try
            {
                return (Window)Assembly.Load(strDLL).CreateInstance(strNamespace);
            }
            catch
            {
                return null;
            }
        }

        public static void Show(Window NowWindow , Window NextWindow)
        {
            if(NowWindow ==null || NextWindow ==null)
            {
                return;
            }
            NextWindow.Owner = NowWindow;
            try
            {
                NextWindow.ShowDialog();
                if (NextWindow.DialogResult == true)
                {
                    NowWindow.Show();
                }
            }
            catch
            {
                //写出错误信息
            }
        }

        public static void BackHome_Event()
        {
            if(windowbackhome != null)
            {
                windowbackhome();
            }
            else
            {
                Debug.WriteLine("窗口home事件为空");
            }
        }

        public static void setStyle(Window _window)
        {
            Page pg = new Page();
            _window.SetBinding(Window.WidthProperty, new Binding("windowWidth") { Source = pg });  //绑定窗体宽度
            //_window.SetBinding(Window.WidthProperty, new Binding("windowWidth") { Source = pg });  //绑定窗体高度
            _window.SetBinding(Window.HeightProperty, new Binding("windowHeight") { Source = pg });  //绑定窗体高度
        }
    }
}
