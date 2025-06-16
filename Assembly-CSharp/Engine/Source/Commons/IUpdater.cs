using Engine.Common;

namespace Engine.Source.Commons;

public interface IUpdater {
	void AddUpdatable(IUpdatable up);

	void RemoveUpdatable(IUpdatable up);

	void ComputeUpdate();
}