using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NotifierCore.IO
{
    public class CsvHelper
    {
        public CsvHelper()
        {

        }

        public static void WriteCsv(String path, string[] columns, object[][] rows, string seperator)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                StringBuilder sb = new StringBuilder();
                foreach (var col in columns)
                {
                    sb.Append(col);
                    sb.Append(seperator);
                }
                sw.WriteLine(sb);
                sb.Clear();
                foreach (var row in rows)
                {
                    foreach (object item in row)
                    {
                        sb.Append(item);
                        sb.Append(seperator);
                    }
                    sw.WriteLine(sb);
                    sb.Clear();
                }
            }
        }

        public static void ReadCsv(String path, out string[] columns, out string[][] rows, string seperator)
        {
            columns = new string[] { };
            rows = new string[][] { };

            List<String[]> tempList = new List<string[]>();

            bool firstRead = false;

            using (StreamReader sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                {
                    String line = sr.ReadLine();
                    String[] arr = line.Split(new String[] { seperator }, StringSplitOptions.None);
                    if (!firstRead)
                    {
                        firstRead = true;
                        columns = arr;
                    }
                    else
                    {
                        tempList.Add(arr);
                    }
                }
            }
            rows = tempList.ToArray();
        }
    }
}
