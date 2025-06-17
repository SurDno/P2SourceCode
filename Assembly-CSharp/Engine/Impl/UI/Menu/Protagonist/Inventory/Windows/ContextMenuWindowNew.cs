using System;
using System.Collections;
using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Components;
using Engine.Source.Inventory;
using Engine.Source.Services.Inputs;
using InputServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

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
    private int curSelectedIndex;
    [SerializeField]
    private Image unityImage;
    [SerializeField]
    private Text unityName;
    [SerializeField]
    private bool useBigPicture;
    private IStorableComponent target;
    private bool submitSubscribed;

    public event Action<IStorableComponent> OnButtonInvestigate;

    public event Action<IStorableComponent> OnButtonDrop;

    public event Action<IStorableComponent> OnButtonWear;

    public event Action<IStorableComponent> OnButtonUse;

    public event Action<IStorableComponent> OnButtonPourOut;

    public event Action<IStorableComponent> OnButtonSplit;

    public event Action OnClose;

    public IStorableComponent Target
    {
      get => target;
      set
      {
        if (target == value)
          return;
        target = value;
        Build();
      }
    }

    private void Build()
    {
      Sprite sprite = null;
      bool flag = unityImage != null;
      if (this.target == null)
      {
        buttonWear.gameObject.SetActive(false);
        buttonUse.gameObject.SetActive(false);
        buttonPourOut.gameObject.SetActive(false);
        buttonSplit.gameObject.SetActive(false);
        if (unityName != null)
          unityName.text = null;
      }
      else
      {
        buttonWear.gameObject.SetActive(StorableComponentUtility.IsWearable(this.target));
        buttonUse.gameObject.SetActive(StorableComponentUtility.IsUsable(this.target));
        buttonPourOut.gameObject.SetActive(StorableComponentUtility.IsBottled(this.target));
        buttonSplit.gameObject.SetActive(StorableComponentUtility.IsSplittable(this.target));
        if (unityName != null)
          unityName.text = ServiceLocator.GetService<LocalizationService>().GetText(this.target.Title);
        if (flag)
        {
          InventoryPlaceholder placeholder = this.target is StorableComponent target ? target.Placeholder : null;
          if (placeholder != null)
            sprite = useBigPicture ? placeholder.ImageInformation.Value : placeholder.ImageInventorySlotBig.Value;
        }
      }
      if (flag)
      {
        unityImage.sprite = sprite;
        unityImage.gameObject.SetActive(sprite != null);
      }
      LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) transform);
    }

    public static ContextMenuWindowNew Instantiate(GameObject prefab)
    {
      GameObject gameObject = Object.Instantiate(prefab);
      gameObject.name = prefab.name;
      ContextMenuWindowNew component = gameObject.GetComponent<ContextMenuWindowNew>();
      component.buttonInvestigate.onClick.AddListener(component.Investigate);
      component.buttonDrop.onClick.AddListener(component.Drop);
      component.buttonWear.onClick.AddListener(component.Wear);
      component.buttonUse.onClick.AddListener(component.Use);
      component.buttonPourOut.onClick.AddListener(component.PourOut);
      component.buttonSplit.onClick.AddListener(component.Split);
      return component;
    }

    public void OnEnable()
    {
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      if (buttons == null)
        buttons = [
          buttonWear,
          buttonUse,
          buttonInvestigate,
          buttonPourOut,
          buttonSplit,
          buttonDrop
        ];
      service.AddListener(GameActionType.Cancel, OnClosePress, true);
      service.AddListener(GameActionType.LStickUp, OnNavigate, true);
      service.AddListener(GameActionType.LStickDown, OnNavigate, true);
      if (selectedLine != null)
        selectedLine.gameObject.SetActive(false);
      StartCoroutine(AfterEnabled());
    }

    private IEnumerator AfterEnabled()
    {
      yield return new WaitForEndOfFrame();
      OnJoystick(InputService.Instance.JoystickUsed);
    }

    private bool OnClosePress(GameActionType type, bool down)
    {
      if (down && OnClose != null)
        OnClose();
      return true;
    }

    private void OnJoystick(bool joystick)
    {
      if (joystick)
      {
        if (!submitSubscribed)
        {
          ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, OnSubmit, true);
          submitSubscribed = true;
        }
        --curSelectedIndex;
        OnNavigate(GameActionType.LStickDown, true);
        if (!(selectedLine != null))
          return;
        selectedLine.gameObject.SetActive(true);
        selectedLine.transform.SetParent(buttons[curSelectedIndex].transform, false);
        selectedLine.rectTransform.sizeDelta = selectedLine.rectTransform.sizeDelta with
        {
          x = buttons[curSelectedIndex].GetComponentInChildren<Text>().preferredWidth
        };
      }
      else
      {
        if (submitSubscribed)
        {
          ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, OnSubmit);
          submitSubscribed = false;
        }
        EventSystem.current.SetSelectedGameObject(null);
        if (selectedLine != null)
          selectedLine.gameObject.SetActive(false);
      }
    }

    public void OnDisable()
    {
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      if (submitSubscribed)
        service.RemoveListener(GameActionType.Submit, OnSubmit);
      service.RemoveListener(GameActionType.Cancel, OnClosePress);
      service.RemoveListener(GameActionType.LStickUp, OnNavigate);
      service.RemoveListener(GameActionType.LStickDown, OnNavigate);
    }

    private bool OnNavigate(GameActionType type, bool down)
    {
      if (!down)
        return false;
      do
      {
        curSelectedIndex += type == GameActionType.LStickUp ? -1 : 1;
        if (curSelectedIndex < 0)
          curSelectedIndex = buttons.Length - 1;
        if (curSelectedIndex >= buttons.Length)
          curSelectedIndex = 0;
      }
      while (!buttons[curSelectedIndex].gameObject.activeSelf);
      EventSystem.current.SetSelectedGameObject(buttons[curSelectedIndex].gameObject);
      if (selectedLine != null)
      {
        selectedLine.gameObject.SetActive(true);
        selectedLine.transform.SetParent(buttons[curSelectedIndex].transform, false);
        selectedLine.rectTransform.sizeDelta = selectedLine.rectTransform.sizeDelta with
        {
          x = buttons[curSelectedIndex].GetComponentInChildren<Text>().preferredWidth
        };
      }
      return true;
    }

    private bool OnSubmit(GameActionType type, bool down)
    {
      if (down)
        buttons[curSelectedIndex].onClick.Invoke();
      return true;
    }

    public void Investigate()
    {
      if (target == null || OnButtonInvestigate == null)
        return;
      OnButtonInvestigate(target);
    }

    public void Drop()
    {
      if (target == null || OnButtonDrop == null)
        return;
      OnButtonDrop(target);
    }

    public void Wear()
    {
      if (target == null || OnButtonWear == null || !StorableComponentUtility.IsWearable(target))
        return;
      OnButtonWear(target);
    }

    public void Use()
    {
      if (target == null || OnButtonUse == null || !StorableComponentUtility.IsUsable(target))
        return;
      OnButtonUse(target);
    }

    public void PourOut()
    {
      if (target == null || OnButtonPourOut == null || !StorableComponentUtility.IsBottled(target))
        return;
      OnButtonPourOut(target);
    }

    public void Split()
    {
      if (target == null || OnButtonSplit == null || !StorableComponentUtility.IsSplittable(target))
        return;
      OnButtonSplit(target);
    }
  }
}
