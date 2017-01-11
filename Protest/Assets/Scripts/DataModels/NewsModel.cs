using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewsModel : Model
{

    public int index;
    public int userIndex;
    public int targetIndex;
    public string text;
    public Type type;

    public string picture;

    public string notificationTime;

    public enum Type
    {
        Follow, Protest
    }

    public NewsModel(JSONObject jsonObj)
    {
        index = (int)jsonObj.GetField("index").n;
        userIndex = (int)jsonObj.GetField("userIndex").n;
        targetIndex = (int)jsonObj.GetField("targetIndex").n;

        text = jsonObj.GetField("text").str;

        picture = jsonObj.GetField("picture").str;

        notificationTime = jsonObj.GetField("time").str;

        type = (Type)Enum.Parse(typeof(Type), jsonObj.GetField("type").str);
    }
}
