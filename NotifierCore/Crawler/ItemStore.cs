//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Raven.Client.Embedded;
//using Scraper.Notifier;

//namespace Scraper.Crawler
//{
//    public class ItemStore
//    {
//        EmbeddableDocumentStore _documentStore;
//        public EmbeddableDocumentStore DocumentStore
//        {
//            get { return _documentStore; }
//            set { _documentStore = value; }
//        }
//        public ItemStore(EmbeddableDocumentStore documentStore)
//        {
//            this.DocumentStore = documentStore;
//        }

//        public void Store(IEnumerable<HotItem> items)
//        {
//            using (var session = DocumentStore.OpenSession())
//            {
//                foreach (var item in items)
//                {
//                    session.Store(new ItemProxy
//                    {
//                        DataId = item.DataId,
//                        SellPrice = item.SellPrice,
//                        BuyPrice = item.BuyPrice,
//                        SellCount = item.SaleVolume,
//                        BuyCount = item.BuyVolume,
//                        DateTime = DateTime.Now
//                    });
//                    session.SaveChanges();
//                }
//            }
//        }

//        public List<ItemProxy> Load(int dataId)
//        {
//            List<ItemProxy> items = new List<ItemProxy>();

//            using (var session = DocumentStore.OpenSession())
//            {
//                items = session.Query<ItemProxy>().Where(x => x.DataId == dataId).ToList();
//            }

//            return items;
//        }
//    }


//}
