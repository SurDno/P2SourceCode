using Engine.Common.Services;
using Engine.Source.Services.Inputs;
using InputServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Engine.Impl.UI.Menu.Main
{
  public class SettingsView : MonoBehaviour
  {
    public static SettingsView Current;
    [SerializeField]
    protected LayoutContainer listLayoutPrefab;
    [SerializeField]
    protected FloatSettingsValueView floatValueViewPrefab;
    [SerializeField]
    protected NamedIntSettingsValueView namedIntValueViewPrefab;
    [SerializeField]
    protected ConfirmationWindow confirmationPrefab;
    [SerializeField]
    protected BoolSettingsValueView boolValueViewPrefab;
    [SerializeField]
    private Button buttonReset;
    private ISettingEntity currentEntity = (ISettingEntity) null;
    private int currentIndex = 0;
    protected LayoutContainer layout;
    private Coroutine scrollCoroutine;
    private Coroutine changingCoroutine;

    protected virtual void Awake()
    {
      SettingsMenuHelper.Instatnce.OnStateSelected += new Action<bool>(this.OnStateSelected);
      this.buttonReset?.onClick.AddListener(new UnityAction(this.OnButtonReset));
    }

    protected virtual void OnEnable()
    {
      SettingsView.Current = this;
      InputService.Instance.onJoystickUsedChanged += new Action<bool>(this.OnJoystick);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickDown, new GameActionHandle(this.OnNavigate), true);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickUp, new GameActionHandle(this.OnNavigate), true);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickLeft, new GameActionHandle(this.OnValueChange), true);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickRight, new GameActionHandle(this.OnValueChange), true);
      this.OnJoystick(InputService.Instance.JoystickUsed);
    }

    protected virtual bool OnResetGameAction(GameActionType type, bool down)
    {
      if (down)
        this.OnButtonReset();
      return down;
    }

    private bool OnValueChange(GameActionType type, bool down)
    {
      if (this.changingCoroutine != null)
        this.StopCoroutine(this.changingCoroutine);
      if (down)
      {
        switch (type)
        {
          case GameActionType.LStickLeft:
            this.currentEntity?.DecrementValue();
            this.changingCoroutine = this.StartCoroutine(this.ChangingCoroutine(true));
            break;
          case GameActionType.LStickRight:
            this.currentEntity?.IncrementValue();
            this.changingCoroutine = this.StartCoroutine(this.ChangingCoroutine(false));
            break;
        }
      }
      return down;
    }

    private bool OnNavigate(GameActionType type, bool down)
    {
      if (this.scrollCoroutine != null)
        this.StopCoroutine(this.scrollCoroutine);
      if (down)
      {
        switch (type)
        {
          case GameActionType.LStickUp:
            this.SelectItem(this.currentIndex - 1);
            this.scrollCoroutine = this.StartCoroutine(this.ScrollCoroutine(true));
            break;
          case GameActionType.LStickDown:
            this.SelectItem(this.currentIndex + 1);
            this.scrollCoroutine = this.StartCoroutine(this.ScrollCoroutine(false));
            break;
        }
      }
      return down;
    }

    protected virtual void OnJoystick(bool isUsed)
    {
      if (isUsed)
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Context, new GameActionHandle(this.OnResetGameAction));
      else
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Context, new GameActionHandle(this.OnResetGameAction));
      if (this.currentEntity != null)
        this.currentEntity.Selected = isUsed;
      if (isUsed)
        return;
      this.OnStateSelected(true);
    }

    protected virtual void OnDestroy()
    {
      SettingsMenuHelper.Instatnce.OnStateSelected -= new Action<bool>(this.OnStateSelected);
    }

    protected virtual void OnDisable()
    {
      InputService.Instance.onJoystickUsedChanged -= new Action<bool>(this.OnJoystick);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickDown, new GameActionHandle(this.OnNavigate));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickUp, new GameActionHandle(this.OnNavigate));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickLeft, new GameActionHandle(this.OnValueChange));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickRight, new GameActionHandle(this.OnValueChange));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Context, new GameActionHandle(this.OnResetGameAction));
    }

    private void OnStateSelected(bool isSelected)
    {
      isSelected = isSelected || !InputService.Instance.JoystickUsed;
      this.gameObject.SetActive(isSelected);
      this.SelectItem(0);
      if (this.currentEntity == null)
        return;
      this.currentEntity.Selected = InputService.Instance.JoystickUsed;
    }

    protected bool SelectItem(int index)
    {
      List<ISettingEntity> all = new List<ISettingEntity>((IEnumerable<ISettingEntity>) this.layout.transform.GetComponentsInChildren<ISettingEntity>()).FindAll((Predicate<ISettingEntity>) (e => e.IsActive() && e.Interactable));
      if (all.Count == 0)
        return false;
      this.currentIndex = index >= all.Count ? 0 : (index < 0 ? all.Count - 1 : index);
      if (this.currentEntity != null)
      {
        if (InputService.Instance.JoystickUsed)
          this.currentEntity.OnDeSelect();
        this.currentEntity.Selected = false;
      }
      this.currentEntity = all[this.currentIndex];
      this.currentEntity.Selected = true;
      if (InputService.Instance.JoystickUsed)
        this.currentEntity.OnSelect();
      this.FillForSelected();
      return this.currentEntity.Interactable;
    }

    public void FillForSelected()
    {
      float height = this.layout.Content.parent.parent.GetComponent<RectTransform>().rect.height;
      float num1 = 84f;
      float num2 = -((Component) this.currentEntity).GetComponent<RectTransform>().anchoredPosition.y + num1;
      RectTransform component = this.layout.Content.parent.GetComponent<RectTransform>();
      Vector2 anchoredPosition = component.anchoredPosition;
      if ((double) num2 - (double) anchoredPosition.y > (double) height)
        anchoredPosition.y = (double) num2 + (double) anchoredPosition.y > (double) height ? num2 - height : 0.0f;
      else if ((double) num2 - (double) num1 * 2.0 - (double) anchoredPosition.y < 0.0)
        anchoredPosition.y = num2 - num1 * 2f;
      component.anchoredPosition = anchoredPosition;
    }

    protected virtual void OnButtonReset()
    {
    }

    private IEnumerator ScrollCoroutine(bool isUp)
    {
      yield return (object) new WaitForSeconds(0.5f);
      while (true)
      {
        int sellected = !isUp ? this.currentIndex + 1 : this.currentIndex - 1;
        this.SelectItem(sellected);
        yield return (object) new WaitForSeconds(0.05f);
      }
    }

    private IEnumerator ChangingCoroutine(bool isDecrement)
    {
      yield return (object) new WaitForSeconds(0.5f);
      while (true)
      {
        if (isDecrement)
          this.currentEntity?.DecrementValue();
        else
          this.currentEntity?.IncrementValue();
        yield return (object) new WaitForSeconds(0.05f);
      }
    }
  }
}
