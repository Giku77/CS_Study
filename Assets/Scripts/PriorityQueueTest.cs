using System;
using System.Collections.Generic;
using UnityEngine;

public class GameTask
{
    public string Name { get; }
    public GameTask(string name) => Name = name;
    public override string ToString() => Name;
}

public class PriorityQueueTest : MonoBehaviour
{
    public void RunTest()
    {
        BasicOperationsTest();
    }

    static void BasicOperationsTest()
    {
        var scheduler = new PriorityQueue<GameTask, int>();

        scheduler.Enqueue(new GameTask("Render"), 2);
        scheduler.Enqueue(new GameTask("Physics"), 1);
        scheduler.Enqueue(new GameTask("AI"), 3);

        Debug.Log($"작업 수: {scheduler.Count}"); // 출력: 3

        GameTask task = scheduler.Dequeue();
        Debug.Log($"첫 번째 작업: {task}"); // 출력: Physics

        task = scheduler.Peek();
        Debug.Log($"다음 작업: {task}"); // 출력: Render

        scheduler.Clear();
        Debug.Log($"Clear 후 작업 수: {scheduler.Count}"); // 출력: 0
    }

}
