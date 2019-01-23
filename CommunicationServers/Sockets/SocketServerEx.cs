using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Services;

namespace CommunicationServers.Sockets
{
    public class SocketServerEx : IDisposable
    {

        public delegate void NewMessage2(IPEndPoint remoteIpEndPoint, string Message);
        private UdpClient receiveUdpClient;
        private static byte[] buffer = new byte[1024];
        public event NewMessage2 NewMessage2Event;
        public event Disconnected ClientDisconnectedEvent;

        private List<IPEndPoint> ipEndPoints = new List<IPEndPoint>();
        public List<IPEndPoint> IpEndPoints
        {
            get
            {
                return ipEndPoints;
            }

            set
            {
                ipEndPoints = value;
            }
        }

        public IPEndPoint IpEndPoint
        {
            get
            {
                return ipEndPoint;
            }

            set
            {
                ipEndPoint = value;
            }
        }

        private IPEndPoint ipEndPoint;

        /// <summary>
        /// 根据ip 和port 获取 IPEndPoint
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public IPEndPoint GetIPEndPoint(string ip ,string port)
        {
            try
            {
                return new IPEndPoint(IPAddress.Parse(ip),int.Parse(port)); 
            }
            catch
            {
                return null;
            }
            
        }

        /// <summary>
        /// 侦听
        /// </summary>
        /// <param name="Port">端口号</param>
        /// <param name="IP">IP地址</param>
        /// <returns></returns>
        public bool Listen(string Port, string IP = "")
        {
            try
            {
                if (PortIsUsed(Convert.ToInt32(Port))) { throw new Exception("网络端口已被占用"); }
                IPAddress ip = IPAddress.Any;
                if (!string.IsNullOrWhiteSpace(IP))
                {
                    IPAddress.TryParse(IP, out ip);
                }
                IPEndPoint point = new IPEndPoint(ip, Convert.ToInt32(Port));
                this.receiveUdpClient = new UdpClient(point);
                Thread receiveThread = new Thread(this.ReceiveMessage);
                receiveThread.Start();
                return true;
            }
            catch (Exception ex)
            {
                SimpleLogHelper.Instance.WriteLog(LogType.Error, ex, "侦听发生错误");
                return false;
            }
        }


        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveMessage()
        {
            IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                try
                {
                    // 关闭receiveUdpClient时此时会产生异常
                    byte[] receiveBytes = this.receiveUdpClient.Receive(ref remoteIpEndPoint);
                    Debug.WriteLine(remoteIpEndPoint);
                    string message = ByteConvertToString(receiveBytes, receiveBytes.Length);
                    NewMessage2Event(remoteIpEndPoint, message);
                }
                catch
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="Message"></param>
        public void Send(IPEndPoint ipEndPoint, byte[] buffer)
        {
            try
            {
                if(this.receiveUdpClient == null)
                {
                    this.receiveUdpClient = new UdpClient(0);
                }
                this.receiveUdpClient.Send(buffer,buffer.Length,ipEndPoint);
            }
            catch (Exception ex)
            {
                SimpleLogHelper.Instance.WriteLog(LogType.Info, "Err: Class(SocketServerEx) Method(Send)");
                SimpleLogHelper.Instance.WriteLog(LogType.Error, ex);
            }
        }

        /// <summary>
        /// 断开Socket连接
        /// </summary>
        /// <param name="ClientSocket"></param>
        /// <returns></returns>
        public bool Disconnect()
        {
            try
            {
                this.receiveUdpClient.Close();
                return true;
            }
            catch (Exception ex)
            {
                SimpleLogHelper.Instance.WriteLog(LogType.Error, ex, "关闭Socket连接错误");
                return false;
            }
        }

        private bool PortIsUsed(int Port)
        {
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();
            int count = ipEndPoints.Where(m => m.Port == Port).ToList().Count;
            if (count == 1)
            {
                return true;
            }
            return false;
        }

        public byte[] ASCIIConvertToByte(string strASCII)
        {
            return Encoding.UTF8.GetBytes(strASCII);
        }

        public string ByteConvertToASCII(byte[] Buffer)
        {
            return Encoding.ASCII.GetString(buffer, 0, Buffer.Length);
        }

        public byte[] StringConvertToByte(string str)
        {
            byte[] buffer = new byte[str.Length / 2];
            for (int i = 0; i <= str.Length / 2 - 1; i++)
            {
                buffer[i] = Convert.ToByte(str.Substring(i * 2, 2), 16);
            }
            return buffer;
        }

        public string ByteConvertToString(byte[] Buffer)
        {
            string message = "";
            for (int i = 0; i <= Buffer.Length - 1; i++)
            {
                message += Buffer[i].ToString("X2").ToUpper();
            }
            return message;
        }

        public string ByteConvertToString(byte[] Buffer, int length)
        {
            string message = "";
            for (int i = 0; i <= length - 1; i++)
            {
                message += Buffer[i].ToString("X2").ToUpper();
            }
            return message;
        }

        public void Dispose()
        {
            try
            {
                if (this.receiveUdpClient != null)
                {
                    this.receiveUdpClient.Close();
                }
            }
            catch(Exception ex)
            {
                SimpleLogHelper.Instance.WriteLog(LogType.Error, ex);
            }
        }
    }
}
