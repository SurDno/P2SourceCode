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
      this.BaseGuid = baseGuid;
      this.Load();
    }

    public void Initialize(IState state) => this.LoadStaticInstance((IObject) state);

    public override EContextVariableCategory Category
    {
      get => EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_STATE;
    }

    public IState State
    {
      get
      {
        if (this.StaticInstance == null && this.BaseGuid > 0UL)
          this.LoadStaticInstance(IStaticDataContainer.StaticDataContainer.GetObjectByGuid(this.BaseGuid));
        return (IState) this.StaticInstance;
      }
    }

    public override VMType Type => VMType.CreateStateSpecialType((IGraphObject) this.State);

    public override bool Empty => this.State == null && base.Empty;

    protected override System.Type NeedInstanceType => typeof (IState);
  }
}
