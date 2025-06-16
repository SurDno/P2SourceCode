using InputServices;

[RequireComponent(typeof (Image))]
public class PlatformDependentImage : MonoBehaviour
{
  [SerializeField]
  private Sprite thumbnail;
  [SerializeField]
  private Sprite thumbnailXBox;
  [SerializeField]
  private Sprite thumbnailPS4;
  private Image imageObject = (Image) null;
  [SerializeField]
  private Text text;

  private void Awake() => imageObject = this.GetComponent<Image>();

  private void OnEnable()
  {
    InputService.Instance.onJoystickUsedChanged += SetupImage;
    SetupImage(InputService.Instance.JoystickUsed);
  }

  private void OnDisable()
  {
    InputService.Instance.onJoystickUsedChanged -= SetupImage;
  }

  private void SetupImage(bool joystick)
  {
    imageObject.sprite = joystick ? thumbnailXBox : thumbnail;
    ((RectTransform) this.transform).sizeDelta = imageObject.sprite.rect.size;
    if (!((UnityEngine.Object) text != (UnityEngine.Object) null))
      return;
    text.gameObject.SetActive(!joystick && (UnityEngine.Object) imageObject.sprite == (UnityEngine.Object) null);
  }
}
