using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services
{
    public class TimeFormatHelper
    {
        /// <summary>
        /// 将十六进制格式表示的时间转换为十进制
        /// </summary>
        /// <param name="HexTime">十六进制格式 "yymmddhhmmss"</param>
        /// <returns>"yyyy/mm/dd hh:mm:ss"</returns>
        public string HexTimeToDecTime(string HexTime)
        {
            MathHelper mathHelper = new MathHelper();
            byte[] times = mathHelper.HexConvertToByte(HexTime);
            string result = string.Format("20{0}/{1}/{2} {3}:{4}:{5}", times[0].ToString("00"), times[1].ToString("00"), times[2].ToString("00"), times[3].ToString("00"), times[4].ToString("00"), times[5].ToString("00"));
            return result;
        }

        /// <summary>
        /// 获取系统时间
        /// </summary>
        /// <returns>"yymmddhhmmss"</returns>
        public string GetDateTime()
        {
            string date = string.Format("{0:yyMMddHHmmss}", DateTime.Now);
            byte[] buffer = new byte[6];
            for (int i = 0; i <= date.Length / 2 - 1; i++)
            {
                buffer[i] = byte.Parse(date.Substring(i * 2, 2));
            }
            MathHelper mathHelper = new MathHelper();
            date = mathHelper.ByteConvertToHex(buffer);
            return date;
        }

        /// <summary>
        /// 获取系统时间
        /// </summary>
        /// <returns>"yymmddhhmmss"</returns>
        public string GetDateTime(DateTime dateTime)
        {
            string date = string.Format("{0:yyMMddHHmmss}", dateTime);
            byte[] buffer = new byte[6];
            for (int i = 0; i <= date.Length / 2 - 1; i++)
            {
                buffer[i] = byte.Parse(date.Substring(i * 2, 2));
            }
            MathHelper mathHelper = new MathHelper();
            date = mathHelper.ByteConvertToHex(buffer);
            return date;
        }

        /// <summary>
        /// 获取系统时间
        /// </summary>
        /// <returns>"yymmddhhmmss"</returns>
        public string GetDateTime( string dateTime)
        {
            string date = string.Format("{0:yyMMddHHmmss}", DateTime.Parse(dateTime));
            byte[] buffer = new byte[6];
            for (int i = 0; i <= date.Length / 2 - 1; i++)
            {
                buffer[i] = byte.Parse(date.Substring(i * 2, 2));
            }
            MathHelper mathHelper = new MathHelper();
            date = mathHelper.ByteConvertToHex(buffer);
            return date;
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static double GetTimestamp(DateTime d)
        {
            TimeSpan ts = d.ToUniversalTime() - new DateTime(1970, 1, 1);
            return ts.TotalMilliseconds;     //精确到毫秒
        }

        public static DateTime StampToDateTime(string timeStamp)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long mTime = long.Parse(timeStamp + "0000");
            TimeSpan toNow = new TimeSpan(mTime);
            return startTime.Add(toNow);
        }
    }
}
