// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.OptimizationService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Source.Settings.External;
using UnityEngine;

#nullable disable
namespace Engine.Source.Services
{
  [RuntimeService(new System.Type[] {typeof (IOptimizationService)})]
  public class OptimizationService : IOptimizationService
  {
    public bool FrameHasSpike { get; set; }

    public bool LazyFsm => ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.LazyFsm;

    public bool IsUnity => Application.isEditor;
  }
}
