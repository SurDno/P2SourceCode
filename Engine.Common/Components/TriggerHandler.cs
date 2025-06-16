namespace Engine.Common.Components;

public delegate void TriggerHandler(
	ref EventArgument<IEntity, ITriggerComponent> eventArguments);