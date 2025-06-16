using SRF;

namespace SRDebugger.UI.Controls
{
  [RequireComponent(typeof (RectTransform))]
  public class ConsoleEntryView : SRMonoBehaviour
  {
    [SerializeField]
    private InputField message;

    public void SetData(string data) => message.text = data;
  }
}
