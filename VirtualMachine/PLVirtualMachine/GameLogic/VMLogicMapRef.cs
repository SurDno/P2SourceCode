using System;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;

namespace PLVirtualMachine.GameLogic
{
  [VMType("ILogicMapRef")]
  [VMFactory(typeof (ILogicMapRef))]
  public class VMLogicMapRef : BaseRef, ILogicMapRef, IRef, IVariable, INamed, IVMStringSerializable
  {
    public void Initialize(ILogicMap logicMap) => LoadStaticInstance(logicMap);

    public override EContextVariableCategory Category
    {
      get => EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_LOGIC_MAP;
    }

    public override VMType Type => new VMType(typeof (ILogicMapRef));

    public ILogicMap LogicMap
    {
      get
      {
        if (StaticInstance == null && BaseGuid > 0UL)
          LoadStaticInstance(IStaticDataContainer.StaticDataContainer.GetObjectByGuid(BaseGuid));
        return (ILogicMap) StaticInstance;
      }
    }

    public override bool Empty => LogicMap == null && base.Empty;

    protected override Type NeedInstanceType => typeof (ILogicMap);
  }
}
