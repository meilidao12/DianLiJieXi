using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Diagnostics;
using Services;
namespace CommunicationServers.Pipe
{
    
    public class PipeServerHelper : PipeHelper
    {
        NamedPipeServerStream NamedPipeServerStream;
        public event NewMessage NewMessageEvent;

        public PipeServerHelper(string PipeName)
        {
            this.PipeName = PipeName;
            SimpleLogHelper.Instance.WriteLog(LogType.Info, PipeName);
        }

        public override void Run()
        {
            this.NamedPipeServerStream = new NamedPipeServerStream(
                PipeName,
                PipeDirection.InOut,
                1,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous
                );
            SimpleLogHelper.Instance.WriteLog(LogType.Info, "开始侦听管道客户端连接");
            this.NamedPipeServerStream.BeginWaitForConnection(new AsyncCallback(PipeServerStart), this.NamedPipeServerStream);
        }

        public override void Stop()
        {
            if (this.NamedPipeServerStream != null)
            {
                this.NamedPipeServerStream.Disconnect();
                this.NamedPipeServerStream.Close();
            }
        }

        public override void SendMessage(string Message)
        {
            if (this.NamedPipeServerStream.IsConnected)
            {
                try
                {
                    byte[] data = encoding.GetBytes(Message);
                    this.NamedPipeServerStream.Write(data, 0, data.Length);
                    this.NamedPipeServerStream.Flush();
                    this.NamedPipeServerStream.WaitForPipeDrain();
                }
                catch
                {
                    SimpleLogHelper.Instance.WriteLog(LogType.Info, "通过管道发送消息失败");
                }
            }
        }

        private void PipeServerStart(IAsyncResult ar)
        {
            var pipeServer = (NamedPipeServerStream)ar.AsyncState;
            pipeServer.EndWaitForConnection(ar);
            SimpleLogHelper.Instance.WriteLog(LogType.Info, "有新的管道连接成功");
            this.NamedPipeServerStream.BeginRead(data, 0, data.Length, new AsyncCallback(PipeReadCallback), this.NamedPipeServerStream);
        }

        private void PipeReadCallback(IAsyncResult ar)
        {
            var pipeServer = (NamedPipeServerStream)ar.AsyncState;
            var count = pipeServer.EndRead(ar);
            if (count > 0)
            {
                string message = encoding.GetString(data, 0, count);
                NewMessageEvent(message);
                pipeServer.BeginRead(data, 0, data.Length, new AsyncCallback(PipeReadCallback), pipeServer);
            }
            else if(count == 0)
            {
                SimpleLogHelper.Instance.WriteLog(LogType.Info, "管道连接已断开");
                pipeServer.Close();
                pipeServer.Dispose();
                Run();
            }  
        }
    }
}
