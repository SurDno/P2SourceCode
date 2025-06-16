namespace PLVirtualMachine.Common;

public class EventInfo {
	public readonly IEvent EventInstance;
	public readonly CommonVariable EventOwner;

	public EventInfo(IEvent eventInstance, CommonVariable eventOwner) {
		EventInstance = eventInstance;
		EventOwner = eventOwner;
	}
}