using System;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;

namespace PLVirtualMachine.GameLogic
{
  [VMType("IStateRef")]
  [VMFactory(typeof (IStateRef))]
  public class VMStateRef : BaseRef, IStateRef, IRef, IVariable, INamed, IVMStringSerializable
  {
    public void Initialize(ulong baseGuid)
    {
      BaseGuid = baseGuid;
      Load();
    }

    public void Initialize(IState state) => LoadStaticInstance(state);

    public override EContextVariableCategory Category
    {
      get => EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_STATE;
    }

    public IState State
    {
      get
      {
        if (StaticInstance == null && BaseGuid > 0UL)
          LoadStaticInstance(IStaticDataContainer.StaticDataContainer.GetObjectByGuid(BaseGuid));
        return (IState) StaticInstance;
      }
    }

    public override VMType Type => VMType.CreateStateSpecialType(State);

    public override bool Empty => State == null && base.Empty;

    protected override Type NeedInstanceType => typeof (IState);
  }
}
