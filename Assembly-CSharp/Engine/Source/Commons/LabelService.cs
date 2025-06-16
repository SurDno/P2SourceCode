using System;
using Cofe.Utility;
using Engine.Common.Services;
using Engine.Impl.Services;
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
        if (label == null)
          Invalidate();
        return label;
      }
    }

    public void Invalidate()
    {
      label = "";
      if (ScriptableObjectInstance<BuildData>.Instance.Label == null)
        return;
      projectName = "Unknown";
      dataVersion = 0;
      VirtualMachineController service = ServiceLocator.GetService<VirtualMachineController>();
      if (service != null)
      {
        if (!service.ProjectName.IsNullOrEmpty())
          projectName = service.ProjectName;
        dataVersion = service.DataVersion;
      }
      label = ScriptableObjectInstance<BuildData>.Instance.Label.Replace("{Version}", Application.version).Replace("{Branch}", ScriptableObjectInstance<BuildData>.Instance.Branch).Replace("{ProjectName}", projectName).Replace("{DataVersion}", dataVersion.ToString());
      Action onInvalidate = OnInvalidate;
      if (onInvalidate == null)
        return;
      onInvalidate();
    }
  }
}
