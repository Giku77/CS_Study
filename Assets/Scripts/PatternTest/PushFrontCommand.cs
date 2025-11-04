using System.Collections.Generic;

public class PushFrontCommand : ICommand
{
    private List<int> intList;
    private int addedNumber;

    public PushFrontCommand(List<int> intList, int addedNumber)
    {
        this.intList = intList;
        this.addedNumber = addedNumber;
    }

    public void Execute()
    {
        intList.Insert(0, addedNumber);
    }

    public void Undo()
    {
        if (intList.Count > 0 && intList[0] == addedNumber)
        {
            intList.RemoveAt(0);
        }
    }
}
