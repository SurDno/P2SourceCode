using System;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;

namespace PLVirtualMachine.GameLogic
{
  [VMType("IGameModeRef")]
  [VMFactory(typeof (IGameModeRef))]
  public class VMGameModeRef : BaseRef, IGameModeRef, IRef, IVariable, INamed, IVMStringSerializable
  {
    public void Initialize(IGameMode gameMode) => LoadStaticInstance(gameMode);

    public override EContextVariableCategory Category => EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_STATE;

    public IGameMode GameMode
    {
      get
      {
        if (StaticInstance == null && BaseGuid > 0UL)
          LoadStaticInstance(IStaticDataContainer.StaticDataContainer.GetObjectByGuid(BaseGuid));
        return (IGameMode) StaticInstance;
      }
    }

    public override VMType Type => new(typeof (IGameModeRef));

    public override bool Empty => GameMode == null && base.Empty;

    protected override Type NeedInstanceType => typeof (IGameMode);
  }
}
