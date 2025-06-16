using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class AnchorGameActionView : GameActionViewBase
  {
    [SerializeField]
    private GameActionView prefab;
    private GameActionView instance;

    private void Awake()
    {
      if (!((Object) this.prefab != (Object) null) || this.GetValue() == 0)
        return;
      this.instance = Object.Instantiate<GameActionView>(this.prefab, this.transform, false);
      this.instance.SetValue(this.GetValue(), true);
    }

    protected override void ApplyValue(bool instant)
    {
      if (this.GetValue() != 0)
      {
        this.instance?.SetValue(this.GetValue(), instant);
        this.instance?.gameObject.SetActive(true);
      }
      else
        this.instance?.gameObject.SetActive(false);
    }
  }
}
