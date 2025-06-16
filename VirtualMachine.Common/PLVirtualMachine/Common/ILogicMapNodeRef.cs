using PLVirtualMachine.Common.Data;

namespace PLVirtualMachine.Common
{
  [VMType("ILogicMapNodeRef")]
  public interface ILogicMapNodeRef : IRef, IVariable, INamed, IVMStringSerializable
  {
    IGraphObject LogicMapNode { get; }
  }
}
