using Engine.Impl.UI.Controls;
using Engine.Source.Commons;
using Engine.Source.Settings;
using System;
using UnityEngine;

public class AdditionalReflectionsEnabled : MonoBehaviour
{
  [SerializeField]
  private HideableView view;

  private void UpdateValue()
  {
    if (!((UnityEngine.Object) this.view != (UnityEngine.Object) null))
      return;
    this.view.Visible = InstanceByRequest<GraphicsGameSettings>.Instance.AdditionalReflections.Value;
  }

  private void OnEnable()
  {
    this.UpdateValue();
    InstanceByRequest<GraphicsGameSettings>.Instance.OnApply += new Action(this.UpdateValue);
  }

  private void OnDisable()
  {
    InstanceByRequest<GraphicsGameSettings>.Instance.OnApply -= new Action(this.UpdateValue);
  }
}
