using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommunicationServers.Sockets;
using Services;
using System.Threading;
using System.Windows.Threading;
using System.Net.Sockets;

namespace DianLiJieXi
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        SocketClientEx sockClient;
        SocketServer sockServer;
        Socket socketClient;
        IniHelper ini;
        DispatcherTimer timer; //启动发送测试验证
        int timerCount;
        string[] yaocevalues;
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ini = new IniHelper(System.IO.Directory.GetCurrentDirectory() + @"\Set.ini");
            this.txtIP.Text = ini.ReadIni("Config", "IP");
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timerCount++;
            if(timerCount == 10)
            {
                timerCount = 0;
                ConnectToClient();
            }
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            //timer.Start();
            //ConnectToClient();
            IniServerSocket();
        }

        private void IniServerSocket()
        {
            sockServer = new SocketServer();
            if(sockServer.Listen("502"))
            {
                AddLog("侦听502端口成功");
            }
            sockServer.NewMessage1Event += SockServer_NewMessage1Event;
            sockServer.NewConnnectionEvent += SockServer_NewConnnectionEvent;
        }

        private void SockServer_NewConnnectionEvent(System.Net.Sockets.Socket socket)
        {
            socketClient = socket;
        }

        private void SockServer_NewMessage1Event(System.Net.Sockets.Socket socket, string Message)
        {
            //00A200000006010303E80020
            //00A2 00 00 00 43 01 03 40 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
            string backdata;
            Console.WriteLine(Message);
            int addr = MathHelper.HexToDec(Message.Substring(16, 4));
            if (addr >= 1000)
            {
                backdata = this.BackYaoCe(Message);
                sockServer.Send(socketClient, sockServer.StringConvertToByte(backdata));
            }
        }

        private string BackYaoCe(string Message)
        {
            string backdata = Message.Substring(0, 10);
            int totalBackLength = MathHelper.HexToDec(Message.Substring(22, 2)) *2 + 3;
            int backlength = MathHelper.HexToDec(Message.Substring(22, 2)) *2;
            backdata += MathHelper.DecToHex(totalBackLength.ToString())+ "0103" + MathHelper.DecToHex(backlength.ToString());
            //数据部分
            int count = backlength / 4;
            int addr = (MathHelper.HexToDec(Message.Substring(16, 4))-1000)/2;
            for (int i=0;i<count;i++)
            {
                string a = ini.ReadIni("YaoCe", (addr + 16385 + i).ToString());
                if(string.IsNullOrEmpty(a))
                {
                    a = "00000000";
                }
                backdata += a;
            }
            return backdata;
        }
        private void ConnectToClient()
        {
            ini.WriteIni("Config", "IP", this.txtIP.Text);
            sockClient = new SocketClientEx();
            sockClient.NewMessageEvent += SockClient_NewMessageEvent;
            if (sockClient.Connnect("2404", this.txtIP.Text))
            {
                AddLog("连接成功" + this.txtIP.Text);
                AddLog("发送：" + "680407000000");
                sockClient.Send(sockClient.ClientSocket, sockClient.StringConvertToByte("680407000000"));
            }
            else
            {
                AddLog("连接失败" + this.txtIP.Text);
            }
        }

        private void SockClient_NewMessageEvent(System.Net.Sockets.Socket socket, byte[] Message)
        {
            timerCount = 0;
            string receiveData = MathHelper.ByteToHexStr(Message);
            AddLog("接收数据：" + receiveData);
            switch(receiveData.ToUpper().Substring(2,2))
            {
                case "04":
                    switch(receiveData.ToUpper())
                    {
                        case "68040B000000":
                            sockClient.Send(sockClient.ClientSocket, sockClient.StringConvertToByte("680e0000000064010600010000000014"));
                            break;
                        case "680443000000":
                            sockClient.Send(sockClient.ClientSocket, sockClient.StringConvertToByte("680483000000"));
                            break;
                        case "680483000000":
                            this.timer.Stop();
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    string leixingbiaoshi = receiveData.ToUpper().Substring(12, 2);
                    int sendcount = MathHelper.HexToDec(receiveData.ToUpper().Substring(6, 2) + receiveData.ToUpper().Substring(4, 2));
                    sendcount += 2;
                    if (sendcount == 65534)
                    {
                        sockClient.Send(sockClient.ClientSocket, sockClient.StringConvertToByte("680e0000000064010600010000000014"));
                        return;
                    }
                    string backdata =  MathHelper.DecToHex(sendcount.ToString()).PadLeft(4,'0');
                    backdata = "68040100" + backdata.Substring(2, 2) + backdata.Substring(0, 2);
                    sockClient.Send(sockClient.ClientSocket, sockClient.StringConvertToByte(backdata));
                    AddLog("S帧: " + backdata);
                    if (receiveData.ToUpper().Substring(2, 2) == "0E") return;
                    switch (leixingbiaoshi)
                    {
                        case "01": //遥信
                            int xinxitishuliang = GetXinXiTiShuLiang(receiveData.ToUpper().Substring(14, 2));
                            int address = GetXinXiTiAddr(receiveData.ToUpper().Substring(24, 4));
                            string baowen = receiveData.ToUpper().Substring(30, xinxitishuliang * 2);
                            this.WriteToSetIni(xinxitishuliang, address, baowen);
                            break;
                        case "0D": //遥测
                            if(receiveData.ToUpper().Substring(16,2) == "03") //传输原因：自发
                            {
                                int count = MathHelper.HexToDec(receiveData.ToUpper().Substring(14, 2));
                                WriteToSetIni(count, receiveData.ToUpper().Substring(24, count * 8 * 2));
                            }
                            break;
                        default:   //电度
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        /// 将数据写入到set.ini文件中 对应遥测
        /// </summary>
        /// <param name="count">信息体个数</param>
        /// <param name="baowen">报文</param>
        private void WriteToSetIni(int count,  string baowen)
        {
            try
            {
                for (int i = 0; i < count; i++)
                {
                    string data = baowen.Substring(i * 16, 16);
                    int addr =MathHelper.HexToDec(data.Substring(2, 2) + data.Substring(0, 2));
                    string value = data.Substring(6, 8);
                    ini.WriteIni("YaoCe", addr.ToString(), value);
                    AddLog(addr.ToString() + ": " + value);
                }
            }
            catch (Exception ex)
            {
                SimpleLogHelper.Instance.WriteLog(LogType.Error, ex);
            }
        }

        /// <summary>
        /// 将数据写入到set.ini文件中 对应遥信
        /// </summary>
        /// <param name="count">信息体个数</param>
        /// <param name="addr">信息体地址</param>
        /// <param name="baowen">报文</param>
        private void WriteToSetIni(int count ,int addr, string baowen)
        {
            try
            {
                for(int i = 0; i < count;i++)
                {
                    int index = i * 2;
                    Console.WriteLine(baowen.Length);
                    string xinxi = baowen.Substring(index + 1, 1);
                    ini.WriteIni("YaoXin", addr.ToString(), xinxi);
                    addr += 1;
                }
            }
            catch (Exception ex)
            {
                SimpleLogHelper.Instance.WriteLog(LogType.Error, ex);
            }
        }

        private int GetXinXiTiShuLiang(string xinxitishuliang)
        {
            int b = MathHelper.HexToDec(xinxitishuliang);
            int c = 127;
            return c & b;
        }
        private int GetXinXiTiAddr(string Addr)
        {
            Addr = Addr.Substring(2, 2) + Addr.Substring(0, 2);
            return MathHelper.HexToDec(Addr);
        }

        private void AddLog(string log)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    log = DateTime.Now + ": " + log;
                    this.lstInfoLog.Items.Add(log);
                    Decorator decorator = (Decorator)VisualTreeHelper.GetChild(lstInfoLog, 0);
                    ScrollViewer scrollViewer = (ScrollViewer)decorator.Child;
                    scrollViewer.ScrollToEnd();
                    if (this.lstInfoLog.Items.Count >= 1000)
                    {
                        int ClearLstCount = lstInfoLog.Items.Count - 1000;
                        this.lstInfoLog.Items.RemoveAt(ClearLstCount);
                    }
                }
                catch (Exception ex)
                {
                    SimpleLogHelper.Instance.WriteLog(LogType.Error, ex);
                }
            }));
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (this.sockClient == null) return;
            this.sockClient.Disconnect();
        }
    }
}
