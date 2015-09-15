using System;
using System.Net;

namespace TradingPostDatabase
{
    public class WebClientEx : WebClient
    {
        public WebClientEx()
        {
            Proxy = null;
        }

        private CookieContainer _cookieContainer = new CookieContainer();

        public CookieContainer CookieContainer
        {
            get { return _cookieContainer; }
            set { _cookieContainer = value; }
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).CookieContainer = _cookieContainer;
            }
            return request;
        }
    }
}
