using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services;
using System.Diagnostics;
namespace ProtocolFamily.YanGang
{
    /// <summary>
    /// 福建澳泰流量积算仪 旧ModbusRtu协议
    /// </summary>
    public class AutecOldModbusRtu : GPRSProtocol
    {
        public override string ComposeSendData(string ElementData)
        {
            string data = string.Format("403A0011{0}F0204010300000006C5C8000D0A", ElementData);
            return data;
        }
        public override AnalysisDataModel AnalysisReceiveData(string RecevieData)
        {
            AnalysisDataModel analysisDataModel = new AnalysisDataModel();
            try
            {
                if (RecevieData.Length != 62) throw new Exception("返回字符串长度不正确 " + RecevieData);
                //---电话号码
                string phoneNum = RecevieData.Substring(8, 11);
                //---瞬时流量
                string HourlyFlowRates = RecevieData.Substring(28, 4);
                HourlyFlowRates = HexToFloat(HourlyFlowRates);
                //---累积流量
                string a = RecevieData.Substring(44, 4);
                string b = RecevieData.Substring(48, 4);
                double d = (Convert.ToInt32(a, 16) * 10000 + Convert.ToInt32(b, 16));
                string TotalFlow = MathHelper.DoubleToHex((d / 10).ToString("#0.0"));
                analysisDataModel.Data0 = phoneNum + HourlyFlowRates + TotalFlow;
                //CRC校验
                analysisDataModel.Data0 += CRC.ToModbusCRC16(analysisDataModel.Data0);
                analysisDataModel.Result = AnalysisDataModel.AnalysisResult.OK;
            }
            catch(Exception ex)
            {
                SimpleLogHelper.Instance.WriteLog(LogType.Error, ex, "AutecOldModbusRtu错误");
                analysisDataModel.Result = AnalysisDataModel.AnalysisResult.ERR;
            }
            return analysisDataModel;
        }

        /// <summary>
        /// 福建澳泰流量积算仪 旧ModbusRtu协议 专用
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        private string HexToFloat(string Data)
        {
            double a = Convert.ToInt16(Data, 16);
            string b = (a / 10).ToString();
            b = MathHelper.DoubleToHex(b); //转为16进制 单精度浮点数
            return b;
        }
    }
}
