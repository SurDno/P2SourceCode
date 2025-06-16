namespace PLVirtualMachine.Dynamic;

public class SubscribtionInfo {
	public SubscribtionInfo(DynamicFSM fsm) {
		SubscribingFSM = fsm;
		Count = 1;
	}

	public void Add() {
		++Count;
	}

	public void Remove() {
		--Count;
	}

	public DynamicFSM SubscribingFSM { get; private set; }

	public int Count { get; private set; }
}