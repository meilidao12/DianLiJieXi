using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtocolFamily.Interface;
using Services;
namespace ProtocolFamily.YanGang
{
    public class AnalysisDataHelper : IAnalysisDataHelper
    {
        public  AnalysisDataModel AnalysisData(string Data)
        {
            AnalysisDataModel model = new AnalysisDataModel();
            string data1 = Data.Substring(0, 43);
            string crc = Data.Substring(43, 4);
            if(CRC.ToModbusCRC16(data1) == crc)
            {
                model.Data0 = data1.Substring(0, 11);  //电话号码
                model.Data1 = MathHelper.HexToDouble(data1.Substring(11,16)); //瞬时流量
                model.Data2 = MathHelper.HexToDouble(data1.Substring(27, 16));
                model.Result = AnalysisDataModel.AnalysisResult.OK;
            }
            else
            {
                model.Result = AnalysisDataModel.AnalysisResult.ERR;
            }
            return model;
        }
    }
}
