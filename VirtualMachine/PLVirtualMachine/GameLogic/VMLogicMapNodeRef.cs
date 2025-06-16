// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.GameLogic.VMLogicMapNodeRef
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;

#nullable disable
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
    public void Initialize(IGraphObject lmNode) => this.LoadStaticInstance((IObject) lmNode);

    public override EContextVariableCategory Category
    {
      get => EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_STATE;
    }

    public IGraphObject LogicMapNode
    {
      get
      {
        if (this.StaticInstance == null && this.BaseGuid > 0UL)
          this.LoadStaticInstance(IStaticDataContainer.StaticDataContainer.GetObjectByGuid(this.BaseGuid));
        return (IGraphObject) this.StaticInstance;
      }
    }

    public override VMType Type => VMType.CreateStateSpecialType(this.LogicMapNode);

    public override bool Empty => this.LogicMapNode == null && base.Empty;

    protected override System.Type NeedInstanceType => typeof (IGraphObject);
  }
}
