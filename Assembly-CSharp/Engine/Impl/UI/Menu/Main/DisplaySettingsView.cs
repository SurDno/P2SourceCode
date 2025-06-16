// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Menu.Main.DisplaySettingsView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Commons;
using Engine.Source.Settings;
using InputServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#nullable disable
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
      this.graphicsGameSettings = InstanceByRequest<GraphicsGameSettings>.Instance;
      this.layout = UnityEngine.Object.Instantiate<LayoutContainer>(this.listLayoutPrefab, this.transform, false);
      this.fullScreenModeView = UnityEngine.Object.Instantiate<NamedIntSettingsValueView>(this.namedIntValueViewPrefab, (Transform) this.layout.Content, false);
      this.fullScreenModeView.SetName("{UI.Menu.Main.Settings.Graphics.FullScreenMode}");
      this.fullScreenModeView.SetValueNames(new string[3]
      {
        "{UI.Menu.Main.Settings.Graphics.FullScreenMode.Window}",
        "{UI.Menu.Main.Settings.Graphics.FullScreenMode.Borderless}",
        "{UI.Menu.Main.Settings.Graphics.FullScreenMode.FullScreen}"
      });
      NamedIntSettingsValueView fullScreenModeView = this.fullScreenModeView;
      fullScreenModeView.VisibleValueChangeEvent = fullScreenModeView.VisibleValueChangeEvent + new Action<SettingsValueView<int>>(this.OnSliderChange<int>);
      this.resolutionView = UnityEngine.Object.Instantiate<NamedIntSettingsValueView>(this.namedIntValueViewPrefab, (Transform) this.layout.Content, false);
      this.resolutionView.SetName("{UI.Menu.Main.Settings.Graphics.Resolution}");
      NamedIntSettingsValueView resolutionView = this.resolutionView;
      resolutionView.VisibleValueChangeEvent = resolutionView.VisibleValueChangeEvent + new Action<SettingsValueView<int>>(this.OnSliderChange<int>);
      this.renderScaleView = UnityEngine.Object.Instantiate<FloatSettingsValueView>(this.floatValueViewPrefab, (Transform) this.layout.Content, false);
      this.renderScaleView.SetName("{UI.Menu.Main.Settings.Graphics.RenderScale}");
      this.renderScaleView.SetMinValue(this.graphicsGameSettings.RenderScale.MinValue);
      this.renderScaleView.SetMaxValue(this.graphicsGameSettings.RenderScale.MaxValue);
      this.renderScaleView.SetValueNameFunction(new Func<float, string>(SettingsViewUtility.PercentValueName));
      this.renderScaleView.SetSetting(this.graphicsGameSettings.RenderScale);
      this.renderScaleView.SetValueValidationFunction(new Func<float, float>(this.RenderScaleValueValidation), 0.05f);
      FloatSettingsValueView renderScaleView = this.renderScaleView;
      renderScaleView.VisibleValueChangeEvent = renderScaleView.VisibleValueChangeEvent + new Action<SettingsValueView<float>>(this.OnSliderChange<float>);
      this.applyButton.onClick.AddListener(new UnityAction(this.OnApplyButtonClick));
      this.InitConsoleSettings(this.layout);
      base.Awake();
    }

    private void InitConsoleSettings(LayoutContainer layout)
    {
      this.gammaView = UnityEngine.Object.Instantiate<FloatSettingsValueView>(this.floatValueViewPrefab, (Transform) layout.Content, false);
      this.gammaView.SetName("{UI.Menu.Main.Settings.Graphics.Gamma}");
      this.gammaView.SetMinValue(this.graphicsGameSettings.Gamma.MinValue);
      this.gammaView.SetMaxValue(this.graphicsGameSettings.Gamma.MaxValue);
      this.gammaView.SetValueNameFunction(new Func<float, string>(SettingsViewUtility.GammaValueName));
      this.gammaView.SetSetting(this.graphicsGameSettings.Gamma);
      this.gammaView.SetValueValidationFunction(new Func<float, float>(SettingsViewUtility.GammaValueValidation), 0.025f);
      FloatSettingsValueView gammaView = this.gammaView;
      gammaView.VisibleValueChangeEvent = gammaView.VisibleValueChangeEvent + new Action<SettingsValueView<float>>(GraphicSettingsHelper.OnAutoValueChange<float>);
      this.graphicsGameSettings = InstanceByRequest<GraphicsGameSettings>.Instance;
      this.fieldOfViewView = UnityEngine.Object.Instantiate<FloatSettingsValueView>(this.floatValueViewPrefab, (Transform) layout.Content, false);
      this.fieldOfViewView.SetName("{UI.Menu.Main.Settings.Graphics.FieldOfView}");
      this.fieldOfViewView.SetMinValue(this.graphicsGameSettings.FieldOfView.MinValue);
      this.fieldOfViewView.SetMaxValue(this.graphicsGameSettings.FieldOfView.MaxValue);
      this.fieldOfViewView.SetValueNameFunction(new Func<float, string>(Convert.ToString));
      this.fieldOfViewView.SetSetting(this.graphicsGameSettings.FieldOfView);
      this.fieldOfViewView.SetValueValidationFunction(new Func<float, float>(SettingsViewUtility.RoundValueTo5), 5f);
      FloatSettingsValueView fieldOfViewView = this.fieldOfViewView;
      fieldOfViewView.VisibleValueChangeEvent = fieldOfViewView.VisibleValueChangeEvent + new Action<SettingsValueView<float>>(GraphicSettingsHelper.OnAutoValueChange<float>);
    }

    private void OnSliderChange<T>(SettingsValueView<T> view) => this.ResetApplyButton();

    private float RenderScaleValueValidation(float value)
    {
      return SettingsViewUtility.RoundValue(value, 0.05f);
    }

    private void OnApplyButtonClick()
    {
      this.prevWidth = Screen.width;
      this.prevHeight = Screen.height;
      this.prevFullScreenMode = Screen.fullScreenMode;
      this.prevRenderScale = this.graphicsGameSettings.RenderScale.Value;
      FullScreenMode fullscreenMode = this.prevFullScreenMode;
      switch (this.fullScreenModeView.VisibleValue)
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
      int visibleValue = this.resolutionView.VisibleValue;
      Screen.SetResolution(this.resolutionWidths[visibleValue], this.resolutionHeights[visibleValue], fullscreenMode);
      this.renderScaleView.ApplyVisibleValue();
      this.graphicsGameSettings.Apply();
      if ((UnityEngine.Object) this.confirmationInstance == (UnityEngine.Object) null)
        this.confirmationInstance = UnityEngine.Object.Instantiate<ConfirmationWindow>(this.confirmationPrefab, this.transform, false);
      this.confirmationInstance.Show("{UI.Menu.Main.Settings.Display.Confirmation}", new Action(this.StopAutorevertCoroutine), new Action(this.Revert));
      this.autorevertCoroutine = this.StartCoroutine(this.AutorevertCoroutine());
      this.gammaView.RevertVisibleValue();
      this.fieldOfViewView.RevertVisibleValue();
    }

    private IEnumerator AutorevertCoroutine()
    {
      yield return (object) new WaitForSeconds(this.confirmationTime);
      this.autorevertCoroutine = (Coroutine) null;
      this.Revert();
    }

    private void Revert()
    {
      this.confirmationInstance.Hide();
      this.StopAutorevertCoroutine();
      Screen.SetResolution(this.prevWidth, this.prevHeight, this.prevFullScreenMode);
      this.graphicsGameSettings.RenderScale.Value = this.prevRenderScale;
      this.graphicsGameSettings.Apply();
    }

    private void StopAutorevertCoroutine()
    {
      if (this.autorevertCoroutine == null)
        return;
      this.StopCoroutine(this.autorevertCoroutine);
      this.autorevertCoroutine = (Coroutine) null;
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      this.ResetSliders();
      this.gammaView?.RevertVisibleValue();
      this.fieldOfViewView?.RevertVisibleValue();
      InstanceByRequest<ScreenGameSettings>.Instance.OnApply += new Action(this.ResetSliders);
      this.graphicsGameSettings.OnApply += new Action(this.ResetSliders);
      InputService.Instance.onJoystickUsedChanged += new Action<bool>(((SettingsView) this).OnJoystick);
      this.OnJoystick(InputService.Instance.JoystickUsed);
    }

    protected override void OnDisable()
    {
      base.OnDisable();
      if (this.autorevertCoroutine != null)
        this.Revert();
      InstanceByRequest<ScreenGameSettings>.Instance.OnApply -= new Action(this.ResetSliders);
      this.graphicsGameSettings.OnApply -= new Action(this.ResetSliders);
      InputService.Instance.onJoystickUsedChanged -= new Action<bool>(((SettingsView) this).OnJoystick);
    }

    protected override void OnJoystick(bool isUsed)
    {
      base.OnJoystick(isUsed);
      isUsed = false;
      this.fullScreenModeView.gameObject.SetActive(!isUsed);
      this.resolutionView.gameObject.SetActive(!isUsed);
      this.renderScaleView.gameObject.SetActive(!isUsed);
      this.fieldOfViewView?.gameObject.SetActive(isUsed);
      this.gammaView?.gameObject.SetActive(isUsed);
    }

    protected override void OnButtonReset()
    {
      if (!this.applyButtonTipObject.activeSelf && !this.applyButton.interactable)
        return;
      this.OnApplyButtonClick();
    }

    private void ResetSliders()
    {
      bool flag1 = false;
      switch (Screen.fullScreenMode)
      {
        case FullScreenMode.ExclusiveFullScreen:
        case FullScreenMode.MaximizedWindow:
          this.fullScreenModeView.VisibleValue = 2;
          break;
        case FullScreenMode.FullScreenWindow:
          this.fullScreenModeView.VisibleValue = 1;
          break;
        case FullScreenMode.Windowed:
          this.fullScreenModeView.VisibleValue = 0;
          break;
        default:
          this.fullScreenModeView.VisibleValue = 0;
          flag1 = true;
          break;
      }
      int num = -1;
      this.names.Clear();
      this.resolutionWidths.Clear();
      this.resolutionHeights.Clear();
      foreach (Resolution resolution in Screen.resolutions)
      {
        if (resolution.height >= 720)
        {
          bool flag2 = false;
          for (int index = 0; index < this.resolutionWidths.Count; ++index)
          {
            if (resolution.width == this.resolutionWidths[index] && resolution.height == this.resolutionHeights[index])
            {
              flag2 = true;
              break;
            }
          }
          if (!flag2)
          {
            if (resolution.width == Screen.width && resolution.height == Screen.height)
              num = this.names.Count;
            this.resolutionWidths.Add(resolution.width);
            this.resolutionHeights.Add(resolution.height);
            this.names.Add(resolution.width.ToString() + " × " + (object) resolution.height);
          }
        }
      }
      if (num == -1)
      {
        num = this.names.Count;
        this.names.Add(Screen.width.ToString() + " × " + (object) Screen.height);
        this.resolutionWidths.Add(Screen.width);
        this.resolutionHeights.Add(Screen.height);
      }
      this.resolutionView.SetValueNames(this.names.ToArray());
      this.resolutionView.VisibleValue = num;
      this.fullScreenModeView.Interactable = !flag1;
      this.resolutionView.Interactable = !flag1;
      this.renderScaleView.RevertVisibleValue();
      this.ResetApplyButton();
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
      if (num != -1 && num != this.fullScreenModeView.VisibleValue)
      {
        this.applyButton.interactable = true;
        this.applyButtonTipObject?.SetActive(true);
      }
      else
      {
        int visibleValue = this.resolutionView.VisibleValue;
        if (Screen.width != this.resolutionWidths[visibleValue] || Screen.height != this.resolutionHeights[visibleValue])
        {
          this.applyButton.interactable = true;
          this.applyButtonTipObject?.SetActive(true);
        }
        else if ((double) this.renderScaleView.VisibleValue != (double) this.graphicsGameSettings.RenderScale.Value)
        {
          this.applyButton.interactable = true;
          this.applyButtonTipObject?.SetActive(true);
        }
        else
        {
          this.applyButton.interactable = false;
          this.applyButtonTipObject?.SetActive(false);
        }
      }
    }
  }
}
