using System.Collections.Generic;

namespace Engine.Common.Components;

public interface ITagsComponent : IComponent {
	IEnumerable<EntityTagEnum> Tags { get; }
}