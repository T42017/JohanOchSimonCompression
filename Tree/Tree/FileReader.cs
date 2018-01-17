using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tree
{
     public class FileReader
    {       
        public static string ReadFile(string filename)
        {
            string text=File.ReadAllText(filename) ;           
            return text;
        }

        public static Dictionary<char, int> FrequencyLookup(string text)
        {
            Dictionary<char,int>outputOfDictionary=new Dictionary<char, int>();
            foreach (char karak in text)
            {
                if (outputOfDictionary.ContainsKey(karak))
                {
                    outputOfDictionary[karak]++;
                }
                else
                {
                    outputOfDictionary.Add(karak, 1);
                }
            }
            return outputOfDictionary;
        }
    }
}
