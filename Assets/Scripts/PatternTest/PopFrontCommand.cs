using System.Collections.Generic;

public class PopFrontCommand : ICommand
{
    private List<int> intList;
    private int removedNumber;

    public PopFrontCommand(List<int> intList)
    {
        this.intList = intList;
    }

    public void Execute()
    {
        if (intList.Count > 0)
        {
            removedNumber = intList[0];
            intList.RemoveAt(0);
        }
    }

    public void Undo()
    {
        intList.Insert(0, removedNumber);
    }
}
