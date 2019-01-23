using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
namespace Services
{
    public class Http
    {
        #region ---获取网络数据  HttpPost(string Url, string postDataStr)
        /// <summary>
        /// 获取网络数据
        /// </summary>
        /// <param name="Url">API URL</param>
        /// <param name="postDataStr">形式为 如果数据为一条 key=value ,  多条 key1=value1&key2=value2</param>
        /// <returns></returns>
        public static string HttpPost(string Url, string postDataStr)
        {
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            //ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = Encoding.GetEncoding("utf-8").GetBytes(postDataStr);
            request.ContentLength = data.Length;
            request.Timeout = 2000;
            Stream reqStream = request.GetRequestStream();
            reqStream.Write(data, 0, data.Length);
            reqStream.Close();
            //---------
            HttpWebResponse resp;
            try
            {
                resp = (HttpWebResponse)request.GetResponse();
            }
            catch
            {
                //建立错误信息列表 请求失败时写入失败原因  待写
                return null;
            }
            Stream stream = resp.GetResponseStream();
            StreamReader Reader = new StreamReader(resp.GetResponseStream(), Encoding.UTF8);
            string A = Reader.ReadToEnd();
            Debug.WriteLine(A);
            return A;
        }
        #endregion

        #region ---获取网络参数  HttpPost(string Url)
        public static string HttpPost(string Url)
        {
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Timeout = 2000;
            //---------
            HttpWebResponse resp;
            try
            {
                resp = (HttpWebResponse)request.GetResponse();
            }
            catch
            {
                //建立错误信息列表 请求失败时写入失败原因  待写
                return null;
            }
            Stream stream = resp.GetResponseStream();
            StreamReader Reader = new StreamReader(resp.GetResponseStream(), Encoding.UTF8);
            string A = Reader.ReadToEnd();
            Debug.WriteLine(A);
            return A;
        }
        #endregion

        #region ---获取网络参数  HttpGet(string Url)
        public static string HttpGet(string Url)
        {
            var request = (HttpWebRequest)WebRequest.Create(Url);
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return responseString.ToString();
        }
        #endregion

        #region 获取服务器图片
        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="url">图片路径</param>
        /// <param name="SaveName">图片保存名称</param>
        //public static void GetPic(string url,string SaveName)
        //{
        //    Uri muri = new Uri(url);
        //    HttpWebRequest mRequest = (HttpWebRequest)WebRequest.Create(muri);
        //    mRequest.Method = "GET";
        //    mRequest.Timeout = 200;
        //    mRequest.ContentType = "text/html;charset=utf-8";
        //    HttpWebResponse mResponse;
        //    Stream mStream;
        //    try
        //    {
        //        mResponse = (HttpWebResponse)mRequest.GetResponse();
        //        mStream = mResponse.GetResponseStream();
        //    }
        //    catch
        //    {
        //        //写入错误信息 待写
        //        return;
        //    }
        //    Image mImage = Image.FromStream(mStream);
        //    mImage.Save(SaveName);
        //    mStream.Close();
        //}
        #endregion
    }
}
