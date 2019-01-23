using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProtocolFamily.Modbus
{
    public class ModbusTcpServer : ModbusFunction
    {
        public override string ResponseRegister()
        {
            return base.ResponseRegister();
        }
        public override bool AnalysisData(string data)
        {
            return base.AnalysisData(data);
        }
    }
}
