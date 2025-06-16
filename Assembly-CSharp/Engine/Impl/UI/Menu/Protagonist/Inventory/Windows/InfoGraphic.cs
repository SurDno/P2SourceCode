using System;
using System.Globalization;
using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Components;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Engine.Impl.UI.Menu.Protagonist.Inventory.Windows
{
  [DisallowMultipleComponent]
  public class InfoGraphic : UIControl
  {
    private float price = float.NaN;
    private IStorableComponent target;
    [SerializeField]
    [FormerlySerializedAs("_Unity_Background_Default")]
    private Image unityBackgroundDefault;
    [SerializeField]
    [FormerlySerializedAs("_Unity_Background_Price")]
    private Image unityBackgroundPrice;
    [SerializeField]
    [FormerlySerializedAs("_Unity_Image")]
    private Image unityImage;
    [SerializeField]
    [FormerlySerializedAs("_Unity_Information")]
    private Text unityInformation;
    [SerializeField]
    [FormerlySerializedAs("_Unity_Name")]
    private Text unityName;
    [SerializeField]
    [FormerlySerializedAs("_Unity_Price")]
    private Text unityPrice;

    public IStorableComponent Target
    {
      get => target;
      set
      {
        if (target == value)
          return;
        target = value;
        if (target == null || target.IsDisposed)
          return;
        LocalizationService service = ServiceLocator.GetService<LocalizationService>();
        unityName.text = service.GetText(target.Title);
        unityInformation.text = service.GetText(target.Tooltip);
        unityImage.sprite = ((StorableComponent) target).Placeholder.ImageInformation.Value;
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
        price = (float) Math.Round(value);
        if (!(unityPrice != null))
          return;
        unityPrice.text = value.ToString(CultureInfo.InvariantCulture);
      }
    }

    public static InfoGraphic Instantiate(bool withPrice, GameObject prefab)
    {
      GameObject gameObject = Instantiate(prefab);
      gameObject.name = prefab.name;
      InfoGraphic component = gameObject.GetComponent<InfoGraphic>();
      if (withPrice)
      {
        component.unityBackgroundPrice.gameObject.SetActive(true);
        component.unityBackgroundDefault.gameObject.SetActive(false);
        if (component.unityPrice != null)
          component.unityPrice.gameObject.SetActive(true);
      }
      else
      {
        component.unityBackgroundPrice.gameObject.SetActive(false);
        component.unityBackgroundDefault.gameObject.SetActive(true);
        if (component.unityPrice != null)
          component.unityPrice.gameObject.SetActive(false);
      }
      return component;
    }
  }
}
