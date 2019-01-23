using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolFamily
{
    public class AnalysisDataModel
    {
        public string Data0 { get; set; }
        public string Data1 { get; set; }
        public string Data2 { get; set; }
        public string Data3 { get; set; }
        public string Data4 { get; set; }
        public string Data5 { get; set; }
        public string Data6 { get; set; }
        public string Data7 { get; set; }
        public string Data8 { get; set; }
        public string Data9 { get; set; }

        public AnalysisResult Result { get; set; }
        public enum AnalysisResult
        {
            OK,
            ERR,
            UNKONWN
        }
    }
}
