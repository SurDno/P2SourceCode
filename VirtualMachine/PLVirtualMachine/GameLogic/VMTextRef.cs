using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;

namespace PLVirtualMachine.GameLogic
{
  [VMType("ITextRef")]
  [VMFactory(typeof (ITextRef))]
  public class VMTextRef : BaseRef, ITextRef, IRef, IVariable, INamed, IVMStringSerializable
  {
    public override EContextVariableCategory Category
    {
      get => EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_TEXT;
    }

    public override VMType Type => new VMType(typeof (ITextRef));

    public IGameString Text
    {
      get
      {
        if (this.StaticInstance == null && this.BaseGuid > 0UL)
          this.LoadStaticInstance(IStaticDataContainer.StaticDataContainer.GetObjectByGuid(this.BaseGuid));
        return (IGameString) this.StaticInstance;
      }
    }

    public override bool Empty => this.Text == null && base.Empty;

    protected override System.Type NeedInstanceType => typeof (IGameString);
  }
}
