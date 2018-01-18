using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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

        public static List<byte> ByteConverter(BinaryReader Reader, bool[] Bits,byte[] Combinationofbytes,int position)
        {
            List<byte> bytes= new List<byte>();
            bool[] bits = Bits;
            int posititon = position;

            
            for (int i = 0; i < Reader.BaseStream.Length; i++)
            {
                bits[posititon] = Reader.ReadBoolean();
                posititon = (posititon + 1) % 8;
                if (posititon == 0)
                {
                    byte b = 0;
                    for(int j = 0; j < bits.Length; j++)
                    {
                        b=(byte)(bits[j] ? b + Combinationofbytes[j] : b);
                    }
                    bytes.Add(b);
                }
               
            }

            return bytes;
        }
    }
}
