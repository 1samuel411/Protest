#if UNITY_IOS
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using DeadMosquito.IosGoodies.Internal;

namespace DeadMosquito.IosGoodies
{
    public static class IGShare
    {
        private static readonly byte[] EmptyByteArray = { };

        /// <summary>
        /// Share the specified text with optional image.
        /// </summary>
        /// <param name="onFinished">Callback invoked when sharing finished.</param>
        /// <param name="text">Text to share.</param>
        /// <param name="image">Image to share.</param>
        public static void Share(Action onFinished, string text, Texture2D image = null)
        {
            if (IGUtils.IsIosCheck())
            {
                return;
            }

            if (onFinished == null)
            {
                throw new ArgumentNullException("onFinished", "Callback cannot be null");
            }

            if (image != null)
            {
                var imageBuffer = image.EncodeToPNG();
                GCHandle handle = GCHandle.Alloc(imageBuffer, GCHandleType.Pinned);
                _showShareMessageWithImage(text, handle.AddrOfPinnedObject(), imageBuffer.Length,
                    IGUtils.ActionVoidCallback, onFinished.GetPointer());
                handle.Free();
                return;
            }

            // Just text
            _showShareMessageWithImage(text, EmptyByteArray.GetPointer(), EmptyByteArray.Length,
                IGUtils.ActionVoidCallback, onFinished.GetPointer());
        }

        /// <summary>
        /// Opens the sms app with provided phone number.
        /// 
        /// You can specify message body with this method but its not documented and NOT guaranteed to work.
        /// </summary>
        /// <param name="phoneNumber">Phone number.</param>
        /// <param name="body">Message body. NOT GUARANTEED TO WORK!</param>
        public static void SendSmsViaDefaultApp(string phoneNumber, string body = null)
        {
            var url = string.Format("sms:{0}", phoneNumber);
            if (!string.IsNullOrEmpty(body))
            {
                url += string.Format("&body={0}", body.Escape());
            }
            IGUtils._openUrl(url);
        }

        public static void SendSmsViaController(string phoneNumber, string body,
                                                Action onSuccess, 
                                                Action onCancel, 
                                                Action onFailure)
        {
            if (IGUtils.IsIosCheck())
            {
                return;
            }

            _sendSms(phoneNumber, body, IGUtils.ActionVoidCallback,
                onSuccess.GetPointer(), onCancel.GetPointer(), onFailure.GetPointer());
        }

        /// <summary>
        /// Sends the email using default mail app.
        /// </summary>
        /// <param name="recipients">Recipient email addresses.</param>
        /// <param name="subject">Subject of email.</param>
        /// <param name="body">Body of email.</param>
        /// <param name="cc">Cc recipients. Cc stands for "carbon copy." 
        /// Anyone you add to the cc: field of a message receives a copy of that message when you send it. 
        /// All other recipients of that message can see that person you designated as a cc
        /// </param>
        /// <param name="bcc">Bcc recipients. Bcc stands for "blind carbon copy." 
        /// Anyone you add to the bcc: field of a message receives a copy of that message when you send it. 
        /// But, bcc: recipients are invisible to all the other recipients of the message including other bcc: recipients.
        /// </param>
        public static void SendEmail(string[] recipients, string subject, string body, string[] cc = null, string[] bcc = null)
        {
            if (IGUtils.IsIosCheck())
            {
                return;
            }

            var url = string.Format("mailto:{0}?subject={1}&body={2}",
                          recipients.JoinByComma(),
                          subject.Escape(), body.Escape()
                      );
            if (cc != null && cc.Length > 0)
            {
                url += string.Format("&cc={0}", cc.JoinByComma());
            }
            if (bcc != null && cc.Length > 0)
            {
                url += string.Format("&bcc={0}", bcc.JoinByComma());
            }

            IGUtils._openUrl(url);
        }

        [DllImport("__Internal")]
        private static extern void _showShareMessageWithImage(string message, IntPtr bufferPtr, int bufferLength,
                                                              IGUtils.ActionVoidCallbackDelegate callback, IntPtr callbackActionPtr);

        [DllImport("__Internal")]
        private static extern void _sendSms(string recipient, string body, IGUtils.ActionVoidCallbackDelegate callback, 
                                            IntPtr successPtr, IntPtr cancelPtr, IntPtr failurePtr);
    }
}
#endif