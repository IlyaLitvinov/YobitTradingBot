using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using YobitTradingBot.Models;
using System.Web.Http;
using System.Text;
using Microsoft.AspNet.Identity;
using System.Security.Cryptography;
using System.Data.Entity;

namespace YobitTradingBot.Controllers
{
    public class BotController : ApiController
    {

        #region                     ПОЛУЧИТЬ ИЛИ СОЗДАТЬ БОТА

        // Контроллер получает данные бота из бд.
        // Если для данного пользователя пока что не создан бот,
        // система создает бота со значениями по умолчанию.
        [HttpGet]
        [ActionName("GetOrCreateBot")]
        public HttpResponseMessage GetOrCreateBot()
        {
            using (var db = new ApplicationContext())
            {
                BotModel bot = new BotModel();
                string userId = User.Identity.GetUserId();

                bot = db.BotModels.FirstOrDefault(b => b.UserId == userId);

                // Если в базе данных не был найден бот
                // прикрепленный к активному пользовотелю,
                // заполняем  бота значениями по умолчанию
                // и записываем его в 
                if (bot == null)
                {
                    bot = new BotModel();
                    // Id пользователя к которому мы прикрепляем бота.
                    bot.UserId = userId;
                    // Максимальное количество одновременно торгуемых пар 
                    bot.TradePairs = 10;
                    // Минимальная цена криптовалюты для торговли (в рублях за одну монету ).
                    bot.MinPriceTrade = 1;
                    // Минимальный размер одного ордера в рублях
                    bot.MinOrder = 10;
                    // Минимальный размер выгоды в процентах
                    bot.MinProfit = 4;
                    // Время ожидания перед продажей, даже если нет выгоды (в часах)
                    bot.TimeLose = 8;
                    // Включен ли бот
                    bot.IsOn = false;

                    db.BotModels.Add(bot);

                    db.SaveChanges();

                }
                return Request.CreateResponse(HttpStatusCode.OK, bot);
            }

        }
        //*-------------------------------------- End ----------------------------------------*

        #endregion

        #region                    ЗАПИСАТЬ НОВЫЕ ДАННЫЕ БОТА

        // Метод получает и обновляет данные бота в бд
        [HttpPut]
        [ActionName("EditBot")]
        public void EditBot([FromBody]BotModel botData)
        {

            using (var db = new ApplicationContext())
            {
                BotModel bot = new BotModel();
                string userId = User.Identity.GetUserId();

                bot = db.BotModels.FirstOrDefault(b => b.UserId == userId);

                bot.TradePairs = botData.TradePairs;
                bot.MinPriceTrade = botData.MinPriceTrade;
                bot.MinOrder = botData.MinOrder;
                bot.MinProfit = botData.MinProfit;
                bot.TimeLose = botData.TimeLose;
                bot.IsOn = botData.IsOn;

                db.Entry(bot).State = EntityState.Modified;

                db.SaveChanges();
            }

        }
        //*-------------------------------------- End ----------------------------------------*

        #endregion



        // Устаревшие контроллеры

        #region                             START TRADING 

        //*----------------------------- Начать торговлю на бирже ---------------------------------* 
        //[HttpGet]
        //[ActionName("StartTrading")]
        //public void StartTrading()
        //{
        //    using (var db = new ApplicationContext())
        //    {
        //        //var user = db.Users.First(u => u.UserName == UserName);
        //        bot.UserId = User.Identity.GetUserId();
        //        db.BotModels.Add(bot);
        //        db.SaveChanges();
        //    }

        //}
        //*-------------------------------------- End ----------------------------------------*

        #endregion

        #region                             GET BOT DATA

        //*----------------------------- Начать торговлю на бирже ---------------------------------* 
        //[HttpGet]
        //[ActionName("GetBotData")]
        //public HttpResponseMessage GetBotData()
        //{
        //    using (var db = new ApplicationContext())
        //    {
        //        string userId = User.Identity.GetUserId();
        //        bot = db.BotModels.First(b => b.UserId == userId); //bot.UserId = User.Identity.GetUserId();
        //        return Request.CreateResponse(HttpStatusCode.OK, bot);
        //    }

        //}
        //*-------------------------------------- End ----------------------------------------*

        #endregion

        #region                             START QUERY PAIRS 

        //*----------------------------- Начать запрос торговых пар ---------------------------------* 
        //[HttpGet]
        //[ActionName("StartQueryPairs")]
        //public HttpResponseMessage StartQueryPairs()
        //{
        //    using (var db = new ApplicationContext())
        //    {
        //        WebClient client = new WebClient();
        //        client.Encoding = Encoding.UTF8;

        //        string json = client.DownloadString("https://yobit.net/api/3/ticker/nmc_rur");
        //        nmcRur = JsonConvert.DeserializeObject<RootObject>(json);
        //        nmcRur.nmc_rur.rank = ((nmcRur.nmc_rur.sell - nmcRur.nmc_rur.buy) / nmcRur.nmc_rur.buy) * nmcRur.nmc_rur.vol;

        //        //json = "https://yobit.net/api/3/ticker/ppc_rur";
        //        //PpcRur ppcRur = JsonConvert.DeserializeObject<PpcRur>(json);

        //        return Request.CreateResponse(HttpStatusCode.OK, nmcRur.nmc_rur.rank);
        //    }
        //}
        //*-------------------------------------- End ----------------------------------------*

        #endregion

        #region                             GET INFO 

        //*----------------------------- Начать запрос торговых пар ---------------------------------* 
        [HttpGet]
        [ActionName("GetInfo")]
        public HttpResponseMessage GetInfo()
        {
            using (var db = new ApplicationContext())
            {
                string parameters = $"method=getInfo&nonce=" + (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

                string address = $"https://yobit.net/tapi//";

                var keyByte = Encoding.UTF8.GetBytes("f7768c9037cd597a59ee43cec2b0f2e3");

                string sign1 = string.Empty;
                byte[] inputBytes = Encoding.UTF8.GetBytes(parameters);
                using (var hmac = new HMACSHA512(keyByte))
                {
                    byte[] hashValue = hmac.ComputeHash(inputBytes);

                    StringBuilder hex1 = new StringBuilder(hashValue.Length * 2);
                    foreach (byte b in hashValue)
                    {
                        hex1.AppendFormat("{0:x2}", b);
                    }
                    sign1 = hex1.ToString();
                }

                WebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(address);
                if (webRequest != null)
                {
                    webRequest.Method = "POST";
                    webRequest.Timeout = 20000;
                    webRequest.ContentType = "application/x-www-form-urlencoded";
                    webRequest.Headers.Add("Key", "38C3B890B0509EBD0C03EFFCB048B40F");
                    webRequest.Headers.Add("Sign", sign1);

                    webRequest.ContentLength = parameters.Length;
                    using (var dataStream = webRequest.GetRequestStream())
                    {
                        dataStream.Write(inputBytes, 0, parameters.Length);
                    }

                    using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                        {
                            var jsonResponse = sr.ReadToEnd();
                            //Console.WriteLine(String.Format("Response: {0}", jsonResponse));
                            return Request.CreateResponse(HttpStatusCode.OK, jsonResponse);
                        }
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, "Damn");
            }
        }
        //*-------------------------------------- End ----------------------------------------*

        #endregion



    }
}