using UnityEngine;
using DeadMosquito.IosGoodies;

namespace DeadMosquito.IosGoodies.Example
{
    public class IGShareExample : MonoBehaviour
    {
        public Texture2D image;

        #if UNITY_IOS
        public void OnShareTextWithImage()
        {
            IGShare.Share(() =>
            Debug.Log("DONE sharing"), "iOS Goodies is a really cool #unity3d plugin!", image);
        }

        public void OnSendSms()
        {
            IGShare.SendSmsViaDefaultApp("123456789", "My message!");
        }

        public void OnSendSmsEmbedded()
        {
            IGShare.SendSmsViaController("123456789", "Hello worksadk wa dwad !!!", () => Debug.Log("Success"),
                () => Debug.Log("Cancel"), () => Debug.Log("Failure"));
        }

        public void OnSendEmail()
        {
            var recipients = new[] {"x@gmail.com", "hello@gmail.com"};
            var ccRecipients = new[] {"cc@gmail.com"};
            var bccRecipients = new[] {"bcc@gmail.com", "bcc-guys@gmail.com"};
            IGShare.SendEmail(recipients, "The Subject", "My email message!\nHello!", ccRecipients, bccRecipients);
        }
        #endif
    }
}
