using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Text;

namespace TextGame
{
    class node
    {
        //consists of:
        //display
        //store options somehow
        public string Name;
        public string Body;
        List<option> Options;

        public node()
        {
            Options = new List<option>();
        }

        public option GetOption(int OptionIndex)
        {
            if (OptionIndex < 0 || OptionIndex >= Options.Count)
            {
                return null;
            }
            else
            {
                return Options[OptionIndex];
            }
        }

        public void AddOption(option a_option)
        {
            Options.Add(a_option);
        }

        public void AddModifiers(option an_option, List<string> List_of_Modifiers_In_Sentence_Form)
        {

        }

        public int Display(statistics stats)
        {
            string[] DisplayStrings = Body.Split("\\n");

            char[] dashStringArr = new char[Console.WindowWidth];

            for (int i = 0; i < Console.WindowWidth; i++)
                dashStringArr[i] = '-';

            Console.WriteLine("\n");
            Console.WriteLine(new string(dashStringArr));
            for (int i = 0; i < DisplayStrings.Length; i++)
            {
                string DisplayString = DisplayStrings[i];

                int len = DisplayString.Length;
                while (len > 0)
                {
                    if (len > Console.WindowWidth)
                    {
                        Console.WriteLine(DisplayString.Substring(0, Console.WindowWidth));
                        DisplayString = DisplayString.Substring(Console.WindowWidth);
                    }
                    else
                    {
                        Console.SetCursorPosition((Console.WindowWidth - DisplayString.Length) / 2, Console.CursorTop);
                        Console.WriteLine(DisplayString);
                    }
                    len -= Console.WindowWidth;
                }
            }
            Console.WriteLine();

            int counter = 0;
            foreach (option o in Options)
            {
                if (o == null)
                {
                    continue;
                }
                else if (o.CanDisplay(stats))
                {
                    counter++;
                    Console.WriteLine($"({counter}): {o.text}");
                }
                else
                {
                    continue;
                }

                
            }

            return counter;
        }

        public override string ToString()
        {
            string retval = "";

            retval += Name + ", ";
            retval += Body;

            foreach (option x in Options)
            {
                retval += ", " + x;
            }
            return retval;
        }
    }
}
