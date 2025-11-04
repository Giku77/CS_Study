using System.Collections.Generic;

public class PushBackCommand : ICommand
{
    private List<int> intList;
    private int addedNumber;

    public PushBackCommand(List<int> intList, int addedNumber)
    {
        this.intList = intList;
        this.addedNumber = addedNumber;
    }

    public void Execute()
    {
        intList.Add(addedNumber);
    }

    public void Undo()
    {
        if (intList.Count > 0 && intList[intList.Count - 1] == addedNumber)
        {
            intList.RemoveAt(intList.Count - 1);
        }
    }
}
