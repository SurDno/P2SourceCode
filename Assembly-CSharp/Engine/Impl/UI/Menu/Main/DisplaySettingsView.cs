using System;
using System.Collections;
using System.Collections.Generic;
using Engine.Source.Commons;
using Engine.Source.Settings;
using InputServices;
using UnityEngine;
using UnityEngine.UI;

namespace Engine.Impl.UI.Menu.Main
{
  public class DisplaySettingsView : SettingsView
  {
    [SerializeField]
    private Button applyButton;
    [SerializeField]
    private float confirmationTime = 10f;
    [SerializeField]
    private GameObject applyButtonTipObject;
    private List<string> names = new List<string>();
    private List<int> resolutionHeights = new List<int>();
    private List<int> resolutionWidths = new List<int>();
    private NamedIntSettingsValueView fullScreenModeView;
    private NamedIntSettingsValueView resolutionView;
    private FloatSettingsValueView renderScaleView;
    private FloatSettingsValueView gammaView;
    private FloatSettingsValueView fieldOfViewView;
    private ConfirmationWindow confirmationInstance;
    private Coroutine autorevertCoroutine;
    private int prevWidth;
    private int prevHeight;
    private FullScreenMode prevFullScreenMode;
    private float prevRenderScale;
    private GraphicsGameSettings graphicsGameSettings;

    protected override void Awake()
    {
      graphicsGameSettings = InstanceByRequest<GraphicsGameSettings>.Instance;
      layout = Instantiate(listLayoutPrefab, transform, false);
      this.fullScreenModeView = Instantiate(namedIntValueViewPrefab, layout.Content, false);
      this.fullScreenModeView.SetName("{UI.Menu.Main.Settings.Graphics.FullScreenMode}");
      this.fullScreenModeView.SetValueNames(new string[3]
      {
        "{UI.Menu.Main.Settings.Graphics.FullScreenMode.Window}",
        "{UI.Menu.Main.Settings.Graphics.FullScreenMode.Borderless}",
        "{UI.Menu.Main.Settings.Graphics.FullScreenMode.FullScreen}"
      });
      NamedIntSettingsValueView fullScreenModeView = this.fullScreenModeView;
      fullScreenModeView.VisibleValueChangeEvent = fullScreenModeView.VisibleValueChangeEvent + OnSliderChange;
      this.resolutionView = Instantiate(namedIntValueViewPrefab, layout.Content, false);
      this.resolutionView.SetName("{UI.Menu.Main.Settings.Graphics.Resolution}");
      NamedIntSettingsValueView resolutionView = this.resolutionView;
      resolutionView.VisibleValueChangeEvent = resolutionView.VisibleValueChangeEvent + OnSliderChange;
      this.renderScaleView = Instantiate(floatValueViewPrefab, layout.Content, false);
      this.renderScaleView.SetName("{UI.Menu.Main.Settings.Graphics.RenderScale}");
      this.renderScaleView.SetMinValue(graphicsGameSettings.RenderScale.MinValue);
      this.renderScaleView.SetMaxValue(graphicsGameSettings.RenderScale.MaxValue);
      this.renderScaleView.SetValueNameFunction(SettingsViewUtility.PercentValueName);
      this.renderScaleView.SetSetting(graphicsGameSettings.RenderScale);
      this.renderScaleView.SetValueValidationFunction(RenderScaleValueValidation, 0.05f);
      FloatSettingsValueView renderScaleView = this.renderScaleView;
      renderScaleView.VisibleValueChangeEvent = renderScaleView.VisibleValueChangeEvent + OnSliderChange;
      applyButton.onClick.AddListener(OnApplyButtonClick);
      InitConsoleSettings(layout);
      base.Awake();
    }

