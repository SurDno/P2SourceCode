using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Services;
using Engine.Source.UI;

namespace Engine.Impl.UI.Controls
{
  public class OpenInventoryWindowEventView : OpenWindowEventView<IInventoryWindow>
  {
    protected override bool PrepareWindow()
    {
      IStorageComponent component = ServiceLocator.GetService<ISimulation>().Player?.GetComponent<IStorageComponent>();
      if (component == null)
        return false;
      InterfaceBlockingService service = ServiceLocator.GetService<InterfaceBlockingService>();
      if (service != null && service.BlockInventoryInterface)
        return false;
      ServiceLocator.GetService<UIService>().Get<IInventoryWindow>().Actor = component;
      return true;
    }
  }
}
