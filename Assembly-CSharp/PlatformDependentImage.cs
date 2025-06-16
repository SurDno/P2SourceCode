using InputServices;
using System;
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
  private Image imageObject = (Image) null;
  [SerializeField]
  private Text text;

  private void Awake() => this.imageObject = this.GetComponent<Image>();

  private void OnEnable()
  {
    InputService.Instance.onJoystickUsedChanged += new Action<bool>(this.SetupImage);
    this.SetupImage(InputService.Instance.JoystickUsed);
  }

  private void OnDisable()
  {
    InputService.Instance.onJoystickUsedChanged -= new Action<bool>(this.SetupImage);
  }

  private void SetupImage(bool joystick)
  {
    this.imageObject.sprite = joystick ? this.thumbnailXBox : this.thumbnail;
    ((RectTransform) this.transform).sizeDelta = this.imageObject.sprite.rect.size;
    if (!((UnityEngine.Object) this.text != (UnityEngine.Object) null))
      return;
    this.text.gameObject.SetActive(!joystick && (UnityEngine.Object) this.imageObject.sprite == (UnityEngine.Object) null);
  }
}
