// Decompiled with JetBrains decompiler
// Type: StatsView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using UnityEngine;

#nullable disable
public class StatsView : MonoBehaviour
{
  [SerializeField]
  private EntityView playerEntityView;
  [SerializeField]
  private HideableView fullVersionView;

  private void Start()
  {
    if ((Object) this.fullVersionView != (Object) null)
      this.fullVersionView.SkipAnimation();
    if (!((Object) this.playerEntityView != (Object) null))
      return;
    this.playerEntityView.Value = ServiceLocator.GetService<ISimulation>()?.Player;
    this.playerEntityView.SkipAnimation();
  }

  private void Update()
  {
    if (!((Object) this.playerEntityView != (Object) null))
      return;
    IEntity player = ServiceLocator.GetService<ISimulation>()?.Player;
    if (this.playerEntityView.Value != player)
      this.playerEntityView.Value = player;
  }

  public void SetFullVersion(bool value)
  {
    if (!((Object) this.fullVersionView != (Object) null))
      return;
    this.fullVersionView.Visible = value;
  }
}
