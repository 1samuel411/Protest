using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProtestBackend.DLL
{
    public class IntegerArrayParser
    {
        public static int[] ParseString(string stringToParse)
        {
            List<int> intList = new List<int>();
            string[] stringsParsed = stringToParse.Split(',');
            for (int i = 0; i < stringsParsed.Length; i++)
            {
                intList.Add(int.Parse(stringsParsed[i]));
            }
            return intList.ToArray();
        }
    }
}