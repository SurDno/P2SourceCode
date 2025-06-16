using Engine.Common.Services;
using Engine.Source.Services;
using Engine.Source.UI;

namespace Engine.Impl.UI.Controls
{
  public class OpenBoundCharactersWindowEventView : OpenWindowEventView<IBoundCharactersWindow>
  {
    protected override bool PrepareWindow()
    {
      InterfaceBlockingService service = ServiceLocator.GetService<InterfaceBlockingService>();
      return service == null || !service.BlockBoundsInterface;
    }
  }
}
