// Decompiled with JetBrains decompiler
// Type: SRDebugger.Internal.Service
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using SRDebugger.Services;
using SRF.Service;

#nullable disable
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
