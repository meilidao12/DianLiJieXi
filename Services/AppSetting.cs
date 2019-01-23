using System;
using System.Configuration;

namespace Services
{
    /// <summary>
    /// 对程序默认配置文件的增、删、改、查
    /// </summary>
    public class AppSetting
    {
        // 简化了的读取配置文件的方法, 读取失败返回空字符串
        public static string Get(string keyName)
        {
            if (!String.IsNullOrEmpty(keyName))
                return System.Configuration.ConfigurationManager.AppSettings[keyName];
            else return string.Empty;
        }

        // 添加
        public static bool Add(string keyName, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            try
            {
                config.AppSettings.Settings.Add(keyName, value);
                config.Save(ConfigurationSaveMode.Modified);//save
                ConfigurationManager.RefreshSection("appSettings");//重新加载新的配置文件
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                config = null;
            }
            return true;
        }

        // 删除
        public static bool Remove(string keyName)
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            try
            {
                config.AppSettings.Settings.Remove(keyName);
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");//重新加载新的配置文件   
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                config = null;
            }
            return true;
        }

        // 修改
        public static bool Set(string keyName, string value)
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            try
            {
                if (config.AppSettings.Settings[keyName] == null)
                {
                    config.AppSettings.Settings.Add(keyName, value);
                }
                else
                {
                    config.AppSettings.Settings[keyName].Value = value;
                }
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");//重新加载新的配置文件   
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                config = null;
            }
            return true;
        }

        /// <summary>
        /// 检测制定节点是否存在
        /// </summary>
        /// <param name="sKeyName"></param>
        /// <returns></returns>
        public static bool Check(string sKeyName)
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            try
            {
                if (config.AppSettings.Settings[sKeyName] == null)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                config = null;
            }
        }

        /// <summary>
        /// 获取指定的配置文件的节点的值
        /// </summary>
        /// <param name="sConfigPath">指定的配置文件的物理路径</param>
        /// <param name="sAppSettingsKey">配置文件内的AppSettings内的节点名称</param>
        /// <returns>节点值</returns>
        public static string GetAppointConfig(string sConfigPath, string sAppSettingsKey)
        {
            try
            {
                if (!System.IO.File.Exists(sConfigPath)) return "";
                System.Configuration.ExeConfigurationFileMap configFileMap = new System.Configuration.ExeConfigurationFileMap();
                configFileMap.ExeConfigFilename = sConfigPath;
                System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(configFileMap, System.Configuration.ConfigurationUserLevel.None);
                if (config == null) return "";
                return config.AppSettings.Settings[sAppSettingsKey].Value.Trim();
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 更新指定的配置文件的节点的值
        /// </summary>
        /// <param name="sConfigPath">指定的配置文件的物理路径</param>
        /// <param name="sAppSettingKey">配置文件内的AppSettings内的节点名称</param>
        /// <param name="value">要更新到的值</param>
        /// <returns>更新是否成功</returns>
        public static void SetAppointConfig(string sConfigPath, string sAppSettingKey, string value)
        {
            try
            {
                if (!System.IO.File.Exists(sConfigPath)) return;
                System.Configuration.ExeConfigurationFileMap configFileMap = new System.Configuration.ExeConfigurationFileMap();
                configFileMap.ExeConfigFilename = sConfigPath;
                System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(configFileMap, System.Configuration.ConfigurationUserLevel.None);
                if (config == null) return;
                config.AppSettings.Settings[sAppSettingKey].Value = value;
                //save
                config.Save(System.Configuration.ConfigurationSaveMode.Modified);
                //reload
                System.Configuration.ConfigurationManager.RefreshSection("appSettings");
            }
            catch (Exception ex)
            {
                throw;

            }
        }
    }
}

