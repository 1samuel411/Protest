using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ProtestModel : Model
{

    public int index;

    public string protestPicture;
    public string name;
    public string description;
    public string location;
    public string date;

    public string datePosted;

    public string donationsEmail;
    public float donationCurrent;
    public float donationTarget;

    public int[] likes;
    public int[] going;
    public int likesCount;
    public int goingCount;

    public int[] contributions;
    public ContributionsModel[] contributionModels;

    public int[] chats;

    public int userCreated;

    public float x;
    public float y;

    public bool active;

    public ProtestModel
        (
            int index,
            string protestPicture, string name, string description, string location, string date,
            string datePosted,
            string donationsEmail, float donationCurrent, float donationTarget,
            int[] likes, int[] going,
            int[] contributions,
            int[] chats,
            int userCreated,
            float x,
            float y,
            bool active
        )
    {
        this.index = index;

        this.protestPicture = protestPicture;
        this.name = name;
        this.description = description;
        this.location = location;
        this.date = date;

        this.datePosted = datePosted;

        this.donationsEmail = donationsEmail;
        this.donationCurrent = donationCurrent;
        this.donationTarget = donationTarget;

        this.likes = likes;
        this.going = going;

        this.contributions = contributions;

        this.chats = chats;

        this.userCreated = userCreated;

        this.x = x;
        this.y = y;

        this.active = active;
    }

    public ProtestModel(JSONObject jsonObj)
    {
        if(jsonObj.HasField("index") == false)
        {
            return;
        }
        index = int.Parse(jsonObj.GetField("index").ToString());
        protestPicture = jsonObj.GetField("protestPicture").str;
        name = jsonObj.GetField("name").str;
        description = jsonObj.GetField("description").str;
        location = jsonObj.GetField("location").str;
        date = jsonObj.GetField("date").str;
        datePosted = jsonObj.GetField("datePosted").str;
        donationsEmail = jsonObj.GetField("donationsEmail").str;
        if (!string.IsNullOrEmpty(description))
            description = description.Replace(@"\t", "");

        going = new int[0];
        likes = new int[0];
        contributions = new int[0];
        chats = new int[0];

        if (jsonObj.HasField("goingCount"))
            goingCount = (int)(jsonObj.GetField("goingCount").n);
        if (jsonObj.HasField("likesCount"))
            likesCount = (int)(jsonObj.GetField("likesCount").n);

        if (jsonObj.HasField("going") && jsonObj.GetField("going").list != null)
            going = DataParser.ParseJsonToIntArray(jsonObj.GetField("going").list);
        if (jsonObj.HasField("likes") && jsonObj.GetField("likes").list != null)
            likes = DataParser.ParseJsonToIntArray(jsonObj.GetField("likes").list);
        if (jsonObj.HasField("contributions") && jsonObj.GetField("contributions").list != null)
            contributions = DataParser.ParseJsonToIntArray(jsonObj.GetField("contributions").list);
        if (jsonObj.HasField("chats") && jsonObj.GetField("chats").list != null)
            chats = DataParser.ParseJsonToIntArray(jsonObj.GetField("chats").list);

        userCreated = (int)jsonObj.GetField("userCreated").n;

        x = jsonObj.GetField("latitude").n;
        y = jsonObj.GetField("longitude").n;

        active = jsonObj.GetField("active").b;
    }
}
