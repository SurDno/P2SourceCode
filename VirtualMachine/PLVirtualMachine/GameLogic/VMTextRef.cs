using System;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;

namespace PLVirtualMachine.GameLogic
{
  [VMType("ITextRef")]
  [VMFactory(typeof (ITextRef))]
  public class VMTextRef : BaseRef, ITextRef, IRef, IVariable, INamed, IVMStringSerializable
  {
    public override EContextVariableCategory Category => EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_TEXT;

    public override VMType Type => new(typeof (ITextRef));

    public IGameString Text
    {
      get
      {
        if (StaticInstance == null && BaseGuid > 0UL)
          LoadStaticInstance(IStaticDataContainer.StaticDataContainer.GetObjectByGuid(BaseGuid));
        return (IGameString) StaticInstance;
      }
    }

    public override bool Empty => Text == null && base.Empty;

    protected override Type NeedInstanceType => typeof (IGameString);
  }
}
