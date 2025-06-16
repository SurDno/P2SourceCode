// Decompiled with JetBrains decompiler
// Type: SRDebugger.Internal.SRDebugStrings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace SRDebugger.Internal
{
  public class SRDebugStrings
  {
    public static readonly SRDebugStrings Current = new SRDebugStrings();
    public readonly string Console_MessageTruncated = "-- Message Truncated --";
    public readonly string Console_NoStackTrace = "-- No Stack Trace Available --";
    public readonly string Profiler_DisableProfilerInfo = "Unity profiler is currently <b>enabled</b>. Disable to improve performance.";
    public readonly string Profiler_EnableProfilerInfo = "Unity profiler is currently <b>disabled</b>. Enable to show more information.";
    public readonly string Profiler_NoProInfo = "Unity profiler is currently <b>disabled</b>. Unity Pro is required to enable it.";
    public readonly string Profiler_NotSupported = "Unity profiler is <b>not supported</b> in this build.";
    public readonly string ProfilerCameraListenerHelp = "This behaviour is attached by the SRDebugger profiler to calculate render times.";
  }
}
