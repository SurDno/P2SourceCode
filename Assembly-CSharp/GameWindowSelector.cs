using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Services;

public class GameWindowSelector : MonoBehaviour
{
  [SerializeField]
  public HideableView mapAvailableView;
  [SerializeField]
  public HideableView mindMapAvailableView;
  [SerializeField]
  public HideableView inventoryAvailableView;
  [SerializeField]
  public HideableView boundCharactersAvailableView;

  private void OnEnable()
  {
    InterfaceBlockingService service = ServiceLocator.GetService<InterfaceBlockingService>();
    if (service == null)
      return;
    if ((Object) mapAvailableView != (Object) null)
      mapAvailableView.Visible = !service.BlockMapInterface;
    if ((Object) mindMapAvailableView != (Object) null)
      mindMapAvailableView.Visible = !service.BlockMindMapInterface;
    if ((Object) inventoryAvailableView != (Object) null)
      inventoryAvailableView.Visible = !service.BlockInventoryInterface;
    if ((Object) boundCharactersAvailableView != (Object) null)
      boundCharactersAvailableView.Visible = !service.BlockBoundsInterface;
  }
}
