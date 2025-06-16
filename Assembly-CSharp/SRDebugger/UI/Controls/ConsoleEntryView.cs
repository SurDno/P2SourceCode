using SRF;
using UnityEngine;
using UnityEngine.UI;

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
