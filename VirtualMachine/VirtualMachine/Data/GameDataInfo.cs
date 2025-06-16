// Decompiled with JetBrains decompiler
// Type: VirtualMachine.Data.GameDataInfo
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using PLVirtualMachine.Common;
using PLVirtualMachine.Objects;
using System.Collections.Generic;

#nullable disable
namespace VirtualMachine.Data
{
  public class GameDataInfo
  {
    public string Name;
    public Dictionary<ulong, IObject> Objects;
    public VMGameRoot Root;
  }
}
