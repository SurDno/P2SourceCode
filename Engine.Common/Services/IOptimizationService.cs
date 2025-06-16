namespace Engine.Common.Services
{
  public interface IOptimizationService
  {
    bool FrameHasSpike { get; set; }

    bool LazyFsm { get; }

    bool IsUnity { get; }
  }
}
