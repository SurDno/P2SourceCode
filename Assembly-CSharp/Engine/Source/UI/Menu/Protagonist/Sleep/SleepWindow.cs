// Decompiled with JetBrains decompiler
// Type: Engine.Source.UI.Menu.Protagonist.Sleep.SleepWindow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Controls;
using Engine.Impl.UI.Menu;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Services;
using Engine.Source.Services.CameraServices;
using Engine.Source.Services.Inputs;
using InputServices;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#nullable disable
namespace Engine.Source.UI.Menu.Protagonist.Sleep
{
  public class SleepWindow : UIWindow, ISleepWindow, IWindow
  {
    [SerializeField]
    private GameObject contols;
    [SerializeField]
    private Slider hoursSlider;
    [SerializeField]
    private IntView hoursIntView;
    [SerializeField]
    private Button sleepButton;
    [SerializeField]
    private Button closeButton;
    [SerializeField]
    private Image fadingBackground;
    [Space]
    [SerializeField]
    private float sleepTimeFactor;
    [SerializeField]
    private float timeFactorBlendHours;
    [SerializeField]
    private float hurtingThreshold;
    [SerializeField]
    private float criticalFatigueThreshold;
    private CameraKindEnum lastCameraKind;
    private TimeSpan startTime;
    private TimeSpan delayTime;
    private float baseTimeFactor = float.NaN;
    private float lastBlendedTimeFactor = float.NaN;
    [SerializeField]
    private GameObject controlPanel;

    public IEntity Actor { get; set; }

    public IEntity Target { get; set; }

    protected override void OnEnable()
    {
      base.OnEnable();
      this.lastCameraKind = ServiceLocator.GetService<CameraService>().Kind;
      ServiceLocator.GetService<CameraService>().Kind = CameraKindEnum.Unknown;
      CursorService.Instance.Free = CursorService.Instance.Visible = true;
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.AddListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      service.AddListener(GameActionType.Submit, new GameActionHandle(this.ControllerListener));
      service.AddListener(GameActionType.LStickLeft, new GameActionHandle(this.ControllerListener));
      service.AddListener(GameActionType.LStickRight, new GameActionHandle(this.ControllerListener));
      this.hoursSlider.value = 1f;
      this.UpdateButtons();
    }

    protected override void OnJoystick(bool joystick)
    {
      base.OnJoystick(joystick);
      this.closeButton.gameObject.SetActive(!joystick);
      this.sleepButton.gameObject.SetActive(!joystick);
      this.controlPanel.SetActive(joystick);
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      if (joystick)
        service.RemoveListener(GameActionType.Sleep, new GameActionHandle(((UIWindow) this).CancelListener));
      else
        service.AddListener(GameActionType.Sleep, new GameActionHandle(((UIWindow) this).CancelListener));
    }

    private bool ControllerListener(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed)
        return false;
      if (type == GameActionType.Submit & down)
      {
        this.sleepButton.onClick.Invoke();
        return true;
      }
      if (type == GameActionType.LStickLeft & down)
      {
        --this.hoursSlider.value;
        return true;
      }
      if (!(type == GameActionType.LStickRight & down))
        return false;
      ++this.hoursSlider.value;
      return true;
    }

    protected override void OnDisable()
    {
      if (EngineApplication.Sleep)
        this.Wakeup();
      ServiceLocator.GetService<CameraService>().Kind = this.lastCameraKind;
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.RemoveListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      service.RemoveListener(GameActionType.Sleep, new GameActionHandle(((UIWindow) this).CancelListener));
      service.RemoveListener(GameActionType.Submit, new GameActionHandle(this.ControllerListener));
      service.RemoveListener(GameActionType.LStickLeft, new GameActionHandle(this.ControllerListener));
      service.RemoveListener(GameActionType.LStickRight, new GameActionHandle(this.ControllerListener));
      base.OnDisable();
    }

    public override void Initialize()
    {
      this.RegisterLayer<ISleepWindow>((ISleepWindow) this);
      this.sleepButton.onClick.AddListener(new UnityAction(this.Sleep));
      this.closeButton.onClick.AddListener(new UnityAction(this.CloseWindow));
      this.hoursSlider.onValueChanged.AddListener(new UnityAction<float>(this.SetHoursText));
      base.Initialize();
    }

    public override System.Type GetWindowType() => typeof (ISleepWindow);

    public void CloseWindow()
    {
      if (EngineApplication.Sleep)
        return;
      ServiceLocator.GetService<UIService>().Pop();
    }

    public void SetHoursText(float value) => this.hoursIntView.IntValue = (int) value;

    private void Update()
    {
      if (this.Actor == null)
        return;
      TimeSpan absoluteGameTime = ServiceLocator.GetService<TimeService>().AbsoluteGameTime;
      if (!EngineApplication.Sleep)
        return;
      TimeSpan timeSpan = absoluteGameTime - this.startTime;
      if (timeSpan >= this.delayTime)
      {
        this.Wakeup();
      }
      else
      {
        float blend = Mathf.Sqrt(Mathf.Clamp01(Mathf.Min((float) timeSpan.TotalHours, (float) (this.delayTime - timeSpan).TotalHours) / this.timeFactorBlendHours));
        this.fadingBackground.color = this.fadingBackground.color with
        {
          a = blend
        };
        this.BlendTimeFactor(blend);
      }
    }

    private void Wakeup()
    {
      if (!EngineApplication.Sleep)
        return;
      EngineApplication.Sleep = false;
      this.ResetTimeFactor();
      ServiceLocator.GetService<LogicEventService>().FireEntityEvent(nameof (Wakeup), this.Target);
      this.UpdateButtons();
      this.UpdateSleepValue();
    }

    private void Sleep()
    {
      if (EngineApplication.Sleep)
        return;
      ServiceLocator.GetService<LogicEventService>().FireEntityEvent(nameof (Sleep), this.Target);
      EngineApplication.Sleep = true;
      this.startTime = ServiceLocator.GetService<TimeService>().AbsoluteGameTime;
      this.delayTime = TimeSpan.FromHours((double) this.hoursSlider.value);
      this.UpdateButtons();
      this.UpdateSleepValue();
    }

    private void BlendTimeFactor(float blend)
    {
      ITimeService service = ServiceLocator.GetService<ITimeService>();
      float gameTimeFactor = service.GameTimeFactor;
      if ((double) gameTimeFactor != (double) this.lastBlendedTimeFactor)
        this.baseTimeFactor = gameTimeFactor;
      this.lastBlendedTimeFactor = Mathf.Lerp(this.baseTimeFactor, this.sleepTimeFactor, blend);
      service.GameTimeFactor = this.lastBlendedTimeFactor;
      service.SolarTimeFactor = this.lastBlendedTimeFactor;
    }

    private void ResetTimeFactor()
    {
      ITimeService service = ServiceLocator.GetService<ITimeService>();
      service.GameTimeFactor = this.baseTimeFactor;
      service.SolarTimeFactor = this.baseTimeFactor;
      this.lastBlendedTimeFactor = float.NaN;
    }

    private void UpdateSleepValue()
    {
      IParameter<bool> byName = this.Actor?.GetComponent<ParametersComponent>()?.GetByName<bool>(ParameterNameEnum.Sleep);
      if (byName == null)
        return;
      byName.Value = EngineApplication.Sleep;
    }

    private void UpdateButtons()
    {
      this.contols.gameObject.SetActive(!EngineApplication.Sleep);
      this.fadingBackground.gameObject.SetActive(EngineApplication.Sleep);
    }
  }
}
