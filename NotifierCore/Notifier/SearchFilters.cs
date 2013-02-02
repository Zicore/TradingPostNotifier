using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scraper.Notifier
{
    public enum SortingMode
    {
        none,
        name,
        level,
        rarity,
        price,
        count,
        reset
    }

    public enum SortDirection
    {
        Disabled,
        Ascending,
        Descending
    }

    public class SearchFilters
    {
        private int _total;
        private string _queryString;
        private int _offset;
        private int _count;

        private String typeId = "";
        private String subTypeId = "";
        private String levelMin = "";
        private String levelMax = "";
        private String rarity = "";

        public bool DescendingSorting
        {
            get { return sortDirection == SortDirection.Descending; }
            set
            {
                sortDirection = value ? SortDirection.Descending : SortDirection.Ascending;
            }
        }

        SortDirection sortDirection = SortDirection.Disabled;
        public SortDirection SortDirection
        {
            get { return sortDirection; }
            set { sortDirection = value; }
        }

        SortingMode sortingMode = SortingMode.none;
        public SortingMode SortingMode
        {
            get { return sortingMode; }
            set { sortingMode = value; }
        }

        public String TypeId
        {
            get { return typeId; }
            set
            {
                typeId = value;
            }
        }

        public String SubTypeId
        {
            get { return subTypeId; }
            set { subTypeId = value; }
        }

        public String LevelMin
        {
            get { return levelMin; }
            set { levelMin = value; }
        }

        public String LevelMax
        {
            get { return levelMax; }
            set { levelMax = value; }
        }

        public String Rarity
        {
            get { return rarity; }
            set { rarity = value; }
        }

        public int Total
        {
            get { return _total; }
            set { _total = value; }
        }

        public string QueryString
        {
            get { return _queryString; }
            set { _queryString = value; }
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
    }
}
