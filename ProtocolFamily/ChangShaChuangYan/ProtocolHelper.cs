using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProtocolFamily.ChangShaChuangYan
{
    public class ProtocolHelper
    {
        public static BaseAnalysis CreateProtocol(string title)
        {
            switch(GetIndex(title))
            {
                case 1:
                    return new DianJiZhiJia();
                case 2:
                    return new FengDaoZhiJia();
                case 3:
                    return new WeiBanYaHe();
                case 4:
                    return new WeiBanGunYa();
                case 5:
                    return new ZuHeYaMao();
                case 6:
                    return new FengDaoFaLan();
                case 7:
                    return new JieYouPanJi();
                default:
                    return null;
            }
        }
        private static int GetIndex(string title)
        {
            switch (title)
            {
                case "电机支架机器人铆接专机":
                    return 1;
                case "风道支架机器人铆接专机":
                    return 2;
                case "围板压合定位专机":
                    return 3;
                case "围板滚压包边专机":
                    return 4;
                case "组合压铆专机":
                    return 5;
                case "风道法兰机器人铆接专机":
                    return 6;
                case "接油盘机器人铆接专机":
                    return 7;
                default:
                    return 0;
            }
        }
    }
}
