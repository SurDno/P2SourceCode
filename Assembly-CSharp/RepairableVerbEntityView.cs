// Decompiled with JetBrains decompiler
// Type: RepairableVerbEntityView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using Engine.Source.Components.Repairing;
using UnityEngine;

#nullable disable
public class RepairableVerbEntityView : EntityViewBase
{
  [SerializeField]
  private StringView view;
  [SerializeField]
  private string defaultVerb;

  protected override void ApplyValue()
  {
    if ((Object) this.view == (Object) null)
      return;
    if (this.Value == null)
    {
      this.view.StringValue = (string) null;
    }
    else
    {
      RepairableSettings settings = this.Value.GetComponent<RepairableComponent>()?.Settings;
      if (settings != null && !string.IsNullOrEmpty(settings.VerbOverride))
        this.view.StringValue = ServiceLocator.GetService<LocalizationService>().GetText(settings.VerbOverride);
      else
        this.view.StringValue = this.defaultVerb;
    }
  }

  public override void SkipAnimation() => this.view?.SkipAnimation();
}
