using System;
using System.Collections.Generic;
using System.Linq;
using GW2DotNET.Entities.Commerce;
using NotifierCore.Crawler;
using NotifierCore.DataProvider;
using NotifierCore.IO;
using NotifierCore.Notifier.Event;
using System.Threading;
using System.Collections.ObjectModel;
using LibraryBase.Wpf.ViewModel;
using System.Threading.Tasks;
using LibraryBase.Wpf.Event;
using System.IO;
using Zicore.Xml;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace NotifierCore.Notifier
{
    public enum GuildWars2Status
    {
        NotRunning,
        SearchingKey,
        FoundKey,
        Loading,
        FinishedLoading
    }

    public class HotItemController : BindableBase, ICsvExport, ICsvImport
    {
        static Random random = new Random();
        static HotItemController _self;
        public static HotItemController Self
        {
            get { return _self; }
            set { _self = value; }
        }

        private ObservableCollection<HotItem> _trendsBuy = new ObservableCollection<HotItem>();
        private ObservableCollection<HotItem> _trendsSell = new ObservableCollection<HotItem>();

        public ObservableCollection<HotItem> TrendsBuy
        {
            get { return _trendsBuy; }
            set
            {
                _trendsBuy = value;
                OnPropertyChanged("TrendsBuy");
            }
        }

        public ObservableCollection<HotItem> TrendsSell
        {
            get { return _trendsSell; }
            set
            {
                _trendsSell = value;
                OnPropertyChanged("TrendsSell");
            }
        }



        public event EventHandler<PriceChangedEventArgs> ItemBuild;
        public event EventHandler<NotificationEventArgs> BuyNotification;
        public event EventHandler<PriceChangedEventArgs> BuyPriceChanged;
        public event EventHandler<PriceChangedEventArgs> SellPriceChanged;
        public event EventHandler<EventArgs<GuildWars2Status>> GuildWars2StatusChanged;
        public event EventHandler RecipesLoaded;

        // ------------------------------------------------------- //

        Thread _tMainWorker;

        // ------------------------------------------------------- //

        private bool _usingBackupApi = false;
        public bool UsingBackupApi
        {
            get { return _usingBackupApi; }
            set { _usingBackupApi = value; }
        }

        private static bool _isTransactionsSupported;
        public static bool IsTransactionsSupported
        {
            get { return HotItemController._isTransactionsSupported; }
            set { HotItemController._isTransactionsSupported = value; }
        }

        private static bool _isMultiLanguageSupported;
        public static bool IsMultiLanguageSupported
        {
            get { return HotItemController._isMultiLanguageSupported; }
            set { HotItemController._isMultiLanguageSupported = value; }
        }

        private static ITradingPostApi _currentApi;
        public static ITradingPostApi CurrentApi
        {
            get { return _currentApi; }
            set { _currentApi = value; }
        }

        private static ITrendApi _currentTrendApi;
        public static ITrendApi CurrentTrendApi
        {
            get { return _currentTrendApi; }
            set { _currentTrendApi = value; }
        }

        private static Config _config;
        public static Config Config
        {
            get { return _config; }
            set { _config = value; }
        }

        private static bool _isOfficialDatasource = false;
        public static bool IsOfficialDatasource
        {
            get { return _isOfficialDatasource; }
            set { _isOfficialDatasource = value; }
        }

        private bool _isSessionKeyValid;
        public bool IsSessionKeyValid
        {
            get { return _isSessionKeyValid; }
            set { _isSessionKeyValid = value; }
        }

        private static ImageCache _cache = new ImageCache("Cache");
        public static ImageCache Cache
        {
            get { return HotItemController._cache; }
            set { HotItemController._cache = value; }
        }

        private static ScrapeHelper _scraperHelper;
        public static ScrapeHelper ScraperHelper
        {
            get { return _scraperHelper; }
            set { _scraperHelper = value; }
        }

        private bool _guildWars2Running = false;
        public bool GuildWars2Running
        {
            get { return _guildWars2Running; }
            set
            {
                _guildWars2Running = value;
                OnPropertyChanged("GuildWars2Running");
            }
        }

        private GuildWars2Status _guildWars2Status;
        public GuildWars2Status GuildWars2Status
        {
            get { return _guildWars2Status; }
            set
            {
                _guildWars2Status = value;
                if (GuildWars2StatusChanged != null)
                {
                    GuildWars2StatusChanged(this, new EventArgs<Notifier.GuildWars2Status>(GuildWars2Status));
                }
            }
        }

        GemManager gem = new GemManager();
        public GemManager Gem
        {
            get { return gem; }
            set { gem = value; }
        }

        ObservableCollection<HotItem> _queue = new ObservableCollection<HotItem>();
        public ObservableCollection<HotItem> Queue
        {
            get { return _queue; }
            private set
            {
                _queue = value;
                OnPropertyChanged("Queue");
            }
        }

        ObservableCollection<HotItem> _recipeQueue = new ObservableCollection<HotItem>();
        public ObservableCollection<HotItem> RecipeQueue
        {
            get { return _recipeQueue; }
            private set
            {
                _recipeQueue = value;
                OnPropertyChanged("RecipeQueue");
            }
        }

        private bool _isRunning = true;
        public bool IsRunning
        {
            get { return _isRunning; }
            set
            {
                _isRunning = value;
                if (!IsRunning)
                {
                    if (_tMainWorker != null)
                    {
                        _tMainWorker.Interrupt();
                    }
                }
                OnPropertyChanged("IsRunning");
            }
        }

        List<Recipe> _recipeDB = new List<Recipe>();
        public List<Recipe> RecipeDB
        {
            get { return _recipeDB; }
            set { _recipeDB = value; }
        }

        List<SlimItem> _itemDB = new List<SlimItem>();
        public List<SlimItem> ItemDB
        {
            get { return _itemDB; }
            set { _itemDB = value; }
        }

        // ------------------------------------------------------- //
        ITradingPostApi gw2SpidyApi = new Gw2SpidyApi();

        public HotItemController()
        {
            Self = this;
            String[] args = Environment.GetCommandLineArgs();
            IsOfficialDatasource = true;
            CurrentTrendApi = new ZicoreApi();

            UsingBackupApi = true;
            CurrentApi = gw2SpidyApi;
            HotItemController.IsMultiLanguageSupported = true;
            HotItemController.IsTransactionsSupported = false; // Sadly currently not supported...
        }

        // ------------------------------------------------------- //

        public void StartWorker()
        {
            _tMainWorker = new Thread(new ThreadStart(ThreadRun));
            _tMainWorker.Start();
        }

        string itemsFile = "DB\\items.json";
        string recipesFile = "DB\\recipes.json";

        Dictionary<int, int> itemIdToDataId = new Dictionary<int, int>();
        public Dictionary<int, int> ItemIdToDataId
        {
            get { return itemIdToDataId; }
            set { itemIdToDataId = value; }
        }

        Dictionary<int, int> _dataIdToItemId = new Dictionary<int, int>();
        public Dictionary<int, int> DataIdToItemId
        {
            get { return _dataIdToItemId; }
            set { _dataIdToItemId = value; }
        }

        Dictionary<int, Recipe> _recipeIdToRecipe = new Dictionary<int, Recipe>();
        public Dictionary<int, Recipe> RecipeIdToRecipe
        {
            get { return _recipeIdToRecipe; }
            set { _recipeIdToRecipe = value; }
        }

        Dictionary<int, Recipe> _createdIdToRecipe = new Dictionary<int, Recipe>();
        public Dictionary<int, Recipe> CreatedIdToRecipe
        {
            get { return _createdIdToRecipe; }
            set { _createdIdToRecipe = value; }
        }

        public int DataIdToRecipeId(int dataId)
        {
            int id = -1;
            if (DataIdToItemId.ContainsKey(dataId))
            {
                int itemId = DataIdToItemId[dataId];
                if (CreatedIdToRecipe.ContainsKey(itemId))
                {
                    return itemId;
                }
            }
            return id;
        }

        private void LoadItems()
        {
            String file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, itemsFile);
            if (File.Exists(file))
            {
                using (var streamReader = new StreamReader(file))
                {
                    using (var jsonTextReader = new JsonTextReader(streamReader))
                    {
                        JToken token = JObject.ReadFrom(jsonTextReader);
                        for (int i = 0; i < token.Count(); i++)
                        {
                            int id = token[i]["ID"].ToObject<int>();
                            int externalId = token[i]["ExternalID"].ToObject<int>();
                            int dataId = token[i]["DataID"].ToObject<int>();

                            DataIdToItemId.Add(dataId, id);
                            ItemIdToDataId.Add(id, dataId);
                        }
                    }
                }
            }
        }

        private void LoadRecipes()
        {
            String file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, recipesFile);
            if (File.Exists(file))
            {
                using (var streamReader = new StreamReader(file))
                {
                    using (var jsonTextReader = new JsonTextReader(streamReader))
                    {
                        JToken token = JObject.ReadFrom(jsonTextReader);
                        for (int i = 0; i < token.Count(); i++)
                        {
                            Recipe r = new Recipe()
                            {
                                Id = token[i]["ID"].ToObject<int>(),
                                ExternalId = token[i]["ExternalID"].ToObject<int>(),
                                DataId = token[i]["DataID"].ToObject<int>(),
                                Name = token[i]["Name"].ToObject<String>(),
                                Rating = token[i]["Rating"].ToObject<int>(),
                                Type = token[i]["Type"].ToObject<int>(),
                                Quantity = token[i]["Count"].ToObject<int>(),
                                CreatedItemId = token[i]["CreatedItemId"].ToObject<int>()
                            };

                            for (int j = 0; j < token[i]["Ingredients"].Count(); j++)
                            {
                                Recipe ingredients = new Recipe()
                                {
                                    ItemId = token[i]["Ingredients"][j]["ItemID"].ToObject<int>(),
                                    Quantity = token[i]["Ingredients"][j]["Count"].ToObject<int>(),
                                };

                                int dataId = ItemIdToDataId[ingredients.ItemId];
                                //SlimItem item = DataIdToSlimItem[dataId];
                                ingredients.DataId = dataId;

                                r.RecipeItems.Add(ingredients);
                            }

                            if (!CreatedIdToRecipe.ContainsKey(r.CreatedItemId))
                            {
                                CreatedIdToRecipe.Add(r.CreatedItemId, r);
                            }

                            RecipeIdToRecipe.Add(r.Id, r);

                            RecipeDB.Add(r);
                        }
                    }
                }
            }

            if (RecipesLoaded != null)
            {
                RecipesLoaded(this, new EventArgs());
            }
        }

        public void Add(HotItem item)
        {
            item.BuyPriceChanged += new EventHandler<PriceChangedEventArgs>(item_BuyPriceChanged);
            item.SellPriceChanged += new EventHandler<PriceChangedEventArgs>(item_SellPriceChanged);
            Queue.Add(item);
        }

        void item_ItemBuild(object sender, PriceChangedEventArgs e)
        {
            if (ItemBuild != null)
            {
                ItemBuild(sender, e);
            }
        }

        public void AddNotification(object sender, NotificationEventArgs e)
        {
            if (BuyNotification != null)
            {
                BuyNotification(sender, e);
            }
        }

        void item_SellPriceChanged(object sender, PriceChangedEventArgs e)
        {
            if (SellPriceChanged != null)
            {
                SellPriceChanged(sender, e);
            }
        }

        void item_BuyPriceChanged(object sender, PriceChangedEventArgs e)
        {
            if (BuyPriceChanged != null)
            {
                BuyPriceChanged(sender, e);
            }
        }

        public void Remove(HotItem item)
        {
            item.BuyPriceChanged -= new EventHandler<PriceChangedEventArgs>(item_BuyPriceChanged);
            item.SellPriceChanged -= new EventHandler<PriceChangedEventArgs>(item_SellPriceChanged);
            Queue.Remove(item);
        }

        //private void CheckGuildWars2Process()
        //{
        //    try
        //    {
        //        var proc = Process.GetProcessesByName("Gw2");
        //        GuildWars2Running = proc != null && proc.Length > 0;
        //    }
        //    catch
        //    {

        //    }
        //}

        SerializableDictionary<int, HotItem> _recipeItemPool = new SerializableDictionary<int, HotItem>();
        public SerializableDictionary<int, HotItem> RecipeItemPool
        {
            get { return _recipeItemPool; }
            set { _recipeItemPool = value; }
        }

        public void RegisterRecipeItem(HotItem item)
        {
            int dataId = item.DataId;
            if (!RecipeItemPool.ContainsKey(dataId))
            {
                var newItem = new HotItem(dataId);
                newItem.ItemCreated += newItem_Built;
                newItem.PriceChanged += newItem_Crawled;
                RecipeItemPool.Add(dataId, newItem);
            }

            RecipeItemPool[dataId].Items.Add(item);
        }

        public void UnregisterRecipeItem(HotItem item)
        {
            UnregisterRecipeItem(item.DataId);
        }

        public void UnregisterRecipeItem(int dataId)
        {
            if (RecipeItemPool.ContainsKey(dataId))
            {
                var newItem = RecipeItemPool[dataId];
                newItem.ItemCreated -= newItem_Built;
                newItem.PriceChanged -= newItem_Crawled;
                RecipeItemPool[dataId].Items.Clear();
                RecipeItemPool.Remove(dataId);
            }
        }

        void newItem_Built(object sender, EventArgs e)
        {
            HotItem item = (HotItem)sender;
            if (item != null)
            {
                for (int i = 0; i < item.Items.Count; i++)
                {
                    HotItem itemInner = item.Items[i];
                    itemInner.MarketItem = item;
                }
            }
        }

        void newItem_Crawled(object sender, EventArgs e)
        {
            HotItem item = (HotItem)sender;
            if (item != null)
            {
                for (int i = 0; i < item.Items.Count; i++)
                {
                    HotItem itemInner = item.Items[i];
                    itemInner.Update();
                }
            }
        }

        bool _forceItemBuild = false;

        public bool ForceItemBuild
        {
            get { return _forceItemBuild; }
            set { _forceItemBuild = value; OnPropertyChanged("ForceItemBuild"); }
        }

        // ------------------------------------------------------------------------- //
        // ************************************************************************* //
        // ------------------------------------------------------------------------- //

        void TrendActionSell(PostResult rs)
        {
            TrendsSell = new ObservableCollection<HotItem>(CurrentTrendApi.ParseTrendSell(JToken.Parse(rs.Result)));
            OnPropertyChanged("TrendsSell");
            CrawlItems(TrendsSell);
        }

        void TrendActionBuy(PostResult rs)
        {
            TrendsBuy = new ObservableCollection<HotItem>(CurrentTrendApi.ParseTrendBuy(JToken.Parse(rs.Result)));
            OnPropertyChanged("TrendsBuy");
            CrawlItems(TrendsBuy);
        }

        private void CrawlItems(IEnumerable<HotItem> items)
        {
            foreach (var item in items)
            {
                item.BuildItem(false);
                item.UpdatePrices();
            }
        }

        public static HotItem FromDataId(int dataId)
        {
            var item = TradingPostApiOfficial.ItemTradingPostDB.Values.FirstOrDefault(x => x.Id == dataId);
            if (item != null)
            {
                var hotItem = new HotItem(dataId)
                    {
                        Name = item.Name,
                        ImgUri = item.Icon,
                        RarityWord = item.Rarity,
                        Level = item.Level
                    };
                return hotItem;
            }
            return null;
        }

        public static void UpdateItemDetails(HotItem hotItem)
        {
            var item = TradingPostApiOfficial.ItemTradingPostDB.Values.FirstOrDefault(x => x.Id == hotItem.DataId);
            if (item != null)
            {
                hotItem.Name = item.Name;
                hotItem.ImgUri = item.Icon;
                hotItem.RarityWord = item.Rarity;
                hotItem.Level = item.Level;
            }
        }

        private void ThreadRun()
        {
            GuildWars2Status = Notifier.GuildWars2Status.Loading;

            LoadItems();
            LoadRecipes();

            TradingPostApiOfficial.Initialize();

            GuildWars2Status = Notifier.GuildWars2Status.FinishedLoading;
            //LoadRavenDB();

            //String uriTrendSell = CurrendTrendApi.UriTrendSell();
            //ScrapeHelper.Get(uriTrendSell, "", null, "", TrendActionSell, "");

            //String uriTrendBuy = CurrendTrendApi.UriTrendBuy();
            //ScrapeHelper.Get(uriTrendBuy, "", null, "", TrendActionBuy, "");

            try
            {
                while (IsRunning)
                {
                    //while (IsSearchInProgress)
                    //{
                    //    Thread.Sleep(10);
                    //}
                    for (int i = 0; i < Queue.Count; i++)
                    {
                        HotItem item = Queue[i];

                        Task.Factory.StartNew(() =>
                        {
                            item.BuildItem(ForceItemBuild); // It checks that the item gets build only once
                            item.UpdatePrices();
                            item.CompareToRules();
                        });

                        //ItemStore.Store(Queue);
                        Thread.Sleep(100);
                    }

                    try
                    {
                        var items = new List<HotItem>(RecipeItemPool.Values);
                        foreach (var item in RecipeItemPool)
                        {
                            UpdateItemDetails(item.Value);
                            item.Value.BuildItem(false);
                        }
                        UpdatePricesMultiple(items);
                    }
                    catch
                    {
                        // Changed the collection
                    }

                    try
                    {
                        Gem.Update();
                    }
                    catch
                    {

                    }

                    ForceItemBuild = false;
                    Thread.Sleep(5000);
                }

            }
            catch (ThreadInterruptedException)
            {
                IsRunning = false;
            }
        }

        private void UpdateItem(HotItem item1, HotItem item2)
        {
            item1.Name = item2.Name;
            item1.SellPrice = item2.SellPrice;
            item1.BuyPrice = item2.BuyPrice;
            item1.Rarity = item2.Rarity;
            item1.RarityWord = item2.RarityWord;
            //item1.BuyContext = item2.BuyContext;
            //item1.SellContext = item2.SellContext;
            item1.SaleVolume = item2.SaleVolume;
            item1.BuyVolume = item2.BuyVolume;
            item1.Image = item2.Image;
            item1.ImgUri = item2.ImgUri;
        }

        //public void EnableSorting(String mode)
        //{
        //    if (CurrentApi.IsAdvancedSearchSupported)
        //    {
        //        SortingMode sortingMode = SortingMode.none;
        //        switch (mode)
        //        {
        //            case "Image": sortingMode = SortingMode.reset;
        //                break;
        //            case "Supply": sortingMode = SortingMode.count;
        //                break;
        //            case "Sell": sortingMode = SortingMode.price;
        //                break;
        //            case "Name": sortingMode = SortingMode.name;
        //                break;
        //            case "Level": sortingMode = SortingMode.level;
        //                break;
        //            case "Count": sortingMode = SortingMode.count;
        //                break;
        //            case "Rarity": sortingMode = SortingMode.rarity;
        //                break;
        //            default:
        //                sortingMode = SortingMode.none;
        //                break;
        //        }
        //        if (sortingMode != SortingMode.none)
        //        {
        //            if (sortingMode != SortingMode.reset)
        //            {
        //                if (CurrentSearchFilters.SortingMode == sortingMode)
        //                {
        //                    switch (CurrentSearchFilters.SortDirection)
        //                    {
        //                        case SortDirection.Disabled:
        //                            CurrentSearchFilters.SortDirection = SortDirection.Ascending;
        //                            break;
        //                        case SortDirection.Ascending:
        //                            CurrentSearchFilters.SortDirection = SortDirection.Descending;
        //                            break;
        //                        case SortDirection.Descending:
        //                            CurrentSearchFilters.SortDirection = SortDirection.Disabled;
        //                            break;
        //                        default:
        //                            CurrentSearchFilters.SortDirection = SortDirection.Disabled;
        //                            break;
        //                    }
        //                }
        //                else
        //                {
        //                    CurrentSearchFilters.SortDirection = SortDirection.Ascending;
        //                    CurrentSearchFilters.SortingMode = sortingMode;
        //                }
        //            }
        //            else
        //            {
        //                CurrentSearchFilters.SortDirection = SortDirection.Disabled;
        //            }

        //            if (CurrentSearchFilters.SortDirection == SortDirection.Disabled)
        //            {
        //                CurrentSearchFilters.SortingMode = SortingMode.none;
        //            }

        //            Search(0, CurrentSearchFilters);
        //        }

        //    }
        //}

        public static void UpdatePricesMultiple(List<HotItem> items)
        {
            const int endPointLimit = 200;
            const int maxEntries = 1000;
            //TradingPostApiOfficial.PriceService.Discover();

            var ids = new List<int>();
            var idGroups = new List<List<int>>();

            for (int i = 1; i <= items.Count && i <= maxEntries; i++)
            {
                var item = items[i - 1];
                if (item != null)
                {
                    ids.Add(items[i - 1].DataId);
                    if (i % endPointLimit == 0 || i == items.Count)
                    {
                        idGroups.Add(ids);
                        ids = new List<int>();
                    }
                }
            }

            foreach (var idGroup in idGroups)
            {
                var result = TradingPostApiOfficial.PriceService.FindAll(idGroup);

                foreach (var aggregateListing in result)
                {
                    var item = items.FirstOrDefault(x => x.DataId == aggregateListing.Value.ItemId);
                    if (item != null)
                    {
                        UpdatePrice(aggregateListing.Value, item);
                    }
                }
            }
        }

        public static void UpdatePrice(AggregateListing listing, HotItem hotItem)
        {
            hotItem.BuyVolume = listing.BuyOffers.Quantity;
            hotItem.BuyPrice = listing.BuyOffers.UnitPrice;

            hotItem.SaleVolume = listing.SellOffers.Quantity;
            hotItem.SellPrice = listing.SellOffers.UnitPrice;

            hotItem.UpdatePriceChanged();
        }


        //void searchScraper_Finished(object sender, DataProvider.Event.ScrapeFinishedEventArgs e)
        //{
        //    ScrapeHelper searchScraper = sender as ScrapeHelper;
        //    if (searchScraper != null)
        //    {
        //        searchScraper.Finished -= new EventHandler<DataProvider.Event.ScrapeFinishedEventArgs>(searchScraper_Finished);
        //    }
        //    IsSearchInProgress = false;

        //    this.LastSearchResult = new SearchResult(e.Value, (String)e.Arg, e.Uri, JsonResultType.Search, e.TransactionType);
        //    this.LastSearchResult.Filters = CurrentSearchFilters;
        //    if (SearchFinished != null)
        //    {
        //        SearchFinished(this, new EventArgs<SearchResult>(this.LastSearchResult));
        //    }
        //}

        public void ChangeLanguage(String language)
        {
            ScrapeHelper.CurrentLanguage = language;
            ForceItemBuild = true;
        }

        public void Export(string filePath)
        {
            try
            {
                String[] columns = new string[]{
                "data_id", "name", "rarity", "restriction_level", "img", "type_id", "sub_type_id", "price_last_changed", "max_offer_unit_price", "min_sale_unit_price", "offer_availability",
                "sale_availability", "gw2db_external_id", "sale_price_change_last_hour", "offer_price_change_last_hour" 
            };

                Object[][] rows = new object[Queue.Count][];

                for (int i = 0; i < rows.Length; i++)
                {
                    rows[i] = new object[columns.Length];

                    HotItem item = Queue[i];

                    rows[i][0] = item.DataId;
                    rows[i][1] = item.Name;
                    rows[i][2] = "";
                    rows[i][3] = "";
                    rows[i][4] = "";
                    rows[i][5] = "";
                    rows[i][6] = "";
                    rows[i][7] = "";
                    rows[i][8] = item.BuyPrice;
                    rows[i][9] = item.SellPrice;
                    rows[i][10] = item.BuyVolume;
                    rows[i][11] = item.SaleVolume;
                    rows[i][12] = "";
                    rows[i][13] = "";
                    rows[i][14] = "";
                }

                CsvHelper.WriteCsv(filePath, columns, rows, ",");
            }
            catch
            {

            }
        }

        public void Import(string filePath)
        {
            try
            {
                string[] columns;
                string[][] rows;
                CsvHelper.ReadCsv(filePath, out columns, out rows, ",");
                int dataIdColumn = -1;
                for (int i = 0; i < columns.Length; i++)
                {
                    if (columns[i] == "data_id")
                    {
                        dataIdColumn = i;
                    }
                }
                if (dataIdColumn >= 0)
                {
                    for (int i = 0; i < rows.Length; i++)
                    {
                        int dataId = int.Parse(rows[i][dataIdColumn]);
                        Add(new HotItem(dataId));
                    }
                }
            }
            catch
            {

            }
        }
    }
}
