// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.ProgressReputationName
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components.Regions;
using RegionReputation;
using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class ProgressReputationName : ProgressView
  {
    [SerializeField]
    private StringView view;
    [SerializeField]
    private RegionReputationNames settings;
    [SerializeField]
    private RegionEnum regionName;

    public override void SkipAnimation() => this.view?.SkipAnimation();

    protected override void ApplyProgress()
    {
      if (!((Object) this.view != (Object) null) || !((Object) this.settings != (Object) null))
        return;
      this.view.StringValue = this.settings.GetReputationName(this.regionName, this.Progress);
    }
  }
}
