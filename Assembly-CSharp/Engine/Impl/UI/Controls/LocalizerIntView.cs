// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.LocalizerIntView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Localization;
using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class LocalizerIntView : IntView
  {
    [SerializeField]
    private Localizer localizer;
    [SerializeField]
    private string signaturePrefix;
    [SerializeField]
    private string signatureSuffix;

    public override void SkipAnimation()
    {
    }

    protected override void ApplyIntValue()
    {
      if ((Object) this.localizer == (Object) null)
        return;
      this.localizer.Signature = this.signaturePrefix + this.IntValue.ToString() + this.signatureSuffix;
    }
  }
}
