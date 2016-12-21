using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class Authentication : Base
{

    public static string auth_token;

    public static bool authenticated;

    private static string localKey = "SessionKey";

    public UserModel ourModel;

    void Awake()
    {
        if(PlayerPrefs.HasKey(localKey))
        {
            Authenticate(PlayerPrefs.GetString(localKey));
        }
    }

    public static void Login_Google(Action<int> callback)
    {
        Log.Create(0, "Login to google event", "Authentication");

        int response = 0;
       
        // Login to facebook and return the key to call the authenticate method

        Authenticate();
        
        callback.Invoke(response);
    }

    public static void Login_Facebook(Action<int> callback)
    {
        int response = 0;
        // Login to facebook and return the key to call the authenticate method

        Authenticate();

        callback.Invoke(response);
    }

    public static void Authenticate(string fbKey = "", string googleKey = "")
    {
        // Set session key on user with the fbKey or googleKey

        // return userModel

        authenticated = true;
        auth_token = GenerateSessionKey();
        PlayerPrefs.SetString(localKey, auth_token);
        Log.Create(2, "Authentication Successful", "Authentication", auth_token);
    }

    public static void Authenticate(string sessionKey)
    {
        Log.Create(2, "Authentication Checking Session: " + sessionKey, "Authentication");

        // Search for session key

        // return usermodel

        authenticated = true;
        auth_token = sessionKey;
        PlayerPrefs.SetString(localKey, auth_token);
        Log.Create(2, "Authentication Successful", "Authentication", auth_token);
    }

    public static void UnAuthenticate()
    {
        Log.Create(2, "Unauthentication Successful", "Authentication", auth_token);
        authenticated = false;
        auth_token = "";
        PlayerPrefs.DeleteKey(localKey);
    }


    // Move this to server
    // *******************************************************************
    private static string character = "aAbBcCdDeEfFgGhHiIjJkKlLmMnNoOpPqQrRsStTuUvVwWxXyYzZ0123456789!@#$%^&*()-=[]/?<>,.";
    private static string GenerateSessionKey()
    {
        DateTime now = DateTime.UtcNow;
        string randomString = "";
        for(int i = 0; i < 20; i++)
        {
            randomString += character[UnityEngine.Random.Range(0, character.Length - 1)];
        }
        return (randomString + "|" + now.ToShortDateString() + "|" + now.ToShortTimeString() + "|" + now.Millisecond).Replace(' ', '_');
    }

    public static string MD5Hash(string input)
    {
        StringBuilder hash = new StringBuilder();
        MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
        byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

        for (int i = 0; i < bytes.Length; i++)
        {
            hash.Append(bytes[i].ToString("x2"));
        }
        return hash.ToString();
    }
}
