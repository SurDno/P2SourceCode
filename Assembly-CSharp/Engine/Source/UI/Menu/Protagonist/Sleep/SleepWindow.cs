using System;
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
using UnityEngine;
using UnityEngine.UI;

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
      lastCameraKind = ServiceLocator.GetService<CameraService>().Kind;
      ServiceLocator.GetService<CameraService>().Kind = CameraKindEnum.Unknown;
      CursorService.Instance.Free = CursorService.Instance.Visible = true;
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.AddListener(GameActionType.Cancel, CancelListener);
      service.AddListener(GameActionType.Submit, ControllerListener);
      service.AddListener(GameActionType.LStickLeft, ControllerListener);
      service.AddListener(GameActionType.LStickRight, ControllerListener);
      hoursSlider.value = 1f;
      UpdateButtons();
    }

    protected override void OnJoystick(bool joystick)
    {
      base.OnJoystick(joystick);
      closeButton.gameObject.SetActive(!joystick);
      sleepButton.gameObject.SetActive(!joystick);
      controlPanel.SetActive(joystick);
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      if (joystick)
        service.RemoveListener(GameActionType.Sleep, CancelListener);
      else
        service.AddListener(GameActionType.Sleep, CancelListener);
    }

    private bool ControllerListener(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed)
        return false;
      if (type == GameActionType.Submit & down)
      {
        sleepButton.onClick.Invoke();
        return true;
      }
      if (type == GameActionType.LStickLeft & down)
      {
        --hoursSlider.value;
        return true;
      }
      if (!(type == GameActionType.LStickRight & down))
        return false;
      ++hoursSlider.value;
      return true;
    }

    protected override void OnDisable()
    {
      if (EngineApplication.Sleep)
        Wakeup();
      ServiceLocator.GetService<CameraService>().Kind = lastCameraKind;
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.RemoveListener(GameActionType.Cancel, CancelListener);
      service.RemoveListener(GameActionType.Sleep, CancelListener);
      service.RemoveListener(GameActionType.Submit, ControllerListener);
      service.RemoveListener(GameActionType.LStickLeft, ControllerListener);
      service.RemoveListener(GameActionType.LStickRight, ControllerListener);
      base.OnDisable();
    }

    public override void Initialize()
    {
      RegisterLayer<ISleepWindow>(this);
      sleepButton.onClick.AddListener(Sleep);
      closeButton.onClick.AddListener(CloseWindow);
      hoursSlider.onValueChanged.AddListener(SetHoursText);
      base.Initialize();
    }

    public override Type GetWindowType() => typeof (ISleepWindow);

    public void CloseWindow()
    {
      if (EngineApplication.Sleep)
        return;
      ServiceLocator.GetService<UIService>().Pop();
    }

    public void SetHoursText(float value) => hoursIntView.IntValue = (int) value;

    private void Update()
    {
      if (Actor == null)
        return;
      TimeSpan absoluteGameTime = ServiceLocator.GetService<TimeService>().AbsoluteGameTime;
      if (!EngineApplication.Sleep)
        return;
      TimeSpan timeSpan = absoluteGameTime - startTime;
      if (timeSpan >= delayTime)
      {
        Wakeup();
      }
      else
      {
        float blend = Mathf.Sqrt(Mathf.Clamp01(Mathf.Min((float) timeSpan.TotalHours, (float) (delayTime - timeSpan).TotalHours) / timeFactorBlendHours));
        fadingBackground.color = fadingBackground.color with
        {
          a = blend
        };
        BlendTimeFactor(blend);
      }
    }

    private void Wakeup()
    {
      if (!EngineApplication.Sleep)
        return;
      EngineApplication.Sleep = false;
      ResetTimeFactor();
      ServiceLocator.GetService<LogicEventService>().FireEntityEvent(nameof (Wakeup), Target);
      UpdateButtons();
      UpdateSleepValue();
    }

    private void Sleep()
    {
      if (EngineApplication.Sleep)
        return;
      ServiceLocator.GetService<LogicEventService>().FireEntityEvent(nameof (Sleep), Target);
      EngineApplication.Sleep = true;
      startTime = ServiceLocator.GetService<TimeService>().AbsoluteGameTime;
      delayTime = TimeSpan.FromHours(hoursSlider.value);
      UpdateButtons();
      UpdateSleepValue();
    }

    private void BlendTimeFactor(float blend)
    {
      ITimeService service = ServiceLocator.GetService<ITimeService>();
      float gameTimeFactor = service.GameTimeFactor;
      if (gameTimeFactor != (double) lastBlendedTimeFactor)
        baseTimeFactor = gameTimeFactor;
      lastBlendedTimeFactor = Mathf.Lerp(baseTimeFactor, sleepTimeFactor, blend);
      service.GameTimeFactor = lastBlendedTimeFactor;
      service.SolarTimeFactor = lastBlendedTimeFactor;
    }

    private void ResetTimeFactor()
    {
      ITimeService service = ServiceLocator.GetService<ITimeService>();
      service.GameTimeFactor = baseTimeFactor;
      service.SolarTimeFactor = baseTimeFactor;
      lastBlendedTimeFactor = float.NaN;
    }

    private void UpdateSleepValue()
    {
      IParameter<bool> byName = Actor?.GetComponent<ParametersComponent>()?.GetByName<bool>(ParameterNameEnum.Sleep);
      if (byName == null)
        return;
      byName.Value = EngineApplication.Sleep;
    }

    private void UpdateButtons()
    {
      contols.gameObject.SetActive(!EngineApplication.Sleep);
      fadingBackground.gameObject.SetActive(EngineApplication.Sleep);
    }
  }
}