    private void InitConsoleSettings(LayoutContainer layout)
    {
      this.gammaView = Instantiate(floatValueViewPrefab, layout.Content, false);
      this.gammaView.SetName("{UI.Menu.Main.Settings.Graphics.Gamma}");
      this.gammaView.SetMinValue(graphicsGameSettings.Gamma.MinValue);
      this.gammaView.SetMaxValue(graphicsGameSettings.Gamma.MaxValue);
      this.gammaView.SetValueNameFunction(SettingsViewUtility.GammaValueName);
      this.gammaView.SetSetting(graphicsGameSettings.Gamma);
      this.gammaView.SetValueValidationFunction(SettingsViewUtility.GammaValueValidation, 0.025f);
      FloatSettingsValueView gammaView = this.gammaView;
      gammaView.VisibleValueChangeEvent = gammaView.VisibleValueChangeEvent + GraphicSettingsHelper.OnAutoValueChange;
      graphicsGameSettings = InstanceByRequest<GraphicsGameSettings>.Instance;
      this.fieldOfViewView = Instantiate(floatValueViewPrefab, layout.Content, false);
      this.fieldOfViewView.SetName("{UI.Menu.Main.Settings.Graphics.FieldOfView}");
      this.fieldOfViewView.SetMinValue(graphicsGameSettings.FieldOfView.MinValue);
      this.fieldOfViewView.SetMaxValue(graphicsGameSettings.FieldOfView.MaxValue);
      this.fieldOfViewView.SetValueNameFunction(Convert.ToString);
      this.fieldOfViewView.SetSetting(graphicsGameSettings.FieldOfView);
      this.fieldOfViewView.SetValueValidationFunction(SettingsViewUtility.RoundValueTo5, 5f);
      FloatSettingsValueView fieldOfViewView = this.fieldOfViewView;
      fieldOfViewView.VisibleValueChangeEvent = fieldOfViewView.VisibleValueChangeEvent + GraphicSettingsHelper.OnAutoValueChange;
    }

    private void OnSliderChange<T>(SettingsValueView<T> view) => ResetApplyButton();

    private float RenderScaleValueValidation(float value)
    {
      return SettingsViewUtility.RoundValue(value, 0.05f);
    }

    private void OnApplyButtonClick()
    {
      prevWidth = Screen.width;
      prevHeight = Screen.height;
      prevFullScreenMode = Screen.fullScreenMode;
      prevRenderScale = graphicsGameSettings.RenderScale.Value;
      FullScreenMode fullscreenMode = prevFullScreenMode;
      switch (fullScreenModeView.VisibleValue)
      {
        case 0:
          fullscreenMode = FullScreenMode.Windowed;
          break;
        case 1:
          fullscreenMode = FullScreenMode.FullScreenWindow;
          break;
        case 2:
          fullscreenMode = FullScreenMode.ExclusiveFullScreen;
          break;
      }
      int visibleValue = resolutionView.VisibleValue;
      Screen.SetResolution(resolutionWidths[visibleValue], resolutionHeights[visibleValue], fullscreenMode);
      renderScaleView.ApplyVisibleValue();
      graphicsGameSettings.Apply();
      if (confirmationInstance == null)
        confirmationInstance = Instantiate(confirmationPrefab, transform, false);
      confirmationInstance.Show("{UI.Menu.Main.Settings.Display.Confirmation}", StopAutorevertCoroutine, Revert);
      autorevertCoroutine = StartCoroutine(AutorevertCoroutine());
      gammaView.RevertVisibleValue();
      fieldOfViewView.RevertVisibleValue();
    }

    private IEnumerator AutorevertCoroutine()
    {
      yield return new WaitForSeconds(confirmationTime);
      autorevertCoroutine = null;
      Revert();
    }

    private void Revert()
    {
      confirmationInstance.Hide();
      StopAutorevertCoroutine();
      Screen.SetResolution(prevWidth, prevHeight, prevFullScreenMode);
      graphicsGameSettings.RenderScale.Value = prevRenderScale;
      graphicsGameSettings.Apply();
    }

    private void StopAutorevertCoroutine()
    {
      if (autorevertCoroutine == null)
        return;
      StopCoroutine(autorevertCoroutine);
      autorevertCoroutine = null;
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      ResetSliders();
      gammaView?.RevertVisibleValue();
      fieldOfViewView?.RevertVisibleValue();
      InstanceByRequest<ScreenGameSettings>.Instance.OnApply += ResetSliders;
      graphicsGameSettings.OnApply += ResetSliders;
      InputService.Instance.onJoystickUsedChanged += new Action<bool>(((SettingsView) this).OnJoystick);
      OnJoystick(InputService.Instance.JoystickUsed);
    }

