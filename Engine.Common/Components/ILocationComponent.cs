using System;

namespace Engine.Common.Components;

public interface ILocationComponent : IComponent {
	bool IsHibernation { get; }

	IEntity Player { get; }

	bool IsIndoor { get; }

	ILocationComponent LogicLocation { get; }

	event Action<ILocationComponent> OnHibernationChanged;

	event Action OnPlayerChanged;
}