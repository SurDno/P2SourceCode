using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services.CameraServices;
using Engine.Source.UI;
using Engine.Source.Utility;
using InputServices;

namespace Engine.Impl.UI.Menu.Main
{
  public abstract class SimpleWindow : UIWindow, IWindow
  {
    private CameraKindEnum lastCameraKind;
    private bool lastPause;
    private bool lastVisibleCursor;

    public override void Initialize()
    {
      this.RegisterLayer();
      base.Initialize();
    }

    protected override void OnDisable()
    {
      ServiceLocator.GetService<CameraService>().Kind = this.lastCameraKind;
      CursorService.Instance.Free = CursorService.Instance.Visible = this.lastVisibleCursor;
      PlayerUtility.ShowPlayerHands(!this.lastPause);
      InstanceByRequest<EngineApplication>.Instance.IsPaused = this.lastPause;
      base.OnDisable();
    }

    protected override void OnEnable()
    {
      this.lastPause = InstanceByRequest<EngineApplication>.Instance.IsPaused;
      InstanceByRequest<EngineApplication>.Instance.IsPaused = true;
      PlayerUtility.ShowPlayerHands(false);
      this.lastVisibleCursor = CursorService.Instance.Visible;
      CursorService.Instance.Free = CursorService.Instance.Visible = true;
      this.lastCameraKind = ServiceLocator.GetService<CameraService>().Kind;
      ServiceLocator.GetService<CameraService>().Kind = CameraKindEnum.Unknown;
      base.OnEnable();
    }

    protected abstract void RegisterLayer();
  }
}
