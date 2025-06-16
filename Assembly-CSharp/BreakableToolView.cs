using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Source.Components;

public class BreakableToolView : ItemView
{
  [SerializeField]
  private Image storableImage;
  [SerializeField]
  private RectTransform fillImage;
  [SerializeField]
  private GameObject brokenImage;
  private Vector2 baseFillSize;
  private StorableComponent storable;
  private bool initialized;

  public override StorableComponent Storable
  {
    get => storable;
    set
    {
      if (storable == value)
        return;
      storable = value;
      Sprite sprite = storable?.Placeholder?.ImageInventorySlot.Value;
      if ((Object) sprite == (Object) null)
        storableImage.gameObject.SetActive(false);
      storableImage.sprite = sprite;
      if ((Object) sprite != (Object) null)
        storableImage.gameObject.SetActive(true);
      UpdateFill();
    }
  }

  private void Initialize()
  {
    if (initialized)
      return;
    baseFillSize = fillImage.sizeDelta;
    initialized = true;
  }

  private void OnEnable() => UpdateFill();

  private void Update() => UpdateFill();

  private void UpdateFill()
  {
    Initialize();
    float f = GetDurability(storable);
    if (float.IsNaN(f))
      f = 1f;
    fillImage.sizeDelta = new Vector2(baseFillSize.x, baseFillSize.y * (1f - f));
    brokenImage.SetActive(f == 0.0);
  }

  public static float GetDurability(IStorableComponent storable)
  {
    if (storable == null)
      return float.NaN;
    ParametersComponent component = storable.GetComponent<ParametersComponent>();
    if (component == null)
      return float.NaN;
    IParameter<float> byName = component.GetByName<float>(ParameterNameEnum.Durability);
    return byName == null ? float.NaN : byName.Value;
  }
}
