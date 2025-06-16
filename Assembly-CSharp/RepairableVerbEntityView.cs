using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using Engine.Source.Components.Repairing;
using UnityEngine;

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
