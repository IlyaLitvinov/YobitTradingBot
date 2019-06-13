using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YobitTradingBot.Models
{

    public class ParamsTradingPair
    {
        public double high { get; set; }
        public double low { get; set; }
        public double avg { get; set; }
        public double vol { get; set; }
        public double vol_cur { get; set; }
        public double last { get; set; }
        public double buy { get; set; }
        public double sell { get; set; }
        public double updated { get; set; }
        public double rank { get; set; }
        public double min_amount { get; set; }
    }

    public class TradingPairs : Dictionary<string, ParamsTradingPair>
    {
    }

    
}