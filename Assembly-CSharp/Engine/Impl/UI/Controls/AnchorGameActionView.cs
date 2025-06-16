namespace Engine.Impl.UI.Controls
{
  public class AnchorGameActionView : GameActionViewBase
  {
    [SerializeField]
    private GameActionView prefab;
    private GameActionView instance;

    private void Awake()
    {
      if (!((Object) prefab != (Object) null) || GetValue() == 0)
        return;
      instance = Object.Instantiate<GameActionView>(prefab, this.transform, false);
      instance.SetValue(GetValue(), true);
    }

    protected override void ApplyValue(bool instant)
    {
      if (GetValue() != 0)
      {
        instance?.SetValue(GetValue(), instant);
        instance?.gameObject.SetActive(true);
      }
      else
        instance?.gameObject.SetActive(false);
    }
  }
}
