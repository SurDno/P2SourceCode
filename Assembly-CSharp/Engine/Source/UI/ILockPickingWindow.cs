using Engine.Common.Components;

namespace Engine.Source.UI
{
  public interface ILockPickingWindow : IWindow
  {
    IStorageComponent Actor { get; set; }

    IDoorComponent Target { get; set; }
  }
}
