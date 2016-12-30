using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ImageAndVideoPicker;

/**
 * Purpose: Take a string data and convert it to a native Data.
**/
public class DataParser : Base
{

    // Example input: 16.11.12.1.30.0  : total is 5
    // Year  : 16
    // Month : 11
    // Day   : 12
    // Hour  : 1
    // Min   : 30
    // Sec   : 0

    private static MonoBehaviour _behaviour;
    public static MonoBehaviour behaviour
    {
        get
        {
            return _behaviour;
        }
        set
        {
            _behaviour = value;
        }
    }

    void Awake()
    {
        behaviour = this;
    }

    public static DateTime ParseDate(string input)
    {
        string[] inputSeperated = input.Split('.'); 
        
        DateTime dateTime = new DateTime(Int32.Parse(inputSeperated[0]) + 2000, Int32.Parse(inputSeperated[1]), Int32.Parse(inputSeperated[2]), Int32.Parse(inputSeperated[3]), Int32.Parse(inputSeperated[4]), Int32.Parse(inputSeperated[5]));
        return dateTime;
    }

    public static string UnparseDate(DateTime date)
    {
        string newDate = (date.Year-2000) + "." + date.Month + "." + date.Day + "." + date.Hour + "." + date.Minute + "." + date.Second;
        return newDate;
    }

    public static void ChangeIcon()
    {
#if UNITY_ANDROID
        AndroidPicker.BrowseImage(true);
#elif UNITY_IPHONE
		IOSPicker.BrowseImage(true); // true for pick and crop
#endif
    }

    public static string UploadImage(Texture2D image)
    {
        return "";
    }

    public static void SetSprite(Image image, string url, int size = 128)
    {
        behaviour.StartCoroutine(GetSpriteLoader(image, url, size));
    }

    private static IEnumerator GetSpriteLoader(Image image, string url, int size)
    {
        WWW www = new WWW(url);

        yield return www;

        if (www.texture)
        {
            image.sprite = Sprite.Create(www.texture, new Rect(0, 0, size, size), Vector2.zero);
        }
    }

    public static void SetSprite(string url, Action<Sprite> CallbackSprite, int size = 128)
    {
        behaviour.StartCoroutine(GetSpriteLoader(url, CallbackSprite, size));
    }

    private static IEnumerator GetSpriteLoader(string url, Action<Sprite> callback, int size)
    {
        WWW www = new WWW(url);

        yield return www;

        if (www.texture)
        {
            callback(Sprite.Create(www.texture, new Rect(0, 0, size, size), Vector2.zero));
        }
    }

    public static void AuthenticateUser(Action<string, string, Authentication.LoginType> Callback, Authentication.LoginType logintType, string sessionKey)
    {
        Callback("", "", logintType);
    }

    public static void AuthenticateUser(Action<string, string, Authentication.LoginType> Callback, Authentication.LoginType logintType, string userId, string sessionKey)
    {
        Callback("", "", logintType);
    }

    public static UserModel GetUser(int id)
    {
        UserModel userModel = new UserModel(id, "", "", "", "http://orig04.deviantart.net/a222/f/2013/016/9/0/128x128_px_mario_by_wildgica-d5rpb6y.jpg", "email.com", "Samuel Arminana", "I am awesome as hell. This can't go longer than 135 characters", new int[6], new int[5], new int[20], new int[5], "hello", "hello", "hello", "hello", false, false, true);
        int[] goingUsers = new int[1];
        goingUsers[0] = 1;
        return userModel;
    }

    public static UserModel GetUser(string token)
    {
        UserModel userModel = new UserModel(0, token, "", "", "http://orig04.deviantart.net/a222/f/2013/016/9/0/128x128_px_mario_by_wildgica-d5rpb6y.jpg", "email.com", "Samuel Arminana", "I am awesome as hell. This can't go longer than 135 characters", new int[6], null, new int[20], new int[5], "hello", "hello", "hello", "hello", false, false, true);
        UserModel[] goingUsers = new UserModel[1];
        goingUsers[0] = new UserModel(0, token, "", "", "http://orig04.deviantart.net/a222/f/2013/016/9/0/128x128_px_mario_by_wildgica-d5rpb6y.jpg", "email.com", "Samuel Arminana", "I am awesome as hell. This can't go longer than 135 characters", new int[6], null, new int[20], new int[5], "hello", "hello", "hello", "hello", false, false, true);
        return userModel;
    }

