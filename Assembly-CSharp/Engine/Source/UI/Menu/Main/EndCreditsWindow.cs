using Engine.Impl.UI.Menu.Main;
using System;

namespace Engine.Source.UI.Menu.Main
{
  public class EndCreditsWindow : CancelableSimpleWindow, IEndCreditsWindow, IWindow
  {
    protected override void RegisterLayer()
    {
      this.RegisterLayer<IEndCreditsWindow>((IEndCreditsWindow) this);
    }

    public override Type GetWindowType() => typeof (IEndCreditsWindow);
  }
}
