// Decompiled with JetBrains decompiler
// Type: SRDebugger.Services.IDebugService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
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
