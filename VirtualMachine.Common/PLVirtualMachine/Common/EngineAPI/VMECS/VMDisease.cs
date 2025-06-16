// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMECS.VMDisease
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Cofe.Loggers;
using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using System;

#nullable disable
namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("DiseaseComponent", typeof (IDiseaseComponent))]
  public class VMDisease : VMEngineComponent<IDiseaseComponent>
  {
    public const string ComponentName = "DiseaseComponent";

    [Method("Set disease value", "value, delta time", "")]
    public void SetDiseaseValue(float value, GameTime delta)
    {
      this.Component.SetDiseaseValue(value, (TimeSpan) delta);
    }

    [Property("Disease value", "", false)]
    public float DiseaseValue
    {
      get
      {
        if (this.Component != null)
          return this.Component.DiseaseValue;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return 0.0f;
      }
    }
  }
}
