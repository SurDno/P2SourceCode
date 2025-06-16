// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Menu.Protagonist.Inventory.Windows.InfoWindow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Components;
using Engine.Source.Inventory;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
namespace Engine.Impl.UI.Menu.Protagonist.Inventory.Windows
{
  [DisallowMultipleComponent]
  public class InfoWindow : UIControl
  {
    private IStorableComponent target;
    private float price;
    [SerializeField]
    private Image unityImage = (Image) null;
    [SerializeField]
    private bool useBigPicture = false;
    [SerializeField]
    private Text unityInformation = (Text) null;
    [SerializeField]
    private Text unityName = (Text) null;
    [SerializeField]
    private PriceItem priceItem;

    public IStorableComponent Target
    {
      get => this.target;
      set
      {
        if (this.target == value)
          return;
        this.Clear();
        this.target = value;
        if (this.target == null || this.target.IsDisposed)
          return;
        LocalizationService service = ServiceLocator.GetService<LocalizationService>();
        this.unityName.text = service.GetText(this.target.Title);
        this.unityInformation.text = service.GetText(this.target.Tooltip);
        InventoryPlaceholder placeholder = ((StorableComponent) this.target).Placeholder;
        this.unityImage.sprite = this.useBigPicture ? placeholder.ImageInformation.Value : placeholder.ImageInformation.Value;
        if ((Object) this.unityImage.sprite == (Object) null)
          this.unityImage.color = Color.black;
        else
          this.unityImage.color = Color.white;
      }
    }

    public float Price
    {
      get => this.price;
      set
      {
        this.price = value;
        if (!((Object) this.priceItem != (Object) null))
          return;
        this.priceItem.SetPrice((int) this.price);
      }
    }

    private void Clear()
    {
      this.unityName.text = "";
      this.unityInformation.text = "";
    }

    public static InfoWindow Instantiate(bool withPrice, GameObject prefab)
    {
      GameObject gameObject = Object.Instantiate<GameObject>(prefab);
      gameObject.name = prefab.name;
      InfoWindow component = gameObject.GetComponent<InfoWindow>();
      if ((Object) component.priceItem != (Object) null)
        component.priceItem.gameObject.SetActive(withPrice);
      return component;
    }
  }
}
