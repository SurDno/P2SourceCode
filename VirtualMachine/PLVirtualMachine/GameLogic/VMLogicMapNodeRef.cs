using System;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;

namespace PLVirtualMachine.GameLogic
{
  [VMType("ILogicMapNodeRef")]
  [VMFactory(typeof (ILogicMapNodeRef))]
  public class VMLogicMapNodeRef : 
    BaseRef,
    ILogicMapNodeRef,
    IRef,
    IVariable,
    INamed,
    IVMStringSerializable
  {
    public void Initialize(IGraphObject lmNode) => LoadStaticInstance(lmNode);

    public override EContextVariableCategory Category
    {
      get => EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_STATE;
    }

    public IGraphObject LogicMapNode
    {
      get
      {
        if (StaticInstance == null && BaseGuid > 0UL)
          LoadStaticInstance(IStaticDataContainer.StaticDataContainer.GetObjectByGuid(BaseGuid));
        return (IGraphObject) StaticInstance;
      }
    }

    public override VMType Type => VMType.CreateStateSpecialType(LogicMapNode);

    public override bool Empty => LogicMapNode == null && base.Empty;

    protected override Type NeedInstanceType => typeof (IGraphObject);
  }
}
