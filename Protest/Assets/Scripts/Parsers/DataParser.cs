using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ImageAndVideoPicker;
using DeadMosquito.AndroidGoodies;
using System.Linq;
using Facebook.Unity;

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

    public const string URL = "http://protestchange.azurewebsites.net";

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

    void OnEnable()
    {
        PickerEventListener.onImageSelect += OnImageSelects;
        PickerEventListener.onImageLoad += OnImageLoads;
        PickerEventListener.onVideoSelect += OnVideoSelects;
        PickerEventListener.onError += OnErrors;
        PickerEventListener.onCancel += OnCancels;
        Debug.Log("Finished registering listeners for image");
    }

    void OnDisable()
    {
        PickerEventListener.onImageSelect -= OnImageSelects;
        PickerEventListener.onImageLoad -= OnImageLoads;
        PickerEventListener.onVideoSelect -= OnVideoSelects;
        PickerEventListener.onError -= OnErrors;
        PickerEventListener.onCancel -= OnCancels;
    }

    void OnVideoSelects(string vidPath)
    {
        Debug.Log("Video Location : " + vidPath);
        Handheld.PlayFullScreenMovie("file://" + vidPath, Color.blue, FullScreenMovieControlMode.Full, FullScreenMovieScalingMode.AspectFill);
    }

    void Awake()
    {
        behaviour = this;
    }

    void OnImageSelects(string imgPath, ImageAndVideoPicker.ImageOrientation imgOrientation)
    {
        Debug.Log("Image Location : " + imgPath);
    }

    void OnImageLoads(string imgPath, Texture2D tex, ImageAndVideoPicker.ImageOrientation imgOrientation)
    {
        Debug.Log("Image Location : " + imgPath);
        GetIconCallback(tex);
    }

    void OnErrors(string errorMsg)
    {
        Debug.Log("Error : " + errorMsg);
    }

    void OnCancels()
    {
        Debug.Log("Cancel by user");
    }

    public void ChangeIconLocal()
    {
#if UNITY_ANDROID
        AndroidPicker.BrowseImage(true);
#elif UNITY_IPHONE
		IOSPicker.BrowseImage(true); // true for pick and crop
#endif
    }

    public static string GetCount(int amount)
    {
        string countString ="";
        if (amount >= 1000)
        {
            countString = Math.Round((amount / 1000.0f), 1).ToString() + "k";
        }
        else
            countString = amount.ToString();

        if (amount >= 1000000)
        {
            countString = Math.Round((amount / 1000000.0f), 0).ToString() + "m";
        }
        return countString;
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

    public static string ParseIntArrayToString(int[] arrayToParse)
    {
        string value = "";
        if (arrayToParse.Length <= 0)
            return "";

        for(int i = 0; i < arrayToParse.Length; i++)
        {
            value += arrayToParse[i].ToString() + ((i >= arrayToParse.Length - 1) ? "" : ",");
        }
        return value;
    }

    public static string ParseStringArrayToString(string[] arrayToParse)
    {
        string value = "";
        if (arrayToParse.Length <= 0)
            return "";

        for (int i = 0; i < arrayToParse.Length; i++)
        {
            value += arrayToParse[i].ToString() + ((i >= arrayToParse.Length - 1) ? "" : ",");
        }
        return value;
    }

    public static int[] ParseJsonToIntArray(List<JSONObject> jsonObjects)
    {
        if (jsonObjects == null)
            return new int[0];
        List<int> intList = new List<int>();
        for (int i = 0; i < jsonObjects.Count; i++)
        {
            if (jsonObjects[i] != null)
                intList.Add((int)jsonObjects[i].n);
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
        string newDate = (date.Year-2000) + "." + date.Month + "." + date.Day + "." + date.Hour + "." + date.Minute + "." + date.Second;
        return newDate;
    }

    public static Action<Texture2D> GetIconCallback;
    public static void ChangeIcon(Action<Texture2D> Callback)
    {
        Debug.Log("Changing icon!");
        GetIconCallback = Callback;
        behaviour.gameObject.GetComponent<DataParser>().ChangeIconLocal();
    }

    public static void GetAtlas(string[] pictures, Action<Texture2D> Callback)
    {
        behaviour.StartCoroutine(GetAtlasCoroutine(ParseStringArrayToString(pictures), Callback));
    }

    public static IEnumerator GetAtlasCoroutine (string pictures, Action<Texture2D> Callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("pictures", pictures);

        WWW www = new WWW(URL + "/Tools/CreateAtlas", form);
        yield return www;

        if(!String.IsNullOrEmpty(www.error))
        {
            Debug.Log("Error: " + www.error);
            yield break;
        }

        JSONObject jsonObj = new JSONObject(www.text);
        if(jsonObj.HasField("error"))
        {
            Debug.Log("Error: " + jsonObj.GetField("error"));
            yield break;
        }

        if(www.texture)
        {
            Callback(www.texture);
        }

        Debug.Log("Error: No texture found!");
    }

    public static string UploadImage(Texture2D image, Action<string> Callback)
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

    public static void AuthenticateUser(Action<string, int, Authentication.LoginType> Callback, Authentication.LoginType loginType, string sessionKey, string profilePicture, string name, string email, string bio, string facebookUserToken, string googleUserToken, string facebookUser)
    {
        SpinnerController.instance.Show();
        behaviour.StartCoroutine(AuthenticateUserCoroutine(Callback, loginType, sessionKey, profilePicture, name, email, bio, facebookUserToken, googleUserToken, facebookUser));
    }

    private static IEnumerator AuthenticateUserCoroutine(Action<string, int, Authentication.LoginType> Callback, Authentication.LoginType loginType, string sessionKey, string profilePicture, string name, string email, string bio, string facebookUserToken, string googleUserToken, string facebookUser)
    {
        WWWForm form = new WWWForm();
        form.AddField("sessionToken", sessionKey);
        form.AddField("profilePicture", profilePicture);
        form.AddField("name", name);
        form.AddField("email", email);
        form.AddField("bio", bio);
        form.AddField("facebookUserToken", facebookUserToken);
        form.AddField("googleUserToken", googleUserToken);
        form.AddField("facebookUser", facebookUser);
        string platform = "Android";
#if UNITY_IOS
        platform = "IOS";
#endif
        form.AddField("platform", Application.platform.ToString());

        WWW www = new WWW(URL + "/User/Authenticate", form);

        yield return www;

        SpinnerController.instance.Hide();

        if (!String.IsNullOrEmpty(www.error))
        {
            FB.LogOut();
            Debug.Log("Error: " + www.error);
            yield break;
        }
        JSONObject jsonObj = new JSONObject(www.text);
        if(jsonObj.GetField("success"))
        {
            Debug.Log("Success! Index: " + jsonObj.GetField("index").ToString());
            Callback(sessionKey, int.Parse(jsonObj.GetField("index").ToString()), loginType);
        }
        else
        {
            Debug.Log("Error: " + jsonObj.GetField("error"));
            FB.LogOut();
        }
    }

    public static void GetUser(int id, Action<UserModel> Callback)
    {
        behaviour.StartCoroutine(GetUserCoroutine(id, Callback));
    }

    public static IEnumerator GetUserCoroutine(int id, Action<UserModel> Callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("index", id);
        WWW www = new WWW(URL + "/User/Find", form);

        yield return www;
        if(!String.IsNullOrEmpty(www.error))
        {
            Debug.Log("Failed: " + www.error);
        }
        else
        {
            JSONObject jsonObj = new JSONObject(www.text);
            if (jsonObj.GetField("error"))
            {
                Debug.Log("Error: " + jsonObj.GetField("error").ToString());
            }
            else
            {
                UserModel model = new UserModel(jsonObj);
                Callback(model);
                Debug.Log("Success: Got user info: " + model.name);
            }
        }
    }

    public static void GetUser(string sessionToken, Action<UserModel> Callback)
    {
        behaviour.StartCoroutine(GetUserCoroutine(sessionToken, Callback));
    }

    public static IEnumerator GetUserCoroutine(string sessionToken, Action<UserModel> Callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("sessionToken", sessionToken);
        WWW www = new WWW(URL + "/User/Find", form);
        
        yield return www;
        if (!String.IsNullOrEmpty(www.error))
        {
            Debug.Log("Failed: " + www.error);
        }
        else
        {
            JSONObject jsonObj = new JSONObject(www.text);
            if (jsonObj.GetField("error"))
            {
                Debug.Log("Error: " + jsonObj.GetField("error").ToString());
            }
            else
            {
                UserModel model = new UserModel(jsonObj);
                Callback(model);
                Debug.Log("Success: Got user info: " + model.name);
            }
        }
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

    public static void GetUsers(int[] users, string searchString, Action<UserModel[]> Callback)
    {
        behaviour.StartCoroutine(GetUsersCoroutine(users, searchString, Callback));
    }

    public static IEnumerator GetUsersCoroutine(int[] users, string searchString, Action<UserModel[]> Callback)
    {
        Debug.Log("Finding users with search:" + searchString + ".");
        WWWForm form = new WWWForm();
        form.AddField("index", ParseIntArrayToString(users));
        form.AddField("name", searchString);

        WWW www = new WWW(URL + "/User/FindUsers", form);

        yield return www;

        if(!String.IsNullOrEmpty(www.error))
        {
            Debug.Log("Error: " + www.error);
            yield break;
        }

        JSONObject jsonObj = new JSONObject(www.text);

        if(jsonObj.HasField("error"))
        {
            Debug.Log("Error: " + jsonObj.GetField("error"));
            Callback(new UserModel[0]);
            yield break;
        }

        List<UserModel> userModels = new List<UserModel>();
        for (int i = 0; i < jsonObj.list.Count; i++)
        {
            userModels.Add(new UserModel(jsonObj.list[i]));
        }
        Callback(userModels.ToArray());
    }

    public static void EditUser(UserModel newModel, string token, Action Callback)
    {
        behaviour.StartCoroutine(EditUserCoroutine(newModel, token, Callback));
    }

    public static IEnumerator EditUserCoroutine(UserModel newModel, string token, Action Callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("sessionToken", token);
        if(!String.IsNullOrEmpty(newModel.profilePicture))
            form.AddField("profilePicture", newModel.profilePicture);
        if (!String.IsNullOrEmpty(newModel.name))
            form.AddField("name", newModel.name);

        form.AddField("bio", newModel.bio);
        form.AddField("snapchatUser", newModel.snapchatUser);
        form.AddField("facebookUser", newModel.facebookUser);
        form.AddField("instagramUser", newModel.instagramUser);
        form.AddField("twitterUser", newModel.twitterUser);
        form.AddField("notifyLikesComments", newModel.notifyLikesComments.ToString());
        form.AddField("notifyFollowers", newModel.notifyFollowers.ToString());
        form.AddField("notifyFollowing", newModel.notifyFollowing.ToString());

        WWW www = new WWW(URL + "/User/Update", form);

        yield return www;

        if(!String.IsNullOrEmpty(www.error))
        {
            Debug.Log("Error: " + www.error);
            yield break;
        }

        JSONObject obj = new JSONObject(www.text);

        if(obj.GetField("error"))
        {
            Debug.Log("Error: " + obj.GetField("error"));
            yield break;
        }

        Debug.Log("Success: " + obj.GetField("success"));

        Callback();
    }

    public static void SearchUsers(string search, Action<UserModel[]> Callback)
    {
        behaviour.StartCoroutine(SearchUsersCoroutine(search, Callback));
    }

    public static IEnumerator SearchUsersCoroutine(string search, Action<UserModel[]> Callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", search);

        WWW www = new WWW(URL + "/User/FindUsers", form);

        yield return www;

        if(!String.IsNullOrEmpty(www.error))
        {
            Debug.Log("Error: " + www.error);
            yield break;
        }
        JSONObject jsonObj = new JSONObject(www.text);
        if(jsonObj.HasField("error"))
        {
            Debug.Log("Error: " + jsonObj.GetField("error"));
            Callback(null);
            yield break;
        }

        List<UserModel> userModels = new List<UserModel>();
        for (int i = 0; i < jsonObj.list.Count; i++)
        {
            userModels.Add(new UserModel(jsonObj.list[i]));
        }
        Callback(userModels.ToArray());
    }

    public static void SendReportUser(int userId, string reason, Action Callback)
    {
        behaviour.StartCoroutine(SendReportUserCoroutine(userId, reason, Callback));
    }

    public static IEnumerator SendReportUserCoroutine(int userId, string reason, Action Callback)
    {
        Debug.Log("Sending a report to user: " + userId + ", for: " + reason);
        WWWForm form = new WWWForm();
        form.AddField("index", userId);
        form.AddField("reason", reason);
        WWW www = new WWW(URL + "/User/Report", form);

        yield return www;

        if(!String.IsNullOrEmpty(www.error))
        {
            Debug.Log("Error: " + www.error);
            yield break;
        }

        JSONObject jsonObj = new JSONObject(www.text);
        if(jsonObj.HasField("error"))
        {
            Debug.Log("Error: " + jsonObj.GetField("error"));
            yield break;
        }

        Debug.Log("Success: " + jsonObj.GetField("success") + " and index is: " + jsonObj.GetField("index"));
        Callback();
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
        return new ProtestModel(0, "http://orig04.deviantart.net/a222/f/2013/016/9/0/128x128_px_mario_by_wildgica-d5rpb6y.jpg", "Our Portest Name", "Description", "426 NW 1st Ave Cape Coral, FL", "16.11.12.1.30.0", null, null, null, 0, 0, new int[2000], new int[2], new int[3], new int[5], Authentication.userIndex);
    }

    public static ProtestModel[] GetProtestList()
    {
        ProtestModel[] models = new ProtestModel[1];
        models[0] = new ProtestModel(0, "http://orig04.deviantart.net/a222/f/2013/016/9/0/128x128_px_mario_by_wildgica-d5rpb6y.jpg", "Our Portest Name", "Description", "426 NW 1st Ave Cape Coral, FL", "16.11.12.1.30.0", null, null, null, 0, 0, new int[20000], new int[3], new int[3], new int[3], Authentication.userIndex);
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
        models[0] = new ProtestModel(0, "http://orig04.deviantart.net/a222/f/2013/016/9/0/128x128_px_mario_by_wildgica-d5rpb6y.jpg", "Our Portest Name", "Description", "426 NW 1st Ave Cape Coral, FL", "16.11.12.1.30.0", null, null, null, 0, 0, new int[2000], new int[0], new int[3], new int[3], Authentication.userIndex);
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

    public static void Follow(int index, Action<bool> Callback)
    {
        behaviour.StartCoroutine(FollowCoroutine(index, Callback));
    }

    public static IEnumerator FollowCoroutine(int index, Action<bool> Callback)
    {
        Debug.Log("Following user: " + index);
        WWWForm form = new WWWForm();
        form.AddField("sessionToken", Authentication.auth_token);
        form.AddField("index", index.ToString());

        WWW www = new WWW(URL + "/User/Follow", form);

        yield return www;

        if(!String.IsNullOrEmpty(www.error))
        {
            Debug.Log("Error: " + www.error);
            yield break;
        }

        JSONObject jsonObj = new JSONObject(www.text);

        if(jsonObj.HasField("error"))
        {
            Debug.Log("Error: " + jsonObj.GetField("error"));
            yield break;
        }
        Debug.Log("Success: " + jsonObj.GetField("success"));
        Callback(!jsonObj.GetField("success").ToString().Contains("unfollowed"));
    }

    public static void GetNotifications(int[] index, Action<int> Callback)
    {
        behaviour.StartCoroutine(GetNotificationsCoroutine(index, Callback));
    }

    public static IEnumerator GetNotificationsCoroutine(int[] index, Action<int> Callback)
    {
        Debug.Log("Finding notifications count");
        WWWForm form = new WWWForm();
        form.AddField("index", ParseIntArrayToString(index));

        WWW www = new WWW(URL + "/Notifications/FindNotificationCount", form);

        yield return www;

        if (!String.IsNullOrEmpty(www.error))
        {
            Debug.Log("Error: " + www.error);
            yield break;
        }

        JSONObject jsonObj = new JSONObject(www.text);

        if (jsonObj.HasField("error"))
        {
            Debug.Log("Error: " + jsonObj.GetField("error"));
            Callback(0);
            yield break;
        }

        Debug.Log(www.text);

        Callback((int)jsonObj.GetField("index").n);
    }

    public static void GetNews(int[] index, string searchString, Action<NewsModel[]> Callback)
    {
        behaviour.StartCoroutine(GetNewsCoroutine(index, searchString, Callback));
    }

    public static IEnumerator GetNewsCoroutine(int[] index, string searchString, Action<NewsModel[]> Callback)
    {
        Debug.Log("Finding notifications with search:" + searchString + ".");
        WWWForm form = new WWWForm();
        form.AddField("index", ParseIntArrayToString(index));
        form.AddField("name", searchString);

        WWW www = new WWW(URL + "/Notifications/FindNotifications", form);

        yield return www;

        if (!String.IsNullOrEmpty(www.error))
        {
            Debug.Log("Error: " + www.error);
            yield break;
        }

        JSONObject jsonObj = new JSONObject(www.text);

        if (jsonObj.HasField("error"))
        {
            Debug.Log("Error: " + jsonObj.GetField("error"));
            Callback(new NewsModel[0]);
            yield break;
        }

        List<NewsModel> newsModels = new List<NewsModel>();
        for (int i = 0; i < jsonObj.list.Count; i++)
        {
            newsModels.Add(new NewsModel(jsonObj.list[i]));
        }
        Callback(newsModels.ToArray());
    }
}
