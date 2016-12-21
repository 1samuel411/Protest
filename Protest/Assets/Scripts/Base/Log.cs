using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class Log : Base
{
    public static int importanceShow = 0;
    public static void Create(int importance, string info, string prefix = "", string suffix = "")
    {
        if(importance < importanceShow)
        {
            return;
        }
        Debug.Log((prefix.Length > 0 ? ("[" + prefix + "] ") : "") + info + (suffix.Length > 0 ? (" [" + suffix + "]") : ""));
    }
}
