using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using System.Linq;
using System;
#if UNITY_ANDROID
using DeadMosquito.AndroidGoodies;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif

using Firebase.Auth;
using Firebase;

public class LoadingController : Controller
{

    public new static LoadingController instance;

    public LoadingView _View;

    void Awake()
    {
        Screen.fullScreen = false;
#if UNITY_EDITOR
        PlayerSettings.statusBarHidden = false;
#endif
        instance = this;
        _View = view.GetComponent<LoadingView>();

        OneSignal.StartInit("a7227cd5-532a-4b87-9dd0-080dfda19ab2")
        .HandleNotificationOpened(HandleNotificationOpened)
        .EndInit();

        OneSignal.SetSubscription(true);
    }

    // Gets called when the player opens the notification.
    private static void HandleNotificationOpened(OSNotificationOpenedResult result)
    {
        if (Authentication.authenticated == false)
            return;
        Debug.Log("Opened a notification!");
        openNotifications = true;
    }

    void Update()
    {
        if(Authentication.authenticated)
        {
            if(openNotifications)
            {
                openNotifications = false;
                ListController.instance.Show(ListController.ShowType.news, Authentication.userModel, ProtestListController.instance);
            }
        }
    }

    public static bool openNotifications;

    public void LoginFacebook()
    {
        Authentication.Login_Facebook(CallbackFacebook);
    }

    public void LoginGoogle()
    {
        //Authentication.Login_Google(CallbackGoogle);
        Popup.Create("Not supported", "Google login is still being developed and will be released soon!", null, "Popup", "Okay");
    }

    private void CallbackGoogle()
    {
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            string name = user.DisplayName;
            string email = user.Email;
            string picture = user.PhotoUrl.AbsolutePath;
            // The user's ID, unique to the Firebase project.
            // Do NOT use this value to authenticate with your backend server,
            // if you have one. Use User.Token() instead.
            string uid = user.UserId;
            string refreshToken = user.RefreshToken;

            Debug.Log("name: " + name + ", email: " + email + ", picture: " + picture + ", uid: " + uid + ", refreshToken: " + refreshToken);
        }
    }

    private void CallbackFacebook(ILoginResult result)
    {
        if (result.Cancelled || result.Error != null)
        {
            Debug.Log("Canceled or Error: " + result.Error);
        }
        else
        {
            if(FB.IsLoggedIn)
            {
                Debug.Log("Getting Data from FB");
                FB.API("/me?fields=name,email", HttpMethod.GET, CallbackFacebookInfo);
            }
        }
    }

    private void CallbackFacebookInfo(IGraphResult result)
    {
        if(!string.IsNullOrEmpty(result.Error))
        {
            Debug.Log("Failed: " + result.Error);
            return;
        }

        string email = "";
        string nameUser = result.ResultDictionary["name"].ToString();
        if(result.ResultDictionary.Keys.Contains("email"))
            email = result.ResultDictionary["email"].ToString();
        string bio = ""; 
        Authentication.Authenticate(AccessToken.CurrentAccessToken.UserId, AccessToken.CurrentAccessToken.TokenString, Authentication.LoginType.Facebook, "https://graph.facebook.com/" + AccessToken.CurrentAccessToken.UserId + "/picture?width=128&height=128", nameUser, email, bio, AccessToken.CurrentAccessToken.UserId, "", "");
    }

    public void Load()
    {
        Authentication.RefreshUserModel(null);
        _View.loading = true;
        GetPermissions();
    }

    void GetPermissions()
    {
#if UNITY_EDITOR
        GetLocation();
        return;
#endif
#if UNITY_IOS
        GetLocation();
        return;
#endif
        // Don't forget to also add the permissions you need to manifest!
        var permissions = new[]
        {
            AGPermissions.READ_EXTERNAL_STORAGE,
            AGPermissions.ACCESS_FINE_LOCATION,
            AGPermissions.READ_CALENDAR
        };

        // Filter permissions so we don't request already granted permissions,
        // otherwise if the user denies already granted permission the app will be killed
        var nonGrantedPermissions = permissions.ToList().Where(x => !AGPermissions.IsPermissionGranted(x)).ToArray();

        if (nonGrantedPermissions.Length == 0)
        {
            Debug.Log("User already granted all these permissions: " + string.Join(",", permissions));
            GetLocation();
            return;
        }

        // Finally request permissions user has not granted yet and log the results
        AGPermissions.RequestPermissions(permissions, results =>
        {
            // Process results of requested permissions
            foreach (var result in results)
            {
                Debug.Log(string.Format("Permission [{0}] is [{1}], should show explanation?: {2}",
                    result.Permission, result.Status, result.ShouldShowRequestPermissionRationale));
                if (result.Status == AGPermissions.PermissionStatus.Denied)
                {
                    // User denied permission, now we need to find out if he clicked "Do not show again" checkbox
                    if (result.ShouldShowRequestPermissionRationale)
                    {
                        PermissionCanceled();
                    }
                    else
                    {
                        PermissionCanceled(false);
                    }
                    return;
                }
            }
        });
    }

    private bool showAgain = true;
    void PermissionCanceled(bool showAgain = true)
    {
        this.showAgain = showAgain;
        Popup.Create("Required", "We require these permissions for Protest to function.\nPlease read our privacy statement for more information.", ResponsePermissionCanceled, "Popup", "Okay");
    }

    void ResponsePermissionCanceled(int response)
    {
        if (this.showAgain == false)
        {
            Authentication.Logout();
            AGSettings.OpenApplicationDetailsSettings("com.ProtestChange.Protest");
        }
        else
            GetPermissions();
    }

    void GetLocation()
    {
        Debug.Log("Getting coordinates!");
#if UNITY_ANDROID
        GetLocationAndroid();
#endif
#if UNITY_IOS
        StartCoroutine(GetLocationIOS());
#endif
#if UNITY_EDITOR
        Authentication.location.x = 0;
        Authentication.location.y = 0;
        ProtestListController.instance.Load(Authentication.location.x, Authentication.location.y, LoadCallback);
#endif
    }

    IEnumerator GetLocationIOS()
    {
        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            print("Timed out");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("Unable to determine device location");
            yield break;
        }
        else
        {
            // Access granted and location value could be retrieveds
            Debug.Log("Our location is: " + Input.location.lastData.latitude + ", " + Input.location.lastData.longitude);
            Authentication.location.x = (float)Input.location.lastData.latitude;
            Authentication.location.y = (float)Input.location.lastData.longitude;
            ProtestListController.instance.Load(Authentication.location.x, Authentication.location.y, LoadCallback);
        }

        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();
    }

#if UNITY_ANDROID
    public void GetLocationAndroid()
    {
        if(AGGPS.DeviceHasGPS() == false || AGGPS.GetLastKnownGPSLocation() == null)
        {
            Popup.Create("GPS Error", "The GPS function is either missing or disabled and no previous location could be found. Setting location to 0, 0, please enable and restart the app with the GPS enabled to get the closest Protests.", null, "Popup", "Okay");
            return;
        }
        GetLocationAndroidFinal(AGGPS.GetLastKnownGPSLocation());
    }

    private void GetLocationAndroidFinal(AGGPS.Location location)
    {
        Debug.Log("Our location is: " + location.Latitude + ", " + location.Longitude);
        Authentication.location.x = (float)location.Latitude;
        Authentication.location.y = (float)location.Longitude;
        ProtestListController.instance.Load(Authentication.location.x, Authentication.location.y, LoadCallback);
        AGGPS.RemoveUpdates();
    }
#endif

    public void LoadCallback(int response)
    {
        if(response == 0)
        {
            Hide();
        }
    }
}