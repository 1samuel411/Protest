using System.Collections;
using System.Collections.Generic;

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
    public float donationTarget;

    public UserModel[] likes;
    public UserModel[] going;

    public ContributionsModel[] contributers;

    public ChatModel[] chats;

    public ProtestModel
        (
            int index,
            string protestPicture, string name, string description, string location, string date,
            string datePosted, string dateUpdated,
            string donationsEmail, float donationTarget,
            UserModel[] likes, UserModel[] going, 
            ContributionsModel[] contributers,
            ChatModel[] chats
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
        this.donationTarget = donationTarget;

        this.likes = likes;
        this.going = going;

        this.contributers = contributers;

        this.chats = chats;
    }
}
