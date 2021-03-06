﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ProtestBackend.DLL
{
    public class Parser
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long ConvertToTimestamp(DateTime value)
        {
            TimeSpan elapsedTime = value - Epoch;
            return (long)elapsedTime.TotalSeconds;
        }

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

        public static DateTime ParseDate(string input)
        {
            string[] inputSeperated = input.Split('.');

            DateTime dateTime = new DateTime(Int32.Parse(inputSeperated[0]) + 2000, Int32.Parse(inputSeperated[1]), Int32.Parse(inputSeperated[2]), Int32.Parse(inputSeperated[3]), Int32.Parse(inputSeperated[4]), Int32.Parse(inputSeperated[5]));
            return dateTime;
        }

        public static string UnparseDate(DateTime date)
        {
            string newDate = (date.Year - 2000) + "." + date.Month + "." + date.Day + "." + date.Hour + "." + date.Minute + "." + date.Second;
            return newDate;
        }

        // This will return the Windows zone that matches the IANA zone, if one exists.
        public static string IanaToWindows(string ianaZoneId)
        {
            var utcZones = new[] { "Etc/UTC", "Etc/UCT", "Etc/GMT" };
            if (utcZones.Contains(ianaZoneId, StringComparer.Ordinal))
                return "UTC";

            var tzdbSource = NodaTime.TimeZones.TzdbDateTimeZoneSource.Default;

            // resolve any link, since the CLDR doesn't necessarily use canonical IDs
            var links = tzdbSource.CanonicalIdMap
                .Where(x => x.Value.Equals(ianaZoneId, StringComparison.Ordinal))
                .Select(x => x.Key);

            // resolve canonical zones, and include original zone as well
            var possibleZones = tzdbSource.CanonicalIdMap.ContainsKey(ianaZoneId)
                ? links.Concat(new[] { tzdbSource.CanonicalIdMap[ianaZoneId], ianaZoneId })
                : links;

            // map the windows zone
            var mappings = tzdbSource.WindowsMapping.MapZones;
            var item = mappings.FirstOrDefault(x => x.TzdbIds.Any(possibleZones.Contains));
            if (item == null) return null;
            return item.WindowsId;
        }
    }
}