using System;

namespace Inspectors;

public struct InspectedContext {
	public IInspectedProvider Provider;
	public Action<object> Setter;
}