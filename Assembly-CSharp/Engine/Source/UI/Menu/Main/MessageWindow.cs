using Engine.Impl.UI.Controls;
using Engine.Impl.UI.Menu.Main;

namespace Engine.Source.UI.Menu.Main
{
  public class MessageWindow : SimpleWindow, IMessageWindow, IWindow
  {
    [SerializeField]
    private StringView textView;

    protected override void RegisterLayer()
    {
      RegisterLayer((IMessageWindow) this);
    }

    public void SetMessage(string text) => textView.StringValue = text;
  }
}
