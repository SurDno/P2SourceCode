// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.GameLogic.VMStateRef
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;

#nullable disable
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
