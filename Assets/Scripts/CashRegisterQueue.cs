using System.Collections.Generic;
using UnityEngine;

public static class CashRegisterQueue
{
    private static readonly Queue<NPC_Movement> waitingQueue = new Queue<NPC_Movement>();

    public static void Enqueue(NPC_Movement npc)
    {
        if (npc == null)
        {
            return;
        }

        foreach (NPC_Movement queuedNpc in waitingQueue)
        {
            if (queuedNpc == npc)
            {
                return;
            }
        }

        waitingQueue.Enqueue(npc);
    }

    public static void Remove(NPC_Movement npc)
    {
        if (npc == null || waitingQueue.Count == 0)
        {
            return;
        }

        Queue<NPC_Movement> rebuiltQueue = new Queue<NPC_Movement>();
        while (waitingQueue.Count > 0)
        {
            NPC_Movement queuedNpc = waitingQueue.Dequeue();
            if (queuedNpc != null && queuedNpc != npc)
            {
                rebuiltQueue.Enqueue(queuedNpc);
            }
        }

        while (rebuiltQueue.Count > 0)
        {
            waitingQueue.Enqueue(rebuiltQueue.Dequeue());
        }
    }

    public static bool ReleaseNextGrandma()
    {
        while (waitingQueue.Count > 0)
        {
            NPC_Movement nextNpc = waitingQueue.Dequeue();
            if (nextNpc != null && nextNpc.IsWaitingAtRegister())
            {
                nextNpc.InteractAtRegister();
                return true;
            }
        }

        return false;
    }
}