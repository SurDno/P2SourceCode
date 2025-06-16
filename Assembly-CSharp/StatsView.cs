using Engine.Common;
using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using UnityEngine;

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
