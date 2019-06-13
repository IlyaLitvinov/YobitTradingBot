using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YobitTradingBot.Models
{

    public class GetInfo
    {
        public int success { get; set; }
        public Return @return { get; set; }
    }

    public class Rights
    {
        public int info { get; set; }
        public int trade { get; set; }
        public int withdraw { get; set; }
    }

    public class Funds : Dictionary<string, double>
    {
    }

    public class FundsInclOrders : Dictionary<string, double>
    {
    }

    public class Return
    {
        public Funds funds { get; set; }
        public FundsInclOrders funds_incl_orders { get; set; }
        public Rights rights { get; set; }
        public int transaction_count { get; set; }
        public int open_orders { get; set; }
        public int server_time { get; set; }
    }

}