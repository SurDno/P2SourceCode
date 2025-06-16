using Engine.Common.Services;
using Engine.Source.Services;
using Engine.Source.UI;

namespace Engine.Impl.UI.Controls
{
  public class OpenMindMapWindowEventView : OpenWindowEventView<IMMWindow>
  {
    protected override bool PrepareWindow()
    {
      if (ServiceLocator.GetService<ISimulation>().Player == null)
        return false;
      InterfaceBlockingService service = ServiceLocator.GetService<InterfaceBlockingService>();
      return service == null || !service.BlockMindMapInterface;
    }
  }
}
