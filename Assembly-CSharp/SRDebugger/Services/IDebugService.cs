using System;

namespace SRDebugger.Services
{
  public interface IDebugService
  {
    Settings Settings { get; }

    bool IsDebugging { get; set; }

    event Action OnDebuggingChanged;

    bool IsDebugPanelVisible { get; }

    bool IsProfilerDocked { get; set; }

    void ShowDebugPanel();

    void ShowDebugPanel(DefaultTabs tab);

    void HideDebugPanel();

    void DestroyDebugPanel();

    event VisibilityChangedDelegate PanelVisibilityChanged;
  }
}
