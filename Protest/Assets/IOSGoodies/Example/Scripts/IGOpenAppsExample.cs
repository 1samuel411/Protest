using DeadMosquito.IosGoodies;
using UnityEngine;

namespace DeadMosquito.IosGoodies.Example
{
    public class IGOpenAppsExample : MonoBehaviour
    {
        #if UNITY_IOS
        public void OnOpenYouTubeVideo()
        {
            const string videoId = "rZ2csdtP440";
            IGApps.OpenYoutubeVideo(videoId);
        }

        public void OnFaceTimeVideoCall()
        {
            IGApps.StartFaceTimeVideoCall("user@example.com");
        }

        public void OnFaceTimeAudioCall()
        {
            IGApps.StartFaceTimeAudioCall("user@example.com");
        }
        #endif
    }
}