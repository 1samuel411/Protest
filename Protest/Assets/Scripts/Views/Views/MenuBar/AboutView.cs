using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AboutView : View
{

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
        }
    }
}
