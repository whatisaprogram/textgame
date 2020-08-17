using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks.Dataflow;
using System.Xml.Schema;

namespace TextGame
{
    class option
    {

        public struct Contingency
        {
            public List<string> ContingentUpon;
        }

        struct statchange
        {
            public string statname;
            public List<string> value;
        }

        public string text;
        public string result;
        public bool HasContingency;

        List<string> PureStatStringChange;
        List<string> PureContingencyStringChange;
        List<statchange> StatChanges;
        List<Contingency> Contingencies;


        public option(string a_text, string a_result, List<string> a_StatChanges, List<string> ContingenciesToDisplay)
        {
            text = a_text;
            result = a_result;
            PureStatStringChange = a_StatChanges;
            PureContingencyStringChange = ContingenciesToDisplay;
            StatChanges = new List<statchange>();
            Contingencies = new List<Contingency>();

            if (ContingenciesToDisplay.Count == 0)
            {
                HasContingency = false;
            }
            else
            {
                HasContingency = true;
            }

            foreach (string s in PureStatStringChange)
            {
                statchange sr;
                string[] Words = s.Split(" ");

                sr.value = new List<string>();
                sr.statname = Words[0];

                if (Words[1] == "=")
                {
                    for (int i = 2; i < Words.Length; i++)
                    {
                        sr.value.Add(Words[i]);
                    }

                }
                else
                {
                    for (int i = 0; i < Words.Length; i++)
                    {
                        sr.value.Add(Words[i]);
                    }
                }

                StatChanges.Add(sr);

            }

            foreach (string s in PureContingencyStringChange)
            {
                Contingency cont;
                string[] Words = s.Split(" ");

                cont.ContingentUpon = new List<string>();

                for (int i = 0; i <Words.Length; i++)
                {
                    cont.ContingentUpon.Add(Words[i]);
                }

                Contingencies.Add(cont);
            }

        }


        // add new functions here! to compute method
   
        public bool CanDisplay (statistics stats)
        {
            if(HasContingency == false)
            {
                return true;
            }
            
            foreach (Contingency c in Contingencies)
            {
                int counter = 0;
                string Regex_equals = @"^[=><]$";
                List<string> PurifiedList = PurifyContingencyList(c.ContingentUpon, stats);

                for (int i = 0; i < PurifiedList.Count; i++)
                {
                    List<string> Expression = new List<string>();

                    if (PurifiedList[i] == "AND" || Regex.IsMatch(PurifiedList[i], Regex_equals))
                    {
                        for (int j = i - counter; j < i; j++)
                        {
                            Expression.Add(PurifiedList[j]);
                        }
                        int result = Evaluate(Expression);
                        Expression.Clear();
                        counter = 0;

                    }
                    else
                    {
                        counter++;
                    }
                }

                if (counter > 0)
                {
                    List<string> Expression = new List<string>();
                    for (int j = PurifiedList.Count - counter; j < PurifiedList.Count; j++)
                    {
                        Expression.Add(PurifiedList[j]);
                    }
                    int result = Evaluate(Expression);
                }

                string PleaseTellMeIfThisANDStatmentisTrueorfalse = String.Join(" ", PurifiedList);

                if (EvaluateANDstatement(PleaseTellMeIfThisANDStatmentisTrueorfalse))
                {
                    return true;
                }
            }

            return false;
        }

