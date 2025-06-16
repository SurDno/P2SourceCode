using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public abstract class SpriteViewBase : SpriteView
  {
    [SerializeField]
    private Sprite value;

    public override Sprite GetValue() => this.value;

    private void OnValidate()
    {
      if (Application.isPlaying)
        return;
      this.ApplyValue(true);
    }

    public override void SetValue(Sprite value, bool instant)
    {
      if ((Object) this.value == (Object) value)
        return;
      this.value = value;
      this.ApplyValue(instant);
    }

    protected abstract void ApplyValue(bool instant);
  }
}
