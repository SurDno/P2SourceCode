using Engine.Common.Components;

namespace Engine.Source.UI
{
  public interface ICraftMixtureWindow : IWindow
  {
    IStorageComponent Actor { get; set; }

    IStorageComponent Target { get; set; }
  }
}
