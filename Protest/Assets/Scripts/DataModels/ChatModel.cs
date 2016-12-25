using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ChatModel : Model
{

    public int index;

    public string body;

    public int userPosted;

    public string datePosted;

    public ChatModel(int index, string body, int userPosted, string datePosted)
    {
        this.body = body;

        this.userPosted = userPosted;

        this.datePosted = datePosted;
    }
}
