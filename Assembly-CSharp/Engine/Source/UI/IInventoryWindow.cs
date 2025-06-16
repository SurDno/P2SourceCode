using Engine.Common.Components;

namespace Engine.Source.UI
{
  public interface IInventoryWindow : IWindow, IPauseMenu
  {
    IStorageComponent Actor { get; set; }
  }
}
