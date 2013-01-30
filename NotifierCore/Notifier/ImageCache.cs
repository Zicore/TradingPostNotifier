using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZicoresUtils.Zicore.Configuration;
using System.Threading;
using System.Net;
using System.IO;
using Zicore.Collections.Generic;
using Scraper.Notifier.Event;

namespace Scraper.Notifier
{
    public class ImageCache
    {
        protected String _cachePath = "";
        LockFreeQueue<Tuple<String, String>> _queue = new LockFreeQueue<Tuple<string, string>>();
        private String _cacheDirectory;
        private bool _isRunning = true;
        public event EventHandler<CacheStoredEventArgs> CacheStored;
        Thread thread;
        public bool IsRunning
        {
            get { return _isRunning; }
            set { _isRunning = value; }
        }

        public String CacheDirectory
        {
            get { return _cacheDirectory; }
            protected set { _cacheDirectory = value; }
        }

        public ImageCache(String cachePath)
        {
            this.CacheDirectory = cachePath;
            string path = ConfigHelper.GetConfigFolder();
            this._cachePath = System.IO.Path.Combine(path, CacheDirectory);
            if (!Directory.Exists(this._cachePath))
            {
                Directory.CreateDirectory(this._cachePath);
            }
        }

        public void StoreAndRequest(string uri, string name)
        {
            Tuple<String, String> item = new Tuple<string, string>(uri, name);
            if (!IsStored(name))
            {
                Start(item); // Enqueue with tuple
            }
            else
            {
                if (CacheStored != null)
                {
                    CacheStored(this, new CacheStoredEventArgs(item.Item2)); // directly give stored file
                }
            }
        }

        public bool IsStored(string name)
        {
            return File.Exists(Path.Combine(_cachePath, name));
        }

        public bool IsPathValid(String path, String name)
        {
            return GetPath(name) == path;
        }

        public MemoryStream Read(string name)
        {
            if (IsStored(name))
            {
                MemoryStream ms = new MemoryStream(File.ReadAllBytes(Path.Combine(_cachePath, name)));
                return ms;
            }
            return null;
        }

        public String GetPath(string name)
        {
            if (IsStored(name))
            {
                return Path.Combine(_cachePath, name);
            }
            return "";
        }

        private void Start(Tuple<String, String> item)
        {
            if (thread == null)
            {
                thread = new Thread(new ParameterizedThreadStart(Worker));
                thread.Start();
            }
            this._queue.Enqueue(item);
        }

        private void Worker(object obj)
        {
            Tuple<String, String> item;
            while (IsRunning)
            {
                while (_queue.Dequeue(out item))
                {
                    if (item != null && !String.IsNullOrEmpty(item.Item1) && !String.IsNullOrEmpty(item.Item2))
                    {
                        try
                        {
                            using (var w = new WebClient())
                            {
                                w.Proxy = null;
                                w.DownloadFile(item.Item1, Path.Combine(_cachePath, item.Item2));
                                Thread.Sleep(5);
                            }
                        }
                        catch
                        {
                            _queue.Enqueue(item);
                        }
                    }
                    if (CacheStored != null)
                    {
                        CacheStored(this, new CacheStoredEventArgs(item.Item2));
                    }
                    Thread.Sleep(1);
                }
                Thread.Sleep(50);
            }
        }
    }
}