        public List<string> PurifyContingencyList(List<string> ContingencyListToBePurified, statistics st)
        {
            List<string> PurifiedList = new List<string>();

            string Regex_numbers = @"^-?[0-9]+$";
            string Regex_operators = @"^[+\-*/]$";
            string Regex_equals = @"^[=><]$";

            for (int i = 0; i < ContingencyListToBePurified.Count; i++)
            {
                if (Regex.IsMatch(ContingencyListToBePurified[i], Regex_numbers) || Regex.IsMatch(ContingencyListToBePurified[i], Regex_operators) || Regex.IsMatch(ContingencyListToBePurified[i], Regex_equals) || ContingencyListToBePurified[i] == "AND")
                {
                    PurifiedList.Add(ContingencyListToBePurified[i]);
                }
                else
                {
                    int dictvalue = 0;
                    bool doesitexist = st.StatDict.TryGetValue(ContingencyListToBePurified[i], out dictvalue);

                    if(!doesitexist)
                    {
                        Console.WriteLine("Error: {0} is not a valid stat!", ContingencyListToBePurified[i]);
                    }
                    PurifiedList.Add(dictvalue.ToString());
                }
            }

            return PurifiedList;
        }

        public List<string> PurifyStatList(List<string> value, statistics st)
        {
            //this method replaces all variables in a given statchange.value to their current numbers and gives you a new list
            List<string> PurifiedList = new List<string>();

            string Regex_numbers = @"^-?[0-9]+$";
            string Regex_operators = @"^[+\-*/]$";

            for (int i = 0; i < value.Count; i++)
            {
                if (Regex.IsMatch(value[i], Regex_numbers) || Regex.IsMatch(value[i], Regex_operators))
                {
                    PurifiedList.Add(value[i]);
                }
                else
                {
                    int dictvalue = 0;
                    bool doesitexist = st.StatDict.TryGetValue(value[i], out dictvalue);
                    if (!doesitexist)
                    {
                        Console.WriteLine("Error: {0} is not a valid stat!", value[i]);
                    }
                    PurifiedList.Add(dictvalue.ToString());
                }
            }

            return PurifiedList;
        }

