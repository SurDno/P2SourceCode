// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.FSM.VMMessageCastInfo
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI;

#nullable disable
namespace PLVirtualMachine.FSM
{
  public class VMMessageCastInfo
  {
    public ContextVariable message;
    public VMType type;

    public VMMessageCastInfo()
    {
      this.message = (ContextVariable) null;
      this.type = (VMType) null;
    }

    public VMMessageCastInfo(ContextVariable message, VMType type)
    {
      this.message = message;
      this.type = type;
    }

    public ContextVariable Message => this.message;

    public VMType CastType => this.type;
  }
}
