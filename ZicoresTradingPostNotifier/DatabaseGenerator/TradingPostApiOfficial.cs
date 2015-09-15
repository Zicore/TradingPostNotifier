using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using TradingPostDatabase;

namespace DatabaseGenerator
{
    public class TradingPostApiOfficial
    {
        public static Dictionary<int, Item> ItemSearch = new Dictionary<int, Item>();

        static WebClientEx client = new WebClientEx();

        public static String DownloadString(WebClientEx client, String uri)
        {
            client.Headers["Content-Type"] = "application/json; charset=utf-8";
            return client.DownloadString(uri);
        }

        public static void Start()
        {
            Console.WriteLine("This can take up to 30 minutes!");

            String jsonString = DownloadString(client, "https://api.guildwars2.com/v2/items");
            var itemsIds = JsonConvert.DeserializeObject<List<int>>(jsonString);

            var idRequests = new List<string>();
            var sb = new StringBuilder();
            int count = 0;
            int maxCount = 200;
            for (int i = 1; i < itemsIds.Count + 1; i++)
            {
                sb.Append(itemsIds[i - 1]);
                if (i % maxCount == 0 || i == itemsIds.Count)
                {
                    idRequests.Add(sb.ToString());
                    sb.Clear();
                }
                else
                {
                    sb.Append(",");
                }
            }
            var languages = new string[] { "de", "en", "fr", "es" }; // Not supported: "ko", "zh"

            foreach (var language in languages)
            {
                var result = new List<Item>();
                Console.WriteLine("Language: {0}", language);
                int itemsFound = 0;
                foreach (var idRequest in idRequests)
                {
                    String uri = String.Format("https://api.guildwars2.com/v2/items?ids={0}&lang={1}", idRequest, language);

                    var items = JsonConvert.DeserializeObject<List<Item>>(DownloadString(client, uri));
                    foreach (var item in items)
                    {
                        Console.WriteLine("{0:0.00}% {1} {2}", (float)itemsFound / itemsIds.Count * 100, item.Id, item.Name);
                        itemsFound++;
                    }

                    result.AddRange(items);
                }

                var jsonItems = JsonConvert.SerializeObject(result);
                using (var sw = new StreamWriter(String.Format("items_{0}.json", language)))
                {
                    sw.Write(jsonItems);
                }
            }
            Console.WriteLine("DONE!!!");
            Console.ReadLine();
        }
    }
}
