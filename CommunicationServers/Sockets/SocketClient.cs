using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Services;
using System.Threading;
namespace CommunicationServers.Sockets
{
    public class SocketClient
    {
        private static byte[] buffer = new byte[1024];
        public event NewMessage NewMessageEvent;
        public event Disconnected ServerDisconnectedEvent;
        private Socket clientSocket;
        public Socket ClientSocket
        {
            get
            {
                return clientSocket;
            }
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="Port"></param>
        /// <param name="IP"></param>
        /// <returns></returns>
        public bool Connnect(string Port, string IP)
        {
            var Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket = Socket;
            IPAddress ip = IPAddress.Any;
            if (!string.IsNullOrWhiteSpace(IP))
            {
                IPAddress.TryParse(IP, out ip );
            }
            else
            {
                return false;
            }
            try
            {
                Socket.Connect(ip, Convert.ToInt32(Port));
                Socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), Socket);
                return true;
            }
            catch(Exception ex)
            {
                SimpleLogHelper.Instance.WriteLog(LogType.Error, ex, "连接服务器时发生错误");
                return false;
            }
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveMessage(IAsyncResult ar)
        {
            var socket = ar.AsyncState as Socket;
            try
            {
                var length = socket.EndReceive(ar);
                if (length == 0) throw new Exception();
                Thread td = new Thread(() => {
                    byte[] message = new byte[length];
                    Array.Copy(buffer, message, length);
                    NewMessageEvent(socket, message);
                });
                td.Start();
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), socket);
            }
            catch (Exception ex)
            {
                clientSocket.Close();
                if (ServerDisconnectedEvent != null) ServerDisconnectedEvent(socket);
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="ServerSocket"></param>
        /// <param name="Message"></param>
        public void Send(byte[] buffer)
        {
            try
            {
                this.clientSocket.Send(buffer);
            }
            catch(Exception ex)
            {
                SimpleLogHelper.Instance.WriteLog(LogType.Error, ex);
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        public bool Disconnect()
        {
            try
            {
                clientSocket.Close();
                return true;
            }
            catch(Exception ex)
            {
                SimpleLogHelper.Instance.WriteLog(LogType.Error, ex, "与服务器断开连接发生错误");
                return false;
            }
        }

        /// <summary>
        /// 判断网络连接状态
        /// </summary>
        /// <returns></returns>
        public bool IsConnected()
        {
            if (clientSocket.Connected)
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
            return Encoding.ASCII.GetString(buffer,0,Buffer.Length);
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
    }
}
