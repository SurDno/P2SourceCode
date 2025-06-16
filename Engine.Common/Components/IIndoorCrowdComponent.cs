using System;

namespace Engine.Common.Components;

public interface IIndoorCrowdComponent : IComponent {
	void AddEntity(IEntity entity);

	void Reset();

	event Action<IEntity> OnCreateEntity;

	event Action<IEntity> OnDeleteEntity;
}