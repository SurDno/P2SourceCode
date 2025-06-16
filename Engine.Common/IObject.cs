using System;

namespace Engine.Common;

public interface IObject {
	Guid Id { get; }

	string Name { get; set; }

	Guid TemplateId { get; }

	string Source { get; }

	IObject Template { get; }

	bool IsTemplate { get; }
}