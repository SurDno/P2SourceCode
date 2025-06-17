namespace PLVirtualMachine.Common
{
  public class EventInfo(IEvent eventInstance, CommonVariable eventOwner) {
    public readonly IEvent EventInstance = eventInstance;
    public readonly CommonVariable EventOwner = eventOwner;
  }
}
