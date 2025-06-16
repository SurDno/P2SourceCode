// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Menu.Protagonist.Inventory.Windows.SplitGraphic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Source.Components;
using Engine.Source.Services.Inputs;
using InputServices;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#nullable disable
namespace Engine.Impl.UI.Menu.Protagonist.Inventory.Windows
{
  [DisallowMultipleComponent]
  [RequireComponent(typeof (CanvasRenderer))]
  public class SplitGraphic : 
    BaseGraphic,
    IPointerClickHandler,
    IEventSystemHandler,
    IPointerDownHandler
  {
    private IStorableComponent actor;
    private bool isCanceled;
    private IStorableComponent target;
    [SerializeField]
    private Slider unitySlider = (Slider) null;
    [SerializeField]
    private Image itemImage;
    [SerializeField]
    private Text itemAmountText;
    [SerializeField]
    private Button buttonSelect;
    [SerializeField]
    private Button buttonCancel;
    private bool isReverted = false;
    [SerializeField]
    private GameObject controls;

    public bool IsReverted
    {
      get => this.isReverted;
      set => this.isReverted = value;
    }

    public IStorableComponent Actor
    {
      get => this.actor;
      set
      {
        this.actor = value;
        this.target = (IStorableComponent) null;
        this.ResetSlider();
        if (this.actor == null)
          return;
        this.itemImage.sprite = ((StorableComponent) this.actor).Placeholder.ImageInventorySlot.Value;
      }
    }

    public IStorableComponent Target => this.target;

    public bool IsCanceled => this.isCanceled;

    protected override void Awake()
    {
      base.Awake();
      this.buttonSelect.onClick.AddListener(new UnityAction(this.Select));
      this.buttonCancel.onClick.AddListener(new UnityAction(this.Cancel));
      this.unitySlider.onValueChanged.AddListener(new UnityAction<float>(this.OnSliderValueChange));
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      if ((UnityEngine.Object) EventSystem.current == (UnityEngine.Object) null)
        return;
      ServiceLocator.GetService<GameActionService>()?.AddListener(GameActionType.Cancel, new GameActionHandle(this.BasicListener));
      InputService.Instance.onJoystickUsedChanged += new Action<bool>(this.OnJoystick);
      this.OnJoystick(InputService.Instance.JoystickUsed);
      ServiceLocator.GetService<GameActionService>()?.AddListener(GameActionType.Submit, new GameActionHandle(this.BasicListener));
      ServiceLocator.GetService<GameActionService>()?.AddListener(GameActionType.LStickLeft, new GameActionHandle(this.BasicListener));
      ServiceLocator.GetService<GameActionService>()?.AddListener(GameActionType.LStickRight, new GameActionHandle(this.BasicListener));
    }

    private bool BasicListener(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed)
        return false;
      if (type == GameActionType.Submit & down)
      {
        this.Select();
        return true;
      }
      if (type == GameActionType.LStickLeft & down)
      {
        --this.unitySlider.value;
        return true;
      }
      if (type == GameActionType.LStickRight & down)
      {
        ++this.unitySlider.value;
        return true;
      }
      if (type == GameActionType.Cancel & down)
        this.Cancel();
      return false;
    }

    private void OnJoystick(bool joystick)
    {
      this.buttonCancel.gameObject.SetActive(!joystick);
      this.buttonSelect.gameObject.SetActive(!joystick);
      this.controls.SetActive(joystick);
    }

    protected override void OnDisable()
    {
      base.OnDisable();
      ServiceLocator.GetService<GameActionService>()?.RemoveListener(GameActionType.Cancel, new GameActionHandle(this.BasicListener));
      InputService.Instance.onJoystickUsedChanged -= new Action<bool>(this.OnJoystick);
      ServiceLocator.GetService<GameActionService>()?.RemoveListener(GameActionType.Submit, new GameActionHandle(this.BasicListener));
      ServiceLocator.GetService<GameActionService>()?.RemoveListener(GameActionType.LStickLeft, new GameActionHandle(this.BasicListener));
      ServiceLocator.GetService<GameActionService>()?.RemoveListener(GameActionType.LStickRight, new GameActionHandle(this.BasicListener));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }

    private void OnSliderValueChange(float value) => this.UpdateCount();

    private void Select()
    {
      int count = Mathf.RoundToInt(this.unitySlider.value);
      if (this.isReverted)
        count = this.actor.Count - count;
      this.isCanceled = false;
      if (this.actor == null || this.actor.IsDisposed)
        this.isCanceled = true;
      else if (count == 0)
        this.isCanceled = true;
      else
        this.target = this.actor.Count != count ? this.actor.Split(count) : this.actor;
      this.IsEnabled = false;
      this.IsReverted = false;
    }

    private void Cancel()
    {
      this.isCanceled = true;
      this.IsEnabled = false;
      this.IsReverted = false;
    }

    protected void ResetSlider()
    {
      if (this.actor == null || this.actor.IsDisposed)
        return;
      this.unitySlider.minValue = 1f;
      this.unitySlider.maxValue = (float) this.actor.Count - 1f;
      this.unitySlider.value = 1f;
      this.UpdateCount();
    }

    public void UpdateCount()
    {
      if (this.actor == null || this.actor.IsDisposed)
        return;
      this.itemAmountText.text = "× " + Mathf.RoundToInt(this.unitySlider.value).ToString();
    }

    public static SplitGraphic Instantiate(GameObject prefab)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
      gameObject.name = prefab.name;
      return gameObject.GetComponent<SplitGraphic>();
    }

    private bool CancelListener(GameActionType type, bool down)
    {
      if (!down)
        return false;
      this.isCanceled = true;
      this.IsEnabled = false;
      return true;
    }
  }
}
