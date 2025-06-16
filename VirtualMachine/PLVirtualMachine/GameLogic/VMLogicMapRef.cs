using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;

namespace PLVirtualMachine.GameLogic
{
  [VMType("ILogicMapRef")]
  [VMFactory(typeof (ILogicMapRef))]
  public class VMLogicMapRef : BaseRef, ILogicMapRef, IRef, IVariable, INamed, IVMStringSerializable
  {
    public void Initialize(ILogicMap logicMap) => this.LoadStaticInstance((IObject) logicMap);

    public override EContextVariableCategory Category
    {
      get => EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_LOGIC_MAP;
    }

    public override VMType Type => new VMType(typeof (ILogicMapRef));

    public ILogicMap LogicMap
    {
      get
      {
        if (this.StaticInstance == null && this.BaseGuid > 0UL)
          this.LoadStaticInstance(IStaticDataContainer.StaticDataContainer.GetObjectByGuid(this.BaseGuid));
        return (ILogicMap) this.StaticInstance;
      }
    }

    public override bool Empty => this.LogicMap == null && base.Empty;

    protected override System.Type NeedInstanceType => typeof (ILogicMap);
  }
}
