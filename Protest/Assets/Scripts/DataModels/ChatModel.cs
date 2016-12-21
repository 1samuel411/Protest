using System.Collections;
using System.Collections.Generic;

public class ChatModel : Model
{

    public int index;

    public string body;

    public UserModel userPosted;

    public string datePosted;

    public ChatModel(int index, string body, UserModel userPosted, string datePosted)
    {
        this.body = body;

        this.userPosted = userPosted;

        this.datePosted = datePosted;
    }
}
