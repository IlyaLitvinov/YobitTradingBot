using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YobitTradingBot.Models
{
    public class ActiveOrders
    {
        public int success { get; set; }
        public Return2 @return { get; set; }

        public static explicit operator ActiveOrders(Dictionary<string, ParamsTradingPair> v)
        {
            throw new NotImplementedException();
        }
    }



    public class ParamsActiveOrders
    {
        public string pair { get; set; }
        public string type { get; set; }
        public float amount { get; set; }
        public float rate { get; set; }
        public int timestamp_created { get; set; }
        public int status { get; set; }
    }

    public class Return2 : Dictionary<string, ParamsActiveOrders>
    {
    }

    
}