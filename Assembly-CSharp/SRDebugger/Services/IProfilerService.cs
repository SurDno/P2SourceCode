// Decompiled with JetBrains decompiler
// Type: SRDebugger.Services.IProfilerService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace SRDebugger.Services
{
  public interface IProfilerService
  {
    float AverageFrameTime { get; }

    float LastFrameTime { get; }

    void SetCustom(double value);

    CircularBuffer.CircularBuffer<ProfilerFrame> FrameBuffer { get; }
  }
}
