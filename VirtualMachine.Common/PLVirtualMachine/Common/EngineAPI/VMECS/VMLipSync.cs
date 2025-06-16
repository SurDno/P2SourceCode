using Engine.Common.Commons;
using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using System;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("LipSync", typeof (ILipSyncComponent))]
  public class VMLipSync : VMEngineComponent<ILipSyncComponent>
  {
    public const string ComponentName = "LipSync";

    protected override void Init()
    {
      if (!this.InstanceValid)
        return;
      this.Component.PlayCompleteEvent += new Action(this.OnPlayCompleteEvent);
    }

    public override void Clear()
    {
      if (!this.InstanceValid)
        return;
      this.Component.PlayCompleteEvent -= new Action(this.OnPlayCompleteEvent);
    }

    private void OnPlayCompleteEvent()
    {
      Action playCompleteEvent = this.PlayCompleteEvent;
      if (playCompleteEvent == null)
        return;
      playCompleteEvent();
    }

    [Event("PlayComplete", "")]
    public event Action PlayCompleteEvent;

    [Method("Play", "lipsync object", "")]
    public void Play(ILipSyncObject lipSync) => this.Component.Play(lipSync, true);

    [Method("Play3D", "lipsync object", "")]
    public void Play3D(ILipSyncObject lipSync) => this.Component.Play3D(lipSync, true);

    [Method("Play3DWithParams", "lipsync object, minDistance, maxDistance", "")]
    public void Play3DWithParams(ILipSyncObject lipSync, float minDistance, float maxDistance)
    {
      this.Component.Play3D(lipSync, minDistance, maxDistance, true);
    }
  }
}
