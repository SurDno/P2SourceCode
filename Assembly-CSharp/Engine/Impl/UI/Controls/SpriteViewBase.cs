using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public abstract class SpriteViewBase : SpriteView
  {
    [SerializeField]
    private Sprite value;

    public override Sprite GetValue() => value;

    private void OnValidate()
    {
      if (Application.isPlaying)
        return;
      ApplyValue(true);
    }

    public override void SetValue(Sprite value, bool instant)
    {
      if (this.value == value)
        return;
      this.value = value;
      ApplyValue(instant);
    }

    protected abstract void ApplyValue(bool instant);
  }
}
