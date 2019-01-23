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
/*
  By:GQ
  Time:2018/8/4
  LastUpdate : 2018/11/14 08:26
  客户端断开时提示断开连接 length=0时是断开
*/
namespace CommunicationServers.Sockets
{
    public delegate void NewConnnetion(Socket socket);
    public delegate void NewMessage(Socket socket, Byte[] Message);
    public delegate void NewMessage1(Socket socket, string Message);
    public delegate void Disconnected(Socket socket);
    public class SocketServer
    {
        private static int MaxConnectionCount = 1024;
        private static byte[] buffer = new byte[1024];
        public event NewConnnetion NewConnnectionEvent;
        public event NewMessage NewMessageEvent;
        public event NewMessage1 NewMessage1Event;
        public event Disconnected ClientDisconnectedEvent;

        private Socket serverSocket;
        public Socket ServerSocket
        {
            get
            {
                return serverSocket;
            }
            set
            {
                serverSocket = value;
            }
        }


        /// <summary>
        /// 侦听
        /// </summary>
        /// <param name="Port">端口号</param>
        /// <param name="IP">IP地址</param>
        /// <returns></returns>
        public bool Listen(string Port, string IP ="")
        {
            try
            {
                if (PortIsUsed(Convert.ToInt32(Port))){ throw new Exception("网络端口已被占用"); }
                ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Any;
                if (!string.IsNullOrWhiteSpace(IP))
                {
                    IPAddress.TryParse(IP, out ip);
                }
                IPEndPoint point = new IPEndPoint(ip, Convert.ToInt32(Port));
                ServerSocket.Bind(point);
                ServerSocket.Listen(MaxConnectionCount);
                //创建监听线程
                ServerSocket.BeginAccept(new AsyncCallback(AcceptNewClient), ServerSocket);
                return true;
            }
            catch(Exception ex)
            {
                SimpleLogHelper.Instance.WriteLog(LogType.Error, ex, "侦听发生错误");
                return false;
            }
        }

        /// <summary>
        /// 接收新的客户端请求
        /// </summary>
        /// <param name="ar"></param>
        public void AcceptNewClient(IAsyncResult ar)
        {
            try
            {
                var socket = ar.AsyncState as Socket;
                ServerSocket = socket;
                var client = socket.EndAccept(ar);
                if(this.NewConnnectionEvent != null)
                {
                    NewConnnectionEvent(client);
                }
                client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), client);
                socket.BeginAccept(new AsyncCallback(AcceptNewClient), socket);
            }
            catch(Exception ex)
            {
                SimpleLogHelper.Instance.WriteLog(LogType.Error, ex, "接受新的客户端请求时错误");
            }
            
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveMessage(IAsyncResult ar)
        {
            var socket = ar.AsyncState as Socket;
            if(socket.Connected)
            {
                try
                {
                    var length = socket.EndReceive(ar);
                    if (length == 0) throw new Exception();
                    string messagex = ByteConvertToString(buffer, length);
                    NewMessage1Event(socket, messagex);
                    socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), socket);
                }
                catch (Exception ex)
                {
                    if (ClientDisconnectedEvent != null) ClientDisconnectedEvent(socket);
                }
            }
            else
            {
                if (ClientDisconnectedEvent != null) ClientDisconnectedEvent(socket);
            }
            
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="ClientSocket"></param>
        /// <param name="Message"></param>
        public void Send(Socket ClientSocket , byte[] buffer )
        {  
            try
            {
                ClientSocket.Send(buffer);
            }
            catch(Exception ex)
            {
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
                this.ServerSocket.Close();
                this.ServerSocket.Dispose();
                return true;
            }
            catch(Exception ex)
            {
                SimpleLogHelper.Instance.WriteLog(LogType.Error, ex, "关闭Socket连接错误");
                return false;
            }
        }

        public bool IsConnected(Socket ClientSocket)
        {
            if (ClientSocket == null) return false;
            if (ClientSocket.Connected)
            {
                return true;
            }
            return false;
        }

        private bool PortIsUsed(int Port)
        {
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();
            int count = ipEndPoints.Where(m => m.Port == Port).ToList().Count;
            Debug.WriteLine(count);
            if (count==1)
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

        public string ByteConvertToString(byte[] Buffer ,int length)
        {
            string message = "";
            for (int i = 0; i <= length - 1; i++)
            {
                message += Buffer[i].ToString("X2").ToUpper();
            }
            return message;
        }
    }
}
