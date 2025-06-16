// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.IDynamicMindMapObject
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using System;

#nullable disable
namespace PLVirtualMachine.Dynamic
{
  public interface IDynamicMindMapObject
  {
    Guid DynamicGuid { get; }

    ulong StaticGuid { get; }
  }
}
