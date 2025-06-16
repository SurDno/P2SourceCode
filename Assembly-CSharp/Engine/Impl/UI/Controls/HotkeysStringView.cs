// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.HotkeysStringView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Source.Services.Inputs;
using UnityEngine;

#nullable disable
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
