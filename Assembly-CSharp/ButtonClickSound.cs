[RequireComponent(typeof (Button))]
public class ButtonClickSound : MonoBehaviour
{
  private void Awake()
  {
    this.GetComponent<Button>().onClick.AddListener(new UnityAction(PlaySound));
  }

  private void PlaySound() => MonoBehaviourInstance<UISounds>.Instance.PlayClickSound();
}
