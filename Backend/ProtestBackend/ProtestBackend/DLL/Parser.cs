using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ProtestBackend.DLL
{
    public class Parser
    {
        public static int[] ParseStringToIntArray(string stringToParse)
        {
            if (String.IsNullOrEmpty(stringToParse))
                return new int[0];
            List<int> intList = new List<int>();
            string[] stringsParsed = stringToParse.Split(',');
            int result;
            for (int i = 0; i < stringsParsed.Length; i++)
            {
                if (!String.IsNullOrEmpty(stringsParsed[i]))
                    if (int.TryParse(stringsParsed[i], out result))
                        intList.Add(result);
            }
            return intList.ToArray();
        }

        public static string[] ParseStringToStringArray(string stringToParse)
        {
            if (String.IsNullOrEmpty(stringToParse))
                return new string[0];
            List<string> stringList = new List<string>();
            string[] stringsParsed = stringToParse.Split(',');
            for (int i = 0; i < stringsParsed.Length; i++)
            {
                if (!String.IsNullOrEmpty(stringsParsed[i]))
                    stringList.Add(stringsParsed[i]);
            }
            return stringList.ToArray();
        }

        public static int[] ParseColumnsToIntArray(DataRowCollection rows, int index)
        {
            if (rows.Count <= 0)
                return new int[0];
            List<int> intList = new List<int>();
            for(int i = 0; i < rows.Count; i++)
            {
                intList.Add(rows[i].Field<int>(0));
            }
            return intList.ToArray();
        }
    }
}