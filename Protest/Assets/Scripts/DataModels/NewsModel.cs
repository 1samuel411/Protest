using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewsModel : Model
{

    public int index;
    public Type type;
    public int userIndex;
    public int targetIndex;
    public string text;

    public string notificationTime;

    public enum Type
    {
        Follow, Protest
    }
}
