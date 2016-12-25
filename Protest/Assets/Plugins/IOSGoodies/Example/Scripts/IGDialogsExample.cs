using UnityEngine;
using DeadMosquito.IosGoodies;

namespace DeadMosquito.IosGoodies.Example
{
    public class IGDialogsExample : MonoBehaviour
    {
#if UNITY_IOS
        public void OnShowConfirmationDialog()
        {
            IGDialogs.ShowOneBtnDialog("Title", "Message", "Confirm", () => Debug.Log("Button clicked!"));
        }

        public void OnShowTwoButtonDialog()
        {
            IGDialogs.ShowTwoBtnDialog("Title", "My awesome message!", 
                "Confirm", () => Debug.Log("Confirm button clicked!"),
                "Cancel", () => Debug.Log("Cancel clicked!"));
        }

        public void OnShowThreeButtonDialog()
        {
            IGDialogs.ShowThreeBtnDialog("Title", "My awesome message!", 
                "Option 1", () => Debug.Log("Option 1 button clicked!"),
                "Option 2", () => Debug.Log("Option 2 button clicked!"),
                "Cancel", () => Debug.Log("Cancel clicked!")
            );
        }
#endif
    }
}