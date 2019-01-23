using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Services;
namespace ProtocolFamily.ChangShaChuangYan
{
    public class BaseAnalysis
    {
        private string powerTime;
        private string authorizeTimer;
        private string errorTimer;
        private string checkTimer;
        private string waitTimer;
        private string runTimer;
        private string cycleTimer;
        private string sumCount;
        private string classCount;
        private string autoRun;
        private string checkState;
        private string errorState;
        private string waitState;

        /// <summary>
        /// 累计上电时间
        /// </summary>
        public string PowerTime
        {
            get
            {
                return powerTime;
            }

            set
            {
                powerTime = value;
            }
        }

        /// <summary>
        /// 累计授权时间
        /// </summary>
        public string AuthorizeTimer
        {
            get
            {
                return authorizeTimer;
            }

            set
            {
                authorizeTimer = value;
            }
        }

        /// <summary>
        /// 累计故障时间
        /// </summary>
        public string ErrorTimer
        {
            get
            {
                return errorTimer;
            }

            set
            {
                errorTimer = value;
            }
        }

        /// <summary>
        /// 累计检修时间
        /// </summary>
        public string CheckTimer
        {
            get
            {
                return checkTimer;
            }

            set
            {
                checkTimer = value;
            }
        }

        /// <summary>
        /// 累计待机时间
        /// </summary>
        public string WaitTimer
        {
            get
            {
                return waitTimer;
            }

            set
            {
                waitTimer = value;
            }
        }

        /// <summary>
        /// 累计运行时间
        /// </summary>
        public string RunTimer
        {
            get
            {
                return runTimer;
            }

            set
            {
                runTimer = value;
            }
        }

        /// <summary>
        /// 节拍周期时间
        /// </summary>
        public string CycleTimer
        {
            get
            {
                return cycleTimer;
            }

            set
            {
                cycleTimer = value;
            }
        }

        /// <summary>
        /// 累计产量
        /// </summary>
        public string SumCount
        {
            get
            {
                return sumCount;
            }

            set
            {
                sumCount = value;
            }
        }

        /// <summary>
        /// 当班产量
        /// </summary>
        public string ClassCount
        {
            get
            {
                return classCount;
            }

            set
            {
                classCount = value;
            }
        }

        /// <summary>
        /// 正常运转状态
        /// </summary>
        public string AutoRun
        {
            get
            {
                return autoRun;
            }

            set
            {
                autoRun = value;
            }
        }

        /// <summary>
        /// 设备检修状态
        /// </summary>
        public string CheckState
        {
            get
            {
                return checkState;
            }

            set
            {
                checkState = value;
            }
        }

        /// <summary>
        /// 设备故障停机状态
        /// </summary>
        public string ErrorState
        {
            get
            {
                return errorState;
            }

            set
            {
                errorState = value;
            }
        }

        /// <summary>
        /// 设备待机状态
        /// </summary>
        public string WaitState
        {
            get
            {
                return waitState;
            }

            set
            {
                waitState = value;
            }
        }

        /// <summary>
        /// 获取累计上电时间
        /// </summary>
        /// <returns></returns>
        protected virtual string GetPowerTime(string data)
        {
            return this.GetValue(301, data);
        }

        /// <summary>
        /// 获取累计授权时间
        /// </summary>
        /// <returns></returns>
        protected virtual string GetAuthorizeTime(string data)
        {
            return this.GetValue(303, data);
        }

        /// <summary>
        /// 获取累计故障时间
        /// </summary>
        /// <returns></returns>
        protected virtual string GetErrorTime(string data)
        {
            return this.GetValue(305, data);
        }

        /// <summary>
        /// 获取累计检修时间
        /// </summary>
        /// <returns></returns>
        protected virtual string GetCheckTime(string data)
        {
            return this.GetValue(307, data);
        }

        /// <summary>
        /// 获取累计待机时间
        /// </summary>
        /// <returns></returns>
        protected virtual string GetWaitTime(string data)
        {
            return this.GetValue(309, data);
        }

        /// <summary>
        /// 获取累计运行时间
        /// </summary>
        /// <returns></returns>
        protected virtual string GetRunTime(string data)
        {
            return this.GetValue(311, data);
        }

        /// <summary>
        /// 获取节拍周期时间
        /// </summary>
        /// <returns></returns>
        protected virtual string GetCycleTime(string data)
        {
            return this.GetValue(319, data);
        }

        /// <summary>
        /// 获取累计产量
        /// </summary>
        /// <returns></returns>
        protected virtual string GetSumCount(string data)
        {
            return this.GetValue(321, data);
        }

        /// <summary>
        /// 获取当班产量
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual string GetClassCount(string data)
        {
            return this.GetValue(323, data);
        }

        /// <summary>
        /// 计算对应plc寄存器地址对应的数据位置
        /// </summary>
        /// <param name="PlcAddress"></param>
        /// <returns></returns>
        protected int CalculateAddress(int PlcAddress)
        {
            return (PlcAddress - 201) * 4;
        }

        /// <summary>
        /// 获取寄存器值
        /// </summary>
        /// <param name="address"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected string GetValue(int address,string data)
        {
            //Console.WriteLine(CalculateAddress(address));
            //Console.WriteLine(data.Length);
            return(MathHelper.HexToDec(data.Substring(CalculateAddress(address), 8)).ToString());
        }

        /// <summary>
        /// 获取设备运转状态
        /// </summary>
        /// <returns></returns>
        protected string GetAutoRun(string data)
        {
            byte a = MathHelper.StrToHexByte(data.Substring(6, 2))[0];
            return ((a & 16) == 16 ? 1 : 0).ToString();
        }

        /// <summary>
        ///  获取设备检修状态
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected string GetCheckState(string data)
        {
            byte a = MathHelper.StrToHexByte(data.Substring(6, 2))[0];
            return ((a & 128) == 128 ? 1 : 0).ToString();
        }

        /// <summary>
        /// 获取设备故障停机状态
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected string GetErrorState(string data)
        {
            byte a = MathHelper.StrToHexByte(data.Substring(4, 2))[0];
            return ((a & 1) == 1 ? 1 : 0).ToString();
        }

        /// <summary>
        /// 获取设备待机状态
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected string GetWaitState(string data)
        {
            byte a = MathHelper.StrToHexByte(data.Substring(4, 2))[0];
            return ((a & 2) == 2 ? 1 : 0).ToString();
        }

        public bool AnalysisData(string data)
        {
            try
            {
                this.powerTime = this.GetPowerTime(data);
                this.authorizeTimer = this.GetAuthorizeTime(data);
                this.errorTimer = this.GetErrorTime(data);
                this.checkTimer = this.GetCheckTime(data);
                this.waitTimer = this.GetWaitTime(data);
                this.runTimer = this.GetRunTime(data);
                this.cycleTimer = this.GetCycleTime(data);
                this.sumCount = this.GetSumCount(data);
                this.classCount = this.GetClassCount(data);
                this.autoRun = this.GetAutoRun(data);
                this.checkState = this.GetCheckState(data);
                this.errorState = this.GetErrorState(data);
                this.waitState = this.GetWaitState(data);
            }
            catch(Exception ex)
            {
                return false;
                SimpleLogHelper.Instance.WriteLog(LogType.Info, ex);
            }
            return true;
        }
    }
}
