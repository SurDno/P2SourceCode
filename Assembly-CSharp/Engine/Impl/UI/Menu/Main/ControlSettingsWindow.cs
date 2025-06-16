// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Menu.Main.ControlSettingsWindow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Engine.Source.Services.CameraServices;
using Engine.Source.Services.Inputs;
using Engine.Source.Settings;
using InputServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

#nullable disable
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
      this.RegisterLayer();
      this.inputGameSetting = InstanceByRequest<InputGameSetting>.Instance;
      this.SetMenuState();
      this.sliderMouseSensitivity.onValueChanged.AddListener(new UnityAction<float>(this.Slider_Mouse_Sensitivity_Value_Changed_Handler));
      this.toggleInvertMouse.onValueChanged.AddListener(new UnityAction<bool>(this.Toggle_Mouse_Invert_Value_Changed_Handler));
      Button[] componentsInChildren = this.GetComponentsInChildren<Button>(true);
      for (int index = 0; index < componentsInChildren.Length; ++index)
      {
        componentsInChildren[index].gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((UnityAction<BaseEventData>) (eventData => this.Button_Click_Handler()));
        componentsInChildren[index].gameObject.GetComponent<EventTrigger>().triggers.Add(entry);
      }
      base.Initialize();
    }

    private void SetMenuState()
    {
      this.toggleInvertMouse.isOn = this.inputGameSetting.MouseInvert.Value;
      this.sliderMouseSensitivity.value = this.inputGameSetting.MouseSensitivity.Value;
    }

    public void Button_Click_Handler()
    {
      if (!this.gameObject.activeInHierarchy)
        return;
      this.gameObject.GetComponent<AudioSource>().PlayOneShot(this.clickSound);
    }

    public void Button_Back_Click_Handler() => ServiceLocator.GetService<UIService>().Pop();

    public void Button_Reset_Click_Handler()
    {
      this.inputGameSetting.MouseInvert.Value = this.inputGameSetting.MouseInvert.DefaultValue;
      this.toggleInvertMouse.isOn = this.inputGameSetting.MouseInvert.Value;
      this.Toggle_Mouse_Invert_Value_Changed_Handler(this.inputGameSetting.MouseInvert.Value);
      this.inputGameSetting.MouseSensitivity.Value = this.inputGameSetting.MouseSensitivity.DefaultValue;
      this.sliderMouseSensitivity.value = this.inputGameSetting.MouseSensitivity.Value;
      this.Slider_Mouse_Sensitivity_Value_Changed_Handler(this.inputGameSetting.MouseSensitivity.Value);
    }

    public void Slider_Mouse_Sensitivity_Value_Changed_Handler(float value)
    {
      this.inputGameSetting.MouseSensitivity.Value = value;
    }

    public void Toggle_Mouse_Invert_Value_Changed_Handler(bool value)
    {
      this.inputGameSetting.MouseInvert.Value = value;
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      this.SetMenuState();
      this.lastCameraKind = ServiceLocator.GetService<CameraService>().Kind;
      ServiceLocator.GetService<CameraService>().Kind = CameraKindEnum.Unknown;
      InstanceByRequest<EngineApplication>.Instance.IsPaused = true;
      CursorService.Instance.Free = CursorService.Instance.Visible = true;
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
    }

    protected override void OnDisable()
    {
      ServiceLocator.GetService<CameraService>().Kind = this.lastCameraKind;
      InstanceByRequest<EngineApplication>.Instance.IsPaused = false;
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      base.OnDisable();
    }
  }
}
