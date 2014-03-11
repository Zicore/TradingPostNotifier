using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NotifierCore.Crawler;
using NotifierCore.Notifier;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Globalization;

namespace NotifierCore.DataProvider
{
    /// <summary>
    /// Generates uris parses the requests and provides some vars to differ between the apis
    /// This is the api to the official trading post
    /// </summary>
    public class TradingPostApi : ITradingPostApi
    {
        private const String UriCoins = "https://exchange-live.ncplatform.net/ws/rates.json?id=undefined&coins={0}";
        private const String UriGems = "https://exchange-live.ncplatform.net/ws/rates.json?id=undefined&gems={0}";

        public SearchResult ParseSearch(SearchResult sr, JObject json)
        {
            if (sr == null)
                sr = new SearchResult();

            try
            {
                sr.Offset = json["args"]["offset"].ToObject<int>();
                sr.Count = json["args"]["count"].ToObject<int>();
                if (json["args"]["text"] != null)
                {
                    sr.Text = json["args"]["text"].ToObject<String>();
                }
                sr.Total = json["total"].ToObject<int>();
                JToken token = json["results"];
                for (int i = 0; i < token.Count(); i++)
                {
                    int dataId = token[i]["data_id"].ToObject<int>();
                    string name = token[i]["name"].ToObject<String>();
                    string imgUri = token[i]["img"].ToObject<String>();

                    HotItem item = new HotItem(dataId)
                    {
                        Name = name,
                        SellPrice = token[i]["sell_price"].ToObject<int>(),
                        SaleVolume = token[i]["sell_count"].ToObject<int>(),
                        BuyPrice = token[i]["buy_price"].ToObject<int>(),
                        BuyVolume = token[i]["buy_count"].ToObject<int>(),
                        ImgUri = imgUri,
                        Rarity = token[i]["rarity"].ToObject<int>(),
                        RarityWord = token[i]["rarity_word"].ToObject<String>(),
                        Level = token[i]["level"].ToObject<int>()
                    };

                    sr.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return sr;
        }

        private T ParseToken<T>(JToken token, String key)
        {
            if (token[key] != null)
                return token[key].ToObject<T>();
            return default(T);
        }

        public SearchResult ParseTransaction(SearchResult sr, JObject json, TransactionType type)
        {
            if (sr == null)
                sr = new SearchResult();

            try
            {
                sr.Total = json["total"].ToObject<int>();
                if (sr.Total > 0)
                {
                    sr.Offset = json["args"]["offset"].ToObject<int>();
                    sr.Count = json["args"]["count"].ToObject<int>();

                    JToken token = json["listings"];
                    for (int i = 0; i < token.Count(); i++)
                    {
                        int dataId = token[i]["data_id"].ToObject<int>();
                        var t = token[i];
                        HotItem item = new HotItem(dataId)
                        {
                            Name = ParseToken<String>(t, "name"),
                            UnitPrice = ParseToken<int>(t, "unit_price"),
                            SellPrice = ParseToken<int>(t, "sell_price"),
                            BuyPrice = ParseToken<int>(t, "buy_price"),
                            ImgUri = ParseToken<String>(t, "img"),
                            Quantity = ParseToken<int>(t, "quantity"),
                            SaleVolume = ParseToken<int>(t, "sell_count"),
                            BuyVolume = ParseToken<int>(t, "buy_count"),
                            DateTimeStamp = ParseToken<String>(t, "created"),
                            ListingId = ParseToken<long>(t, "listing_id"),
                            Rarity = ParseToken<int>(t, "rarity"),
                        };

                        if (type == TransactionType.Bought)
                        {
                            item.TransactionTime = ParseToken<DateTime>(t, "purchased");
                        }

                        if (type == TransactionType.Sold)
                        {
                            item.TransactionTime = ParseToken<DateTime>(t, "purchased");
                        }

                        sr.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return sr;
        }

        public HotItem ParseItemListing(HotItem item, JObject json)
        {
            if (item == null)
                item = new HotItem();
            try
            {
                try
                {
                    if (json["listings"] != null && json["listings"]["buys"] != null && json["listings"]["buys"].Any())
                    {
                        item.BuyPrice = json["listings"]["buys"][0]["unit_price"].ToObject<int>();
                    }
                }
                catch
                {

                }
                try
                {
                    if (json["listings"] != null && json["listings"]["sells"] != null && json["listings"]["sells"].Any())
                    {
                        item.SellPrice = json["listings"]["sells"][0]["unit_price"].ToObject<int>();
                    }
                }
                catch
                {

                }

                item.BuyVolume = CalculateSum(json, "buys");
                item.SaleVolume = CalculateSum(json, "sells");
            }
            catch
            {

            }
            return item;
        }

        public HotItem ParseItem(HotItem item, JObject json)
        {
            if (item == null)
                item = new HotItem();
            try
            {
                // ----------------------------------------------------------------------
                // Assume it's an item
                // ----------------------------------------------------------------------

                if (json["result"] != null)
                {
                    try
                    {
                        if (json["result"]["name"] != null)
                        {
                            item.Name = json["result"]["name"].ToObject<String>();
                        }
                    }
                    catch
                    {

                    }
                    try
                    {
                        if (json["result"]["rarity"] != null)
                        {
                            item.Rarity = json["result"]["rarity"].ToObject<int>();
                        }
                    }
                    catch
                    {
                    }
                    try
                    {
                        if (json["result"]["rarity_word"] != null)
                        {
                            item.RarityWord = json["result"]["rarity_word"].ToObject<String>();
                        }
                    }
                    catch
                    {
                    }

                    try
                    {
                        if (json["result"]["img"] != null)
                        {
                            item.ImgUri = json["result"]["img"].ToObject<String>();
                        }
                    }
                    catch
                    {

                    }

                    try
                    {
                        if (json["result"]["level"] != null)
                        {
                            item.Level = json["result"]["level"].ToObject<int>();
                        }
                    }
                    catch
                    {

                    }

                } // End item parsing

                // ----------------------------------------------------------------------
                // Assume it's the search
                // ----------------------------------------------------------------------

                if (json["args"] != null)
                {
                    try
                    {
                        if (json["results"][0]["name"] != null)
                        {
                            item.Name = json["results"][0]["name"].ToObject<String>();
                        }
                    }
                    catch
                    {

                    }
                    try
                    {
                        if (json["results"][0]["rarity"] != null)
                        {
                            item.Rarity = json["results"][0]["rarity"].ToObject<int>();
                        }
                    }
                    catch
                    {
                    }
                    try
                    {
                        if (json["results"][0]["rarity_word"] != null)
                        {
                            item.RarityWord = json["results"][0]["rarity_word"].ToObject<String>();
                        }
                    }
                    catch
                    {
                    }

                    try
                    {
                        if (json["results"][0]["img"] != null)
                        {
                            item.ImgUri = json["results"][0]["img"].ToObject<String>();
                        }
                    }
                    catch
                    {

                    }
                    try
                    {
                        if (json["results"][0]["level"] != null)
                        {
                            item.Level = json["results"][0]["level"].ToObject<int>();
                        }
                    }
                    catch
                    {

                    }
                } // End search parsing
            }
            catch
            {

            }

            return item;
        }

        private long CalculateSum(JObject json, String key)
        {
            long result = 0;
            try
            {
                if (json["listings"] != null)
                {
                    JToken token = json["listings"][key];
                    int length = token.Count();
                    for (int i = 0; i < length; i++)
                    {
                        result += token[i]["quantity"].ToObject<long>();
                    }
                }
            }
            catch
            {

            }

            return result;
        }


        public string UriSearch(
            string query, int offset, int count,
            String type = null, String subType = null,
            String rarity = null, String levelmin = null, String levelmax = null,
            String sortingMode = null, bool descending = false
            )
        {
            UriHelper uri = new UriHelper().UseSearchApi().Add("text", query).Add("offset", offset.ToString()).Add("count", count.ToString());
            if (!String.IsNullOrEmpty(type))
            {
                uri.Add("type", type);
            }
            if (!String.IsNullOrEmpty(subType))
            {
                uri.Add("subtype", subType);
            }
            if (!String.IsNullOrEmpty(rarity))
            {
                uri.Add("rarity", rarity);
            }
            if (!String.IsNullOrEmpty(levelmin))
            {
                uri.Add("levelmin", levelmin);
            }
            if (!String.IsNullOrEmpty(levelmax))
            {
                uri.Add("levelmax", levelmax);
            }

            if (!String.IsNullOrEmpty(sortingMode))
            {
                uri.Add("orderby", sortingMode);
                if (descending)
                {
                    uri.Add("sortdescending", "1");
                }
            }

            var retUri = uri.Generate();
            return retUri;
        }

        public string UriBuildItem(int dataId)
        {
            return new UriHelper().UseSearchApi().Add("ids", dataId.ToString()).Generate();
        }

        public string UriListingItem(int dataId)
        {
            return new UriHelper().UseListingApi().AddId(dataId.ToString()).Generate();
        }

        public string UriTransaction(TransactionType type, int offset, int count)
        {
            String strOffset = offset.ToString(CultureInfo.InvariantCulture);
            String strCount = count.ToString(CultureInfo.InvariantCulture);
            String uri = "";
            UriHelper h = new UriHelper();
            switch (type)
            {
                case TransactionType.Selling:
                    uri = h.UseMeSellListingApi().Add("offset", strOffset).Add("count", strCount).Generate();
                    break;
                case TransactionType.Sold:
                    uri = h.UseMeSoldListingApi().Add("offset", strOffset).Add("count", strCount).Generate();
                    break;
                case TransactionType.Buying:
                    uri = h.UseMeBuyListingApi().Add("offset", strOffset).Add("count", strCount).Generate();
                    break;
                case TransactionType.Bought:
                    uri = h.UseMeBoughtListingApi().Add("offset", strOffset).Add("count", strCount).Generate();
                    break;
                default:
                    break;
            }
            return uri;
        }

        public string ExchangeHost
        {
            get { return "exchange-live.ncplatform.net"; }
        }

        public string ExchangeReferer
        {
            get { return "https://exchange-live.ncplatform.net/"; }
        }

        public string UriBuyGems(int count)
        {
            return String.Format(UriCoins, count); // Gems
        }

        public string UriBuyGold(int count)
        {
            return String.Format(UriGems, count); // Gold
        }

        public bool IsTransactionApiSupported
        {
            get { return true; }
        }

        public bool IsAdvancedSearchSupported
        {
            get { return true; }
        }

        public bool IsUnsafe // Distinction between the trading post api and safer api's
        {
            get { return true; }
        }

        public int WorkerTimeOut
        {
            get { return 12500; }
        }

        public int WorkerTransactionTimeOut
        {
#if DEBUG // I don't like to wait while debugging :)
            get { return 3500; }
#else
            get { return 35000; }
#endif
        }

        public int ItemsPerPage
        {
            get { return 10; }
        }

        public string UserAgent
        {
            get { return "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:16.0) Gecko/20100101 Firefox/16.0"; }
        }

        public double ParseBuyGemValue(JObject json)
        {
            try
            {
                double value = json["results"]["gems"]["coins_per_gem"].ToObject<int>();
                return value;
            }
            catch { }
            return 0;
        }

        public double ParseBuyGoldValue(JObject json)
        {
            try
            {
                double value = json["results"]["coins"]["coins_per_gem"].ToObject<int>();
                return value;
            }
            catch { }
            return 0;
        }
    }
}
