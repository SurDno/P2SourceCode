// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.ComponentInfo
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using System.Collections.Generic;

#nullable disable
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
