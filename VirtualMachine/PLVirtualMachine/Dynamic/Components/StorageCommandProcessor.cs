using System;
using System.Collections.Generic;

namespace PLVirtualMachine.Dynamic.Components
{
  public class StorageCommandProcessor : IAssyncUpdateable
  {
    private bool isActive;
    protected Queue<IStorageCommand> commandsQueue = new(1000);
    private static float FINISH_INTERVAL = 1f;
    private static float MAX_FRAME_INTERVAL = 0.1f;
    private static int CommnadProcNum;
    private const int CommandQueueSizeMax = 1000;

    public void Update(TimeSpan timeDelta)
    {
      int num1 = 1;
      if (timeDelta.TotalMilliseconds > 0.0)
      {
        double num2 = 1.0 / 1000.0 * timeDelta.TotalMilliseconds;
        if (num2 > MAX_FRAME_INTERVAL)
          num2 = MAX_FRAME_INTERVAL;
        int num3 = commandsQueue.Count / (int) Math.Ceiling(FINISH_INTERVAL / num2);
        if (num3 > 1)
          num1 = num3;
      }
      for (int index = 0; index < num1 && commandsQueue.Count != 0; ++index)
        ProcessCommand(commandsQueue.Dequeue());
      if (commandsQueue.Count == 0)
        isActive = false;
      ++CommnadProcNum;
    }

    public bool IsStorageOperationsProcessing => commandsQueue.Count > 0;

    public virtual void Clear()
    {
      if (commandsQueue == null)
        return;
      int count = commandsQueue.Count;
      for (int index = 0; index < count; ++index)
        commandsQueue.Dequeue().Clear();
      commandsQueue.Clear();
      commandsQueue = null;
    }

    public bool Active
    {
      get => isActive;
      protected set => isActive = value;
    }

    public void MakeStorageCommand(IStorageCommand storageCommand, bool assyncProcess = false)
    {
      if (assyncProcess)
        PushCommandToProcessQueue(storageCommand);
      else
        ProcessCommand(storageCommand);
    }

    protected virtual void ProcessCommand(IStorageCommand storageCommand)
    {
    }

    private void PushCommandToProcessQueue(IStorageCommand storageCommand)
    {
      commandsQueue.Enqueue(storageCommand);
      isActive = true;
    }
  }
}
