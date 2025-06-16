using Engine.Common.Services;
using Engine.Impl.Services;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof (Button))]
public class WindowCloseButton : MonoBehaviour
{
  private void Awake()
  {
    GetComponent<Button>().onClick.AddListener(CloseActiveWindow);
  }

  private void CloseActiveWindow() => ServiceLocator.GetService<UIService>().Pop();
}
