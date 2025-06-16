using System.Collections.Generic;
using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using Engine.Source.Inventory;
using Engine.Source.Services.Inputs;
using InputServices;

namespace Engine.Impl.UI.Menu.Protagonist.Inventory.Windows
{
  [DisallowMultipleComponent]
  public class InfoWindowNew : UIControl
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
    private GameObject priceItem;
    [SerializeField]
    private Text priceText;
    [SerializeField]
    private Image priceCoin;
    [SerializeField]
    private Sprite coinSprite;
    [SerializeField]
    private Sprite handSprite;
    [SerializeField]
    private DurabilityView durabilityView;
    [SerializeField]
    private GameObject keyCodeSeparator;
    [SerializeField]
    private GameObject imageSeparator;
    [SerializeField]
    private GameObject keyCodeLinePrefab;
    [SerializeField]
    private GameObject keyCodePrefab;
    private List<UnityEngine.Transform> keyCodeLines;

    public IStorableComponent Target
    {
      get => target;
      set
      {
        if (value != null && value.IsDisposed)
          value = null;
        if (this.target == value)
          return;
        this.target = value;
        if (this.target == null)
        {
          unityName.text = (string) null;
          unityInformation.text = (string) null;
          durabilityView.Value = null;
          unityImage.sprite = (Sprite) null;
          unityImage.gameObject.SetActive(false);
        }
        else
        {
          LocalizationService service = ServiceLocator.GetService<LocalizationService>();
          unityName.text = service.GetText(this.target.Title);
          unityInformation.text = service.GetText(this.target.Tooltip);
          durabilityView.Value = this.target.Owner;
          if (!(this.target is StorableComponent target))
          {
            unityImage.sprite = (Sprite) null;
            unityImage.gameObject.SetActive(false);
          }
          else
          {
            InventoryPlaceholder placeholder = target.Placeholder;
            unityImage.sprite = useBigPicture ? placeholder?.ImageInformation.Value : placeholder?.ImageInventorySlotBig.Value;
            unityImage.gameObject.SetActive((Object) unityImage.sprite != (Object) null);
          }
        }
      }
    }

    public float Price
    {
      get => price;
      set
      {
        price = value;
        if (price <= 0.0)
        {
          priceCoin.gameObject.SetActive(false);
          priceText.text = ServiceLocator.GetService<LocalizationService>().GetText("{StorableTooltip.NotInteresting}");
        }
        else
        {
          priceCoin.gameObject.SetActive(true);
          priceCoin.SetNativeSize();
          priceText.text = ((int) price).ToString();
        }
      }
    }

    public void AddActionTooltip(GameActionType gameAction, string text)
    {
      if ((Object) keyCodeLinePrefab == (Object) null || (Object) keyCodePrefab == (Object) null)
        return;
      UnityEngine.Transform parent = (UnityEngine.Transform) null;
      if (keyCodeLines != null && keyCodeLines.Count > 0)
      {
        UnityEngine.Transform keyCodeLine = keyCodeLines[keyCodeLines.Count - 1];
        if (keyCodeLine.childCount < 2)
          parent = keyCodeLine;
      }
      if ((Object) parent == (Object) null)
      {
        if (keyCodeLines == null)
          keyCodeLines = new List<UnityEngine.Transform>();
        parent = Object.Instantiate<GameObject>(keyCodeLinePrefab, keyCodeSeparator.transform, false).transform;
        keyCodeLines.Add(parent);
        keyCodeSeparator.SetActive(true);
      }
      GameObject gameObject = Object.Instantiate<GameObject>(keyCodePrefab, parent, false);
      gameObject.GetComponent<GameActionView>().SetValue(gameAction, true);
      gameObject.GetComponent<StringView>().StringValue = text;
    }

    public void BarterMode(bool b)
    {
      priceCoin.sprite = b ? handSprite : coinSprite;
      priceCoin.SetNativeSize();
    }

    public void ClearActionTooltips()
    {
      if (keyCodeLines == null)
        return;
      foreach (Component keyCodeLine in keyCodeLines)
        Object.Destroy((Object) keyCodeLine.gameObject);
      keyCodeLines.Clear();
      keyCodeSeparator.SetActive(false);
    }

    public static InfoWindowNew Instantiate(bool withPrice, GameObject prefab)
    {
      GameObject gameObject = Object.Instantiate<GameObject>(prefab);
      gameObject.name = prefab.name;
      InfoWindowNew component = gameObject.GetComponent<InfoWindowNew>();
      if ((Object) component.priceItem != (Object) null)
        component.priceItem.gameObject.SetActive(withPrice);
      return component;
    }

    public void ShowSimpliedWindow(bool simplified, bool showActionTooltips = true)
    {
      bool flag = (Object) unityImage.sprite != (Object) null && !simplified;
      unityImage.gameObject.SetActive(flag);
      imageSeparator.SetActive(flag);
      if (InputService.Instance.JoystickUsed)
      {
        keyCodeSeparator.SetActive(showActionTooltips);
        foreach (Component keyCodeLine in keyCodeLines)
          keyCodeLine.gameObject.SetActive(showActionTooltips);
      }
      else if (keyCodeLines != null && keyCodeLines.Count > 0)
      {
        keyCodeSeparator.SetActive(true);
        foreach (Component keyCodeLine in keyCodeLines)
          keyCodeLine.gameObject.SetActive(true);
      }
      else
        keyCodeSeparator.SetActive(false);
      LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
    }
  }
}
