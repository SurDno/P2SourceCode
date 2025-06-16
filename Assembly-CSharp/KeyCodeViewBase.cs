using UnityEngine;

public abstract class KeyCodeViewBase : KeyCodeView
{
  [SerializeField]
  private KeyCode value;

  private void OnValidate()
  {
    if (Application.isPlaying)
      return;
    this.ApplyValue(true);
  }

  public override KeyCode GetValue() => this.value;

  public override void SetValue(KeyCode value, bool instant)
  {
    if (this.value == value)
      return;
    this.value = value;
    this.ApplyValue(instant);
  }

  protected abstract void ApplyValue(bool instant);
}
