using System.Collections.Generic;

namespace PLVirtualMachine.Common.EngineAPI
{
  public class ComponentInfo
  {
    public readonly string ComponentName;
    public readonly List<APIMethodInfo> Methods = new List<APIMethodInfo>();
    public readonly List<APIEventInfo> Events = new List<APIEventInfo>();
    public readonly List<APIPropertyInfo> Properties = new List<APIPropertyInfo>();

    public ComponentInfo(string componentName) => this.ComponentName = componentName;
  }
}
