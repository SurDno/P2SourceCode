using PLVirtualMachine.Common.Data;

namespace PLVirtualMachine.Common
{
  [VMType("IBlueprintRef")]
  public interface IBlueprintRef : IRef, IVariable, INamed, IVMStringSerializable, IEngineTemplated
  {
    IBlueprint Blueprint { get; }
  }
}
