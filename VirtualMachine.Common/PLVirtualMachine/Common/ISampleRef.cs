using PLVirtualMachine.Common.Data;

namespace PLVirtualMachine.Common
{
  [VMType("ISampleRef")]
  public interface ISampleRef : IRef, IVariable, INamed, IVMStringSerializable, IEngineTemplated
  {
    ISample Sample { get; }
  }
}
