using System.Collections;
using System.Collections.Generic;

public class ContributionsModel : Model
{

    public int index;

    public string name;
    public int amountNeeded;
    public int currentAmount;

    public ProtestModel protest;

    public UserModel[] contributers;

	public ContributionsModel(int index, string name, int amountNeeded, int currentAmount, ProtestModel protest, UserModel[] contributers)
    {
        this.index = index;

        this.name = name;
        this.amountNeeded = amountNeeded;
        this.currentAmount = currentAmount;

        this.protest = protest;

        this.contributers = contributers;
    }
}
