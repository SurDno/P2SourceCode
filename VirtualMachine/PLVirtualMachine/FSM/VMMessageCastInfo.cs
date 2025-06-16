using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI;

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
