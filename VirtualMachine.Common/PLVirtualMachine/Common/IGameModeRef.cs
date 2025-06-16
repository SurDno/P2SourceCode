using PLVirtualMachine.Common.Data;

namespace PLVirtualMachine.Common
{
  [VMType("IGameModeRef")]
  public interface IGameModeRef : IRef, IVariable, INamed, IVMStringSerializable
  {
    IGameMode GameMode { get; }
  }
}
