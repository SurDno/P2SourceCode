using Engine.Common.Services;
using Engine.Impl.Services;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof (Button))]
public class WindowCloseButton : MonoBehaviour
{
  private void Awake()
  {
    this.GetComponent<Button>().onClick.AddListener(new UnityAction(this.CloseActiveWindow));
  }

  private void CloseActiveWindow() => ServiceLocator.GetService<UIService>().Pop();
}
