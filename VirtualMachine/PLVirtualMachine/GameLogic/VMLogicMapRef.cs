// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.GameLogic.VMLogicMapRef
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;

#nullable disable
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
