// Decompiled with JetBrains decompiler
// Type: AdditionalReflectionsEnabled
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Impl.UI.Controls;
using Engine.Source.Commons;
using Engine.Source.Settings;
using UnityEngine;

public class AdditionalReflectionsEnabled : MonoBehaviour
{
  [SerializeField]
  private HideableView view;

  private void UpdateValue()
  {
    if (!(view != null))
      return;
    view.Visible = InstanceByRequest<GraphicsGameSettings>.Instance.AdditionalReflections.Value;
  }

  private void OnEnable()
  {
    UpdateValue();
    InstanceByRequest<GraphicsGameSettings>.Instance.OnApply += UpdateValue;
  }

  private void OnDisable()
  {
    InstanceByRequest<GraphicsGameSettings>.Instance.OnApply -= UpdateValue;
  }
}
