// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes.EventAttribute
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using System;

#nullable disable
namespace PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes
{
  [AttributeUsage(AttributeTargets.Event, AllowMultiple = false, Inherited = false)]
  public class EventAttribute : Attribute
  {
    public readonly string Name;
    public readonly string InputTypesDesc;
    public readonly bool AtOnce;

    public EventAttribute(string name, string inputTypesDescription)
    {
      this.Name = name;
      this.InputTypesDesc = inputTypesDescription;
      this.AtOnce = false;
    }

    public EventAttribute(string name, string inputTypesDescription, bool bAtOnce)
    {
      this.Name = name;
      this.InputTypesDesc = inputTypesDescription;
      this.AtOnce = bAtOnce;
    }
  }
}
