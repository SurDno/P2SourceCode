// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.LocalizerStringView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Localization;
using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class LocalizerStringView : StringView
  {
    [SerializeField]
    private Localizer localizer;

    public override void SkipAnimation()
    {
    }

    protected override void ApplyStringValue()
    {
      if (!((Object) this.localizer != (Object) null))
        return;
      this.localizer.Signature = this.StringValue;
    }
  }
}
