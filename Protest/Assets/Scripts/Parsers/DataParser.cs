using System;
using System.Collections;
using System.Collections.Generic;

/**
 * Purpose: Take a string data and convert it to a native Data.
**/
public class DataParser
{

    // Example input: 16.11.12.1.30.0  : total is 5
    // Year  : 16
    // Month : 11
    // Day   : 12
    // Hour  : 1
    // Min   : 30
    // Sec   : 0

    public static DateTime ParseDate(string input)
    {
        string[] inputSeperated = input.Split('.'); 
        
        DateTime dateTime = new DateTime(Int32.Parse(inputSeperated[0]), Int32.Parse(inputSeperated[1]), Int32.Parse(inputSeperated[2]), Int32.Parse(inputSeperated[3]), Int32.Parse(inputSeperated[4]), Int32.Parse(inputSeperated[5]));
        return dateTime;
    }

    public static string unparseDate(DateTime date)
    {
        string newDate = date.Year + "." + date.Month + "." + date.Day + "." + date.Hour + "." + date.Minute + "." + date.Second;
        return newDate;
    }
}
