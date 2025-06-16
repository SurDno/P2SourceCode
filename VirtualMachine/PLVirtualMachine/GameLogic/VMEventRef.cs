// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.GameLogic.VMEventRef
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;

#nullable disable
namespace PLVirtualMachine.GameLogic
{
  [VMType("IEventRef")]
  [VMFactory(typeof (IEventRef))]
  public class VMEventRef : BaseRef, IEventRef, IRef, IVariable, INamed, IVMStringSerializable
  {
    public void Initialize(IEvent evnt) => this.LoadStaticInstance((IObject) evnt);

    public override EContextVariableCategory Category
    {
      get => EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_EVENT;
    }

    public IEvent Event
    {
      get
      {
        if (this.StaticInstance == null && this.BaseGuid > 0UL)
          this.LoadStaticInstance(IStaticDataContainer.StaticDataContainer.GetObjectByGuid(this.BaseGuid));
        return (IEvent) this.StaticInstance;
      }
    }

    public override VMType Type => new VMType(typeof (IEventRef));

    public override bool Empty => this.Event == null && base.Empty;

    protected override System.Type NeedInstanceType => typeof (IEvent);
  }
}
