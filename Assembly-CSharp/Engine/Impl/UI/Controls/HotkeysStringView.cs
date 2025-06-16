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
      if (!((Object) this.view != (Object) null))
        return;
      this.view.SkipAnimation();
    }

    protected override void ApplyStringValue()
    {
      if (!((Object) this.view != (Object) null))
        return;
      this.view.StringValue = ServiceLocator.GetService<GameActionService>() == null ? this.StringValue : TextHelper.ReplaceTags(this.StringValue, "<b><color=#e4b450>", "</color></b>");
    }
  }
}
