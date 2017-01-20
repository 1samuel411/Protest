using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ImageAndVideoPicker;
#if UNITY_ANDROID
using DeadMosquito.AndroidGoodies;
#endif
#if UNITY_IOS
using DeadMosquito.IosGoodies;
#endif
using System.Linq;
using Facebook.Unity;
using System.Text.RegularExpressions;
using System.Net.Mail;

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

    public const string TESTURL = "http://localhost:41264";
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

    void Awake()
    {
        behaviour = this;
    }

    public static bool IsValid(string emailaddress)
    {
        try
        {
            MailAddress m = new MailAddress(emailaddress);

            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    #region Distance
    public const double EarthRadiusInMiles = 3956.0;
    public const double EarthRadiusInKilometers = 6367.0;

    public static double ToRadian(double val) { return val * (Math.PI / 180); }
    public static double DiffRadian(double val1, double val2) { return ToRadian(val2) - ToRadian(val1); }

    public static double CalcDistance(double lat1, double lng1, double lat2, double lng2)
    {
        double radius = EarthRadiusInMiles;

        return radius * 2 * Math.Asin(Math.Min(1, Math.Sqrt((Math.Pow(Math.Sin((DiffRadian(lat1, lat2)) / 2.0), 2.0) + Math.Cos(ToRadian(lat1)) * Math.Cos(ToRadian(lat2)) * Math.Pow(Math.Sin((DiffRadian(lng1, lng2)) / 2.0), 2.0)))));
    }
    #endregion

    #region Picker
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
#if UNITY_EDITOR
        GetIconCallback(ListController.instance._atlas);
#endif
    }

    #endregion

    #region Parsers
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
        if (arrayToParse == null)
            return "";

        if (arrayToParse.Length <= 0)
            return "";

        string value = "";
        for (int i = 0; i < arrayToParse.Length; i++)
        {
            value += arrayToParse[i].ToString() + ((i >= arrayToParse.Length - 1) ? "" : ",");
        }
        return value;
    }

    public static string ParseStringArrayToString(string[] arrayToParse)
    {
        string value = "";
        if(arrayToParse == null)
            return "";
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
    #endregion

    #region Images
    public static void UploadImage(Texture2D image, Action<string> Callback)
    {
        behaviour.StartCoroutine(UploadImageCoroutine(image, Callback));
    }

    public static IEnumerator UploadImageCoroutine(Texture2D image, Action<string> Callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("sessionToken", Authentication.auth_token);
        byte[] bytes = image.EncodeToPNG();
        form.AddBinaryData("image", bytes);

        WWW www = new WWW(URL + "/Tools/UploadIcon", form);

        yield return www;

        if(!String.IsNullOrEmpty(www.error))
        {
            Debug.Log("Error: " + www.error);
            yield break;
        }

        JSONObject jsonObj = new JSONObject(www.text);
        if(jsonObj.HasField("error"))
        {
            Debug.Log("Error: " + jsonObj.GetField("error").str);
            yield break;
        }

        string url = jsonObj.GetField("url").str;
        Debug.Log("Uploaded image to the url: " + url);
        Callback(url);
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
    #endregion

    #region Authentication
    public static void AuthenticateUser(Action<string, int, Authentication.LoginType> Callback, Authentication.LoginType loginType, string sessionKey, string profilePicture, string name, string email, string bio, string facebookUserToken, string googleUserToken, string facebookUser)
    {
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
        form.AddField("platform", Application.platform.ToString());

        WWW www = new WWW(URL + "/User/Authenticate", form);

        yield return www;

        if (!String.IsNullOrEmpty(www.error))
        {
            FB.LogOut();
            Debug.Log("Error: " + www.error);
            LoadingController.instance._View.loading = false;
            yield break;
        }
        JSONObject jsonObj = new JSONObject(www.text);
        if(jsonObj.HasField("success"))
        {
            string successMessage = jsonObj.GetField("success").str;
            if(successMessage.Contains("created"))
            {
                Popup.Create("Welcome!", "Thank you for joining Protest, we ask that you please be kind and courteous towards others to ensure an optimal experience for everyone.", null, "Popup", "Okay");
            }
            Debug.Log("Success! Index: " + jsonObj.GetField("index").ToString());
            Callback(sessionKey, int.Parse(jsonObj.GetField("index").ToString()), loginType);
        }
        else
        {
            string error = jsonObj.GetField("error").str;
            if(error.Contains("banned"))
            {
                Popup.Create("Banned", "You are banned from Protest. If you believe this is an error please contact us for support.", null, "Popup", "Okay...");
            }
            Debug.Log("Error: " + error);
            FB.LogOut();
            LoadingController.instance._View.loading = false;
        }
    }
    #endregion

    #region User
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
        if(String.IsNullOrEmpty(newModel.name))
        {
            SpinnerController.instance.Hide();
            Popup.Create("Missing information", "A name must be entered", null);
            return;
        }
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

    #endregion

    #region Protests
    public static void CreateProtest(ProtestModel model, string token, Action<int> Callback)
    {
        behaviour.StartCoroutine(CreateProtestCoroutine(model, token, Callback));
    }

    public static IEnumerator CreateProtestCoroutine(ProtestModel model, string token, Action<int> Callback)
    {
        if (String.IsNullOrEmpty(model.name.Trim()))
        {
            SpinnerController.instance.Hide();
            Popup.Create("Invalid", "There must be a name filled out!", null);
            yield break;
        }
        if (String.IsNullOrEmpty(model.location.Trim()) || model.x == 0 || model.y == 0)
        { 
            SpinnerController.instance.Hide();
            Popup.Create("Invalid", "There must be a location filled out!", null);
            yield break;
        }
        if (String.IsNullOrEmpty(model.date.Trim()))
        {
            SpinnerController.instance.Hide();
            Popup.Create("Invalid", "There must be a date filled out!", null);
            yield break;
        }
        if (!String.IsNullOrEmpty(model.donationsEmail.Trim()))
        {
            if (!IsValid(model.donationsEmail))
            {
                SpinnerController.instance.Hide();
                Popup.Create("Invalid", "Email is not entered correctly, double check because this is where funds will be sent!", null);
                yield break;
            }
            if (model.donationTarget <= 0)
            {
                SpinnerController.instance.Hide();
                Popup.Create("Invalid", "A donation target must be set if you entered an email", null);
                yield break;
            }

        }

        if(String.IsNullOrEmpty(model.protestPicture))
        {
            model.protestPicture = Authentication.userModel.profilePicture;
        }
        Debug.Log("Creating Protest: " + model.name);
        WWWForm form = new WWWForm();
        form.AddField("protestPicture", model.protestPicture);
        form.AddField("name", model.name);
        form.AddField("description", model.description);
        form.AddField("location", model.location);
        form.AddField("latitude", model.x.ToString());
        form.AddField("longitude", model.y.ToString());
        form.AddField("date", model.date);
        form.AddField("donationsEmail", model.donationsEmail);
        form.AddField("donationsTarget", model.donationTarget.ToString());
        form.AddField("sessionToken", token);
        WWW www = new WWW(URL + "/Protest/Create", form);

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
            SpinnerController.instance.Hide();
            yield break;
        }

        Callback((int)jsonObj.GetField("index").n);
    }

    public static void EditProtest(ProtestModel newModel, string token, Action<int> Callback)
    {
        behaviour.StartCoroutine(EditProtestCoroutine(newModel, token, Callback));
    }

    public static IEnumerator EditProtestCoroutine(ProtestModel model, string token, Action<int> Callback)
    {
        if (String.IsNullOrEmpty(model.name.Trim()))
        {
            SpinnerController.instance.Hide();
            Popup.Create("Invalid", "There must be a name filled out!", null);
            yield break;
        }
        if (String.IsNullOrEmpty(model.location.Trim()) || model.x == 0 || model.y == 0)
        {
            SpinnerController.instance.Hide();
            Popup.Create("Invalid", "There must be a location filled out!", null);
            yield break;
        }
        if (String.IsNullOrEmpty(model.date.Trim()))
        {
            SpinnerController.instance.Hide();
            Popup.Create("Invalid", "There must be a date filled out!", null);
            yield break;
        }
        if (!String.IsNullOrEmpty(model.donationsEmail.Trim()))
        {
            if (!IsValid(model.donationsEmail))
            {
                SpinnerController.instance.Hide();
                Popup.Create("Invalid", "Email is not entered correctly, double check because this is where funds will be sent!", null);
                yield break;
            }
            if (model.donationTarget <= 0)
            {
                SpinnerController.instance.Hide();
                Popup.Create("Invalid", "A donation target must be set if you entered an email", null);
                yield break;
            }

        }

        if (String.IsNullOrEmpty(model.protestPicture))
        {
            model.protestPicture = Authentication.userModel.profilePicture;
        }
        Debug.Log("Updating Protest: " + model.name);
        WWWForm form = new WWWForm();
        form.AddField("protestPicture", model.protestPicture);
        form.AddField("index", model.index);
        form.AddField("name", model.name);
        form.AddField("description", model.description);
        form.AddField("location", model.location);
        form.AddField("latitude", model.x.ToString());
        form.AddField("longitude", model.y.ToString());
        form.AddField("date", model.date);
        form.AddField("donationsEmail", model.donationsEmail);
        form.AddField("donationsTarget", model.donationTarget.ToString());
        form.AddField("sessionToken", token);
        WWW www = new WWW(URL + "/Protest/Update", form);

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
            SpinnerController.instance.Hide();
            yield break;
        }

        Callback((int)jsonObj.GetField("index").n);
    }

    public static void SendReportProtest(int protestId, string reason, Action Callback)
    {
        behaviour.StartCoroutine(SendReportProtestCoroutine(protestId, reason, Callback));
    }

    public static IEnumerator SendReportProtestCoroutine(int protestId, string reason, Action Callback)
    {
        Debug.Log("Sending a report to Protest: " + protestId + ", for: " + reason);
        WWWForm form = new WWWForm();
        form.AddField("index", protestId);
        form.AddField("reason", reason);
        WWW www = new WWW(URL + "/Protest/Report", form);

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
            yield break;
        }

        Debug.Log("Success: " + jsonObj.GetField("success") + " and index is: " + jsonObj.GetField("index"));
        Callback();
    }

    public static void GetProtest(int protest, Action<ProtestModel> Callback)
    {
        behaviour.StartCoroutine(GetProtestCoroutine(protest, Callback));
    }

    public static IEnumerator GetProtestCoroutine(int id, Action<ProtestModel> Callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("index", id);
        WWW www = new WWW(URL + "/Protest/Find", form);

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
                ProtestModel model = new ProtestModel(jsonObj);
                Callback(model);
                Debug.Log("Success: Got protest info: " + model.name);
            }
        }
    }

    public static void GetProtestIcon(int id, Action<string> Callback)
    {
        behaviour.StartCoroutine(GetProtestIconCoroutine(id, Callback));
    }
    
    public static IEnumerator GetProtestIconCoroutine(int id, Action<string> Callback)
    {
        yield return new WaitForSeconds(1);
        Callback("https://pbs.twimg.com/media/Coz8twTWcAADpJQ.png");
    }

    public static void GetProtests(int[] protests, float lat, float lng, string searchString, Action<ProtestModel[]> Callback)
    {
        behaviour.StartCoroutine(GetProtestsCoroutine(protests, lat, lng, searchString, Callback));
    }

    public static IEnumerator GetProtestsCoroutine(int[] protests, float lat, float lng, string searchString, Action<ProtestModel[]> Callback)
    {
        Debug.Log("Finding protests with search: " + searchString + ".");
        WWWForm form = new WWWForm();
        form.AddField("index", ParseIntArrayToString(protests));
        form.AddField("name", searchString);
        form.AddField("latitude", lat.ToString());
        form.AddField("longitude", lng.ToString());

        WWW www = new WWW(URL + "/Protest/FindProtests", form);

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
            Callback(null);
            yield break;
        }

        List<ProtestModel> protestModels = new List<ProtestModel>();
        for (int i = 0; i < jsonObj.list.Count; i++)
        {
            protestModels.Add(new ProtestModel(jsonObj.list[i]));
        }
        Callback(protestModels.ToArray());
    }

    public static void DeleteProtest(int index)
    {
        behaviour.StartCoroutine(DeleteProtestCoroutine(index));
    }
    public static IEnumerator DeleteProtestCoroutine(int index)
    {
        Debug.Log("Deleting Protest: " + index + ".");
        WWWForm form = new WWWForm();
        form.AddField("index", index);
        form.AddField("sessionToken", Authentication.auth_token);

        WWW www = new WWW(URL + "/Protest/Delete", form);

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
            yield break;
        }
    }
    #endregion

    #region Contributions

    public static void AddContribution(int model, Action Callback)
    {
        behaviour.StartCoroutine(AddContributionCoroutine(model, Callback));
    }

    private static IEnumerator AddContributionCoroutine(int model, Action Callback)
    {
        Debug.Log("Adding Contributions: " + model);
        WWWForm form = new WWWForm();
        form.AddField("index", model);
        form.AddField("sessionToken", Authentication.auth_token);
        WWW www = new WWW(URL + "/Contributions/Add", form);

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
            SpinnerController.instance.Hide();
            yield break;
        }
        Callback();
    }

    public static void CreateContribution(ContributionsModel model, Action<ContributionsModel> Callback)
    {
        behaviour.StartCoroutine(CreateContributionCoroutine(model, Callback));
    }

    private static IEnumerator CreateContributionCoroutine(ContributionsModel model, Action<ContributionsModel> Callback)
    {
        if (String.IsNullOrEmpty(model.name.Trim()))
        {
            SpinnerController.instance.Hide();
            Popup.Create("Invalid", "There must be a name filled out!", null);
            yield break;
        }
        if (model.amountNeeded <= 0)
        {
            SpinnerController.instance.Hide();
            Popup.Create("Invalid", "There must be an amount filled out!", null);
            yield break;
        }

        Debug.Log("Creating Contributions: " + model.name + ", " + model.amountNeeded + ", " + model.protest);
        WWWForm form = new WWWForm();
        form.AddField("name", model.name);
        form.AddField("amountNeeded", model.amountNeeded);
        form.AddField("protest", model.protest);
        form.AddField("sessionToken", Authentication.auth_token);
        WWW www = new WWW(URL + "/Contributions/Create", form);

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
            SpinnerController.instance.Hide();
            yield break;
        }
        model.index = (int)jsonObj.GetField("index").n;
        Callback(model);
    }

    public static void DeleteContribution(int index, Action<int> Callback)
    {
        behaviour.StartCoroutine(DeleteContributionCoroutine(index, Callback));
    }

    private static IEnumerator DeleteContributionCoroutine(int index, Action<int> Callback)
    {
        Debug.Log("Deleting Contributions: " + index);
        WWWForm form = new WWWForm();
        form.AddField("index", index);
        form.AddField("sessionToken", Authentication.auth_token);
        WWW www = new WWW(URL + "/Contributions/Delete", form);

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
            SpinnerController.instance.Hide();
            yield break;
        }

        Callback(index);
    }

    public static void GetContributions(int[] contributions, Action<ContributionsModel[]> Callback)
    {
        behaviour.StartCoroutine(GetContributionsCoroutine(contributions, Callback));
    }

    private static IEnumerator GetContributionsCoroutine(int[] indexes, Action<ContributionsModel[]> Callback)
    {
        Debug.Log("Gettings Contributions: " + indexes);
        WWWForm form = new WWWForm();
        form.AddField("index", DataParser.ParseIntArrayToString(indexes));
        WWW www = new WWW(URL + "/Contributions/Find", form);

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
            SpinnerController.instance.Hide();
            yield break;
        }

        List<ContributionsModel> models = new List<ContributionsModel>();
        for (int i = 0; i < jsonObj.list.Count; i++)
        {
            models.Add(new ContributionsModel(jsonObj.list[i]));
        }
        Callback(models.ToArray());
    }
    #endregion

    #region Chats
    public static void GetChats(int protest, Action<ChatModel[]> Callback)
    {
        behaviour.StartCoroutine(GetChatsCoroutine(protest, Callback));
    }

    private static IEnumerator GetChatsCoroutine(int protest, Action<ChatModel[]> Callback)
    {
        Debug.Log("Getting chats for protest: " + protest);
        WWWForm form = new WWWForm();
        form.AddField("protest", protest);

        WWW www = new WWW(URL + "/Chat/Find", form);

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
            yield break;
        }

        List<ChatModel> chatModels = new List<ChatModel>();
        for (int i = 0; i < jsonObj.list.Count; i++)
        {
            chatModels.Add(new ChatModel(jsonObj.list[i]));
        }

        Callback(chatModels.ToArray());
    }

    public static void SendChat(int protest, string body, Action Callback)
    {
        behaviour.StartCoroutine(SendChatCoroutine(protest, body, Callback));
    }

    private static IEnumerator SendChatCoroutine(int protest, string body, Action Callback)
    {
        Debug.Log("Sending chat: " + body);
        WWWForm form = new WWWForm();
        form.AddField("sessionToken", Authentication.auth_token);
        form.AddField("body", body);
        form.AddField("protest", protest);

        WWW www = new WWW(URL + "/Chat/Create", form);

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
            yield break;
        }

        Callback();
    }
    #endregion

    #region Likes and going
    public static void LikeProtest(int protest, Action<bool> Callback)
    {
        behaviour.StartCoroutine(LikeProtestCoroutine(protest, Callback));
    }

    public static IEnumerator LikeProtestCoroutine(int index, Action<bool> Callback)
    {
        Debug.Log("Liking Protest: " + index);
        WWWForm form = new WWWForm();
        form.AddField("sessionToken", Authentication.auth_token);
        form.AddField("index", index.ToString());

        WWW www = new WWW(URL + "/Protest/Like", form);

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
            yield break;
        }
        Debug.Log("Success: " + jsonObj.GetField("success"));
        Callback(!jsonObj.GetField("success").ToString().Contains("unliked"));
    }

    public static void GoingProtest(int protest, Action<bool> Callback)
    {
        behaviour.StartCoroutine(GoingProtestCoroutine(protest, Callback));
    }

    public static IEnumerator GoingProtestCoroutine(int index, Action<bool> Callback)
    {
        Debug.Log("Going to Protest: " + index);
        WWWForm form = new WWWForm();
        form.AddField("sessionToken", Authentication.auth_token);
        form.AddField("index", index.ToString());

        WWW www = new WWW(URL + "/Protest/Going", form);

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
            yield break;
        }
        Debug.Log("Success: " + jsonObj.GetField("success"));
        Callback(!jsonObj.GetField("success").ToString().Contains("unwent"));
    }
    #endregion

    #region Follow User
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
    #endregion

    #region Notifications and news
    public static void GetNotifications(int[] index, int[] protests, Action<int> Callback)
    {
        behaviour.StartCoroutine(GetNotificationsCoroutine(index, protests, Callback));
    }

    public static IEnumerator GetNotificationsCoroutine(int[] index, int[] protests, Action<int> Callback)
    {
        Debug.Log("Finding notifications count");
        WWWForm form = new WWWForm();
        form.AddField("index", ParseIntArrayToString(index));
        form.AddField("protestIndex", ParseIntArrayToString(protests));

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

    public static void GetNews(int[] index, int[] protests, string searchString, Action<NewsModel[]> Callback)
    {
        behaviour.StartCoroutine(GetNewsCoroutine(index, protests, searchString, Callback));
    }

    public static IEnumerator GetNewsCoroutine(int[] index, int[] protests, string searchString, Action<NewsModel[]> Callback)
    {
        Debug.Log("Finding notifications with search:" + searchString + ".");
        WWWForm form = new WWWForm();
        form.AddField("index", ParseIntArrayToString(index));
        form.AddField("protestIndex", ParseIntArrayToString(protests));
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
    #endregion

    #region Locations
    public static void GetLocation(string location, Action <string, float, float> Callback)
    {
        behaviour.StartCoroutine(GetLocationCoroutine(location, Callback));
    }

    public static IEnumerator GetLocationCoroutine(string location, Action<string, float, float> Callback)
    {
        Debug.Log("Finding locations with search:" + location + ".");
        WWWForm form = new WWWForm();
        form.AddField("location", location);

        WWW www = new WWW(URL + "/Location/GetCoords", form);

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
            SpinnerController.instance.Hide();
            Popup.Create("Unknown", "The address or location entered was unable to be found, please try again, perhaps be more specific?", null, "Popup", "Okay");
            yield break;
        }

        Callback(jsonObj.GetField("address").str, jsonObj.GetField("x").f, jsonObj.GetField("y").f);
    }
    #endregion
}