    protected override void OnDisable()
    {
      base.OnDisable();
      if (autorevertCoroutine != null)
        Revert();
      InstanceByRequest<ScreenGameSettings>.Instance.OnApply -= ResetSliders;
      graphicsGameSettings.OnApply -= ResetSliders;
      InputService.Instance.onJoystickUsedChanged -= new Action<bool>(((SettingsView) this).OnJoystick);
    }

    protected override void OnJoystick(bool isUsed)
    {
      base.OnJoystick(isUsed);
      isUsed = false;
      fullScreenModeView.gameObject.SetActive(!isUsed);
      resolutionView.gameObject.SetActive(!isUsed);
      renderScaleView.gameObject.SetActive(!isUsed);
      fieldOfViewView?.gameObject.SetActive(isUsed);
      gammaView?.gameObject.SetActive(isUsed);
    }

    protected override void OnButtonReset()
    {
      if (!applyButtonTipObject.activeSelf && !applyButton.interactable)
        return;
      OnApplyButtonClick();
    }

    private void ResetSliders()
    {
      bool flag1 = false;
      switch (Screen.fullScreenMode)
      {
        case FullScreenMode.ExclusiveFullScreen:
        case FullScreenMode.MaximizedWindow:
          fullScreenModeView.VisibleValue = 2;
          break;
        case FullScreenMode.FullScreenWindow:
          fullScreenModeView.VisibleValue = 1;
          break;
        case FullScreenMode.Windowed:
          fullScreenModeView.VisibleValue = 0;
          break;
        default:
          fullScreenModeView.VisibleValue = 0;
          flag1 = true;
          break;
      }
      int num = -1;
      names.Clear();
      resolutionWidths.Clear();
      resolutionHeights.Clear();
      foreach (Resolution resolution in Screen.resolutions)
      {
        if (resolution.height >= 720)
        {
          bool flag2 = false;
          for (int index = 0; index < resolutionWidths.Count; ++index)
          {
            if (resolution.width == resolutionWidths[index] && resolution.height == resolutionHeights[index])
            {
              flag2 = true;
              break;
            }
          }
          if (!flag2)
          {
            if (resolution.width == Screen.width && resolution.height == Screen.height)
              num = names.Count;
            resolutionWidths.Add(resolution.width);
            resolutionHeights.Add(resolution.height);
            names.Add(resolution.width + " × " + resolution.height);
          }
        }
      }
      if (num == -1)
      {
        num = names.Count;
        names.Add(Screen.width + " × " + Screen.height);
        resolutionWidths.Add(Screen.width);
        resolutionHeights.Add(Screen.height);
      }
      resolutionView.SetValueNames(names.ToArray());
      resolutionView.VisibleValue = num;
      fullScreenModeView.Interactable = !flag1;
      resolutionView.Interactable = !flag1;
      renderScaleView.RevertVisibleValue();
      ResetApplyButton();
    }

    private void ResetApplyButton()
    {
      int num = -1;
      switch (Screen.fullScreenMode)
      {
        case FullScreenMode.ExclusiveFullScreen:
        case FullScreenMode.MaximizedWindow:
          num = 2;
          break;
        case FullScreenMode.FullScreenWindow:
          num = 1;
          break;
        case FullScreenMode.Windowed:
          num = 0;
          break;
      }
      if (num != -1 && num != fullScreenModeView.VisibleValue)
      {
        applyButton.interactable = true;
        applyButtonTipObject?.SetActive(true);
      }
      else
      {
        int visibleValue = resolutionView.VisibleValue;
        if (Screen.width != resolutionWidths[visibleValue] || Screen.height != resolutionHeights[visibleValue])
        {
          applyButton.interactable = true;
          applyButtonTipObject?.SetActive(true);
        }
        else if (renderScaleView.VisibleValue != (double) graphicsGameSettings.RenderScale.Value)
        {
          applyButton.interactable = true;
          applyButtonTipObject?.SetActive(true);
        }
        else
        {
          applyButton.interactable = false;
          applyButtonTipObject?.SetActive(false);
        }
      }
    }
  }
}
