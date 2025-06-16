using System;
using System.Collections.Generic;

namespace PLVirtualMachine.Dynamic.Components
{
  public class StorageCommandProcessor : IAssyncUpdateable
  {
    private bool isActive;
    protected Queue<IStorageCommand> commandsQueue = new Queue<IStorageCommand>(1000);
    private static float FINISH_INTERVAL = 1f;
    private static float MAX_FRAME_INTERVAL = 0.1f;
    private static int CommnadProcNum = 0;
    private const int CommandQueueSizeMax = 1000;

    public void Update(TimeSpan timeDelta)
    {
      int num1 = 1;
      if (timeDelta.TotalMilliseconds > 0.0)
      {
        double num2 = 1.0 / 1000.0 * timeDelta.TotalMilliseconds;
        if (num2 > (double) StorageCommandProcessor.MAX_FRAME_INTERVAL)
          num2 = (double) StorageCommandProcessor.MAX_FRAME_INTERVAL;
        int num3 = this.commandsQueue.Count / (int) Math.Ceiling((double) StorageCommandProcessor.FINISH_INTERVAL / num2);
        if (num3 > 1)
          num1 = num3;
      }
      for (int index = 0; index < num1 && this.commandsQueue.Count != 0; ++index)
        this.ProcessCommand(this.commandsQueue.Dequeue());
      if (this.commandsQueue.Count == 0)
        this.isActive = false;
      ++StorageCommandProcessor.CommnadProcNum;
    }

    public bool IsStorageOperationsProcessing => this.commandsQueue.Count > 0;

    public virtual void Clear()
    {
      if (this.commandsQueue == null)
        return;
      int count = this.commandsQueue.Count;
      for (int index = 0; index < count; ++index)
        this.commandsQueue.Dequeue().Clear();
      this.commandsQueue.Clear();
      this.commandsQueue = (Queue<IStorageCommand>) null;
    }

    public bool Active
    {
      get => this.isActive;
      protected set => this.isActive = value;
    }

    public void MakeStorageCommand(IStorageCommand storageCommand, bool assyncProcess = false)
    {
      if (assyncProcess)
        this.PushCommandToProcessQueue(storageCommand);
      else
        this.ProcessCommand(storageCommand);
    }

    protected virtual void ProcessCommand(IStorageCommand storageCommand)
    {
    }

    private void PushCommandToProcessQueue(IStorageCommand storageCommand)
    {
      this.commandsQueue.Enqueue(storageCommand);
      this.isActive = true;
    }
  }
}
