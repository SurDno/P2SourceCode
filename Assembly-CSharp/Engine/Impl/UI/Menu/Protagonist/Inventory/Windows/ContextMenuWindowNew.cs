using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Components;
using Engine.Source.Inventory;
using Engine.Source.Services.Inputs;
using InputServices;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Engine.Impl.UI.Menu.Protagonist.Inventory.Windows
{
  [DisallowMultipleComponent]
  public class ContextMenuWindowNew : UIControl
  {
    [SerializeField]
    private Button buttonInvestigate;
    [SerializeField]
    private Button buttonDrop;
    [SerializeField]
    private Button buttonWear;
    [SerializeField]
    private Button buttonUse;
    [SerializeField]
    private Button buttonPourOut;
    [SerializeField]
    private Button buttonSplit;
    [SerializeField]
    private Image selectedLine;
    private Button[] buttons;
    private int curSelectedIndex = 0;
    [SerializeField]
    private Image unityImage = (Image) null;
    [SerializeField]
    private Text unityName = (Text) null;
    [SerializeField]
    private bool useBigPicture = false;
    private IStorableComponent target;
    private bool submitSubscribed = false;

    public event Action<IStorableComponent> OnButtonInvestigate;

    public event Action<IStorableComponent> OnButtonDrop;

    public event Action<IStorableComponent> OnButtonWear;

    public event Action<IStorableComponent> OnButtonUse;

    public event Action<IStorableComponent> OnButtonPourOut;

    public event Action<IStorableComponent> OnButtonSplit;

    public event Action OnClose;

    public IStorableComponent Target
    {
      get => this.target;
      set
      {
        if (this.target == value)
          return;
        this.target = value;
        this.Build();
      }
    }

    private void Build()
    {
      Sprite sprite = (Sprite) null;
      bool flag = (UnityEngine.Object) this.unityImage != (UnityEngine.Object) null;
      if (this.target == null)
      {
        this.buttonWear.gameObject.SetActive(false);
        this.buttonUse.gameObject.SetActive(false);
        this.buttonPourOut.gameObject.SetActive(false);
        this.buttonSplit.gameObject.SetActive(false);
        if ((UnityEngine.Object) this.unityName != (UnityEngine.Object) null)
          this.unityName.text = (string) null;
      }
      else
      {
        this.buttonWear.gameObject.SetActive(StorableComponentUtility.IsWearable(this.target));
        this.buttonUse.gameObject.SetActive(StorableComponentUtility.IsUsable(this.target));
        this.buttonPourOut.gameObject.SetActive(StorableComponentUtility.IsBottled(this.target));
        this.buttonSplit.gameObject.SetActive(StorableComponentUtility.IsSplittable(this.target));
        if ((UnityEngine.Object) this.unityName != (UnityEngine.Object) null)
          this.unityName.text = ServiceLocator.GetService<LocalizationService>().GetText(this.target.Title);
        if (flag)
        {
          InventoryPlaceholder placeholder = this.target is StorableComponent target ? target.Placeholder : (InventoryPlaceholder) null;
          if (placeholder != null)
            sprite = this.useBigPicture ? placeholder.ImageInformation.Value : placeholder.ImageInventorySlotBig.Value;
        }
      }
      if (flag)
      {
        this.unityImage.sprite = sprite;
        this.unityImage.gameObject.SetActive((UnityEngine.Object) sprite != (UnityEngine.Object) null);
      }
      LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) this.transform);
    }

    public static ContextMenuWindowNew Instantiate(GameObject prefab)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
      gameObject.name = prefab.name;
      ContextMenuWindowNew component = gameObject.GetComponent<ContextMenuWindowNew>();
      component.buttonInvestigate.onClick.AddListener(new UnityAction(component.Investigate));
      component.buttonDrop.onClick.AddListener(new UnityAction(component.Drop));
      component.buttonWear.onClick.AddListener(new UnityAction(component.Wear));
      component.buttonUse.onClick.AddListener(new UnityAction(component.Use));
      component.buttonPourOut.onClick.AddListener(new UnityAction(component.PourOut));
      component.buttonSplit.onClick.AddListener(new UnityAction(component.Split));
      return component;
    }

    public void OnEnable()
    {
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      if (this.buttons == null)
        this.buttons = new Button[6]
        {
          this.buttonWear,
          this.buttonUse,
          this.buttonInvestigate,
          this.buttonPourOut,
          this.buttonSplit,
          this.buttonDrop
        };
      service.AddListener(GameActionType.Cancel, new GameActionHandle(this.OnClosePress), true);
      service.AddListener(GameActionType.LStickUp, new GameActionHandle(this.OnNavigate), true);
      service.AddListener(GameActionType.LStickDown, new GameActionHandle(this.OnNavigate), true);
      if ((UnityEngine.Object) this.selectedLine != (UnityEngine.Object) null)
        this.selectedLine.gameObject.SetActive(false);
      this.StartCoroutine(this.AfterEnabled());
    }

    private IEnumerator AfterEnabled()
    {
      yield return (object) new WaitForEndOfFrame();
      this.OnJoystick(InputService.Instance.JoystickUsed);
    }

    private bool OnClosePress(GameActionType type, bool down)
    {
      if (down && this.OnClose != null)
        this.OnClose();
      return true;
    }

    private void OnJoystick(bool joystick)
    {
      if (joystick)
      {
        if (!this.submitSubscribed)
        {
          ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, new GameActionHandle(this.OnSubmit), true);
          this.submitSubscribed = true;
        }
        --this.curSelectedIndex;
        this.OnNavigate(GameActionType.LStickDown, true);
        if (!((UnityEngine.Object) this.selectedLine != (UnityEngine.Object) null))
          return;
        this.selectedLine.gameObject.SetActive(true);
        this.selectedLine.transform.SetParent(this.buttons[this.curSelectedIndex].transform, false);
        this.selectedLine.rectTransform.sizeDelta = this.selectedLine.rectTransform.sizeDelta with
        {
          x = this.buttons[this.curSelectedIndex].GetComponentInChildren<Text>().preferredWidth
        };
      }
      else
      {
        if (this.submitSubscribed)
        {
          ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, new GameActionHandle(this.OnSubmit));
          this.submitSubscribed = false;
        }
        EventSystem.current.SetSelectedGameObject((GameObject) null);
        if ((UnityEngine.Object) this.selectedLine != (UnityEngine.Object) null)
          this.selectedLine.gameObject.SetActive(false);
      }
    }

    public void OnDisable()
    {
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      if (this.submitSubscribed)
        service.RemoveListener(GameActionType.Submit, new GameActionHandle(this.OnSubmit));
      service.RemoveListener(GameActionType.Cancel, new GameActionHandle(this.OnClosePress));
      service.RemoveListener(GameActionType.LStickUp, new GameActionHandle(this.OnNavigate));
      service.RemoveListener(GameActionType.LStickDown, new GameActionHandle(this.OnNavigate));
    }

    private bool OnNavigate(GameActionType type, bool down)
    {
      if (!down)
        return false;
      do
      {
        this.curSelectedIndex += type == GameActionType.LStickUp ? -1 : 1;
        if (this.curSelectedIndex < 0)
          this.curSelectedIndex = this.buttons.Length - 1;
        if (this.curSelectedIndex >= this.buttons.Length)
          this.curSelectedIndex = 0;
      }
      while (!this.buttons[this.curSelectedIndex].gameObject.activeSelf);
      EventSystem.current.SetSelectedGameObject(this.buttons[this.curSelectedIndex].gameObject);
      if ((UnityEngine.Object) this.selectedLine != (UnityEngine.Object) null)
      {
        this.selectedLine.gameObject.SetActive(true);
        this.selectedLine.transform.SetParent(this.buttons[this.curSelectedIndex].transform, false);
        this.selectedLine.rectTransform.sizeDelta = this.selectedLine.rectTransform.sizeDelta with
        {
          x = this.buttons[this.curSelectedIndex].GetComponentInChildren<Text>().preferredWidth
        };
      }
      return true;
    }

    private bool OnSubmit(GameActionType type, bool down)
    {
      if (down)
        this.buttons[this.curSelectedIndex].onClick.Invoke();
      return true;
    }

    public void Investigate()
    {
      if (this.target == null || this.OnButtonInvestigate == null)
        return;
      this.OnButtonInvestigate(this.target);
    }

    public void Drop()
    {
      if (this.target == null || this.OnButtonDrop == null)
        return;
      this.OnButtonDrop(this.target);
    }

    public void Wear()
    {
      if (this.target == null || this.OnButtonWear == null || !StorableComponentUtility.IsWearable(this.target))
        return;
      this.OnButtonWear(this.target);
    }

    public void Use()
    {
      if (this.target == null || this.OnButtonUse == null || !StorableComponentUtility.IsUsable(this.target))
        return;
      this.OnButtonUse(this.target);
    }

    public void PourOut()
    {
      if (this.target == null || this.OnButtonPourOut == null || !StorableComponentUtility.IsBottled(this.target))
        return;
      this.OnButtonPourOut(this.target);
    }

    public void Split()
    {
      if (this.target == null || this.OnButtonSplit == null || !StorableComponentUtility.IsSplittable(this.target))
        return;
      this.OnButtonSplit(this.target);
    }
  }
}
