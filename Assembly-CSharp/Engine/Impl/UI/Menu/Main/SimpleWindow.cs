using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services.CameraServices;
using Engine.Source.UI;
using Engine.Source.Utility;
using InputServices;

namespace Engine.Impl.UI.Menu.Main;

public abstract class SimpleWindow : UIWindow, IWindow {
	private CameraKindEnum lastCameraKind;
	private bool lastPause;
	private bool lastVisibleCursor;

	public override void Initialize() {
		RegisterLayer();
		base.Initialize();
	}

	protected override void OnDisable() {
		ServiceLocator.GetService<CameraService>().Kind = lastCameraKind;
		CursorService.Instance.Free = CursorService.Instance.Visible = lastVisibleCursor;
		PlayerUtility.ShowPlayerHands(!lastPause);
		InstanceByRequest<EngineApplication>.Instance.IsPaused = lastPause;
		base.OnDisable();
	}

	protected override void OnEnable() {
		lastPause = InstanceByRequest<EngineApplication>.Instance.IsPaused;
		InstanceByRequest<EngineApplication>.Instance.IsPaused = true;
		PlayerUtility.ShowPlayerHands(false);
		lastVisibleCursor = CursorService.Instance.Visible;
		CursorService.Instance.Free = CursorService.Instance.Visible = true;
		lastCameraKind = ServiceLocator.GetService<CameraService>().Kind;
		ServiceLocator.GetService<CameraService>().Kind = CameraKindEnum.Unknown;
		base.OnEnable();
	}

	protected abstract void RegisterLayer();
}