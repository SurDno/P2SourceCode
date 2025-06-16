using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Services;
using UnityEngine;

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
    if (mapAvailableView != null)
      mapAvailableView.Visible = !service.BlockMapInterface;
    if (mindMapAvailableView != null)
      mindMapAvailableView.Visible = !service.BlockMindMapInterface;
    if (inventoryAvailableView != null)
      inventoryAvailableView.Visible = !service.BlockInventoryInterface;
    if (boundCharactersAvailableView != null)
      boundCharactersAvailableView.Visible = !service.BlockBoundsInterface;
  }
}
