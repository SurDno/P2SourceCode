using Engine.Common.Services;
using Engine.Source.Services.Inputs;
using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class HotkeysStringView : StringView
  {
    [SerializeField]
    private StringView view;

    public override void SkipAnimation()
    {
      if (!(view != null))
        return;
      view.SkipAnimation();
    }

    protected override void ApplyStringValue()
    {
      if (!(view != null))
        return;
      view.StringValue = ServiceLocator.GetService<GameActionService>() == null ? StringValue : TextHelper.ReplaceTags(StringValue, "<b><color=#e4b450>", "</color></b>");
    }
  }
}
