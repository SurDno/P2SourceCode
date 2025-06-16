// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Menu.Protagonist.Investigation.InvestigationWindow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Services;
using Engine.Source.Services.CameraServices;
using Engine.Source.Services.Inputs;
using Engine.Source.UI;
using Engine.Source.Utility;
using InputServices;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

#nullable disable
namespace Engine.Impl.UI.Menu.Protagonist.Investigation
{
  public class InvestigationWindow : UIWindow, IInvestigationWindow, IWindow, IPauseMenu
  {
    [SerializeField]
    [FormerlySerializedAs("_Image")]
    private Image image;
    private CameraKindEnum lastCameraKind;
    [SerializeField]
    [FormerlySerializedAs("_TextInformation")]
    private StringView textInformation;
    [SerializeField]
    [FormerlySerializedAs("_TextTitle")]
    private StringView textTitle;

    public IStorageComponent Actor { get; set; }

    public IStorableComponent Target { get; set; }

    protected void UI_Cancel_Click_Handler() => ServiceLocator.GetService<UIService>().Pop();

    private void Clear()
    {
      this.image.sprite = (Sprite) null;
      this.textTitle.StringValue = (string) null;
      this.textInformation.StringValue = (string) null;
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      this.lastCameraKind = ServiceLocator.GetService<CameraService>().Kind;
      ServiceLocator.GetService<CameraService>().Kind = CameraKindEnum.Unknown;
      CursorService.Instance.Free = CursorService.Instance.Visible = true;
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      PlayerUtility.ShowPlayerHands(false);
      InstanceByRequest<EngineApplication>.Instance.IsPaused = true;
      this.Clear();
      if (this.Target == null || this.Target.IsDisposed)
        return;
      LocalizationService service = ServiceLocator.GetService<LocalizationService>();
      service.GetText(this.Target.Title);
      this.textTitle.StringValue = service.GetText(this.Target.Title);
      this.textInformation.StringValue = service.GetText(this.Target.Description);
      Sprite sprite = (Sprite) null;
      if (((StorableComponent) this.Target).Placeholder != null)
        sprite = ((StorableComponent) this.Target).Placeholder.ImageInformation.Value;
      this.image.sprite = sprite;
      ServiceLocator.GetService<LogicEventService>().FireEntityEvent("Investigation", (IEntity) this.Target.Owner.Template);
    }

    protected override void OnDisable()
    {
      this.Clear();
      ServiceLocator.GetService<CameraService>().Kind = this.lastCameraKind;
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      InstanceByRequest<EngineApplication>.Instance.IsPaused = false;
      PlayerUtility.ShowPlayerHands(true);
      base.OnDisable();
    }

    public override void Initialize()
    {
      this.RegisterLayer<IInvestigationWindow>((IInvestigationWindow) this);
      base.Initialize();
    }

    public override System.Type GetWindowType() => typeof (IInvestigationWindow);
  }
}
