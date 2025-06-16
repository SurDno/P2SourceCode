using System;
using Engine.Common.Commons;

namespace Engine.Common.Components;

public interface IBehaviorComponent : IComponent {
	event Action<IBehaviorComponent> SuccessEvent;

	event Action<IBehaviorComponent> FailEvent;

	event Action<IBehaviorComponent, string> CustomEvent;

	IBehaviorObject BehaviorObject { get; set; }

	IBehaviorObject BehaviorObjectForced { get; set; }

	void SetValue(string name, IEntity value);

	void SetBoolValue(string name, bool value);

	void SetIntValue(string name, int value);

	void SetFloatValue(string name, float value);

	void SetBehaviorForced(IBehaviorObject behaviorObject);
}