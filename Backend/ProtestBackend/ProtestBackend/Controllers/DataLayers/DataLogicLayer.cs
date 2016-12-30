using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProtestBackend.Controllers.DataLayers
{
    public class DataLogicLayer
    {
        public static int[] ParseString(string stringToParse)
        {
            List<int> intList = new List<int>();
            string[] stringsParsed = stringToParse.Split(',');
            for(int i = 0; i < stringsParsed.Length; i++)
            {
                if(stringsParsed[i] != null && stringsParsed[i] != "")
                    intList.Add(int.Parse(stringsParsed[i]));
            }
            return intList.ToArray();
        }
    }
}