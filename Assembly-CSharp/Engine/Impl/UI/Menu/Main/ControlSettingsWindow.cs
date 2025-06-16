using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Engine.Source.Services.CameraServices;
using Engine.Source.Services.Inputs;
using Engine.Source.Settings;
using InputServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Engine.Impl.UI.Menu.Main
{
  public abstract class ControlSettingsWindow : UIWindow
  {
    private CameraKindEnum lastCameraKind;
    [Header("Sounds")]
    [SerializeField]
    [FormerlySerializedAs("ClickSound")]
    private AudioClip clickSound;
    [SerializeField]
    [FormerlySerializedAs("Slider_MouseSensitivity")]
    private Slider sliderMouseSensitivity;
    [SerializeField]
    [FormerlySerializedAs("Toggle_InvertMouse")]
    private Toggle toggleInvertMouse;
    private InputGameSetting inputGameSetting;

    protected abstract void RegisterLayer();

    public override void Initialize()
    {
      RegisterLayer();
      inputGameSetting = InstanceByRequest<InputGameSetting>.Instance;
      SetMenuState();
      sliderMouseSensitivity.onValueChanged.AddListener(Slider_Mouse_Sensitivity_Value_Changed_Handler);
      toggleInvertMouse.onValueChanged.AddListener(Toggle_Mouse_Invert_Value_Changed_Handler);
      Button[] componentsInChildren = GetComponentsInChildren<Button>(true);
      for (int index = 0; index < componentsInChildren.Length; ++index)
      {
        componentsInChildren[index].gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener(eventData => Button_Click_Handler());
        componentsInChildren[index].gameObject.GetComponent<EventTrigger>().triggers.Add(entry);
      }
      base.Initialize();
    }

    private void SetMenuState()
    {
      toggleInvertMouse.isOn = inputGameSetting.MouseInvert.Value;
      sliderMouseSensitivity.value = inputGameSetting.MouseSensitivity.Value;
    }

    public void Button_Click_Handler()
    {
      if (!gameObject.activeInHierarchy)
        return;
      gameObject.GetComponent<AudioSource>().PlayOneShot(clickSound);
    }

    public void Button_Back_Click_Handler() => ServiceLocator.GetService<UIService>().Pop();

    public void Button_Reset_Click_Handler()
    {
      inputGameSetting.MouseInvert.Value = inputGameSetting.MouseInvert.DefaultValue;
      toggleInvertMouse.isOn = inputGameSetting.MouseInvert.Value;
      Toggle_Mouse_Invert_Value_Changed_Handler(inputGameSetting.MouseInvert.Value);
      inputGameSetting.MouseSensitivity.Value = inputGameSetting.MouseSensitivity.DefaultValue;
      sliderMouseSensitivity.value = inputGameSetting.MouseSensitivity.Value;
      Slider_Mouse_Sensitivity_Value_Changed_Handler(inputGameSetting.MouseSensitivity.Value);
    }

    public void Slider_Mouse_Sensitivity_Value_Changed_Handler(float value)
    {
      inputGameSetting.MouseSensitivity.Value = value;
    }

    public void Toggle_Mouse_Invert_Value_Changed_Handler(bool value)
    {
      inputGameSetting.MouseInvert.Value = value;
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      SetMenuState();
      lastCameraKind = ServiceLocator.GetService<CameraService>().Kind;
      ServiceLocator.GetService<CameraService>().Kind = CameraKindEnum.Unknown;
      InstanceByRequest<EngineApplication>.Instance.IsPaused = true;
      CursorService.Instance.Free = CursorService.Instance.Visible = true;
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, CancelListener);
    }

    protected override void OnDisable()
    {
      ServiceLocator.GetService<CameraService>().Kind = lastCameraKind;
      InstanceByRequest<EngineApplication>.Instance.IsPaused = false;
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, CancelListener);
      base.OnDisable();
    }
  }
}
