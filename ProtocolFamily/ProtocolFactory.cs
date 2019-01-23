using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtocolFamily.YanGang;
namespace ProtocolFamily
{
    public class ProtocolFactory
    {
        public static GPRSProtocol CreateGPRSProtocol(ProtocolName Name)
        {
            switch(Name)
            {
                case ProtocolName.AutecAory5000:
                    return new AutecAory5000();
                case ProtocolName.AutecNewModbusRtu:
                    return new AutecNewModbusRtu();
                case ProtocolName.AutecOldModbusRtu:
                    return new AutecOldModbusRtu();
                case ProtocolName.HuiZhongSCL_61D:
                    return new HuiZhongSCL_61D();
                default:
                    return new GPRSProtocol();  
            }
        }
        public enum ProtocolName
        {
            NoFind = 0,
            AutecAory5000 = 1,
            AutecNewModbusRtu = 2,
            AutecOldModbusRtu = 3,
            HuiZhongSCL_61D = 4
            
        }

    }
}
