using Engine.Common;

namespace Engine.Source.Commons;

public interface IEntityEventsListener {
	void OnEntityEvent(IEntity sender, EntityEvents kind);
}