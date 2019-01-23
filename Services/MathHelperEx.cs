using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services
{
    public class MathHelperEx
    {
        public static string ASCIIToStr(string ASCII)
        {

            return "";
        }

        public static string StrToASCII(string str)
        {
            string ascii = "";
            byte[] a =   System.Text.Encoding.Default.GetBytes(str);
            foreach(var item in a)
            {
                ascii += item.ToString("X2").ToUpper();
            }
            return ascii;
        }

        public static string StrToASCII1(string str)
        {
            string ascii = "";
            byte[] a = System.Text.Encoding.Default.GetBytes(str);
            foreach (var item in a)
            {
                ascii += item.ToString("X4").ToUpper();
            }
            return ascii;
        }

    }
}
