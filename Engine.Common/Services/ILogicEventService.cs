using System;

namespace Engine.Common.Services;

public interface ILogicEventService {
	event Action<string> OnCommonEvent;

	event Action<string, string> OnValueEvent;

	event Action<string, IEntity> OnEntityEvent;
}