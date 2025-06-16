namespace Engine.Common.Components;

public delegate void RegionHandler(
	ref EventArgument<IEntity, IRegionComponent> eventArguments);