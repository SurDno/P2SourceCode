// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.LocalizationStringView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Impl.Services;
using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class LocalizationStringView : StringView
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
      if ((Object) this.view == (Object) null)
        return;
      if (Application.isPlaying)
      {
        LocalizationService service = ServiceLocator.GetService<LocalizationService>();
        if (service != null)
        {
          this.view.StringValue = service.GetText(this.StringValue);
          return;
        }
      }
      this.view.StringValue = this.StringValue;
    }
  }
}
