using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Engine.Impl.UI.Controls
{
  [RequireComponent(typeof (Button))]
  public class ButtonListener : MonoBehaviour
  {
    [SerializeField]
    private EventView view;

    private void Awake()
    {
      this.GetComponent<Button>().onClick.AddListener(new UnityAction(this.OnClick));
    }

    private void OnClick() => this.view?.Invoke();
  }
}
