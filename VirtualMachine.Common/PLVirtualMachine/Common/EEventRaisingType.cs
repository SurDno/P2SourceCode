// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EEventRaisingType
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using System.ComponentModel;

#nullable disable
namespace PLVirtualMachine.Common
{
  public enum EEventRaisingType
  {
    [Description("Condition")] EVENT_RAISING_TYPE_CONDITION,
    [Description("Parameter change")] EVENT_RAISING_TYPE_PARAM_CHANGE,
    [Description("Time")] EVENT_RAISING_TYPE_TIME,
    [Description("By engine")] EVENT_RAISING_TYPE_BY_ENGINE,
  }
}
