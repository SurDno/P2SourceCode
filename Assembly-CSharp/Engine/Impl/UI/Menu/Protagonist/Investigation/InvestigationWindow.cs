using System;
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

namespace Engine.Impl.UI.Menu.Protagonist.Investigation;

public class InvestigationWindow : UIWindow, IInvestigationWindow, IWindow, IPauseMenu {
	[SerializeField] [FormerlySerializedAs("_Image")]
	private Image image;

	private CameraKindEnum lastCameraKind;

	[SerializeField] [FormerlySerializedAs("_TextInformation")]
	private StringView textInformation;

	[SerializeField] [FormerlySerializedAs("_TextTitle")]
	private StringView textTitle;

	public IStorageComponent Actor { get; set; }

	public IStorableComponent Target { get; set; }

	protected void UI_Cancel_Click_Handler() {
		ServiceLocator.GetService<UIService>().Pop();
	}

	private void Clear() {
		image.sprite = null;
		textTitle.StringValue = null;
		textInformation.StringValue = null;
	}

	protected override void OnEnable() {
		base.OnEnable();
		lastCameraKind = ServiceLocator.GetService<CameraService>().Kind;
		ServiceLocator.GetService<CameraService>().Kind = CameraKindEnum.Unknown;
		CursorService.Instance.Free = CursorService.Instance.Visible = true;
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, CancelListener);
		PlayerUtility.ShowPlayerHands(false);
		InstanceByRequest<EngineApplication>.Instance.IsPaused = true;
		Clear();
		if (Target == null || Target.IsDisposed)
			return;
		var service = ServiceLocator.GetService<LocalizationService>();
		service.GetText(Target.Title);
		textTitle.StringValue = service.GetText(Target.Title);
		textInformation.StringValue = service.GetText(Target.Description);
		Sprite sprite = null;
		if (((StorableComponent)Target).Placeholder != null)
			sprite = ((StorableComponent)Target).Placeholder.ImageInformation.Value;
		image.sprite = sprite;
		ServiceLocator.GetService<LogicEventService>().FireEntityEvent("Investigation", (IEntity)Target.Owner.Template);
	}

	protected override void OnDisable() {
		Clear();
		ServiceLocator.GetService<CameraService>().Kind = lastCameraKind;
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, CancelListener);
		InstanceByRequest<EngineApplication>.Instance.IsPaused = false;
		PlayerUtility.ShowPlayerHands(true);
		base.OnDisable();
	}

	public override void Initialize() {
		RegisterLayer<IInvestigationWindow>(this);
		base.Initialize();
	}

	public override Type GetWindowType() {
		return typeof(IInvestigationWindow);
	}
}