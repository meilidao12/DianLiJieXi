using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Pipes;
using System.Security.Principal;
using System.Threading;
using System.IO;

namespace CommunicationServers.Pipe
{
    
    public class PipeClientHelper : PipeHelper
    {
        public event NewMessage NewMessageEvent;
        public event ConnectFail ConnectFailEvent;
        NamedPipeClientStream NamedPipeClientStream;

        public PipeClientHelper(string PipeName)
        {
            this.PipeName = PipeName;
        }

        /// <summary>
        /// 属性：客户端是否已连接服务端
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if(this.NamedPipeClientStream != null)
                {
                    return this.NamedPipeClientStream.IsConnected;
                }
                else
                {
                    return false;
                }
            }
        } 

        /// <summary>
        /// 启动客户端
        /// </summary>
        public override void Run()
        {
            try
            {
                this.NamedPipeClientStream = new NamedPipeClientStream(".", this.PipeName, PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.WriteThrough);
                this.NamedPipeClientStream.Connect(200);
                this.NamedPipeClientStream.ReadMode = PipeTransmissionMode.Message;
                this.NamedPipeClientStream.BeginRead(data, 0, data.Length, new AsyncCallback(PipeReadCallback), this.NamedPipeClientStream);
            }
            catch
            {
                ConnectFailEvent?.Invoke();
            }
        }

        /// <summary>
        /// 停止客户端
        /// </summary>
        public override void Stop()
        {
            if (this.NamedPipeClientStream != null)
            {
                this.NamedPipeClientStream.Close();
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="Message">消息内容</param>
        public override void SendMessage(string Message)
        {
            if (this.NamedPipeClientStream.IsConnected)
            {
                try
                {
                    byte[] data = encoding.GetBytes(Message);
                    this.NamedPipeClientStream.Write(data, 0, data.Length);
                    this.NamedPipeClientStream.Flush();
                    this.NamedPipeClientStream.WaitForPipeDrain();
                }
                catch
                {
                    //写日志
                }
            }
        }

        /// <summary>
        /// 接收数据的回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void PipeReadCallback(IAsyncResult ar)
        {
            this.NamedPipeClientStream = (NamedPipeClientStream)ar.AsyncState;
            var count = this.NamedPipeClientStream.EndRead(ar);
            if (count > 0)
            {
                string message = encoding.GetString(data, 0, count);
                NewMessageEvent?.Invoke(message); //简化调用方式 等同于if(NewMessageEvent != Null) {NewMessageEvent(message)}
                this.NamedPipeClientStream.BeginRead(data, 0, data.Length, new AsyncCallback(PipeReadCallback), this.NamedPipeClientStream);
            }
            else if (count == 0)
            {
                ConnectFailEvent?.Invoke();
            }
        }

    }
}
