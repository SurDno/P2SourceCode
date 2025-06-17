using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI;

namespace PLVirtualMachine.FSM
{
  public class VMMessageCastInfo(ContextVariable message, VMType type) {
    public ContextVariable message = message;
    public VMType type = type;

    public VMMessageCastInfo() : this(null, null) { }

    public ContextVariable Message => message;

    public VMType CastType => type;
  }
}
