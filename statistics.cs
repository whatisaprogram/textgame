using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TextGame
{

    public class statistics
    {
        public Dictionary<string, int> StatDict;

        public Dictionary<string, int> OriginalCopyofStatDict;

        public Dictionary<string, bool> CanDisplay;

        public statistics()
        {
            StatDict = new Dictionary<string, int>();
        }

        public statistics(Dictionary<string, int> dict)
        {
            StatDict = dict;

            OriginalCopyofStatDict = new Dictionary<string, int>();

            CanDisplay = new Dictionary<string, bool>();

        }

        public void MakeOriginalCopy()
        {
            foreach (KeyValuePair<string, int> KeyValue in StatDict)
            {
                OriginalCopyofStatDict.Add(KeyValue.Key, KeyValue.Value);
            }
        }

        public void Make(string s, int x)
        {
            StatDict.TryAdd(s, x);
        }

        public void Display()
        {
            Console.WriteLine();
            foreach (var stat in StatDict)
            {
                bool temp = false;
                CanDisplay.TryGetValue(stat.Key, out temp);

                if (temp == true)
                {
                    Console.WriteLine($"{stat.Key}: {stat.Value}");
                }
            }
        }

        public int ReturnValue(string SupposedKey)
        {
            bool DoesTheKeyExist = StatDict.ContainsKey(SupposedKey);

            if(!DoesTheKeyExist)
            {
                return 1;
                Console.WriteLine($"Looks like you've made a typo when you tried to set a value for one of the options! :-( It's this: \"{SupposedKey}\"");
            }
            else
            {
                int buffer = 0;
                StatDict.TryGetValue(SupposedKey, out buffer);
                return buffer;
            }
        }
    }




}
