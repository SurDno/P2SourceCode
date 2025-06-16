using System;
using SRDebugger.Internal;
using SRF;
using SRF.Service;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SRDebugger.Services.Implementation;

[Service(typeof(IDebugService))]
public class SRDebugService : IDebugService {
	private readonly IDebugPanelService _debugPanelService;
	private bool _IsDebugging;

	public SRDebugService() {
		SRServiceManager.RegisterService<IDebugService>(this);
		SRServiceManager.GetService<IProfilerService>();
		_debugPanelService = SRServiceManager.GetService<IDebugPanelService>();
		_debugPanelService.VisibilityChanged += DebugPanelServiceOnVisibilityChanged;
		if (Settings.EnableKeyboardShortcuts)
			SRServiceManager.GetService<KeyboardShortcutListenerService>();
		Object.DontDestroyOnLoad(Hierarchy.Get("SRDebugger").gameObject);
	}

	public Settings Settings => Settings.Instance;

	public bool IsDebugging {
		get => _IsDebugging;
		set {
			_IsDebugging = value;
			var debuggingChanged = OnDebuggingChanged;
			if (debuggingChanged == null)
				return;
			debuggingChanged();
		}
	}

	public event Action OnDebuggingChanged;

	public bool IsDebugPanelVisible => _debugPanelService.IsVisible;

	public bool IsProfilerDocked {
		get => Service.PinnedUI.IsProfilerPinned;
		set => Service.PinnedUI.IsProfilerPinned = value;
	}

	public void ShowDebugPanel() {
		_debugPanelService.IsVisible = true;
	}

	public void ShowDebugPanel(DefaultTabs tab) {
		_debugPanelService.IsVisible = true;
		_debugPanelService.OpenTab(tab);
	}

	public void HideDebugPanel() {
		_debugPanelService.IsVisible = false;
	}

	public void DestroyDebugPanel() {
		_debugPanelService.IsVisible = false;
		_debugPanelService.Unload();
	}

	public event VisibilityChangedDelegate PanelVisibilityChanged;

	private void DebugPanelServiceOnVisibilityChanged(IDebugPanelService debugPanelService, bool b) {
		if (PanelVisibilityChanged == null)
			return;
		try {
			PanelVisibilityChanged(b);
		} catch (Exception ex) {
			Debug.LogError("[SRDebugger] Event target threw exception (IDebugService.PanelVisiblityChanged)");
			Debug.LogException(ex);
		}
	}
}