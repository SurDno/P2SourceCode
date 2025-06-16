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
      message = null;
      type = null;
    }

    public VMMessageCastInfo(ContextVariable message, VMType type)
    {
      this.message = message;
      this.type = type;
    }

    public ContextVariable Message => message;

    public VMType CastType => type;
  }
}
