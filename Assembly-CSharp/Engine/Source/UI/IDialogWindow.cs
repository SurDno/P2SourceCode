using Engine.Common.Components;

namespace Engine.Source.UI
{
  public interface IDialogWindow : IWindow
  {
    ISpeakingComponent Actor { get; set; }

    ISpeakingComponent Target { get; set; }
  }
}
