// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.ProfileStringView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Services.Profiles;
using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class ProfileStringView : StringView
  {
    [SerializeField]
    private StringView view;
    [SerializeField]
    private string formatTag;

    public override void SkipAnimation()
    {
      if (!((Object) this.view != (Object) null))
        return;
      this.view.SkipAnimation();
    }

    protected override void ApplyStringValue()
    {
      if (!((Object) this.view != (Object) null) || !Application.isPlaying)
        return;
      this.view.StringValue = ProfilesUtility.ConvertProfileName(this.StringValue, this.formatTag);
    }
  }
}
