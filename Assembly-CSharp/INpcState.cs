using UnityEngine;

public interface INpcState
{
  GameObject GameObject { get; }

  NpcStateStatusEnum Status { get; }

  void Shutdown();

  void OnAnimatorMove();

  void OnAnimatorEventEvent(string obj);

  void Update();

  void OnLodStateChanged(bool enabled);
}
