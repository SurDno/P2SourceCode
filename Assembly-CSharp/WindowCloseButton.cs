using Engine.Common.Services;
using Engine.Impl.Services;

[RequireComponent(typeof (Button))]
public class WindowCloseButton : MonoBehaviour
{
  private void Awake()
  {
    this.GetComponent<Button>().onClick.AddListener(new UnityAction(CloseActiveWindow));
  }

  private void CloseActiveWindow() => ServiceLocator.GetService<UIService>().Pop();
}
