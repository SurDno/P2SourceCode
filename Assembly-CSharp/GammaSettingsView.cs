using System.Collections;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services.Inputs;
using Engine.Source.Settings;
using InputServices;
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
  private bool InputBlocked;

  private void Awake()
  {
    IValue<float> gamma = InstanceByRequest<GraphicsGameSettings>.Instance.Gamma;
    this.view.SetName("{UI.Menu.Main.Settings.Graphics.Gamma}");
    this.view.SetMinValue(gamma.MinValue);
    this.view.SetMaxValue(gamma.MaxValue);
    this.view.SetValueNameFunction(SettingsViewUtility.GammaValueName);
    this.view.SetSetting(gamma);
    this.view.SetValueValidationFunction(SettingsViewUtility.GammaValueValidation, 0.025f);
    FloatSettingsValueView view = this.view;
    view.VisibleValueChangeEvent = view.VisibleValueChangeEvent + OnAutoValueChange;
  }

  private IEnumerator ChangingCoroutine(bool isDecrement)
  {
    yield return new WaitForSeconds(0.5f);
    while (true)
    {
      if (isDecrement)
        view?.DecrementValue();
      else
        view?.IncrementValue();
      yield return new WaitForSeconds(0.05f);
    }
  }

  private bool OnValueChange(GameActionType type, bool down)
  {
    if (changingCoroutine != null)
      StopCoroutine(changingCoroutine);
    if (down)
    {
      switch (type)
      {
        case GameActionType.LStickLeft:
          view?.DecrementValue();
          changingCoroutine = StartCoroutine(ChangingCoroutine(true));
          break;
        case GameActionType.LStickRight:
          view?.IncrementValue();
          changingCoroutine = StartCoroutine(ChangingCoroutine(false));
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
    view.RevertVisibleValue();
    ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickLeft, OnValueChange, true);
    ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickRight, OnValueChange, true);
    InputService.Instance.onJoystickUsedChanged += OnJoystick;
    OnJoystick(InputService.Instance.JoystickUsed);
  }

  private void OnDisable()
  {
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickLeft, OnValueChange);
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickRight, OnValueChange);
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, OnSubmit);
    InputService.Instance.onJoystickUsedChanged -= OnJoystick;
    InputBlocked = false;
  }

  private void OnJoystick(bool joystick)
  {
    pcButton.gameObject.SetActive(!joystick);
    controllButtonHint.SetActive(joystick);
    if (joystick)
    {
      CoroutineService.Instance.WaitFrame(1, () => InputBlocked = false);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, OnSubmit, true);
    }
    else
    {
      InputBlocked = true;
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, OnSubmit);
    }
  }

  private bool OnSubmit(GameActionType type, bool down)
  {
    if (InputBlocked)
    {
      InputBlocked = false;
      return false;
    }
    if (!InputService.Instance.JoystickUsed)
      return false;
    if (down)
      controllButtonHint?.GetComponent<Button>()?.onClick.Invoke();
    return down;
  }
}
