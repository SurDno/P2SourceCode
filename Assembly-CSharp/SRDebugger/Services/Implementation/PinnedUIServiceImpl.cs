using SRDebugger.UI.Other;
using SRF;
using SRF.Service;
using UnityEngine;

namespace SRDebugger.Services.Implementation;

[Service(typeof(IPinnedUIService))]
public class PinnedUIServiceImpl : SRServiceBase<IPinnedUIService>, IPinnedUIService {
	private PinnedUIRoot _uiRoot;

	public bool IsProfilerPinned {
		get => !(_uiRoot == null) && _uiRoot.Profiler.activeSelf;
		set {
			if (_uiRoot == null)
				Load();
			_uiRoot.Profiler.SetActive(value);
		}
	}

	protected override void Awake() {
		base.Awake();
		CachedTransform.SetParent(Hierarchy.Get("SRDebugger/UI"));
	}

	private void Load() {
		var prefab = Resources.Load<PinnedUIRoot>("SRDebugger/UI/Prefabs/PinnedUI");
		if (prefab == null)
			Debug.LogError("[SRDebugger.PinnedUI] Error loading ui prefab");
		else {
			var pinnedUiRoot = SRInstantiate.Instantiate(prefab);
			pinnedUiRoot.CachedTransform.SetParent(CachedTransform, false);
			_uiRoot = pinnedUiRoot;
			UpdateAnchors();
		}
	}

	private void UpdateAnchors() {
		switch (Settings.Instance.ProfilerAlignment) {
			case PinAlignment.TopLeft:
			case PinAlignment.BottomLeft:
			case PinAlignment.CenterLeft:
				_uiRoot.Profiler.transform.SetSiblingIndex(0);
				break;
			case PinAlignment.TopRight:
			case PinAlignment.BottomRight:
			case PinAlignment.CenterRight:
				_uiRoot.Profiler.transform.SetSiblingIndex(1);
				break;
		}

		switch (Settings.Instance.ProfilerAlignment) {
			case PinAlignment.TopLeft:
			case PinAlignment.TopRight:
				_uiRoot.ProfilerVerticalLayoutGroup.childAlignment = TextAnchor.UpperCenter;
				break;
			case PinAlignment.BottomLeft:
			case PinAlignment.BottomRight:
				_uiRoot.ProfilerVerticalLayoutGroup.childAlignment = TextAnchor.LowerCenter;
				break;
			case PinAlignment.CenterLeft:
			case PinAlignment.CenterRight:
				_uiRoot.ProfilerVerticalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
				break;
		}

		_uiRoot.ProfilerHandleManager.SetAlignment(Settings.Instance.ProfilerAlignment);
	}
}