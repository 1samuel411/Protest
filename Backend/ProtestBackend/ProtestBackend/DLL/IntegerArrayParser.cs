using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ProtestBackend.DLL
{
    public class IntegerArrayParser
    {
        public static int[] ParseStringToInts(string stringToParse)
        {
            if (!String.IsNullOrEmpty(stringToParse)) {
                return (int[]) stringToParse.Split(',').ToArray()
                    .Where(x => Regex.IsMatch(x, @"^\d+$"))
                    .Select(y => Int32.Parse(y));
            }
            return new int[0];
        }

        //public static int[] ParseString(string stringToParse)
        //{
        //    List<int> intList = new List<int>();
        //    string[] stringsParsed = stringToParse.Split(',');
        //    for (int i = 0; i < stringsParsed.Length; i++) {
        //        intList.Add(int.Parse(stringsParsed[i]));
        //    }
        //    return intList.ToArray();
        //}
    }
}