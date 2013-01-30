using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scraper.Notifier;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Scraper.Crawler
{
    public class Gw2SpidyApi : ITradingPostApi
    {
        public SearchResult ParseSearch(SearchResult sr, JObject json)
        {
            if (sr == null)
                sr = new SearchResult();

            try
            {
                sr.Count = json["count"].ToObject<int>();

                int page = json["page"].ToObject<int>();
                if (page == 0)
                    page++;
                page--;// Yeah that makes sense \o/

                sr.Offset = page * ItemsPerPage + 1;
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
                        SellPrice = token[i]["min_sale_unit_price"].ToObject<int>(),
                        BuyPrice = token[i]["max_offer_unit_price"].ToObject<int>(),
                        BuyVolume = token[i]["offer_availability"].ToObject<long>(),
                        SaleVolume = token[i]["sale_availability"].ToObject<long>(),
                        DateTimeStamp = token[i]["price_last_changed"].ToObject<String>(),
                        Rarity = token[i]["rarity"].ToObject<int>(),
                        ImgUri = imgUri,
                        Quantity = token[i]["sale_availability"].ToObject<int>(),
                        Level = token[i]["restriction_level"].ToObject<int>()
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

        public SearchResult ParseTransaction(SearchResult sr, JObject json, TransactionType type)
        {
            if (sr == null)
                sr = new SearchResult();
            return sr; // Not supported yet
            //try
            //{
            //    sr.Total = json["total"].ToObject<int>();
            //    if (sr.Total > 0)
            //    {
            //        sr.Offset = json["args"]["offset"].ToObject<int>();
            //        sr.Count = json["args"]["count"].ToObject<int>();

            //        JToken token = json["listings"];
            //        for (int i = 0; i < token.Count(); i++)
            //        {
            //            int dataId = token[i]["data_id"].ToObject<int>();

            //            HotItem item = new HotItem(dataId)
            //            {
            //                Name = token[i]["name"].ToObject<String>(),
            //                UnitPrice = token[i]["unit_price"].ToObject<int>(),
            //                ImgUri = token[i]["img"].ToObject<String>(),
            //                Quantity = token[i]["quantity"].ToObject<int>(),
            //                SaleVolume = token[i]["sale_availability"].ToObject<int>(),
            //                BuyVolume = token[i]["offer_availability"].ToObject<int>(),
            //                DateTimeStamp = token[i]["created"].ToObject<String>()
            //            };
            //            //if (TransactionType == Notifier.TransactionType.Buying || TransactionType == Notifier.TransactionType.Selling)
            //            //{
            //            //    item.Crawl(); // we crawl buy and sell listing
            //            //}
            //            sr.Items.Add(item);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Debug.WriteLine(ex);
            //}
            //return sr;
            //GroupItems();
        }

        public HotItem ParseItemListing(HotItem item, JObject json)
        {
            if (item == null)
                item = new HotItem();
            try
            {
                try
                {
                    item.BuyPrice = json["result"]["max_offer_unit_price"].ToObject<int>();
                }
                catch
                {

                }
                try
                {
                    item.SellPrice = json["result"]["min_sale_unit_price"].ToObject<int>();
                }
                catch
                {

                }

                item.BuyVolume = json["result"]["offer_availability"].ToObject<int>();
                item.SaleVolume = json["result"]["sale_availability"].ToObject<int>();
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
                try
                {
                    item.Name = json["result"]["name"].ToObject<String>();
                }
                catch
                {

                }

                try
                {
                    item.ImgUri = json["result"]["img"].ToObject<String>();
                }
                catch
                {

                }
            }
            catch
            {

            }
            return item;
        }

        // TODO: Make the gw2spidy host variable.
        private const String Gw2SpidyHost = "http://www.gw2spidy.com";

        private const String ApiVersion = "v0.9";
        private const String ApiType = "json";
        private const String SearchApi = "item-search";
        private const String ItemApi = "item";

        public string UriSearch(string query, int offset, int count, String type = null, String subType = null, String rarity = null, String levelmin = null, String levelmax = null, String sortingMode = null, bool descending = false)
        {
            //gw2spidy.com/api/v0.9/json/item-search/uni/
            offset += ItemsPerPage; // That makes sense too \o/
            offset = offset / ItemsPerPage;
            String result = String.Format("{0}/api/{1}/{2}/{3}/{4}/{5}", Gw2SpidyHost, ApiVersion, ApiType, SearchApi, query, offset);
            return result;
        }

        public string UriBuildItem(int dataId)
        {
            //gw2spidy.com/api/v0.9/json/item/20323
            return String.Format("{0}/api/{1}/{2}/{3}/{4}", Gw2SpidyHost, ApiVersion, ApiType, ItemApi, dataId);
        }

        public string UriListingItem(int dataId)
        {
            //gw2spidy.com/api/v0.9/json/item/20323
            return String.Format("{0}/api/{1}/{2}/{3}/{4}", Gw2SpidyHost, ApiVersion, ApiType, ItemApi, dataId);
        }

        public string UriTransaction(TransactionType type, int offset, int count)
        {
            return ""; // Not supported
            //return uri.Add("offset", "0").Add("count", "20").Generate();
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

        static String uriCoins = "http://www.gw2spidy.com/api/v0.9/json/gem-price";
        static String uriGems = "http://www.gw2spidy.com/api/v0.9/json/gem-price";

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
            //api/{version}/{format}/gem-price
            //http://www.gw2spidy.com/api/v0.9/json/gem-price
            return String.Format(uriCoins); // Gems
        }

        public string UriBuyGold(int count)
        {
            //http://www.gw2spidy.com/api/v0.9/json/gem-price
            return String.Format(uriGems); // Gold
        }

        public double ParseBuyGemValue(JObject json)
        {
            try
            {
                double value = json["result"]["gold_to_gem"].ToObject<int>(); // Gold value
                value = value / 100.0; // Gold value per 100 gems according to the api 0.9

                return value;
            }
            catch { }
            return 0;
        }

        public double ParseBuyGoldValue(JObject json)
        {
            try
            {
                double value = json["result"]["gem_to_gold"].ToObject<int>(); // Gem value (This is wrong, it's a gold value too)
                // TODO: apply fix when the api changes
                value = value / 100.0;

                return value;
            }
            catch { }
            return 0;
        }
    }
}
