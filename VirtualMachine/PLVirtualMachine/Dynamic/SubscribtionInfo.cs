namespace PLVirtualMachine.Dynamic
{
  public class SubscribtionInfo
  {
    public SubscribtionInfo(DynamicFSM fsm)
    {
      this.SubscribingFSM = fsm;
      this.Count = 1;
    }

    public void Add() => ++this.Count;

    public void Remove() => --this.Count;

    public DynamicFSM SubscribingFSM { get; private set; }

    public int Count { get; private set; }
  }
}
