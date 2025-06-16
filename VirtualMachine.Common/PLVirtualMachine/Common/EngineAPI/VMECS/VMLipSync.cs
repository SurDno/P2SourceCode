using System;
using Engine.Common.Commons;
using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("LipSync", typeof (ILipSyncComponent))]
  public class VMLipSync : VMEngineComponent<ILipSyncComponent>
  {
    public const string ComponentName = "LipSync";

    protected override void Init()
    {
      if (!InstanceValid)
        return;
      Component.PlayCompleteEvent += OnPlayCompleteEvent;
    }

    public override void Clear()
    {
      if (!InstanceValid)
        return;
      Component.PlayCompleteEvent -= OnPlayCompleteEvent;
    }

    private void OnPlayCompleteEvent()
    {
      Action playCompleteEvent = PlayCompleteEvent;
      if (playCompleteEvent == null)
        return;
      playCompleteEvent();
    }

    [Event("PlayComplete", "")]
    public event Action PlayCompleteEvent;

    [Method("Play", "lipsync object", "")]
    public void Play(ILipSyncObject lipSync) => Component.Play(lipSync, true);

    [Method("Play3D", "lipsync object", "")]
    public void Play3D(ILipSyncObject lipSync) => Component.Play3D(lipSync, true);

    [Method("Play3DWithParams", "lipsync object, minDistance, maxDistance", "")]
    public void Play3DWithParams(ILipSyncObject lipSync, float minDistance, float maxDistance)
    {
      Component.Play3D(lipSync, minDistance, maxDistance, true);
    }
  }
}
