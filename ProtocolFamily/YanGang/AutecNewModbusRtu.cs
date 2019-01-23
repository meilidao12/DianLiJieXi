using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services;
namespace ProtocolFamily.YanGang
{
    /// <summary>
    /// 福建澳泰流量积算仪 新ModbusRtu协议
    /// </summary>
    class AutecNewModbusRtu : GPRSProtocol
    {
        public override string ComposeSendData(string ElementData)
        {
            string data = string.Format("403A0011{0}F020201030000000C45CF000D0A", ElementData);
            return data;
        }
        public override AnalysisDataModel AnalysisReceiveData(string RecevieData)
        {
            AnalysisDataModel analysisDataModel = new AnalysisDataModel();
            try
            {
                if(RecevieData.Length != 86) throw new Exception("返回字符串长度不正确 " + RecevieData);
                //---电话号码
                string phoneNum = RecevieData.Substring(8, 11);
                //---瞬时流量
                string HourlyFlowRates = RecevieData.Substring(28, 8);
                HourlyFlowRates = MathHelper.HexToSingle(HourlyFlowRates);
                HourlyFlowRates = MathHelper.DoubleToHex(HourlyFlowRates);
                //---累积流量
                string a = RecevieData.Substring(60, 8);
                string b = RecevieData.Substring(68, 8);
                double c = Convert.ToInt32(MathHelper.HexToSingle(b)) * 10000 + Convert.ToInt32(MathHelper.HexToSingle(a));
                string TotalFlow = MathHelper.DoubleToHex(c.ToString());
                analysisDataModel.Data0 = phoneNum + HourlyFlowRates + TotalFlow;
                //CRC校验
                analysisDataModel.Data0 += CRC.ToModbusCRC16(analysisDataModel.Data0);
                analysisDataModel.Result = AnalysisDataModel.AnalysisResult.OK;
            }
            catch(Exception ex)
            {
                SimpleLogHelper.Instance.WriteLog(LogType.Error, ex, "AutecNewModbusRtu错误");
                analysisDataModel.Result = AnalysisDataModel.AnalysisResult.ERR;
            }
            return analysisDataModel;
        }
    }
}
