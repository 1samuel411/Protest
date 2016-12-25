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
    public string dateUpdated;

    public string donationsEmail;
    public float donationCurrent;
    public float donationTarget;

    public int[] likes;
    public int[] going;

    public int[] contributions;

    public int[] chats;

    public int userCreated;

    public ProtestModel
        (
            int index,
            string protestPicture, string name, string description, string location, string date,
            string datePosted, string dateUpdated,
            string donationsEmail, float donationCurrent, float donationTarget,
            int[] likes, int[] going,
            int[] contributions,
            int[] chats,
            int userCreated
        )
    {
        this.index = index;

        this.protestPicture = protestPicture;
        this.name = name;
        this.description = description;
        this.location = location;
        this.date = date;

        this.datePosted = datePosted;
        this.dateUpdated = dateUpdated;

        this.donationsEmail = donationsEmail;
        this.donationCurrent = donationCurrent;
        this.donationTarget = donationTarget;

        this.likes = likes;
        this.going = going;

        this.contributions = contributions;

        this.chats = chats;

        this.userCreated = userCreated;
    }
}
