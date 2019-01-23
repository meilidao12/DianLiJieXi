using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services;
namespace ProtocolFamily.YanGang
{
    /*
    分析数据，提取出瞬时流量和累积流量
    握手 403A001118531762394F
    发送 403A001118531762394F020401030000000AC5CD000D0A
    接收 403A002118531762394F02010314A4064126AAAB4126000000000000000036BA0001317B110D0A
    */
    public class AutecAory5000 : GPRSProtocol
    {
        public override string ComposeSendData(string ElementData)
        {
            string data = string.Format("403A0011{0}F020401030000000AC5CD000D0A",ElementData);
            return data;
        }

        /*
         解析出来的数据分为4部分 电话号（11个字节) + 瞬时流量（16个字节，为双精度浮点数） + 累计流量（16个字节，为双精度浮点数） + CRC16校验（4个字节）
        */
        public override AnalysisDataModel AnalysisReceiveData(string RecevieData)
        {
            AnalysisDataModel analysisDataModel = new AnalysisDataModel();
            try
            {
                if (RecevieData.Length != 78) throw new Exception("返回字符串长度不正确 " + RecevieData);
                //---电话号码
                string phoneNum = RecevieData.Substring(8, 11);
                //---瞬时流量
                string HourlyFlowRates = RecevieData.Substring(28,8);
                HourlyFlowRates = HourlyFlowRates.Substring(4, 2) + HourlyFlowRates.Substring(6, 2) + HourlyFlowRates.Substring(0, 2) + HourlyFlowRates.Substring(2, 2);
                HourlyFlowRates = MathHelper.HexToSingle(HourlyFlowRates);
                HourlyFlowRates = MathHelper.DoubleToHex(HourlyFlowRates);
                //---累积流量
                string a = RecevieData.Substring(60, 8);
                a = a.Substring(4, 4) + a.Substring(0, 4);
                int b = Convert.ToInt32(a, 16);
                string TotalFlow =  MathHelper.DoubleToHex(b.ToString());
                analysisDataModel.Data0 = phoneNum + HourlyFlowRates + TotalFlow;
                //CRC校验
                analysisDataModel.Data0 += CRC.ToModbusCRC16(analysisDataModel.Data0);
                analysisDataModel.Result = AnalysisDataModel.AnalysisResult.OK;
            }
            catch(Exception ex)
            {
                SimpleLogHelper.Instance.WriteLog(LogType.Error, ex, "AutecAory5000错误");
                analysisDataModel.Result = AnalysisDataModel.AnalysisResult.ERR;
            }
            return analysisDataModel;
        }
    }
}
