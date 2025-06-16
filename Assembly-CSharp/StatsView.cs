using Engine.Common;
using Engine.Common.Services;
using Engine.Impl.UI.Controls;

public class StatsView : MonoBehaviour
{
  [SerializeField]
  private EntityView playerEntityView;
  [SerializeField]
  private HideableView fullVersionView;

  private void Start()
  {
    if ((Object) fullVersionView != (Object) null)
      fullVersionView.SkipAnimation();
    if (!((Object) playerEntityView != (Object) null))
      return;
    playerEntityView.Value = ServiceLocator.GetService<ISimulation>()?.Player;
    playerEntityView.SkipAnimation();
  }

  private void Update()
  {
    if (!((Object) playerEntityView != (Object) null))
      return;
    IEntity player = ServiceLocator.GetService<ISimulation>()?.Player;
    if (playerEntityView.Value != player)
      playerEntityView.Value = player;
  }

  public void SetFullVersion(bool value)
  {
    if (!((Object) fullVersionView != (Object) null))
      return;
    fullVersionView.Visible = value;
  }
}
