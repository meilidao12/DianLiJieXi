using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CharacterConversion
    {
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
    }
}
