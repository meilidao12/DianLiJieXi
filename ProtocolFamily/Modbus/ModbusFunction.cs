using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProtocolFamily.Modbus
{
    
    public class ModbusFunction
    {
        private string slaveId;
        private string affairID;
        private string protocolID;
        private string length;
        private string address;
        private string registerAddress;
        private string backDataLength;

        /// <summary>
        /// 单元标识符
        /// </summary>
        public string SlaveId
        {
            set
            {
                slaveId = value;
            }
            get
            {
                return slaveId;
            }
        }
        
        /// <summary>
        /// 单元标识符 1字节
        /// 整数内部转化为HEX 
        /// </summary>
        public string Address
        {
            set
            {
                address = value;
            }
        }

        /// <summary>
        /// 长度 2字节
        /// 整数内部转化为HEX
        /// </summary>
        public string Length
        {
            get
            {
                return length;
            }

            set
            {
                length = value;
            }
        }

        /// <summary>
        /// 协议标识符 2字节 0000 MODBUS协议
        /// </summary>
        public string ProtocolID
        {
            get
            {
                return protocolID;
            }

            set
            {
                protocolID = value;
            }
        }

        /// <summary>
        /// 事务元标识符 2字节
        /// 16进制
        /// </summary>
        public string AffairID
        {
            get
            {
                return affairID;
            }

            set
            {
                affairID = value;
            }
        }

        /// <summary>
        /// 读寄存器
        /// </summary>
        public static string ReadHoldingRegisters
        {
            get
            {
                return "03"; //HEX
            }
        }

        /// <summary>
        /// 写寄存器
        /// </summary>
        public static string WriteMultipleRegisters
        {
            get
            {
                return "10"; //HEX
            }
        }

        /// <summary>
        /// 寄存器地址
        /// </summary>
        public string RegisterAddress
        {
            get
            {
                return registerAddress;
            }

            set
            {
                registerAddress = value;
            }
        }

        public string BackDataLength
        {
            get
            {
                return backDataLength;
            }

            set
            {
                backDataLength = value;
            }
        }

        /// <summary>
        /// 读寄存器
        /// </summary>
        /// <returns></returns>
        public virtual string ReadRegister()
        {
            return "";
        }

        /// <summary>
        /// 写寄存器
        /// </summary>
        /// <returns></returns>
        public virtual string WriteRegister()
        {
            return "";
        }

        /// <summary>
        /// 相应寄存器 作为服务端使用
        /// </summary>
        /// <returns></returns>
        public virtual string ResponseRegister()
        {
            return "";
        }

        public virtual bool AnalysisData(string data)
        {
            return false;
        }
    }
}
