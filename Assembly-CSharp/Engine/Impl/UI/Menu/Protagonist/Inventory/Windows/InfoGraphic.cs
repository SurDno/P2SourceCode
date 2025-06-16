using System;
using System.Globalization;
using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Components;

namespace Engine.Impl.UI.Menu.Protagonist.Inventory.Windows
{
  [DisallowMultipleComponent]
  public class InfoGraphic : UIControl
  {
    private float price = float.NaN;
    private IStorableComponent target;
    [SerializeField]
    [FormerlySerializedAs("_Unity_Background_Default")]
    private Image unityBackgroundDefault = (Image) null;
    [SerializeField]
    [FormerlySerializedAs("_Unity_Background_Price")]
    private Image unityBackgroundPrice = (Image) null;
    [SerializeField]
    [FormerlySerializedAs("_Unity_Image")]
    private Image unityImage = (Image) null;
    [SerializeField]
    [FormerlySerializedAs("_Unity_Information")]
    private Text unityInformation = (Text) null;
    [SerializeField]
    [FormerlySerializedAs("_Unity_Name")]
    private Text unityName = (Text) null;
    [SerializeField]
    [FormerlySerializedAs("_Unity_Price")]
    private Text unityPrice = (Text) null;

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
        if ((UnityEngine.Object) unityImage.sprite == (UnityEngine.Object) null)
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
        if (!((UnityEngine.Object) unityPrice != (UnityEngine.Object) null))
          return;
        unityPrice.text = value.ToString(CultureInfo.InvariantCulture);
      }
    }

    public static InfoGraphic Instantiate(bool withPrice, GameObject prefab)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
      gameObject.name = prefab.name;
      InfoGraphic component = gameObject.GetComponent<InfoGraphic>();
      if (withPrice)
      {
        component.unityBackgroundPrice.gameObject.SetActive(true);
        component.unityBackgroundDefault.gameObject.SetActive(false);
        if ((UnityEngine.Object) component.unityPrice != (UnityEngine.Object) null)
          component.unityPrice.gameObject.SetActive(true);
      }
      else
      {
        component.unityBackgroundPrice.gameObject.SetActive(false);
        component.unityBackgroundDefault.gameObject.SetActive(true);
        if ((UnityEngine.Object) component.unityPrice != (UnityEngine.Object) null)
          component.unityPrice.gameObject.SetActive(false);
      }
      return component;
    }
  }
}
