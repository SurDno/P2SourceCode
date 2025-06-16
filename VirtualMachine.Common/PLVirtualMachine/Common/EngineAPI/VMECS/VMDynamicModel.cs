// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMECS.VMDynamicModel
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Engine.Common;
using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

#nullable disable
namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("Model", typeof (IDynamicModelComponent))]
  public class VMDynamicModel : VMEngineComponent<IDynamicModelComponent>
  {
    public const string ComponentName = "Model";

    [Property("Model name", "", false)]
    public IModel Object
    {
      get => this.Component.Model;
      set => this.Component.Model = value;
    }
  }
}
