// Decompiled with JetBrains decompiler
// Type: KeyCodeStringView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Impl.UI.Controls;
using InputServices;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
public class KeyCodeStringView : StringView
{
  [SerializeField]
  private Image image;
  [SerializeField]
  private GameObject textObject;
  [SerializeField]
  private Text text;
  [SerializeField]
  private StringSpriteMap mouseMap;
  [SerializeField]
  private StringSpriteMap xboxMap;
  [SerializeField]
  private StringSpriteMap psMap;
  [SerializeField]
  private StringMap keyMap;

  public override void SkipAnimation()
  {
  }

  private void OnEnable() => this.Apply(InputService.Instance.JoystickUsed);

  private void OnDisable()
  {
  }

  protected override void ApplyStringValue() => this.Apply(InputService.Instance.JoystickUsed);

  private void Apply(bool joystick)
  {
    Sprite sprite = joystick ? this.xboxMap?.GetValue(this.StringValue) : this.mouseMap?.GetValue(this.StringValue);
    if ((Object) sprite != (Object) null)
    {
      this.textObject.gameObject.SetActive(false);
      this.image.sprite = sprite;
      this.image.gameObject.SetActive(true);
      ((RectTransform) this.image.transform).sizeDelta = this.image.sprite.rect.size;
    }
    else
    {
      string str = this.keyMap?.GetValue(this.StringValue) ?? this.StringValue;
      this.image.gameObject.SetActive(false);
      this.text.text = str;
      this.textObject.gameObject.SetActive(true);
    }
  }
}
