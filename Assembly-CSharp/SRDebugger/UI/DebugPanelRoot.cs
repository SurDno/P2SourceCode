// Decompiled with JetBrains decompiler
// Type: SRDebugger.UI.DebugPanelRoot
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using SRDebugger.Scripts;
using SRDebugger.Services;
using SRF;
using SRF.Service;
using UnityEngine;

#nullable disable
namespace SRDebugger.UI
{
  public class DebugPanelRoot : SRMonoBehaviour
  {
    public CanvasGroup CanvasGroup;
    public DebuggerTabController TabController;

    public void Close() => SRServiceManager.GetService<IDebugService>().HideDebugPanel();

    public void CloseAndDestroy()
    {
      SRServiceManager.GetService<IDebugService>().DestroyDebugPanel();
    }
  }
}
