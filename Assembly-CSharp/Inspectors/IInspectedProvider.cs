using System;
using System.Reflection;

namespace Inspectors;

public interface IInspectedProvider : IExpandedProvider {
	string ElementName { get; set; }

	void DrawInspected(
		string name,
		Type type,
		object value,
		bool mutable,
		object target,
		MemberInfo member,
		Action<object> setter);

	Guid DrawId { get; }

	Guid NameId { get; }

	void SetHeader(string name);

	int ContextIndex { get; set; }

	Action ContextItemMenu { get; set; }

	object ContextObject { get; set; }
}