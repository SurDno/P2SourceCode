using SRDebugger.Scripts;
using SRDebugger.Services;
using SRF;
using SRF.Service;
using UnityEngine;

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
