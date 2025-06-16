// Decompiled with JetBrains decompiler
// Type: SRDebugger.Services.Implementation.KeyboardShortcutListenerService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using SRF;
using SRF.Service;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace SRDebugger.Services.Implementation
{
  [SRF.Service.Service(typeof (KeyboardShortcutListenerService))]
  public class KeyboardShortcutListenerService : SRServiceBase<KeyboardShortcutListenerService>
  {
    private List<Settings.KeyboardShortcut> shortcuts;
    private bool needClosePanel;

    protected override void Awake()
    {
      base.Awake();
      this.CachedTransform.SetParent(Hierarchy.Get("SRDebugger"));
      this.shortcuts = new List<Settings.KeyboardShortcut>((IEnumerable<Settings.KeyboardShortcut>) Settings.Instance.KeyboardShortcuts);
    }

    private void ToggleTab(DefaultTabs t)
    {
      DefaultTabs? activeTab = SRDebugger.Internal.Service.Panel.ActiveTab;
      if (SRDebugger.Internal.Service.Panel.IsVisible && activeTab.HasValue && activeTab.Value == t)
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
          this.ToggleTab(DefaultTabs.SystemInformation);
          break;
        case Settings.ShortcutActions.OpenLoggerTab:
          this.ToggleTab(DefaultTabs.Logger);
          break;
        case Settings.ShortcutActions.OpenProfilerTab:
          this.ToggleTab(DefaultTabs.Profiler);
          break;
        case Settings.ShortcutActions.OpenConsoleTab:
          this.ToggleTab(DefaultTabs.Console);
          break;
        case Settings.ShortcutActions.OpenInspectorTab:
          this.ToggleTab(DefaultTabs.Inspector);
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
          Debug.LogWarning((object) ("[SRDebugger] Unhandled keyboard shortcut: " + (object) shortcut.Action));
          break;
      }
    }

    private void Update()
    {
      if (Settings.Instance.KeyboardEscapeClose && Input.GetKeyUp(KeyCode.Escape) && SRDebugger.Internal.Service.Panel.IsVisible)
        SRDebug.Instance.HideDebugPanel();
      bool flag1 = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
      bool flag2 = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
      bool flag3 = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
      for (int index = 0; index < this.shortcuts.Count; ++index)
      {
        Settings.KeyboardShortcut shortcut = this.shortcuts[index];
        if ((!shortcut.Control || flag1) && (!shortcut.Shift || flag3) && (!shortcut.Alt || flag2) && Input.GetKeyDown(shortcut.Key))
        {
          this.ExecuteShortcut(shortcut);
          break;
        }
      }
    }
  }
}
