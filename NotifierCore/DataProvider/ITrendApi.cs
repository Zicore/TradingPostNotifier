using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using NotifierCore.Notifier;

namespace NotifierCore.DataProvider
{
    public interface ITrendApi
    {
        String UriTrendSell();
        String UriTrendBuy();

        IList<HotItem> ParseTrendSell(JToken json);
        IList<HotItem> ParseTrendBuy(JToken json);
    }
}
