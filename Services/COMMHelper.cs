using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Ports;
namespace Services
{
    public delegate void DataReceiveEventHandler(object sender , COMMEventArgs e);
    public class COMMHelper
    {
        #region  ---变量定义
        SerialPort serialPort = new SerialPort();
        public event DataReceiveEventHandler DataReceiveEvent;
        #endregion

        #region  ---打开串口
        public Boolean OpenPort(string portName , string BaudRate)
        {
            if (portName == null)
            {
                return false;
            }
            try
            {
                serialPort.PortName = portName;
                serialPort.BaudRate = int.Parse(BaudRate);
                serialPort.DataBits = 8;
                serialPort.Open();
                this.serialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.OnDataReceived);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public Boolean OpenPort(string portName)
        {
            if (portName == null)
            {
                return false;
            }
            try
            {
                serialPort.BaudRate = 9600;
                serialPort.PortName = portName;
                serialPort.DataBits = 8;
                serialPort.Open();
                this.serialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.OnDataReceived);
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region  ---关闭串口
        public Boolean ClosePort()
        {
            try
            {
                serialPort.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region ---发送数据
        public Boolean Send(byte[] Data)
        {
            try
            {
                serialPort.Write(Data, 0, Data.Length);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public bool SendAsASCII(String ascii)
        {

            try
            {
                if (string.IsNullOrEmpty(ascii)) throw new Exception();
                CharacterConversion characterConversion = new CharacterConversion();
                byte[] data = characterConversion.ASCIIConvertToByte(ascii);
                serialPort.Write(data, 0, data.Length);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public bool SendAsString(String str)
        {

            try
            {
                if (string.IsNullOrEmpty(str)) throw new Exception();
                CharacterConversion characterConversion = new CharacterConversion();
                byte[] data = characterConversion.HexConvertToByte(str);
                serialPort.Write(data, 0, data.Length);
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region  ---获取串口状态
        public bool ComState
        {
            get
            {
                return serialPort.IsOpen;
            }
        }

        #endregion

        #region ---获取串口号
        public string[] GetPort()
        {
            string[] ArryPort = SerialPort.GetPortNames();
            return ArryPort;
        }
        #endregion

        #region  ---接收串口数据事件
        public void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if(serialPort.IsOpen)
            {
                try
                {
                    System.Threading.Thread.Sleep(100);//延时100ms等待接收完数据
                    byte[] readBuffer = new byte[serialPort.BytesToRead];
                    serialPort.Read(readBuffer, 0, readBuffer.Length);
                    COMMEventArgs commEventArgs = new COMMEventArgs(readBuffer);
                    if(DataReceiveEvent != null)
                    {
                        DataReceiveEvent(this, commEventArgs);
                    }
                }
                catch
                {
                }
            }
        }
        #endregion
    } 

    public class COMMEventArgs : EventArgs
    {
        CharacterConversion characterConversion = new CharacterConversion();
        private string backDataAsHex;
        private string backDataAsASCII;
        private byte[] backData;

        public COMMEventArgs(byte[] backData)
        {
            this.backData = backData;
            this.backDataAsHex = characterConversion.ByteConvertToHex(backData);
            this.backDataAsASCII = characterConversion.ByteConvertToASCII(backData);
        }

        public string BackDataAsHex { get { return backDataAsHex; } }

        public string BackDataAsASCII { get { return backDataAsASCII; } }

        public byte[] BackData
        {
            get { return backData; }
        }

    }
}
