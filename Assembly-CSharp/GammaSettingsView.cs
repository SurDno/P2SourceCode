using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services.Inputs;
using Engine.Source.Settings;
using InputServices;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GammaSettingsView : MonoBehaviour
{
  [SerializeField]
  private FloatSettingsValueView view;
  [SerializeField]
  private Button pcButton;
  [SerializeField]
  private GameObject controllButtonHint;
  private Coroutine changingCoroutine;
  private bool InputBlocked = false;

  private void Awake()
  {
    IValue<float> gamma = InstanceByRequest<GraphicsGameSettings>.Instance.Gamma;
    this.view.SetName("{UI.Menu.Main.Settings.Graphics.Gamma}");
    this.view.SetMinValue(gamma.MinValue);
    this.view.SetMaxValue(gamma.MaxValue);
    this.view.SetValueNameFunction(new Func<float, string>(SettingsViewUtility.GammaValueName));
    this.view.SetSetting(gamma);
    this.view.SetValueValidationFunction(new Func<float, float>(SettingsViewUtility.GammaValueValidation), 0.025f);
    FloatSettingsValueView view = this.view;
    view.VisibleValueChangeEvent = view.VisibleValueChangeEvent + new Action<SettingsValueView<float>>(this.OnAutoValueChange<float>);
  }

  private IEnumerator ChangingCoroutine(bool isDecrement)
  {
    yield return (object) new WaitForSeconds(0.5f);
    while (true)
    {
      if (isDecrement)
        this.view?.DecrementValue();
      else
        this.view?.IncrementValue();
      yield return (object) new WaitForSeconds(0.05f);
    }
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
          this.view?.DecrementValue();
          this.changingCoroutine = this.StartCoroutine(this.ChangingCoroutine(true));
          break;
        case GameActionType.LStickRight:
          this.view?.IncrementValue();
          this.changingCoroutine = this.StartCoroutine(this.ChangingCoroutine(false));
          break;
      }
    }
    return down;
  }

  private void OnAutoValueChange<T>(SettingsValueView<T> view)
  {
    view.ApplyVisibleValue();
    InstanceByRequest<GraphicsGameSettings>.Instance.Apply();
  }

  private void OnEnable()
  {
    this.view.RevertVisibleValue();
    ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickLeft, new GameActionHandle(this.OnValueChange), true);
    ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickRight, new GameActionHandle(this.OnValueChange), true);
    InputService.Instance.onJoystickUsedChanged += new Action<bool>(this.OnJoystick);
    this.OnJoystick(InputService.Instance.JoystickUsed);
  }

  private void OnDisable()
  {
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickLeft, new GameActionHandle(this.OnValueChange));
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickRight, new GameActionHandle(this.OnValueChange));
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, new GameActionHandle(this.OnSubmit));
    InputService.Instance.onJoystickUsedChanged -= new Action<bool>(this.OnJoystick);
    this.InputBlocked = false;
  }

  private void OnJoystick(bool joystick)
  {
    this.pcButton.gameObject.SetActive(!joystick);
    this.controllButtonHint.SetActive(joystick);
    if (joystick)
    {
      CoroutineService.Instance.WaitFrame(1, (Action) (() => this.InputBlocked = false));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, new GameActionHandle(this.OnSubmit), true);
    }
    else
    {
      this.InputBlocked = true;
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, new GameActionHandle(this.OnSubmit));
    }
  }

  private bool OnSubmit(GameActionType type, bool down)
  {
    if (this.InputBlocked)
    {
      this.InputBlocked = false;
      return false;
    }
    if (!InputService.Instance.JoystickUsed)
      return false;
    if (down)
      this.controllButtonHint?.GetComponent<Button>()?.onClick.Invoke();
    return down;
  }
}
