using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json.Linq;
using NotifierCore.Crawler;
using NotifierCore.Notifier;

namespace NotifierCore.DataProvider
{
    public class ZicoreApi : ITradingPostApi, ITrendApi
    {
        public SearchResult ParseSearch(SearchResult sr, JObject json)
        {
            throw new NotImplementedException();
        }

        public SearchResult ParseTransaction(SearchResult sr, JObject json, TransactionType type)
        {
            throw new NotImplementedException();
        }

        public HotItem ParseItemListing(HotItem item, JObject json)
        {
            throw new NotImplementedException();
        }

        public HotItem ParseItem(HotItem item, JObject json)
        {
            throw new NotImplementedException();
        }

        public string UriSearch(string query, int offset, int count, String type = null, String subType = null, String rarity = null, String levelmin = null, String levelmax = null, String sortingMode = null, bool descending = false)
        {
            throw new NotImplementedException();
        }

        public string UriBuildItem(int dataId)
        {
            throw new NotImplementedException();
        }

        public string UriListingItem(int dataId)
        {
            throw new NotImplementedException();
        }

        public string UriTransaction(TransactionType type, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public bool IsTransactionApiSupported
        {
            get { return false; }
        }

        public bool IsAdvancedSearchSupported
        {
            get { return false; }
        }

        public bool IsUnsafe // Distinction between the trading post api and safer api's
        {
            get { return false; }
        }

        public int WorkerTimeOut
        {
            get { return 60000; }
        }

        public int WorkerTransactionTimeOut
        {
            get { return 60000; }
        }

        public int ItemsPerPage
        {
            get { return 50; }
        }

        public string UserAgent // Make sure you change this when you modify the notifier
        {
            get { return "ztpn-r10"; }
        }

        public string ExchangeHost
        {
            get { return ""; }
        }

        public string ExchangeReferer
        {
            get { return ""; }
        }

        public string UriBuyGems(int count)
        {
            throw new NotImplementedException();
        }

        public string UriBuyGold(int count)
        {
            throw new NotImplementedException();
        }

        public double ParseBuyGemValue(JObject json)
        {
            throw new NotImplementedException();
        }

        public double ParseBuyGoldValue(JObject json)
        {
            throw new NotImplementedException();
        }

        private const String Host = "http://notifier.zicore.de";
        private const String TrendApi = "trend";
        private const String TrendSell = "sell";
        private const String TrendBuy = "buy";

        public string UriTrendSell()
        {
            return String.Format("{0}/{1}/{2}", Host, TrendApi, TrendSell);
        }

        public string UriTrendBuy()
        {
            return String.Format("{0}/{1}/{2}", Host, TrendApi, TrendBuy);
        }

        public IList<HotItem> ParseTrendSell(JToken json)
        {
            var items = new List<HotItem>();
            foreach (var token in json)
            {
                var item = new HotItem(token["data_id"].ToObject<int>())
                    {
                        SellPriceMoveCurrent = token["sell_price"].ToObject<float>(),
                        BuyPriceMoveCurrent = token["buy_price"].ToObject<float>(),
                        SellCountMove = token["sell_count"].ToObject<double>(),
                        BuyCountMove = token["buy_count"].ToObject<double>(),

                        DateTimeTrend = token["datetime"].ToObject<DateTime>(),

                        SellPriceMove = token["sell_price_move"].ToObject<float>(),
                        BuyPriceMove = token["buy_price_move"].ToObject<float>(),
                        SellCountMovePercent = token["sell_count_move"].ToObject<float>(),
                        BuyCountMovePercent = token["buy_count_move"].ToObject<float>(),
                    };
                items.Add(item);
            }
            return items;
        }

        public IList<HotItem> ParseTrendBuy(JToken json)
        {
            var items = new List<HotItem>();
            foreach (var token in json)
            {
                var item = new HotItem(token["data_id"].ToObject<int>())
                {
                    SellPriceMoveCurrent = token["sell_price"].ToObject<float>(),
                    BuyPriceMoveCurrent = token["buy_price"].ToObject<float>(),
                    SellCountMove = token["sell_count"].ToObject<double>(),
                    BuyCountMove = token["buy_count"].ToObject<double>(),

                    DateTimeTrend = token["datetime"].ToObject<DateTime>(),

                    SellPriceMove = token["sell_price_move"].ToObject<float>(),
                    BuyPriceMove = token["buy_price_move"].ToObject<float>(),
                    SellCountMovePercent = token["sell_count_move"].ToObject<float>(),
                    BuyCountMovePercent = token["buy_count_move"].ToObject<float>(),
                };

                items.Add(item);
            }
            return items;
        }
    }
}