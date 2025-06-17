namespace PLVirtualMachine.Dynamic
{
  public class SubscribtionInfo(DynamicFSM fsm) {
    public void Add() => ++Count;

    public void Remove() => --Count;

    public DynamicFSM SubscribingFSM { get; private set; } = fsm;

    public int Count { get; private set; } = 1;
  }
}
