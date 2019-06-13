using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YobitTradingBot.Models
{
    public class InfoModel
    {
        public int server_time { get; set; }
        public Pairs pairs { get; set; }
    }

    public class Pairs : Dictionary <string, InfoParam>
    {
    }

    public class InfoParam
    {
        public int decimal_places { get; set; }
        public double min_price { get; set; }
        public int max_price { get; set; }
        public double min_amount { get; set; }
        public double min_total { get; set; }
        public int hidden { get; set; }
        public double fee { get; set; }
        public double fee_buyer { get; set; }
        public double fee_seller { get; set; }
    }
}