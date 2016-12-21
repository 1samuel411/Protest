using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuoteManager : Base
{

    private static bool initialized;
    [System.Serializable]
    private struct Quote
    {
        public string quote;
        public string name;
    }
    private static List<Quote> quotes = new List<Quote>();
    
    public static string GetQuote()
    {
        if (!initialized)
            Initialize();

        int index = Random.Range(0, quotes.Count);
        return "<i>\"" + quotes[index].quote + "\" </i>\n  - " + quotes[index].name;
    }

    public static void Reset()
    {
        initialized = false;
    }

    private static void Initialize()
    {
        // Reset quotes
        quotes = new List<Quote>();

        // Add quotes
        PopulateQuotes();

        initialized = true;
    }

    private static void PopulateQuotes()
    {
        AddQuote("Courage is resistance to fear, mastery of fear, not absence of fear.", "Mark Twain");
        AddQuote("You must be the change you wish to see in the world.", "Mahatma Gandhi"); 
    }

    private static void AddQuote(string quote, string name)
    {
        Quote newQuote = new Quote();
        newQuote.name = name;
        newQuote.quote = quote;
        quotes.Add(newQuote);
    }
}
