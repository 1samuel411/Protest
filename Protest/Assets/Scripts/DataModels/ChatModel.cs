using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ChatModel : Model
{

    public int index;
    public string body;
    public int user;
    public string name;
    public string time;
    public int protest;

    public string picture;

    public ChatModel(JSONObject jsonObj)
    {
        if (jsonObj == null)
            return;
        if (jsonObj.HasField("id") == false)
        {
            return;
        }
        index = int.Parse(jsonObj.GetField("id").ToString());

        body = jsonObj.GetField("body").str;

        user = (int)jsonObj.GetField("user").n;
        name = jsonObj.GetField("name").str;
        protest = (int)jsonObj.GetField("protest").n;

        time = jsonObj.GetField("time").str;

        if (jsonObj.HasField("picture"))
            picture = jsonObj.GetField("picture").str;
    }

    public ChatModel()
    {
        index = 0;
        body = "";
        user = 0;
        name = "";
        time = "";
        protest = 0;
    }
}
