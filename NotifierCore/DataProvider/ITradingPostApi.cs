using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NotifierCore.Notifier;
using Newtonsoft.Json.Linq;

namespace NotifierCore.DataProvider
{
    public interface ITradingPostApi
    {
        SearchResult ParseSearch(SearchResult sr, JObject json);
        SearchResult ParseTransaction(SearchResult sr, JObject json, TransactionType type);
        HotItem ParseItemListing(HotItem item, JObject json);
        HotItem ParseItem(HotItem item, JObject json);

        String UriSearch(String query, int offset, int count, String type = null, String subType = null, String rarity = null, String levelmin = null, String levelmax = null, String sortingMode = null, bool descending = false);
        String UriBuildItem(int dataId);
        String UriListingItem(int dataId);
        String UriTransaction(TransactionType type, int offset, int count);

        bool IsUnsafe { get; }
        bool IsAdvancedSearchSupported { get; }
        int WorkerTimeOut { get; }
        int WorkerTransactionTimeOut { get; }

        int ItemsPerPage { get; }
        string UserAgent { get; }
        string ExchangeReferer { get; }
        bool IsTransactionApiSupported { get; }
        string ExchangeHost { get; }
        //String UriTransaction(
        string UriBuyGems(int count);
        string UriBuyGold(int count);

        double ParseBuyGemValue(JObject json);
        double ParseBuyGoldValue(JObject json);
    }
}
