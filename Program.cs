using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace TextGame
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: <FirstNode> <File1> <File2> ...");
                return;
            }

            string BEGINNINGText = args[0];

            Dictionary<string, node> nodeDict = new Dictionary<string, node>();
            Dictionary<string, int> stat_dict = new Dictionary<string, int>();

            statistics Stats = new statistics(stat_dict);

            for (int i = 1; i < args.Length; i++)
            {
                string file = args[i];
                ReadText(file, nodeDict, stat_dict, Stats.CanDisplay);

            }

            node CurrentNode;
            string NodeKey = BEGINNINGText;
            Stats.MakeOriginalCopy();

           
            while (nodeDict.TryGetValue(NodeKey, out CurrentNode))
            {
                int OptCount = CurrentNode.Display(Stats);
                Stats.Display();

                bool dumbUser = false;

                if (OptCount == 0)
                {
                    Console.WriteLine("Press Any Key To Restart!");
                    Stats.StatDict.Clear();
                    foreach (KeyValuePair<string, int> KeyValue in Stats.OriginalCopyofStatDict)
                    {
                        Stats.StatDict.Add(KeyValue.Key, KeyValue.Value);
                    }
                    Console.ReadKey(true);
                    NodeKey = BEGINNINGText;
                    continue;
                }



                do
                {
                    ConsoleKeyInfo SelectedOption = Console.ReadKey(true);

                    int OptionConfirmedSelected = (int)(SelectedOption.KeyChar - '0');
                    option selectedOption = CurrentNode.GetOption(OptionConfirmedSelected - 1);

                    if (selectedOption != null)
                    {
                        selectedOption.ExecuteStatChanges(Stats);
                        NodeKey = selectedOption.result;
                        dumbUser = false;
                    }
                    else if (!dumbUser)
                    {
                        Console.WriteLine("Please press a Key Next to an Option!");
                        dumbUser = true;
                    }
                }
                while (NodeKey == null || dumbUser);

            }
            



        }

        private static void hhhhh()
        {
            string x = Directory.GetCurrentDirectory();
            Console.WriteLine(x);
        }

        private static void ReadText(string a_textfilepath, Dictionary<string, node> Node_Dict, Dictionary<string, int> stats, Dictionary <string, bool> CanDisplayStats)
        {
            try
            {
                using (StreamReader test = new StreamReader(a_textfilepath))
                {

                    string line = test.ReadLine();
                    while (line != null)
                    {
                        if (line.Contains("<define_stats>"))
                        {
                            line = test.ReadLine();
                            while (!line.Contains("</define_stats>"))
                            {
                                bool CanDisplay;

                                string TrimmedLine = line.Trim();
                                string[] SplitLine = TrimmedLine.Split('=');
                                string StatName;
                                string StatValue;

                                if (SplitLine[1].Contains(" , Display") || SplitLine[1].Contains(", Display"))
                                {
                                    CanDisplay = true;
                                    StatName = SplitLine[0].Trim();

                                    string[] SplitonComma = SplitLine[1].Split(',');
                                    StatValue = SplitonComma[0].Trim();
                                    

                                }
                                else
                                {
                                    CanDisplay = false;
                                    StatName = SplitLine[0].Trim();
                                    StatValue = SplitLine[1].Trim();
                                }
                                
                                

                                int CheckedStatValue = 0;

                                if (Int32.TryParse(StatValue, out CheckedStatValue) == false)
                                {
                                    line = test.ReadLine();
                                    continue;
                                }

                                bool Repeated = false;
                                foreach (string key in stats.Keys)
                                {
                                    if (key == StatName)
                                    {
                                        Repeated = true;
                                    }
                                }

                                if (StatName == null || Repeated == true)
                                {
                                    line = test.ReadLine();
                                    continue;
                                }

                                stats.Add(StatName, CheckedStatValue);
                                CanDisplayStats.Add(StatName, CanDisplay);
                                line = test.ReadLine();

                            }
                        }

                        if (line.Contains("<event>"))
                        {
                            node n = new node();
                            line = test.ReadLine();
                            while (!line.Contains("</event>"))
                            {
                                if (line.Contains("<name>"))
                                {
                                    line = test.ReadLine();
                                    while (!line.Contains("</name>"))
                                    {
                                        n.Name = line.Trim();
                                        line = test.ReadLine();
                                    }
                                }
                                else if (line.Contains("<body>"))
                                {
                                    line = test.ReadLine();
                                    while (!line.Contains("</body>"))
                                    {
                                        n.Body = line.Trim();
                                        line = test.ReadLine();
                                    }
                                }
                                else if (line.Contains("<option>") || line.Contains("<options>"))
                                {
                                    string optname = "";
                                    string optresult = "";
                                    List<string> Modifiers = new List<string>();
                                    List<string> Contingency = new List<string>();
                                    line = test.ReadLine();
                                    while (!line.Contains("</option>"))
                                    {
                                        if (line.Contains("<text>"))
                                        {
                                            line = test.ReadLine();
                                            while (!line.Contains("</text>"))
                                            {
                                                optname = line.Trim();
                                                line = test.ReadLine();
                                            }
                                        }
                                        else if (line.Contains("<trigger>"))
                                        {
                                            line = test.ReadLine();
                                            while (!line.Contains("</trigger>"))
                                            {
                                                optresult = line.Trim();
                                                line = test.ReadLine();
                                            }
                                        }
                                        else if (line.Contains("<statupdate>"))
                                        {
                                            line = test.ReadLine();
                                            while (!line.Contains("</statupdate>"))
                                            {
                                                string unalteredtrimmed = line.Trim();
                                                //string pattern = @"[\w-]+\s[+-/*=]\s[0-9]+(\s[+-/*]\s[0-9]+)*"; this is my original regex before i potentially messed it up plz no touchy
                                                string pattern = @"[\w-]+\s[+-/*=]\s-?[0-9]+(\s[+-/*]\s-?[0-9]+)*$";
                                                if (Regex.IsMatch(unalteredtrimmed, pattern))
                                                {
                                                    Modifiers.Add(unalteredtrimmed);
                                                }
                                                else
                                                {
                                                    Console.WriteLine($"ERROR: \"{unalteredtrimmed}\" is not a correct modifier and will not be taken into account in this run of the game.");
                                                }

                                                line = test.ReadLine();
                                            }
                                        }
                                        else if (line.Contains("<contingent>"))
                                        {
                                            line = test.ReadLine();
                                            while (!line.Contains("</contingent>"))
                                            {
                                                string TrimmedLine = line.Trim();
                                                string pattern = @"^[\w-]+\s([*/+-]\s[\w-]+\s)*[=><]\s[\w-]+(\s[*/+-]\s[\w-]+)*(\sAND\s[\w-]+\s([*/+-]\s[\w-]+\s)*[=><]\s[\w-]+(\s[*/+-]\s[\w-]+)*)*$";

                                                if (Regex.IsMatch(TrimmedLine, pattern))
                                                {
                                                    Contingency.Add(TrimmedLine);
                                                }
                                                else
                                                {
                                                    Console.WriteLine($"ERROR: \"{TrimmedLine}\" is not a correct contingency and will not be taken into account in this run of the game.");
                                                }


                                                line = test.ReadLine();
                                            }
                                        }
                                        else
                                        {
                                            line = test.ReadLine();
                                        }


                                    }

                                    option CurrentNodeOptions = new option(optname, optresult, Modifiers, Contingency);
                                    n.AddOption(CurrentNodeOptions);

                                }
                                else
                                {
                                    line = test.ReadLine();
                                }


                            }
                            //Console.WriteLine(n); some debug stuff
                            Node_Dict.Add(n.Name, n);
                        }

                        line = test.ReadLine();
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File \"" + a_textfilepath + "\" not found");
            }
        }

        
    }
}