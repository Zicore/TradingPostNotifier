using System;
using System.Collections.Generic;
using System.Text;

namespace NotifierCore.Crawler
{
    public class UriHelper
    {
        //listings.json?id=20323
        private String _uriBase = "https://tradingpost-live.ncplatform.net/ws/";
        private List<KeyValueString> _paramters = new List<KeyValueString>();
        private string _apiUri;

        public String UriBase
        {
            get { return _uriBase; }
            private set { _uriBase = value; }
        }

        public string ApiUri
        {
            get { return _apiUri; }
            private set { _apiUri = value; }
        }

        public readonly static String JsonListing = "listings.json";
        public readonly static String JsonSearch = "search.json";
        public readonly static String JsonMe = "me.json";

        public List<KeyValueString> Paramters
        {
            get { return _paramters; }
            set { _paramters = value; }
        }

        public UriHelper Add(String arg, String val)
        {
            Paramters.Add(new KeyValueString(arg, val));
            return this;
        }

        public UriHelper AddId(String val)
        {
            return Add("id", val);
        }

        public UriHelper UseCustomApi(String apiUri)
        {
            Paramters.Clear();
            this.ApiUri = apiUri;
            return this;
        }

        public UriHelper UseListingApi()
        {
            Paramters.Clear();
            this.ApiUri = JsonListing;
            return this;
        }

        public UriHelper UseSearchApi()
        {
            Paramters.Clear();
            this.ApiUri = JsonSearch;
            return this;
        }

        public UriHelper UseMeSellListingApi()
        {
            //?time=now&type=sell&offset=0&count=20
            return UseMeApi().Add("time", "now").Add("type", "sell");
        }

        public UriHelper UseMeSoldListingApi()
        {
            //?time=past&type=sell&offset=0&count=20
            return UseMeApi().Add("time", "past").Add("type", "sell");
        }

        public UriHelper UseMeBuyListingApi()
        {
            //?time=now&type=buy&offset=0&count=20
            return UseMeApi().Add("time", "now").Add("type", "buy");
        }

        public UriHelper UseMeBoughtListingApi()
        {
            //?time=now&type=buy&offset=0&count=20
            return UseMeApi().Add("time", "past").Add("type", "buy");
        }

        public UriHelper UseMeApi()
        {
            Paramters.Clear();
            this.ApiUri = JsonMe;
            return this;
        }

        public UriHelper()
        {

        }

        public UriHelper(String uriBase)
            : this()
        {
            this.UriBase = uriBase;
        }

        public String Generate()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(UriBase);
            sb.Append(ApiUri);
            foreach (KeyValueString p in Paramters)
            {
                if (p == Paramters[0])
                {
                    sb.Append("?");
                }
                else
                {
                    sb.Append("&");
                }
                sb.AppendFormat("{0}={1}", p.Key, p.Value);
            }

            return sb.ToString();
        }
    }
}
