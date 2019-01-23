using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Services
{
    public class IniHelper
    {
        //====函数======
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern long GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        //====字段======
        //System.IO.Directory.GetCurrentDirectory() + @"\Set.ini"
        //System.AppDomain.CurrentDomain.BaseDirectory + @"\Set.ini"  //windows服务用
        string FilePath;   //路径

        #region ---构造函数
        public IniHelper(string Path)
        {
            FilePath = Path;
        }
        #endregion

        #region   ---读ini文件
        public String ReadIni(String section, String Key, String filePath)
        {
            StringBuilder temp = new StringBuilder(1024);
            try
            {
                GetPrivateProfileString(section, Key, "", temp, temp.Capacity, filePath);
            }
            catch
            {
                MessageBox.Show("读取配置文件失败", "消息提示");
            }
            return temp.ToString();
        }
        //---
        public String ReadIni(String section, String Key)
        {
            StringBuilder temp = new StringBuilder(1024);
            try
            {
                GetPrivateProfileString(section, Key, "", temp, temp.Capacity, FilePath);
            }
            catch
            {
                MessageBox.Show("读取配置文件失败", "消息提示");
            }
            return temp.ToString();
        }
        #endregion

        #region   ---写ini文件
        public void WriteIni(String section, String Key, string value, String filePath)
        {
            WritePrivateProfileString(section, Key, value, filePath);
        }
        //---
        public void WriteIni(String section, String Key, string value)
        {
            WritePrivateProfileString(section, Key, value, FilePath);
        }
        #endregion
    }
}
