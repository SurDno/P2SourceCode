using Engine.Common.Services;
using Engine.Source.Settings.External;
using UnityEngine;

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
