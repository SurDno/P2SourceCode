using PLVirtualMachine.Common.Data;

namespace PLVirtualMachine.Common
{
  [VMType("ITextRef")]
  public interface ITextRef : IRef, IVariable, INamed, IVMStringSerializable
  {
    IGameString Text { get; }
  }
}
