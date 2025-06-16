using Engine.Common.Components;

namespace Engine.Source.UI
{
  public interface ICraftBreweryWindow : IWindow
  {
    IStorageComponent Actor { get; set; }

    IStorageComponent Target { get; set; }
  }
}
