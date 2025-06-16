using InputServices;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof (Image))]
public class PlatformDependentImage : MonoBehaviour
{
  [SerializeField]
  private Sprite thumbnail;
  [SerializeField]
  private Sprite thumbnailXBox;
  [SerializeField]
  private Sprite thumbnailPS4;
  private Image imageObject;
  [SerializeField]
  private Text text;

  private void Awake() => imageObject = GetComponent<Image>();

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
    ((RectTransform) transform).sizeDelta = imageObject.sprite.rect.size;
    if (!(text != null))
      return;
    text.gameObject.SetActive(!joystick && imageObject.sprite == null);
  }
}
