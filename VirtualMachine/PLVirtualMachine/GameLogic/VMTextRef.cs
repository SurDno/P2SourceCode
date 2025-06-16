// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.GameLogic.VMTextRef
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;

#nullable disable
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
