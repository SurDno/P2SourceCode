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
    if (view == null)
      return;
    if (Value == null)
    {
      view.StringValue = null;
    }
    else
    {
      RepairableSettings settings = Value.GetComponent<RepairableComponent>()?.Settings;
      if (settings != null && !string.IsNullOrEmpty(settings.VerbOverride))
        view.StringValue = ServiceLocator.GetService<LocalizationService>().GetText(settings.VerbOverride);
      else
        view.StringValue = defaultVerb;
    }
  }

  public override void SkipAnimation() => view?.SkipAnimation();
}
