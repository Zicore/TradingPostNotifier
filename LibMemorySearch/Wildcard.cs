using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LibMemorySearch
{
    public class Wildcards
    {
        private Dictionary<char, Regex> matchers = new Dictionary<char, Regex>();

        public Wildcards()
        {

        }

        public Wildcards(string[] pairs)
        {
            if (pairs.Length % 2 == 0)
            {
                for (int i = 0; i < pairs.Length; i += 2)
                {
                    Add(pairs[i][0], pairs[i + 1]);
                }
            }
        }

        public void Add(char key, string matcher)
        {
            if (matchers.ContainsKey(key) == false)
            {
                matchers.Add(key, new Regex(String.Format("{0}{{1}}", matcher), RegexOptions.Compiled));
            }
        }

        public bool IsMatch(char key, char data)
        {
            if (matchers.ContainsKey(key))
            {
                return matchers[key].IsMatch(data.ToString());
            }
            return false;
        }
    }
}
