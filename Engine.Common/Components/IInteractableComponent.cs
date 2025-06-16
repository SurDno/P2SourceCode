using System;
using Engine.Common.Components.Interactable;
using Engine.Common.Types;

namespace Engine.Common.Components;

public interface IInteractableComponent : IComponent {
	bool IsEnabled { get; set; }

	LocalizedText Title { get; set; }

	event Action<IEntity, IInteractableComponent, IInteractItem> BeginInteractEvent;

	event Action<IEntity, IInteractableComponent, IInteractItem> EndInteractEvent;
}