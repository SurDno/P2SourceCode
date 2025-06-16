[RequireComponent(typeof (Button))]
public class OpenURLButton : MonoBehaviour
{
  [SerializeField]
  private string url;

  private void Awake()
  {
    this.GetComponent<Button>().onClick.AddListener(new UnityAction(OnClick));
  }

  private void OnClick() => Application.OpenURL(url);
}
