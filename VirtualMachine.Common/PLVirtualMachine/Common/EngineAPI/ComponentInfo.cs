using System.Collections.Generic;

namespace PLVirtualMachine.Common.EngineAPI;

public class ComponentInfo {
	public readonly string ComponentName;
	public readonly List<APIMethodInfo> Methods = new();
	public readonly List<APIEventInfo> Events = new();
	public readonly List<APIPropertyInfo> Properties = new();

	public ComponentInfo(string componentName) {
		ComponentName = componentName;
	}
}