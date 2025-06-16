using System;
using Engine.Common.Components.Interactable;
using Engine.Common.Components.Parameters;

namespace Engine.Common.Components;

public interface IControllerComponent : IComponent {
	IParameterValue<bool> IsRun { get; }

	IParameterValue<bool> IsWalk { get; }

	IParameterValue<bool> IsStelth { get; }

	IParameterValue<bool> IsFlashlight { get; }

	event Action<IEntity, IInteractableComponent, IInteractItem> BeginInteractEvent;

	event Action<IEntity, IInteractableComponent, IInteractItem> EndInteractEvent;
}