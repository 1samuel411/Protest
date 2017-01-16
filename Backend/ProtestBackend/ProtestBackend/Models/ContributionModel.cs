using ProtestBackend.DLL;
using System.Collections;
using System.Collections.Generic;
using System.Data;

[System.Serializable]
public class ContributionModel
{

    public int index;

    public string name;

    public int amountNeeded;
    public int currentAmount;

    public int protest;

    public ContributionModel(DataRow dataTable, bool hide = false)
    {
        this.index = int.Parse(dataTable["id"].ToString());

        if (dataTable.Table.Columns.Contains("name"))
            name = dataTable["name"].ToString();

        if (dataTable.Table.Columns.Contains("amountNeeded"))
            amountNeeded = int.Parse(dataTable["amountNeeded"].ToString());
        if (dataTable.Table.Columns.Contains("currentAmount"))
            currentAmount = int.Parse(dataTable["currentAmount"].ToString());

        if (dataTable.Table.Columns.Contains("protest"))
            protest = int.Parse(dataTable["protest"].ToString());
    }
}
