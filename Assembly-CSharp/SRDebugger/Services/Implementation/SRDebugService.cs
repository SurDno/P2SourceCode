// Decompiled with JetBrains decompiler
// Type: SRDebugger.Services.Implementation.SRDebugService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using SRF;
using SRF.Service;
using System;
using UnityEngine;

#nullable disable
namespace SRDebugger.Services.Implementation
{
  [SRF.Service.Service(typeof (IDebugService))]
  public class SRDebugService : IDebugService
  {
    private readonly IDebugPanelService _debugPanelService;
    private bool _IsDebugging;

    public SRDebugService()
    {
      SRServiceManager.RegisterService<IDebugService>((object) this);
      SRServiceManager.GetService<IProfilerService>();
      this._debugPanelService = SRServiceManager.GetService<IDebugPanelService>();
      this._debugPanelService.VisibilityChanged += new Action<IDebugPanelService, bool>(this.DebugPanelServiceOnVisibilityChanged);
      if (this.Settings.EnableKeyboardShortcuts)
        SRServiceManager.GetService<KeyboardShortcutListenerService>();
      UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object) Hierarchy.Get("SRDebugger").gameObject);
    }

    public Settings Settings => Settings.Instance;

    public bool IsDebugging
    {
      get => this._IsDebugging;
      set
      {
        this._IsDebugging = value;
        Action debuggingChanged = this.OnDebuggingChanged;
        if (debuggingChanged == null)
          return;
        debuggingChanged();
      }
    }

    public event Action OnDebuggingChanged;

    public bool IsDebugPanelVisible => this._debugPanelService.IsVisible;

    public bool IsProfilerDocked
    {
      get => SRDebugger.Internal.Service.PinnedUI.IsProfilerPinned;
      set => SRDebugger.Internal.Service.PinnedUI.IsProfilerPinned = value;
    }

    public void ShowDebugPanel() => this._debugPanelService.IsVisible = true;

    public void ShowDebugPanel(DefaultTabs tab)
    {
      this._debugPanelService.IsVisible = true;
      this._debugPanelService.OpenTab(tab);
    }

    public void HideDebugPanel() => this._debugPanelService.IsVisible = false;

    public void DestroyDebugPanel()
    {
      this._debugPanelService.IsVisible = false;
      this._debugPanelService.Unload();
    }

    public event VisibilityChangedDelegate PanelVisibilityChanged;

    private void DebugPanelServiceOnVisibilityChanged(IDebugPanelService debugPanelService, bool b)
    {
      if (this.PanelVisibilityChanged == null)
        return;
      try
      {
        this.PanelVisibilityChanged(b);
      }
      catch (Exception ex)
      {
        Debug.LogError((object) "[SRDebugger] Event target threw exception (IDebugService.PanelVisiblityChanged)");
        Debug.LogException(ex);
      }
    }
  }
}
