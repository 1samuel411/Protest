using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageView : View
{
    public Image icon;

    public void SetImage(Sprite newImage)
    {
        icon.sprite = newImage;
        icon.rectTransform.localScale = new Vector3(1, 1, 1);
    }

    public float zoomSpeed = 0.5f;        // The rate of change of the image scale.

    void Update()
    {
        // If there are two touches on the device...
        if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // ... change the orthographic size based on the change in distance between the touches.
            float newSize = deltaMagnitudeDiff * zoomSpeed;
            icon.rectTransform.localScale += new Vector3(newSize, newSize, 1);

            // Make sure the orthographic size never drops below zero.
            icon.rectTransform.localScale = new Vector3(Mathf.Clamp(icon.rectTransform.localScale.x, 3.0f, 0.5f), Mathf.Clamp(icon.rectTransform.localScale.y, 3.0f, 0.5f), 1);
        }
    }

    public void Hide()
    {
        ImageViewController.instance.Hide();
    }
}
