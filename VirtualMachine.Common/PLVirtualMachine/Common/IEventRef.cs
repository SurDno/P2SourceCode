using PLVirtualMachine.Common.Data;

namespace PLVirtualMachine.Common
{
  [VMType("IEventRef")]
  public interface IEventRef : IRef, IVariable, INamed, IVMStringSerializable
  {
    IEvent Event { get; }
  }
}
