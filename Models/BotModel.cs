using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using YobitTradingBot.Models;

namespace YobitTradingBot.Models
{
    public class BotModel
    {
        [Key]
        public int Id { get; set; }
        // Id пользователя к которому мы прикрепляем бота.
        public string UserId { get; set; }
        // Максимальное количество одновременно торгуемых пар.
        public int TradePairs { get; set; }
        // Минимальная цена криптовалюты для торговли (в рублях за одну монету ).
        public int MinPriceTrade { get; set; }
        // Минимальный размер одного ордера в рублях
        public int MinOrder { get; set; }
        // Минимальный размер выгоды в процентах
        public int MinProfit { get; set; }
        // Время ожидания перед продажей, даже если нет выгоды (дней)
        public int TimeLose { get; set; }
        // Включен ли бот
        public bool IsOn { get; set; }
    }

}