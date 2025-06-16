using Cofe.Utility;
using Engine.Common.Services;
using Engine.Impl.Services;
using System;
using UnityEngine;

namespace Engine.Source.Commons
{
  public class LabelService : InstanceByRequest<LabelService>
  {
    private string label;
    private string projectName;
    private int dataVersion;

    public event Action OnInvalidate;

    public string Label
    {
      get
      {
        if (this.label == null)
          this.Invalidate();
        return this.label;
      }
    }

    public void Invalidate()
    {
      this.label = "";
      if (ScriptableObjectInstance<BuildData>.Instance.Label == null)
        return;
      this.projectName = "Unknown";
      this.dataVersion = 0;
      VirtualMachineController service = ServiceLocator.GetService<VirtualMachineController>();
      if (service != null)
      {
        if (!service.ProjectName.IsNullOrEmpty())
          this.projectName = service.ProjectName;
        this.dataVersion = service.DataVersion;
      }
      this.label = ScriptableObjectInstance<BuildData>.Instance.Label.Replace("{Version}", Application.version).Replace("{Branch}", ScriptableObjectInstance<BuildData>.Instance.Branch).Replace("{ProjectName}", this.projectName).Replace("{DataVersion}", this.dataVersion.ToString());
      Action onInvalidate = this.OnInvalidate;
      if (onInvalidate == null)
        return;
      onInvalidate();
    }
  }
}
