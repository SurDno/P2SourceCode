namespace Engine.Impl.UI.Controls
{
  [RequireComponent(typeof (Button))]
  public class ButtonListener : MonoBehaviour
  {
    [SerializeField]
    private EventView view;

    private void Awake()
    {
      this.GetComponent<Button>().onClick.AddListener(new UnityAction(OnClick));
    }

    private void OnClick() => view?.Invoke();
  }
}
