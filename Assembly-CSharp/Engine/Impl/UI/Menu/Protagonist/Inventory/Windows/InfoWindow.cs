using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Components;
using Engine.Source.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace Engine.Impl.UI.Menu.Protagonist.Inventory.Windows
{
  [DisallowMultipleComponent]
  public class InfoWindow : UIControl
  {
    private IStorableComponent target;
    private float price;
    [SerializeField]
    private Image unityImage;
    [SerializeField]
    private bool useBigPicture;
    [SerializeField]
    private Text unityInformation;
    [SerializeField]
    private Text unityName;
    [SerializeField]
    private PriceItem priceItem;

    public IStorableComponent Target
    {
      get => target;
      set
      {
        if (target == value)
          return;
        Clear();
        target = value;
        if (target == null || target.IsDisposed)
          return;
        LocalizationService service = ServiceLocator.GetService<LocalizationService>();
        unityName.text = service.GetText(target.Title);
        unityInformation.text = service.GetText(target.Tooltip);
        InventoryPlaceholder placeholder = ((StorableComponent) target).Placeholder;
        unityImage.sprite = useBigPicture ? placeholder.ImageInformation.Value : placeholder.ImageInformation.Value;
        if (unityImage.sprite == null)
          unityImage.color = Color.black;
        else
          unityImage.color = Color.white;
      }
    }

    public float Price
    {
      get => price;
      set
      {
        price = value;
        if (!(priceItem != null))
          return;
        priceItem.SetPrice((int) price);
      }
    }

    private void Clear()
    {
      unityName.text = "";
      unityInformation.text = "";
    }

    public static InfoWindow Instantiate(bool withPrice, GameObject prefab)
    {
      GameObject gameObject = Instantiate(prefab);
      gameObject.name = prefab.name;
      InfoWindow component = gameObject.GetComponent<InfoWindow>();
      if (component.priceItem != null)
        component.priceItem.gameObject.SetActive(withPrice);
      return component;
    }
  }
}