    public static UserModel[] GetUsers(int[] users)
    {
        UserModel[] models = new UserModel[users.Length];
        for(int i = 0; i < users.Length; i++)
        {
            models[i] = GetUser(users[i]);
        }
        return models;
    }

    public static UserModel EditUser(UserModel newModel, int id, string token)
    {
        UserModel userModel = newModel;
        userModel.index = id;
        userModel.sessionToken = token;
        return userModel;
    }

    public static UserModel[] SearchUsers(string search)
    {
        UserModel[] models = new UserModel[1];
        models[0] = new UserModel(0, null, null, null, "http://orig04.deviantart.net/a222/f/2013/016/9/0/128x128_px_mario_by_wildgica-d5rpb6y.jpg", "email.com", "Samuel Arminana", "I am awesome as hell. This can't go longer than 135 characters", new int[600], new int[3], new int[20], new int[5], "hello", "hello", "hello", "hello", true, true, true);
        return models;
    }

    public static void SendReportUser(int userId, string reason)
    {

    }

    public static ProtestModel EditProtest(ProtestModel newModel, string token)
    {
        ProtestModel protestModel = newModel;
        // Check token
        return protestModel;
    }
    
    public static void SendReportProtest(int protestId, string reason)
    {

    }

    public static ProtestModel GetProtest(int protest)
    {
        return new ProtestModel(0, "http://orig04.deviantart.net/a222/f/2013/016/9/0/128x128_px_mario_by_wildgica-d5rpb6y.jpg", "Our Portest Name", "Description", "426 NW 1st Ave Cape Coral, FL", "16.11.12.1.30.0", null, null, null, 0, 0, new int[2000], new int[2], new int[3], new int[5], Authentication.user.index);
    }

    public static ProtestModel[] GetProtestList()
    {
        ProtestModel[] models = new ProtestModel[1];
        models[0] = new ProtestModel(0, "http://orig04.deviantart.net/a222/f/2013/016/9/0/128x128_px_mario_by_wildgica-d5rpb6y.jpg", "Our Portest Name", "Description", "426 NW 1st Ave Cape Coral, FL", "16.11.12.1.30.0", null, null, null, 0, 0, new int[20000], new int[3], new int[3], new int[3], Authentication.user.index);
        return models;
    }

    public static void DeleteProtest(int index)
    {

    }

    public static ProtestModel CreateProtest(ProtestModel model)
    {
        return model;
    }

    public static ProtestModel[] SearchProtests(string search)
    {
        ProtestModel[] models = new ProtestModel[1];
        models[0] = new ProtestModel(0, "http://orig04.deviantart.net/a222/f/2013/016/9/0/128x128_px_mario_by_wildgica-d5rpb6y.jpg", "Our Portest Name", "Description", "426 NW 1st Ave Cape Coral, FL", "16.11.12.1.30.0", null, null, null, 0, 0, new int[2000], new int[0], new int[3], new int[3], Authentication.user.index);
        return models;
    }

    public static ProtestModel[] GetProtests(int[] protests)
    {
        ProtestModel[] models = new ProtestModel[protests.Length];
        for (int i = 0; i < protests.Length; i++)
        {
            models[i] = GetProtest(protests[i]);
        }
        return models;
    }

    public static void AddContribution(int id)
    {

    }

    public static void RemoveContribution(int id)
    {

    }

    public static ContributionsModel CreateContribution(ContributionsModel model)
    {
        return model;
    }

    public static void DeleteContribution(int index)
    {

    }

    public static ContributionsModel[] GetContributions(int[] contributions)
    {
        ContributionsModel[] models = new ContributionsModel[contributions.Length];
        for (int i = 0; i < models.Length; i++)
        {
            models[i] = FindContribution(contributions[i]);
        }
        return models;
    }

    public static ContributionsModel FindContribution(int index)
    {
        return new ContributionsModel(0, "Needs", 5, 2, 0);
    }

    public static ChatModel[] GetChats(int[] chats)
    {
        ChatModel[] models = new ChatModel[chats.Length];
        for (int i = 0; i < models.Length; i++)
        {
            models[i] = FindChat(chats[i]);
        }
        return models;
    }

    public static ChatModel FindChat(int chat)
    {
        ChatModel model = new ChatModel(0, "Hello", 0, "16.4.4.1.30.0");
        return model;
    }
    
    public static void LikeProtest(int protest)
    {

    }

    public static void UnLikeProtest(int protest)
    {

    }

    public static void GoingProtest(int protest)
    {

    }

    public static void NotGoingProtest(int protest)
    {

    }

    public static void Follow(int index)
    {

    }
}
