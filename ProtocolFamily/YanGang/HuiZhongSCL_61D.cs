using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolFamily.YanGang
{
    /// <summary>
    /// 汇中 GPRS 关于SCL-61D通讯协议
    /// 此通讯协议 应为GPRS模块与服务器之间的通讯协议 非透明传输
    /// </summary>
    class HuiZhongSCL_61D : GPRSProtocol
    {
        public override string ComposeSendData(string ElementData)
        {
            return base.ComposeSendData(ElementData);
        }

        public override AnalysisDataModel AnalysisReceiveData(string RecevieData)
        {
            return base.AnalysisReceiveData(RecevieData);
        }
    }
}
