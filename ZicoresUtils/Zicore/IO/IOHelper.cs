using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ZicoresUtils.Zicore.IO
{
    public static class IOHelper
    {
        public static String ApplicationPath = AppDomain.CurrentDomain.BaseDirectory;

        public static String CreateFolderPath(String folder)
        {
            return System.IO.Path.Combine(ApplicationPath, folder);
        }

        public static void SaveList(List<String> list, String path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                foreach (String str in list)
                {
                    sw.WriteLine(str);
                }
                sw.Close();
            }
        }

        public static List<String> ReadList(String path)
        {
            List<String> list = new List<string>();

            using (StreamReader sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    list.Add(line);
                }
            }

            list.Sort();
            return list;
        }
    }
}
