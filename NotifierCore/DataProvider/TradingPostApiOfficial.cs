using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GW2DotNET;
using GW2DotNET.Entities.Commerce;
using GW2DotNET.V2.Common;
using NotifierCore.Crawler;
using NotifierCore.DB;
using NotifierCore.Notifier;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Globalization;
using Newtonsoft.Json;

namespace NotifierCore.DataProvider
{
    public class TradingPostApiOfficial
    {
        public static ServiceManager ServiceManager = new ServiceManager();
        public static IRepository<int, AggregateListing> PriceService = ServiceFactory.Default().GetPriceService();
        public static ICollection<int> TradingPostItems;

        public static Dictionary<int, Item> ItemDB = new Dictionary<int, Item>();
        public static Dictionary<int, Item> ItemTradingPostDB = new Dictionary<int, Item>();
        public static List<String> CategoriesDB = new List<String>();
        private static int _itemsPerPage = 1000;

        public static int ItemsPerPage
        {
            get { return _itemsPerPage; }
            set { _itemsPerPage = value; }
        }

        public static void Initialize()
        {
            TradingPostItems = PriceService.Discover();
            var items = LoadItemFile(HotItemController.Config.LanguageKey);

            ItemDB = new Dictionary<int, Item>();
            ItemTradingPostDB = new Dictionary<int, Item>();

            foreach (var item in items)
            {
                ItemDB[item.Id] = item;
                if (TradingPostItems.Contains(item.Id))
                {
                    item.TradingPostItem = true;
                    ItemTradingPostDB[item.Id] = item;
                }
            }

            LoadCategories(ItemTradingPostDB);
        }

        public static void LoadCategories(Dictionary<int, Item> items)
        {
            CategoriesDB = items.Values
              .GroupBy(p => p.Type)
              .Select(g => g.First().Type)
              .OrderBy(x => x)
              .ToList();
        }

        public static List<Item> LoadItemFile(String language)
        {
            lock (ItemTradingPostDB)
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DB",
                                        String.Format("items_{0}.json", language));
                using (var sr = new StreamReader(path))
                {
                    String json = sr.ReadToEnd();
                    var items = JsonConvert.DeserializeObject<List<Item>>(json);
                    return items;
                }
            }
        }


    }
}
