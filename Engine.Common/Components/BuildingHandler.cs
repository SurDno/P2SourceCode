namespace Engine.Common.Components;

public delegate void BuildingHandler(
	ref EventArgument<IEntity, IBuildingComponent> eventArguments);