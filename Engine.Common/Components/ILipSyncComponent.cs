using Engine.Common.Commons;
using System;

namespace Engine.Common.Components
{
  public interface ILipSyncComponent : IComponent
  {
    event Action PlayCompleteEvent;

    void Play(ILipSyncObject lipSync, bool usePause);

    void Play3D(ILipSyncObject lipSync, bool usePause);

    void Play3D(ILipSyncObject lipSync, float minDistance, float maxDistance, bool usePause);
  }
}
