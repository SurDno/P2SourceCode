// Decompiled with JetBrains decompiler
// Type: BreakableToolView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Source.Components;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
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
    get => this.storable;
    set
    {
      if (this.storable == value)
        return;
      this.storable = value;
      Sprite sprite = this.storable?.Placeholder?.ImageInventorySlot.Value;
      if ((Object) sprite == (Object) null)
        this.storableImage.gameObject.SetActive(false);
      this.storableImage.sprite = sprite;
      if ((Object) sprite != (Object) null)
        this.storableImage.gameObject.SetActive(true);
      this.UpdateFill();
    }
  }

  private void Initialize()
  {
    if (this.initialized)
      return;
    this.baseFillSize = this.fillImage.sizeDelta;
    this.initialized = true;
  }

  private void OnEnable() => this.UpdateFill();

  private void Update() => this.UpdateFill();

  private void UpdateFill()
  {
    this.Initialize();
    float f = BreakableToolView.GetDurability((IStorableComponent) this.storable);
    if (float.IsNaN(f))
      f = 1f;
    this.fillImage.sizeDelta = new Vector2(this.baseFillSize.x, this.baseFillSize.y * (1f - f));
    this.brokenImage.SetActive((double) f == 0.0);
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
