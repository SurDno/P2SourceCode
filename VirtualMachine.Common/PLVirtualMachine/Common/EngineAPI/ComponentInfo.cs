using System.Collections.Generic;

namespace PLVirtualMachine.Common.EngineAPI
{
  public class ComponentInfo(string componentName) {
    public readonly string ComponentName = componentName;
    public readonly List<APIMethodInfo> Methods = [];
    public readonly List<APIEventInfo> Events = [];
    public readonly List<APIPropertyInfo> Properties = [];
  }
}
