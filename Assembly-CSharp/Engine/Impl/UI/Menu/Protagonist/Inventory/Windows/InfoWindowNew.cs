using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using Engine.Source.Inventory;
using Engine.Source.Services.Inputs;
using InputServices;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
      get => this.target;
      set
      {
        if (value != null && value.IsDisposed)
          value = (IStorableComponent) null;
        if (this.target == value)
          return;
        this.target = value;
        if (this.target == null)
        {
          this.unityName.text = (string) null;
          this.unityInformation.text = (string) null;
          this.durabilityView.Value = (IEntity) null;
          this.unityImage.sprite = (Sprite) null;
          this.unityImage.gameObject.SetActive(false);
        }
        else
        {
          LocalizationService service = ServiceLocator.GetService<LocalizationService>();
          this.unityName.text = service.GetText(this.target.Title);
          this.unityInformation.text = service.GetText(this.target.Tooltip);
          this.durabilityView.Value = this.target.Owner;
          if (!(this.target is StorableComponent target))
          {
            this.unityImage.sprite = (Sprite) null;
            this.unityImage.gameObject.SetActive(false);
          }
          else
          {
            InventoryPlaceholder placeholder = target.Placeholder;
            this.unityImage.sprite = this.useBigPicture ? placeholder?.ImageInformation.Value : placeholder?.ImageInventorySlotBig.Value;
            this.unityImage.gameObject.SetActive((Object) this.unityImage.sprite != (Object) null);
          }
        }
      }
    }

    public float Price
    {
      get => this.price;
      set
      {
        this.price = value;
        if ((double) this.price <= 0.0)
        {
          this.priceCoin.gameObject.SetActive(false);
          this.priceText.text = ServiceLocator.GetService<LocalizationService>().GetText("{StorableTooltip.NotInteresting}");
        }
        else
        {
          this.priceCoin.gameObject.SetActive(true);
          this.priceCoin.SetNativeSize();
          this.priceText.text = ((int) this.price).ToString();
        }
      }
    }

    public void AddActionTooltip(GameActionType gameAction, string text)
    {
      if ((Object) this.keyCodeLinePrefab == (Object) null || (Object) this.keyCodePrefab == (Object) null)
        return;
      UnityEngine.Transform parent = (UnityEngine.Transform) null;
      if (this.keyCodeLines != null && this.keyCodeLines.Count > 0)
      {
        UnityEngine.Transform keyCodeLine = this.keyCodeLines[this.keyCodeLines.Count - 1];
        if (keyCodeLine.childCount < 2)
          parent = keyCodeLine;
      }
      if ((Object) parent == (Object) null)
      {
        if (this.keyCodeLines == null)
          this.keyCodeLines = new List<UnityEngine.Transform>();
        parent = Object.Instantiate<GameObject>(this.keyCodeLinePrefab, this.keyCodeSeparator.transform, false).transform;
        this.keyCodeLines.Add(parent);
        this.keyCodeSeparator.SetActive(true);
      }
      GameObject gameObject = Object.Instantiate<GameObject>(this.keyCodePrefab, parent, false);
      gameObject.GetComponent<GameActionView>().SetValue(gameAction, true);
      gameObject.GetComponent<StringView>().StringValue = text;
    }

    public void BarterMode(bool b)
    {
      this.priceCoin.sprite = b ? this.handSprite : this.coinSprite;
      this.priceCoin.SetNativeSize();
    }

    public void ClearActionTooltips()
    {
      if (this.keyCodeLines == null)
        return;
      foreach (Component keyCodeLine in this.keyCodeLines)
        Object.Destroy((Object) keyCodeLine.gameObject);
      this.keyCodeLines.Clear();
      this.keyCodeSeparator.SetActive(false);
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
      bool flag = (Object) this.unityImage.sprite != (Object) null && !simplified;
      this.unityImage.gameObject.SetActive(flag);
      this.imageSeparator.SetActive(flag);
      if (InputService.Instance.JoystickUsed)
      {
        this.keyCodeSeparator.SetActive(showActionTooltips);
        foreach (Component keyCodeLine in this.keyCodeLines)
          keyCodeLine.gameObject.SetActive(showActionTooltips);
      }
      else if (this.keyCodeLines != null && this.keyCodeLines.Count > 0)
      {
        this.keyCodeSeparator.SetActive(true);
        foreach (Component keyCodeLine in this.keyCodeLines)
          keyCodeLine.gameObject.SetActive(true);
      }
      else
        this.keyCodeSeparator.SetActive(false);
      LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
    }
  }
}
