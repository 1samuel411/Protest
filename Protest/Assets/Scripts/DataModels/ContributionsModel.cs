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
}
