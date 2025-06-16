using SRDebugger.Services;
using SRF.Service;

namespace SRDebugger.Internal
{
  public static class Service
  {
    private static IDebugPanelService _debugPanelService;
    private static IPinnedUIService _pinnedUiService;

    public static IDebugPanelService Panel
    {
      get
      {
        if (SRDebugger.Internal.Service._debugPanelService == null)
          SRDebugger.Internal.Service._debugPanelService = SRServiceManager.GetService<IDebugPanelService>();
        return SRDebugger.Internal.Service._debugPanelService;
      }
    }

    public static IPinnedUIService PinnedUI
    {
      get
      {
        if (SRDebugger.Internal.Service._pinnedUiService == null)
          SRDebugger.Internal.Service._pinnedUiService = SRServiceManager.GetService<IPinnedUIService>();
        return SRDebugger.Internal.Service._pinnedUiService;
      }
    }
  }
}
