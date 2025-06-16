using PLVirtualMachine.Common.Data;

namespace PLVirtualMachine.Common
{
  [VMType("IObjRef")]
  public interface IObjRef : 
    IRef,
    IVariable,
    INamed,
    IVMStringSerializable,
    IEngineInstanced,
    IEngineTemplated
  {
    IBlueprint Object { get; }

    IEngineInstanced EngineInstance { get; }
  }
}
