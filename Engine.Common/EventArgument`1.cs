namespace Engine.Common;

public struct EventArgument<TActor> {
	public TActor Actor;
	public bool IsCanceled;
}