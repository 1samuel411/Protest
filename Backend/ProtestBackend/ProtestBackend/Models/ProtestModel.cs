using ProtestBackend.DLL;
using System.Collections;
using System.Collections.Generic;
using System.Data;

[System.Serializable]
public class ProtestModel
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

    public int[] contributions;

    public int[] chats;

    public int userCreated;

    public float latitude;
    public float longitude;

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
            float latitude,
            float longitude,
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

        this.latitude = latitude;
        this.longitude = longitude;

        this.active = active;
    }

    public ProtestModel(DataRow dataTable, bool hide = false)
    {
        this.index = int.Parse(dataTable["id"].ToString());
        if (dataTable.Table.Columns.Contains("protestPicture"))
            protestPicture = dataTable["protestPicture"].ToString();
        if (dataTable.Table.Columns.Contains("name"))
            name = dataTable["name"].ToString();
        if (dataTable.Table.Columns.Contains("description"))
            description = dataTable["description"].ToString();
        if (dataTable.Table.Columns.Contains("location"))
            location = dataTable["location"].ToString();
        if (dataTable.Table.Columns.Contains("date"))
            date = dataTable["date"].ToString();

        if (dataTable.Table.Columns.Contains("time"))
            datePosted = dataTable["time"].ToString();

        if (dataTable.Table.Columns.Contains("donationsEmail"))
            donationsEmail = dataTable["donationsEmail"].ToString();
        if (dataTable.Table.Columns.Contains("donationCurrent"))
            donationCurrent = int.Parse(dataTable["donationCurrent"].ToString());
        if (dataTable.Table.Columns.Contains("donationTarget"))
            donationTarget = int.Parse(dataTable["donationTarget"].ToString());

        if (dataTable.Table.Columns.Contains("likes"))
            likes = Parser.ParseStringToIntArray(dataTable["likes"].ToString());
        if (dataTable.Table.Columns.Contains("going"))
            going = Parser.ParseStringToIntArray(dataTable["going"].ToString());

        if (dataTable.Table.Columns.Contains("contributions"))
            contributions = Parser.ParseStringToIntArray(dataTable["contributions"].ToString());

        if (dataTable.Table.Columns.Contains("chats"))
            chats = Parser.ParseStringToIntArray(dataTable["chats"].ToString());

        if (dataTable.Table.Columns.Contains("userCreated"))
            userCreated = int.Parse(dataTable["userCreated"].ToString());

        if (dataTable.Table.Columns.Contains("latitude"))
            latitude = float.Parse(dataTable["latitude"].ToString());
        if (dataTable.Table.Columns.Contains("longitude"))
            longitude = float.Parse(dataTable["longitude"].ToString());

        if (dataTable.Table.Columns.Contains("active"))
            active = bool.Parse(dataTable["active"].ToString());
    }
}
