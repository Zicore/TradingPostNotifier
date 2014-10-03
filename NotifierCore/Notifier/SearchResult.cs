using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using NotifierCore.DB;
using NotifierCore.DataProvider;

namespace NotifierCore.Notifier
{
    public enum JsonResultType
    {
        Search,
        Transactions
    }

    public enum TransactionType
    {
        Selling,
        Sold,
        Buying,
        Bought
    }

    public class SearchResult
    {
        private String _jsonString;
        private JObject _jsonObject;
        private int _total;
        private string _text;
        private int _offset;
        private int _count;

        private List<HotItem> _items = new List<HotItem>();
        private String _searchUri;
        private JsonResultType _jsonResultType = JsonResultType.Search;
        private String _dateTimeStamp;
        private TransactionType _transactionType;

        public JsonResultType JsonResultType
        {
            get { return _jsonResultType; }
            set { _jsonResultType = value; }
        }

        public TransactionType TransactionType
        {
            get { return _transactionType; }
            set { _transactionType = value; }
        }

        public SearchResult()
        {

        }

        public static SearchResult ParseSearchResult(IEnumerable<Item> items)
        {
            var result = new SearchResult();
            result.JsonResultType = JsonResultType.Search;

            result.Items.Clear();
            var hotItems = new List<HotItem>();
            foreach (var itemDetail in items)
            {
                var item = HotItemController.FromDataId(itemDetail.Id);
                hotItems.Add(item);
            }
            HotItemController.UpdatePricesMultiple(hotItems);
            result.Items = hotItems;
            return result;
        }

        public SearchResult(String jsonString, String searchString, String searchUri, JsonResultType jsonResultType, TransactionType transactionType)
        {
            this.TransactionType = transactionType;
            this.JsonResultType = jsonResultType;
            this.SearchUri = searchUri;
            this.JsonString = jsonString;
            this.Text = searchString;
        }

        public String SearchUri
        {
            get { return _searchUri; }
            set { _searchUri = value; }
        }

        public String JsonString
        {
            get { return _jsonString; }
            set
            {
                _jsonString = value;
                if (!String.IsNullOrEmpty(JsonString))
                {
                    try
                    {
                        this.JsonObject = JObject.Parse(this.JsonString);
                    }
                    catch
                    {

                    }
                }
            }
        }

        public JObject JsonObject
        {
            get { return _jsonObject; }
            set
            {
                _jsonObject = value;
                switch (JsonResultType)
                {
                    case JsonResultType.Search:
                        //ParseSearchResult();
                        break;
                    case JsonResultType.Transactions:
                        ParseTransactionResult();
                        break;
                    default:
                        break;
                }

            }
        }

        SearchFilters _filters = new SearchFilters();

        public SearchFilters Filters
        {
            get { return _filters; }
            set { _filters = value; }
        }

        public int Total
        {
            get { return _total; }
            set { _total = value; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public int Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }

        public String DateTimeStamp
        {
            get { return _dateTimeStamp; }
            set { _dateTimeStamp = value; }
        }

        public int Page
        {
            get { return (int)(Offset / TradingPostApiOfficial.ItemsPerPage); }
        }

        public List<HotItem> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        private void ParseTransactionResult()
        {
            Items.Clear();
            HotItemController.CurrentApi.ParseTransaction(this, this.JsonObject, TransactionType);
            this.Items = GroupItems(Items);
        }

        public static List<HotItem> GroupItems(List<HotItem> items)
        {
            List<HotItem> uniqueItems = new List<HotItem>();
            try
            {

                foreach (HotItem item in items)
                {
                    try
                    {
                        if (!uniqueItems.Exists(x => x.DataId == item.DataId))
                        {
                            HotItem groupItem = HotItem.CreateGroupItem(item, items);

                            uniqueItems.Add(groupItem);
                        }
                    }
                    catch
                    {

                    }
                }

            }
            catch
            {

            }
            return uniqueItems;
        }
    }
}
