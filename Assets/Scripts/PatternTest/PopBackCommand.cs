using System.Collections.Generic;

public class PopBackCommand : ICommand
{
    private List<int> intList;
    private int removedNumber;

    public PopBackCommand(List<int> intList)
    {
        this.intList = intList;
    }

    public void Execute()
    {
        if (intList.Count > 0)
        {
            removedNumber = intList[intList.Count - 1];
            intList.RemoveAt(intList.Count - 1);
        }
    }

    public void Undo()
    {
        intList.Add(removedNumber);
    }
}
