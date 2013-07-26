using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Diagnostics;
using LibMemorySearch;
using System.Text.RegularExpressions;
using System.Threading;
using System.Globalization;

namespace SessionKeyReader
{
    public class KeyHelper
    {
        bool running = false;

        public bool Running
        {
            get { return running; }
        }
        private String _uri;

        public String Key
        {
            get { return _key; }
            set { _key = value; }
        }

        const string searchString = "-live.ncplatform.net/authenticate?session_key=XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX&source=/";
        static readonly Wildcards searchWildcards = new Wildcards(new string[] { "X", "[A-Z0-9]" });
        static readonly Regex sessionKeyValidator = new Regex("^([0-9A-F]){8}-([0-9A-F]){4}-([0-9A-F]){4}-([0-9A-F]){4}-([0-9A-F]){12}$", RegexOptions.Compiled);
        private String _key;

        public event EventHandler<UriFoundEventArgs> UriFound;

        public static void CheckArgsAndPost(string sessionKey, string[] args)
        {
            if (args != null && args.Length > 0)
            {
                string url = args[0];
                string secret = args.Length == 2 ? args[1] : "THESECRETKEYGOESHERE";
                UploadSessionKey(sessionKey, url, secret);
            }
        }

        public static void UploadSessionKey(string sessionKey, string url, string secret)
        {
            try
            {
                string data = String.Format("session_key={0}&admin_secret={1}&game_session=1", sessionKey, secret);
                ServicePointManager.Expect100Continue = false;
                using (WebClient wc = new WebClient())
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    Debug.WriteLine(wc.UploadString(url, data));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public static bool IsValidSessionKey(string sessionKey)
        {
            return sessionKeyValidator.IsMatch(sessionKey);
        }

        public static string ParseSessionKey(string url)
        {
            return url.Between("-live.ncplatform.net/authenticate?session_key=", "&source=/");
        }

        public void FindKey()
        {
            if (!running)
            {
                running = true;
                ThreadPool.QueueUserWorkItem(WaitForUrlThread);
            }
        }
        public String Uri
        {
            get { return _uri; }
            set { _uri = value; }
        }

        const string KEY = "tradingpost-live.ncplatform.net";

        public string ReadKey()
        {
            using (
                StreamReader stream =
                    new StreamReader(
                        File.Open(
                            Directory.GetParent(Process.GetProcessesByName("awesomium_process")[0].MainModule.FileName)
                                     .FullName + @"\data\Cookies", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                )
            {
                string cookies = stream.ReadToEnd();
                stream.Close();

                string sessionId = cookies.Substring(cookies.IndexOf(KEY, System.StringComparison.Ordinal) + KEY.Length + 1, 36);
                return sessionId;
            }
        }

        private void WaitForUrlThread(object s)
        {
            //Process p = WaitForProcess();

            //MemorySearcher ms = new MemorySearcher(p);
            try
            {
                //Debug.WriteLine("Caching Gw2 memory regions...");
                //ms.BuildCache();
                //Debug.WriteLine("Searching for url...");
                //string url = ms.FindString(searchString, searchWildcards);
                //while (url == null)
                //{
                //    Debug.WriteLine("Url not found, waiting 1 second and trying again.");
                //    Thread.Sleep(1000);
                //    Debug.WriteLine("Caching new Gw2 memory regions...");
                //    ms.BuildCache();
                //    Debug.WriteLine("Searching for url...");
                //    url = ms.FindString(searchString, searchWildcards);
                //}
                //Debug.WriteLine(String.Format("Url found {0}", url));
                //this.Uri = url;
                //Key = ParseSessionKey(url);
                //event fired
                try
                {
                    Key = ReadKey();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            finally
            {
                //ms.ClearCache();
                running = false;

                if (UriFound != null)
                {
                    UriFound(this, new UriFoundEventArgs(Uri, Key));
                }
            }
        }

        public static Process WaitForProcess()
        {
            Debug.WriteLine("Searching for Gw2 process...");
            Process p = Process.GetProcessesByName("Gw2").FirstOrDefault(); ;
            while (p == null)
            {
                Debug.WriteLine("Process not found, waiting 1 second and trying again.");
                Thread.Sleep(1000);
                Debug.WriteLine("Searching for Gw2 process...");
                p = Process.GetProcessesByName("Gw2").FirstOrDefault();
            }
            Debug.WriteLine("Process found, pid is {0}", p.Id);
            return p;
        }
    }
}
