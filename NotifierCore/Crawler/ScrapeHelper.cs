using System;
using System.Net;
using NotifierCore.DataProvider;
using NotifierCore.DataProvider.Event;
using NotifierCore.Notifier;
using System.Diagnostics;

namespace NotifierCore.Crawler
{
    public class ScrapeHelper : IDisposable
    {
        private static string _currentLanguage;
        public readonly static string TradingPostVerifyUri = "https://tradingpost-live.ncplatform.net/item/";
        public readonly static string TradingPostHost = "tradingpost-live.ncplatform.net";
        public readonly static string TradingPostAuthUri = "https://tradingpost-live.ncplatform.net/authenticate?session_key=";
        public readonly static string TradingPostBuyUri = "https://tradingpost-live.ncplatform.net/ws/item/{0}/buy";
        public readonly static string TradingPostItemUri = "https://tradingpost-live.ncplatform.net/ws/item/{0}";

        WebClientEx _client;

        public static string CurrentLanguage
        {
            get { return _currentLanguage; }
            set { _currentLanguage = value; }
        }

        private String _cookieValue;
        private TransactionType _transactionType = TransactionType.Buying;


        public TransactionType TransactionType
        {
            get { return _transactionType; }
            set { _transactionType = value; }
        }

        public ScrapeHelper(String sessionKey)
        {
            this.CookieValue = sessionKey;
            EatCookie(sessionKey);
        }

        public void EatCookie(string sessionKey)
        {
            _client = new WebClientEx();
            var c = new Cookie("s", sessionKey, "/", TradingPostHost);
            _client.CookieContainer.Add(c);
        }

        public String CookieValue
        {
            get { return _cookieValue; }
            set { _cookieValue = value; }
        }

        public event EventHandler<ScrapeFinishedEventArgs> Finished;

        /// <summary>
        /// Get request to a website
        /// </summary>
        /// <param name="uri">Uri to request</param>
        /// <param name="host">Host for header</param>
        /// <param name="cookie">Cookie for authentication</param>
        /// <param name="referer">Referer for header</param>
        /// <param name="action">Callback</param>
        /// <param name="userAgent">Useragent for header</param>
        /// <remarks>Requires refactoring, make it more general and replace CrawlString with it</remarks>
        public static void Get(String uri, String host, Cookie cookie, String referer, Action<PostResult> action, String userAgent)
        {
            using (var client = new WebClientEx())
            {
                if (cookie != null)
                {
                    client.CookieContainer.Add(cookie);
                }
                client.Headers["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                client.Headers["Accept-Language"] = "de-de,de;q=0.8,en-us;q=0.5,en;q=0.3";
                client.Headers["Cache-Control"] = "no-cache";
                client.Headers["User-Agent"] = userAgent;
                if (!String.IsNullOrEmpty(referer))
                {
                    client.Headers["Referer"] = referer;
                }
                if (!String.IsNullOrEmpty(host))
                {
                    client.Headers["Host"] = host;
                }
                client.Headers["X-Requested-With"] = "XMLHttpRequest";
                var response = client.DownloadString(uri);
                PostResult ps = new PostResult() { Result = response };
                action(ps);
            }
        }

        /// <summary>
        /// Basically the same as <seealso cref="Get"/> but more specially and it requires the instance of ScrapeHelper.
        /// </summary>
        /// <param name="uri">Uri to request</param>
        /// <param name="id">Unique id per <seealso cref="ScrapeHelper"/> to keep track of it when the event fires</param>
        /// <param name="arg">Object argument for the for the event</param>
        /// <param name="api">Dataprovider Api which is used</param>
        /// <remarks type="caution">Requires Refactoring</remarks>
        public void CrawlString(String uri, int id, object arg, ITradingPostApi api)
        {
            if (String.IsNullOrEmpty(uri))
            {
                throw new ArgumentException("Uri must be not null or empty");
            }

            if (Finished != null) // it makes no sense to start without attached handlers
            {
                try
                {
                    var sw = Stopwatch.StartNew();
                    String response = "";
                    int responseCode = 0;
                    try
                    {
                        PrepareScraper(_client); // setting headers and what not...
                        response = _client.DownloadString(uri);
                    }
                    catch (WebException ex)
                    {
                        HttpStatusCode code = (((HttpWebResponse)ex.Response).StatusCode);
                        responseCode = (int)code;
                    }
                    sw.Stop();
                    ScrapeFinishedEventArgs e = new ScrapeFinishedEventArgs(id, uri, response, arg) { ResponseCode = responseCode, TransactionType = this.TransactionType };
                    e.Duration = sw.Elapsed;
                    if (Finished != null) // check again to be sure there was no dettaching in the processing time
                    {
                        Finished(this, e);
                    }
                }
                catch
                {

                }
            }
        }

        private void PrepareScraper(WebClient client)
        {
            client.Headers["Content-Type"] = "text/html; charset=UTF-8";
            client.Headers["Accept-Language"] = CurrentLanguage;
            if (!String.IsNullOrEmpty(HotItemController.CurrentApi.UserAgent))
            {
                client.Headers["User-Agent"] = HotItemController.CurrentApi.UserAgent;
            }
        }

        //
        public bool VerifyKey()
        {
            String response = "";
            int responseCode = 0;
            try
            {
                response = _client.DownloadString(TradingPostVerifyUri);
                return true;
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpStatusCode code = (((HttpWebResponse)ex.Response).StatusCode);
                    responseCode = (int)code;
                }
            }
            catch (Exception)
            {

            }
            return false;
        }

        public void Dispose()
        {

        }
    }
}
