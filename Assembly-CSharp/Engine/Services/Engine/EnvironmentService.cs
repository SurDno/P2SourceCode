using System;
using Engine.Common;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Engine.Source.Services;
using Rain;

namespace Engine.Services.Engine
{
  [Depend(typeof (TimeService))]
  [RuntimeService(typeof (EnvironmentService))]
  public class EnvironmentService : IUpdatable, IInitialisable
  {
    private LeafManager fallingLeaves;
    private TOD_Sky tod;
    private Transform cameraTransform;
    private RainManager rain;
    [FromLocator]
    private ITimeService timeService;

    public event Action OnInvalidate;

    public TOD_Sky Tod => tod;

    public LeafManager FallingLeaves => fallingLeaves;

    public void Initialise()
    {
      InstanceByRequest<UpdateService>.Instance.EnvironmentUpdater.AddUpdatable(this);
    }

    public void Terminate()
    {
      fallingLeaves = null;
      tod = null;
      cameraTransform = (Transform) null;
      rain = null;
      InstanceByRequest<UpdateService>.Instance.EnvironmentUpdater.RemoveUpdatable(this);
    }

    public void ComputeUpdate()
    {
      bool flag = false;
      if ((UnityEngine.Object) cameraTransform == (UnityEngine.Object) null)
      {
        cameraTransform = GameCamera.Instance.CameraTransform;
        if ((UnityEngine.Object) cameraTransform != (UnityEngine.Object) null)
          flag = true;
      }
      if ((UnityEngine.Object) rain == (UnityEngine.Object) null)
      {
        rain = RainManager.Instance;
        if ((UnityEngine.Object) rain != (UnityEngine.Object) null)
          flag = true;
      }
      if ((UnityEngine.Object) fallingLeaves == (UnityEngine.Object) null)
      {
        fallingLeaves = MonoBehaviourInstance<LeafManager>.Instance;
        if ((UnityEngine.Object) fallingLeaves != (UnityEngine.Object) null)
          flag = true;
      }
      if ((UnityEngine.Object) tod == (UnityEngine.Object) null)
      {
        tod = TOD_Sky.Instance;
        if ((UnityEngine.Object) tod != (UnityEngine.Object) null)
          flag = true;
      }
      if ((UnityEngine.Object) rain != (UnityEngine.Object) null && (UnityEngine.Object) cameraTransform != (UnityEngine.Object) null)
        rain.playerPosition = cameraTransform.position;
      if ((UnityEngine.Object) fallingLeaves != (UnityEngine.Object) null && (UnityEngine.Object) cameraTransform != (UnityEngine.Object) null)
        fallingLeaves.playerPosition = cameraTransform.position;
      if ((UnityEngine.Object) tod != (UnityEngine.Object) null)
        tod.Cycle.DateTime = ScriptableObjectInstance<GameSettingsData>.Instance.OffsetTime.Value + timeService.SolarTime;
      if (!flag)
        return;
      Action onInvalidate = OnInvalidate;
      if (onInvalidate == null)
        return;
      onInvalidate();
    }
  }
}
