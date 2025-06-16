using CircularBuffer;

namespace SRDebugger.Services
{
  public interface IProfilerService
  {
    float AverageFrameTime { get; }

    float LastFrameTime { get; }

    void SetCustom(double value);

    CircularBuffer<ProfilerFrame> FrameBuffer { get; }
  }
}
