using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;

public class InAppBrowser : System.Object {

	public struct DisplayOptions {
		public string pageTitle;
		public string backButtonText;
		public string barBackgroundColor;
		public string textColor;
		[MarshalAs(UnmanagedType.U1)]
		public bool displayURLAsPageTitle;
		[MarshalAs(UnmanagedType.U1)]
		public bool hidesTopBar;
		[MarshalAs(UnmanagedType.U1)]
		public bool pinchAndZoomEnabled;
		[MarshalAs(UnmanagedType.U1)]
		public bool androidBackButtonCustomBehaviour;
	}

	private static DisplayOptions CreateDefaultOptions() {
		DisplayOptions displayOptions = new DisplayOptions();
		displayOptions.displayURLAsPageTitle = true;
		return displayOptions;
	}

	private static string PathCombine(string path1, string path2)
	{
		if (Path.IsPathRooted(path2))
		{
			path2 = path2.TrimStart(Path.DirectorySeparatorChar);
			path2 = path2.TrimStart(Path.AltDirectorySeparatorChar);
		}

		return Path.Combine(path1, path2);
	}

	public static void OpenURL(string URL) {
		OpenURL(URL, CreateDefaultOptions());
	}

	public static void OpenLocalFile(string fileName) {
		OpenLocalFile(fileName, CreateDefaultOptions());
	}

	public static void LoadHTML(string HTML) {
		LoadHTML(HTML, CreateDefaultOptions() );
	}

	public static void OpenURL(string URL, DisplayOptions displayOptions) {
		#if UNITY_IOS && !UNITY_EDITOR
			iOSInAppBrowser.OpenURL(URL, displayOptions);
		#elif UNITY_ANDROID && !UNITY_EDITOR
			AndroidInAppBrowser.OpenURL(URL, displayOptions); 
		#endif
	}

	public static void OpenLocalFile(string filePath, DisplayOptions displayOptions) {
		#if UNITY_IOS && !UNITY_EDITOR
			var path = InAppBrowser.PathCombine(Application.streamingAssetsPath, filePath);
			iOSInAppBrowser.OpenURL(path, displayOptions);
		#elif UNITY_ANDROID && !UNITY_EDITOR
			AndroidInAppBrowser.OpenURL(filePath, displayOptions); 
		#endif
	}

	public static void LoadHTML(string HTML, DisplayOptions options) {
		#if UNITY_IOS && !UNITY_EDITOR 
			iOSInAppBrowser.LoadHTML(HTML, options);
		#elif UNITY_ANDROID && !UNITY_EDITOR
			AndroidInAppBrowser.LoadHTML(HTML, options);
		#endif
	}

	public static void CloseBrowser() {
		#if UNITY_IOS && !UNITY_EDITOR 
			iOSInAppBrowser.CloseBrowser();
		#elif UNITY_ANDROID && !UNITY_EDITOR
			AndroidInAppBrowser.CloseBrowser();
		#endif
	}

	public static void ExecuteJS(string JSCommand) {
		#if UNITY_IOS && !UNITY_EDITOR 
			iOSInAppBrowser.ExecuteJS(JSCommand);
		#elif UNITY_ANDROID && !UNITY_EDITOR
			AndroidInAppBrowser.ExecuteJS(JSCommand);
		#endif
	}

	public static void ClearCache() {
		#if UNITY_IOS && !UNITY_EDITOR 
			iOSInAppBrowser.ClearCache();
		#elif UNITY_ANDROID && !UNITY_EDITOR
			AndroidInAppBrowser.ClearCache();
		#endif
	}

	#if UNITY_ANDROID && !UNITY_EDITOR
	private class AndroidInAppBrowser {
		public static void OpenURL(string URL, DisplayOptions displayOptions) {
			var currentActivity = GetCurrentUnityActivity();
			GetInAppBrowserHelper().CallStatic("OpenInAppBrowser", currentActivity, URL, CreateJavaDisplayOptions(displayOptions));                                 
		}

		public static void LoadHTML(string HTML, DisplayOptions displayOptions) {
			var currentActivity = GetCurrentUnityActivity();
			GetInAppBrowserHelper().CallStatic("LoadHTML", currentActivity, HTML, CreateJavaDisplayOptions(displayOptions));                                 
		}

		public static void CloseBrowser() {
			var currentActivity = GetCurrentUnityActivity();
			GetInAppBrowserHelper().CallStatic("CloseInAppBrowser", currentActivity);
		}

		public static void ExecuteJS(string JSCommand) {
			var currentActivity = GetCurrentUnityActivity();
			GetInAppBrowserHelper().CallStatic("SendJSMessage", currentActivity, JSCommand);      
		}

		public static void ClearCache() {
			var currentActivity = GetCurrentUnityActivity();
			GetInAppBrowserHelper().CallStatic("ClearCache", currentActivity);
		}

		private static AndroidJavaObject GetCurrentUnityActivity() {
			var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			return currentActivity;
		}

		private static AndroidJavaObject GetInAppBrowserHelper() {
			var helper = new AndroidJavaClass("inappbrowser.kokosoft.com.Helper");
			return helper;
		}

		private static AndroidJavaObject CreateJavaDisplayOptions(DisplayOptions displayOptions) {
			var ajc = new AndroidJavaObject("inappbrowser.kokosoft.com.DisplayOptions");
			if (displayOptions.pageTitle != null) {
				ajc.Set<string>("pageTitle", displayOptions.pageTitle);
			}

			if (displayOptions.backButtonText != null) {
				ajc.Set<string>("backButtonText", displayOptions.backButtonText);
			}

			if (displayOptions.barBackgroundColor != null) {
				ajc.Set<string>("barBackgroundColor", displayOptions.barBackgroundColor);
			}

			if (displayOptions.textColor != null) {
				ajc.Set<string>("textColor", displayOptions.textColor);
			}

			ajc.Set<bool>("displayURLAsPageTitle", displayOptions.displayURLAsPageTitle);
			ajc.Set<bool>("hidesTopBar", displayOptions.hidesTopBar);
			ajc.Set<bool>("pinchAndZoomEnabled", displayOptions.pinchAndZoomEnabled);
			ajc.Set<bool>("androidBackButtonCustomBehaviour", displayOptions.androidBackButtonCustomBehaviour);
			return ajc;
		}

	}
	#endif

	#if UNITY_IPHONE && !UNITY_EDITOR
	private class iOSInAppBrowser {

		[DllImport ("__Internal")]
		private static extern void _OpenInAppBrowser(string URL, DisplayOptions displayOptions);

		[DllImport ("__Internal")]
		private static extern void _LoadHTML(string HTML, DisplayOptions displayOptions);

		[DllImport ("__Internal")]
		private static extern void _CloseInAppBrowser();

		[DllImport ("__Internal")]
		private static extern void _SendJSMessage(string message);

		[DllImport ("__Internal")]
		private static extern void _ClearCache();

		public static void OpenURL(string URL, DisplayOptions displayOptions) {
			_OpenInAppBrowser(URL, displayOptions);
		}

		public static void LoadHTML(string HTML, DisplayOptions displayOptions) {
			_LoadHTML(HTML, displayOptions);
		}

		public static void CloseBrowser() {
			_CloseInAppBrowser();
		}

		public static void ExecuteJS(string JSCommand) {
			_SendJSMessage(JSCommand);
		}

		public static void ClearCache() {
			_ClearCache();
		}
	}
	#endif
}
