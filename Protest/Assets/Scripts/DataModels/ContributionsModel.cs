using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ContributionsModel : Model
{

    public int index;

    public string name;
    public int amountNeeded;
    public int currentAmount;

    public int protest;

    public ContributionsModel(int index, string name, int amountNeeded, int currentAmount, int protest)
    {
        this.index = index;

        this.name = name;
        this.amountNeeded = amountNeeded;
        this.currentAmount = currentAmount;

        this.protest = protest;
    }

    public ContributionsModel(JSONObject jsonObj)
    {
        if (jsonObj.HasField("index") == false)
        {
            return;
        }
        index = int.Parse(jsonObj.GetField("index").ToString());

        name = jsonObj.GetField("name").str;
        amountNeeded = (int)jsonObj.GetField("amountNeeded").n;
        currentAmount = (int)jsonObj.GetField("currentAmount").n;

        protest = (int)jsonObj.GetField("protest").n;
    }
}