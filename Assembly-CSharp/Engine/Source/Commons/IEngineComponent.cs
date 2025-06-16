using Engine.Common;

namespace Engine.Source.Commons;

public interface IEngineComponent {
	void OnChangeEnabled();

	void PrepareAdded();

	void PostRemoved();

	IEntity Owner { set; }
}