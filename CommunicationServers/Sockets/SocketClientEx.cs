using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CommunicationServers.Sockets
{
    /// <summary>
    /// 异步SocketClient
    /// </summary>
    public class SocketClientEx
    {
        private ManualResetEvent receiveDone = new ManualResetEvent(false);
        private ManualResetEvent connectDone = new ManualResetEvent(false);
        private ManualResetEvent sendDone = new ManualResetEvent(false);
        // The response from the remote device.
        private String response = String.Empty;
        private byte[] buffer = new byte[1024];
        public event NewMessage NewMessageEvent;
        public event Disconnected ServerDisconnectedEvent;
        private Socket clientSocket;
        public  Socket ClientSocket
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
                IPAddress.TryParse(IP, out ip);
            }
            else
            {
                return false;
            }
            try
            {
                IPEndPoint remoteEP = new IPEndPoint(ip, int.Parse(Port));
                clientSocket.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), clientSocket);
                if(!connectDone.WaitOne(100,false)) //100ms没有连接成功则认为连接失败
                {
                    return false;
                }
                Receive(clientSocket);
                return true;
            }
            catch (Exception ex)
            {
                SimpleLogHelper.Instance.WriteLog(LogType.Error, ex, "连接服务器时发生错误");
                return false;
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;
                // Complete the connection.
                client.EndConnect(ar);
                client.RemoteEndPoint.ToString();
                // Signal that the connection has been made.
                connectDone.Set();
            }
            catch (Exception ex)
            {
                SimpleLogHelper.Instance.WriteLog(LogType.Error, ex);
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name = "ServerSocket" ></ param >
        /// < param name="Message"></param>
        public void Send(Socket client, byte[] buffer)
        {
            try
            {
                if (client.Connected == true)
                {
                    client.BeginSend(buffer, 0, buffer.Length, 0, new AsyncCallback(SendCallback), client);
                }
            }
            catch (Exception ex)
            {
                SimpleLogHelper.Instance.WriteLog(LogType.Error, ex);
            }
        }

        public void Send(Socket client, String data)
        {
            try
            {
                if(client.Connected == true)
                {
                    byte[] byteData = Encoding.ASCII.GetBytes(data);
                    client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
                }
            }
            catch(Exception ex)
            {
                SimpleLogHelper.Instance.WriteLog(LogType.Error, ex);
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;
                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);
                // Signal that all bytes have been sent.
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void Receive(Socket client)
        {
            try
            {
                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = client;
                // Begin receiving the data from the remote device.
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;
                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);
                if (bytesRead == 0) throw new Exception();
                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                    // Get the rest of the data.
                    byte[] message = new byte[bytesRead];
                    Array.Copy(state.buffer,message,bytesRead);
                    NewMessageEvent(client, message);
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    // All the data has arrived; put it in response.
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                    }
                    // Signal that all bytes have been received.
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                IsConnected = false;
                SimpleLogHelper.Instance.WriteLog(LogType.Info, "客户端连接断开");
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
            catch (Exception ex)
            {
                SimpleLogHelper.Instance.WriteLog(LogType.Error, ex, "与服务器断开连接发生错误");
                return false;
            }
        }

        private bool isConnected;
        /// <summary>
        /// 判断网络连接状态
        /// </summary>
        /// <returns></returns>
        public bool IsConnected
        {
            get
            {
                return isConnected;
            }
            set
            {
                isConnected = value;
            }
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
    }
    public class StateObject

    {

        // Client socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1000;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();

    }
}