        public bool EvaluateANDstatement(string PurifiedANDString)
        {
            string[] idklol = PurifiedANDString.Split("AND");
            for (int i = 0; i < idklol.Length; i++)
            {
                idklol[i] = idklol[i].Trim(); 
            }

            foreach (string s in idklol)
            {
                string[] lmao = s.Split(" ");
                if (lmao[1] == "=")
                {
                    int left = 0;
                    Int32.TryParse(lmao[0], out left);

                    int right = 0;
                    Int32.TryParse(lmao[2], out right);

                    if(left == right)
                    {
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (lmao[1] == ">")
                {
                    int left = 0;
                    Int32.TryParse(lmao[0], out left);

                    int right = 0;
                    Int32.TryParse(lmao[2], out right);

                    if (left > right)
                    {
                        continue;
                    }   
                    else
                    {
                        return false;
                    }
                }
                else if (lmao[1] == "<")
                {
                    int left = 0;
                    Int32.TryParse(lmao[0], out left);

                    int right = 0;
                    Int32.TryParse(lmao[2], out right);

                    if (left < right)
                    {
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Oops! Something went wrong!! Please check that all your expressions are written properly.");
                }

            }

            return true;
        }
        public int Evaluate(List<string> some_expression)
        {

            string Regex_operators = @"^[+\-*/]$";
            string Regex_numbers = @"^-?[0-9]+$";

            Stack<string> ShuntingYardStack = new Stack<string>();
            Queue<string> ShuntingYardQueue = new Queue<string>();

            //shunting-yard algorithm
            //i used the shunting yard algorithm : https://en.wikipedia.org/wiki/Shunting-yard_algorithm
            foreach (string s in some_expression)
            {
                if (Regex.IsMatch(s, Regex_operators))
                {
                    if (s == "*")
                    {
                        ShuntingYardStack.Push(s);
                    }
                    else if (s == "/")
                    {
                        string temp = null;
                        ShuntingYardStack.TryPeek(out temp);
                        if (temp == null || temp != "*")
                        {
                            ShuntingYardStack.Push(s);
                        }
                        else
                        {
                            ShuntingYardQueue.Enqueue(temp);
                            ShuntingYardStack.Pop();
                            ShuntingYardStack.Push(s);
                        }
                    }
                    else if (s == "+")
                    {
                        string temp = null;
                        ShuntingYardStack.TryPeek(out temp);
                        if (temp == null || temp != "*" || temp != "/")
                        {
                            ShuntingYardStack.Push(s);
                        }
                        else
                        {
                            ShuntingYardQueue.Enqueue(temp);
                            ShuntingYardStack.Pop();
                            ShuntingYardStack.Push(s);
                        }
                    }
                    else
                    {
                        string temp = null;
                        ShuntingYardStack.TryPeek(out temp);
                        if (temp == null || temp != "*" || temp != "/" || temp != "+")
                        {
                            ShuntingYardStack.Push(s);
                        }
                        else
                        {
                            ShuntingYardQueue.Enqueue(temp);
                            ShuntingYardStack.Pop();
                            ShuntingYardStack.Push(s);
                        }
                    }
                }
                else
                {
                    ShuntingYardQueue.Enqueue(s);
                }

            }
            while (ShuntingYardStack.Count != 0)
            {
                string temp = null;
                ShuntingYardStack.TryPeek(out temp);
                if (temp != null)
                {
                    ShuntingYardQueue.Enqueue(temp);
                    ShuntingYardStack.Pop();
                }
            }

            //evaluate reverse polish

            while (ShuntingYardQueue.Count != 0)
            {
                string temp = null;
                ShuntingYardQueue.TryPeek(out temp);

                if (temp != null && Regex.IsMatch(temp, Regex_numbers))
                {
                    ShuntingYardStack.Push(temp);
                    ShuntingYardQueue.Dequeue();
                }
                else if (temp != null && Regex.IsMatch(temp, Regex_operators))
                {
                    int number_right = 1;
                    int number_left = 1;

                    string right = null;
                    ShuntingYardStack.TryPeek(out right);
                    if(right != null)
                    {
                        int temp2 = 1;
                        Int32.TryParse(right, out temp2);
                        number_right = temp2;
                    }

                    ShuntingYardStack.Pop();

                    string left = null;
                    ShuntingYardStack.TryPeek(out left);
                    if(left != null)
                    {
                        int temp2 = 1;
                        Int32.TryParse(left, out temp2);
                        number_left = temp2;
                    }

                    ShuntingYardStack.Pop();
                    int result = 1;

                    switch(temp)
                    {
                        case ("*"):
                            result = number_left * number_right;
                            break;
                        case ("/"):
                            result = number_left / number_right;
                            break;
                        case ("+"):
                            result = number_left + number_right;
                            break;
                        case ("-"):
                            result = number_left - number_right;
                            break;

                    }
                    ShuntingYardQueue.Dequeue();

                    string ResultButString = result.ToString();
                    ShuntingYardStack.Push(ResultButString);
         
                }
                else
                {
                }
            }

            string FinalResult = null;
            ShuntingYardStack.TryPeek(out FinalResult);

            int ReturnResult;
            if (FinalResult == null)
            {
                ReturnResult = 1;
            }
            else
            {
                ReturnResult = Int32.Parse(FinalResult);
            }

            return ReturnResult;
        }

        public void ExecuteStatChanges(statistics stats)
        {
            foreach (statchange a_statchange in StatChanges)
            {
                List<string> Expression = PurifyStatList(a_statchange.value, stats);
                if (Expression.Count == 1)
                {
                    int temp = 1;
                    Int32.TryParse(Expression[0], out temp);

                    int tempvalue = 1;
                    bool exists = stats.StatDict.TryGetValue(a_statchange.statname, out tempvalue);
                    if (exists)
                    {
                        stats.StatDict[a_statchange.statname] = tempvalue;
                    }

                }
                else
                {
                    int result = Evaluate(Expression);

                    int tempvalue = 1;
                    bool exists = stats.StatDict.TryGetValue(a_statchange.statname, out tempvalue);
                    if (exists)
                    {
                        stats.StatDict[a_statchange.statname] = result;
                    }
                }
            }
        }

        public override string ToString()
        {
            return text + "->" + result;
        }
    }
}
