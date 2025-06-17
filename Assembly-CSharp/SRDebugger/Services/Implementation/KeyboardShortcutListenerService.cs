using System.Collections.Generic;
using SRDebugger.Internal;
using SRF;
using SRF.Service;
using UnityEngine;

namespace SRDebugger.Services.Implementation
{
  [Service(typeof (KeyboardShortcutListenerService))]
  public class KeyboardShortcutListenerService : SRServiceBase<KeyboardShortcutListenerService>
  {
    private List<Settings.KeyboardShortcut> shortcuts;
    private bool needClosePanel;

    protected override void Awake()
    {
      base.Awake();
      CachedTransform.SetParent(Hierarchy.Get("SRDebugger"));
      shortcuts = [..Settings.Instance.KeyboardShortcuts];
    }

    private void ToggleTab(DefaultTabs t)
    {
      DefaultTabs? activeTab = Service.Panel.ActiveTab;
      if (Service.Panel.IsVisible && activeTab.HasValue && activeTab.Value == t)
        SRDebug.Instance.HideDebugPanel();
      else
        SRDebug.Instance.ShowDebugPanel(t);
    }

    private void ExecuteShortcut(Settings.KeyboardShortcut shortcut)
    {
      switch (shortcut.Action)
      {
        case Settings.ShortcutActions.ToggleDebugging:
          SRDebug.Instance.IsDebugging = !SRDebug.Instance.IsDebugging;
          break;
        case Settings.ShortcutActions.OpenSystemInfoTab:
          ToggleTab(DefaultTabs.SystemInformation);
          break;
        case Settings.ShortcutActions.OpenLoggerTab:
          ToggleTab(DefaultTabs.Logger);
          break;
        case Settings.ShortcutActions.OpenProfilerTab:
          ToggleTab(DefaultTabs.Profiler);
          break;
        case Settings.ShortcutActions.OpenConsoleTab:
          ToggleTab(DefaultTabs.Console);
          break;
        case Settings.ShortcutActions.OpenInspectorTab:
          ToggleTab(DefaultTabs.Inspector);
          break;
        case Settings.ShortcutActions.ClosePanel:
          SRDebug.Instance.HideDebugPanel();
          break;
        case Settings.ShortcutActions.OpenPanel:
          SRDebug.Instance.ShowDebugPanel();
          break;
        case Settings.ShortcutActions.TogglePanel:
          if (SRDebug.Instance.IsDebugPanelVisible)
          {
            SRDebug.Instance.HideDebugPanel();
            break;
          }
          SRDebug.Instance.ShowDebugPanel();
          break;
        case Settings.ShortcutActions.ToggleDockedProfiler:
          SRDebug.Instance.IsProfilerDocked = !SRDebug.Instance.IsProfilerDocked;
          break;
        default:
          Debug.LogWarning("[SRDebugger] Unhandled keyboard shortcut: " + shortcut.Action);
          break;
      }
    }

    private void Update()
    {
      if (Settings.Instance.KeyboardEscapeClose && Input.GetKeyUp(KeyCode.Escape) && Service.Panel.IsVisible)
        SRDebug.Instance.HideDebugPanel();
      bool flag1 = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
      bool flag2 = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
      bool flag3 = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
      for (int index = 0; index < shortcuts.Count; ++index)
      {
        Settings.KeyboardShortcut shortcut = shortcuts[index];
        if ((!shortcut.Control || flag1) && (!shortcut.Shift || flag3) && (!shortcut.Alt || flag2) && Input.GetKeyDown(shortcut.Key))
        {
          ExecuteShortcut(shortcut);
          break;
        }
      }
    }
  }
}
