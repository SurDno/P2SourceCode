// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMECS.VMScene
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

#nullable disable
namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("Scene", null)]
  public class VMScene : VMComponent
  {
    public const string ComponentName = "Scene";

    public override void Initialize(VMBaseEntity parent) => base.Initialize(parent);
  }
}
