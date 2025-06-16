using System;

namespace Engine.Source.Connections;

public interface IEngineSerializable {
	Guid Id { get; set; }

	Type Type { get; }
}