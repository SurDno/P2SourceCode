using Engine.Common.Components;

namespace Engine.Source.UI
{
  public interface IInvestigationWindow : IWindow, IPauseMenu
  {
    IStorageComponent Actor { get; set; }

    IStorableComponent Target { get; set; }
  }
}
