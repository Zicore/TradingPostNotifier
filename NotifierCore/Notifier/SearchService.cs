using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibraryBase.Wpf.Event;
using NotifierCore.DB;
using NotifierCore.DataProvider;

namespace NotifierCore.Notifier
{
    public class SearchService
    {


        public event EventHandler<EventArgs<SearchResult>> SearchFinished;

        private bool _isSearchInProgress = false;
        private SearchFilters _filter = new SearchFilters();
        private SearchResult _searchResult = new SearchResult();

        public bool IsSearchInProgress
        {
            get { return _isSearchInProgress; }
            set { _isSearchInProgress = value; }
        }
        public SearchResult SearchResult
        {
            get { return _searchResult; }
            set { _searchResult = value; }
        }
        public SearchFilters Filter
        {
            get { return _filter; }
            set { _filter = value; }
        }

        public static SearchResult SearchItems(SearchFilters filter)
        {
            return Search(filter, TradingPostApiOfficial.ItemTradingPostDB.Values);
        }

        public static SearchResult SearchRecipes(SearchFilters filter)
        {
            return Search(filter, TradingPostApiOfficial.ItemRecipesDB.Values);
        }

        public static SearchResult Search(SearchFilters filter, IEnumerable<Item> dataSource)
        {
            IEnumerable<Item> result;

            result = dataSource.Where(x => x.Level >= filter.LevelMin && x.Level <= filter.LevelMax);

            if (!String.IsNullOrEmpty(filter.QueryString))
            {
                result =
                    TradingPostApiOfficial.ItemTradingPostDB.Values.Where(
                        x => x.Name.ToUpperInvariant().Contains(filter.QueryString.ToUpperInvariant()));
            }

            if (!String.IsNullOrEmpty(filter.Rarity))
            {
                result = result.Where(x => x.Rarity == filter.Rarity);
            }

            if (!String.IsNullOrEmpty(filter.TypeId))
            {
                result = result.Where(x => x.Type == filter.TypeId);
            }

            if (!String.IsNullOrEmpty(filter.SubTypeId))
            {
                result = result.Where(x => x.Details.Type == filter.SubTypeId);
            }

            var list = result.ToList();
            int countAll = list.Count;

            list = list.Skip(filter.Offset).Take(filter.ItemsPerPage).ToList();


            var searchResult = SearchResult.ParseSearchResult(list);

            filter.Total = countAll;

            searchResult.Total = filter.Total;
            searchResult.Offset = filter.Offset;

            return searchResult;
        }

        public void Search(int page, SearchFilters filter)
        {
            if (SearchFinished != null)
            {
                IsSearchInProgress = true;
                Filter = filter;
                Filter.Offset = page * filter.ItemsPerPage;
                if (filter.SearchType == SearchType.Items)
                {
                    SearchResult = SearchItems(Filter);
                }
                else
                {
                    SearchResult = SearchRecipes(Filter);
                }

                IsSearchInProgress = false;

                if (SearchFinished != null)
                {
                    SearchFinished(this, new EventArgs<SearchResult>(this.SearchResult));
                }
            }
        }

        public void SearchPage(int page)
        {
            if (SearchResult != null)
            {
                if ((page - 1) * Filter.ItemsPerPage < SearchResult.Total) // -1 because to correct the offset
                {
                    Search(page - 1, Filter);
                }
            }
        }

        public void SearchNextPage()
        {
            if (SearchResult != null)
            {
                if (SearchResult.Offset + Filter.ItemsPerPage < SearchResult.Total)
                {
                    Search(SearchResult.Page + 1, Filter);
                }
            }
        }

        public void SearchPreviousPage()
        {
            if (SearchResult != null)
            {
                if (SearchResult.Offset > 1)
                {
                    Search(SearchResult.Page - 1, Filter);
                }
            }
        }
    }
}
