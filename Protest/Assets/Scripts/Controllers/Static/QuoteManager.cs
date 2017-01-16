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
        AddQuote("Life is 10% what happens to you and 90% how you react to it.", "Charles R. Swindoll");
        AddQuote("In order to succeed, we must first believe that we can.", "Nikos Kazantzakis");
        AddQuote("Keep your eyes on the stars, and your feet on the ground.", "Theodore Roosevelt");
        AddQuote("It always seems impossible until it's done.", "Nelson Mandela");
        AddQuote("You can't cross the sea merely by standing and staring at the water.", "Rabindranath Tagore");
        AddQuote("It does not matter how slowly you go as long as you do not stop.", "Confucius");
        AddQuote("Problems are not stop signs, they are guidelines.", "Robert H. Schuller");
        AddQuote("We should not give up and we should not allow the problem to defeat us.", "A. P. J. Abdul Kalam");
        AddQuote("Be kind whenever possible. It is always possible.", "Dalai Lama");
        AddQuote("Life has no limitations, except the ones you make.", "Les Brown");
    }

    private static void AddQuote(string quote, string name)
    {
        Quote newQuote = new Quote();
        newQuote.name = name;
        newQuote.quote = quote;
        quotes.Add(newQuote);
    }
}
