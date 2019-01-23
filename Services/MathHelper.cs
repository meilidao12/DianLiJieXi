using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    /// <summary>
    /// 计算帮助类
    /// </summary>
    public class MathHelper
    {
        /// <summary>
        /// 十六进制单精度浮点数 转为 实数
        /// </summary>
        /// <param name="Data">字符串表示的十六进制单精度浮点数</param>
        /// <returns></returns>
        public static string HexToSingle(string Data)
        {
            if (Data.Length != 8) return "Err";
            try
            {
                byte[] bytes = new byte[] { Convert.ToByte(Data.Substring(6, 2), 16), Convert.ToByte(Data.Substring(4, 2), 16),
                                            Convert.ToByte(Data.Substring(2, 2), 16), Convert.ToByte(Data.Substring(0, 2), 16) };
                return BitConverter.ToSingle(bytes.ToArray(), 0).ToString();
            }
            catch
            {
                return "Err";
            }
        }

        /// <summary>
        /// 实数转为十六进制单精度浮点数
        /// </summary>
        /// <param name="Data">实数</param>
        /// <returns></returns>
        public static string SingleToHex(string Data)
        {
            try
            {
                float a = float.Parse(Data);
                byte[] bytes = BitConverter.GetBytes(a);
                string b = "";
                for (int i = 3; i >= 0; i--)
                {
                    b += Convert.ToString(bytes[i], 16).PadLeft(2, '0');
                }
                return b.ToUpper();
            }
            catch
            {
                return "Err";
            }
        }

        /// <summary>
        /// 十六进制双精度浮点数转为实数
        /// </summary>
        /// <param name="Data">十六进制双精度浮点数</param>
        /// <returns></returns>
        public static string HexToDouble(string Data)
        {
            if (Data.Length != 16) return "Err";
            try
            {
                byte[] bytes = new byte[] { Convert.ToByte(Data.Substring(14, 2), 16), Convert.ToByte(Data.Substring(12, 2), 16),
                                            Convert.ToByte(Data.Substring(10, 2), 16), Convert.ToByte(Data.Substring(8, 2), 16),
                                            Convert.ToByte(Data.Substring(6, 2), 16), Convert.ToByte(Data.Substring(4, 2), 16),
                                            Convert.ToByte(Data.Substring(2, 2), 16), Convert.ToByte(Data.Substring(0, 2), 16) };
                return BitConverter.ToDouble(bytes.ToArray(), 0).ToString();
            }
            catch
            {
                return "Err";
            }
        }

        /// <summary>
        /// 实数转为十六进制双精度浮点数
        /// </summary>
        /// <param name="Data">实数</param>
        /// <returns></returns>
        public static string DoubleToHex(string Data)
        {

            try
            {
                double a = double.Parse(Data);
                byte[] bytes = BitConverter.GetBytes(a);
                string b = "";
                for (int i = 7; i >= 0; i--)
                {
                    b += Convert.ToString(bytes[i], 16).PadLeft(2, '0');
                }
                return b.ToUpper();
            }
            catch
            {
                return "Err";
            }
        }

        public byte[] ASCIIConvertToByte(string strASCII)
        {
            return Encoding.UTF8.GetBytes(strASCII);
        }

        public string ByteConvertToASCII(byte[] buffer)
        {
            return Encoding.ASCII.GetString(buffer, 0, buffer.Length);
        }

        public byte[] HexConvertToByte(string str)
        {
            byte[] buffer = new byte[str.Length / 2];
            for (int i = 0; i <= str.Length / 2 - 1; i++)
            {
                buffer[i] = Convert.ToByte(str.Substring(i * 2, 2), 16);
            }
            return buffer;
        }

        public string ByteConvertToHex(byte[] buffer)
        {
            string message = "";
            for (int i = 0; i <= buffer.Length - 1; i++)
            {
                message += buffer[i].ToString("X2").ToUpper();
            }
            return message;
        }

        public string ByteConvertToHex(byte[] buffer, int length)
        {
            string message = "";
            for (int i = 0; i <= length - 1; i++)
            {
                message += buffer[i].ToString("X2").ToUpper();
            }
            return message;
        }

        public static string DecToHex(string dec)
        {
            return Convert.ToString(int.Parse(dec), 16).ToUpper();
        }
        public static int HexToDec(string hex)
        {
            return Convert.ToInt32(hex, 16);
        }

        public byte[] StrToByte(string str)
        {
            return System.Text.Encoding.Default.GetBytes(str);
        }



        #region 字符串和Byte之间的转化
        /// <summary>
        /// 数字和字节之间互转
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static int IntToBitConverter(int num)
        {
            int temp = 0;
            byte[] bytes = BitConverter.GetBytes(num);//将int32转换为字节数组
            temp = BitConverter.ToInt32(bytes, 0);//将字节数组内容再转成int32类型
            return temp;
        }

        /// <summary>
        /// 将字符串转为16进制字符，允许中文
        /// </summary>
        /// <param name="s"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string StringToHexString(string s, Encoding encode, string spanString)
        {
            byte[] b = encode.GetBytes(s);//按照指定编码将string编程字节数组
            string result = string.Empty;
            for (int i = 0; i < b.Length; i++)//逐字节变为16进制字符
            {
                result += Convert.ToString(b[i], 16) + spanString;
            }
            return result;
        }
        /// <summary>
        /// 将16进制字符串转为字符串
        /// </summary>
        /// <param name="hs"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string HexStringToString(string hs, Encoding encode)
        {
            string strTemp = "";
            byte[] b = new byte[hs.Length / 2];
            for (int i = 0; i < hs.Length / 2; i++)
            {
                strTemp = hs.Substring(i * 2, 2);
                b[i] = Convert.ToByte(strTemp, 16);
            }
            //按照指定编码将字节数组变为字符串
            return encode.GetString(b);
        }
        /// <summary>
        /// byte[]转为16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ByteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }
        /// <summary>
        /// 将16进制的字符串转为byte[]
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] StrToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        #endregion

    }



    /*
      调试数据：
            Debug.WriteLine(HexToSingle("4126A406"));
            Debug.WriteLine(SingleToHex("10.41504"));
            Debug.WriteLine(HexToDouble("4024D47AE147AE14"));
            Debug.WriteLine(DoubleToHex("10.415"));
    */
}
