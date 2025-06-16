using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons;
using Engine.Source.Components;
using Inspectors;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomizableObject : MonoBehaviour, IEntityAttachable, IChangeParameterListener
{
  [SerializeField]
  private CustomizableObject.Preset[] presets;
  private int presetIndex;
  private IParameter<int> parameter;

  [Inspected(Mutable = true)]
  [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
  public int PresetIndex
  {
    get => this.presetIndex;
    set
    {
      this.presetIndex = value;
      if (!Application.isPlaying)
        return;
      this.OnInvalidate();
    }
  }

  private void OnInvalidate()
  {
    CustomizableObject.Preset preset = ((IEnumerable<CustomizableObject.Preset>) this.presets).ElementAtOrDefault<CustomizableObject.Preset>(this.presetIndex);
    if (preset == null)
      return;
    for (int index = 0; index < preset.Infos.Length; ++index)
    {
      CustomizableObject.SubMeshInfo info = preset.Infos[index];
      if (!((UnityEngine.Object) info.Renderer == (UnityEngine.Object) null) && info.Materials != null)
        info.Renderer.materials = info.Materials;
    }
  }

  public void Attach(IEntity owner)
  {
    ParametersComponent component = owner.GetComponent<ParametersComponent>();
    if (component == null)
      return;
    IParameter<int> byName = component.GetByName<int>(ParameterNameEnum.Customization);
    if (byName != null)
    {
      byName.AddListener((IChangeParameterListener) this);
      this.OnParameterChanged((IParameter) byName);
    }
  }

  public void Detach()
  {
    if (this.parameter == null)
      return;
    this.parameter.RemoveListener((IChangeParameterListener) this);
    this.parameter = (IParameter<int>) null;
  }

  public void OnParameterChanged(IParameter parameter)
  {
    if (this.presets.Length == 0)
      return;
    this.PresetIndex = ((IParameter<int>) parameter).Value % this.presets.Length;
  }

  [Serializable]
  public class SubMeshInfo
  {
    public Renderer Renderer;
    public Material[] Materials;
  }

  [Serializable]
  public class Preset
  {
    public CustomizableObject.SubMeshInfo[] Infos;
  }
}
