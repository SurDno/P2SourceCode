// Decompiled with JetBrains decompiler
// Type: Engine.Services.Engine.EnvironmentService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Engine.Source.Services;
using Rain;
using System;
using UnityEngine;

#nullable disable
namespace Engine.Services.Engine
{
  [Depend(typeof (TimeService))]
  [RuntimeService(new System.Type[] {typeof (EnvironmentService)})]
  public class EnvironmentService : IUpdatable, IInitialisable
  {
    private LeafManager fallingLeaves;
    private TOD_Sky tod;
    private Transform cameraTransform;
    private RainManager rain;
    [FromLocator]
    private ITimeService timeService;

    public event Action OnInvalidate;

    public TOD_Sky Tod => this.tod;

    public LeafManager FallingLeaves => this.fallingLeaves;

    public void Initialise()
    {
      InstanceByRequest<UpdateService>.Instance.EnvironmentUpdater.AddUpdatable((IUpdatable) this);
    }

    public void Terminate()
    {
      this.fallingLeaves = (LeafManager) null;
      this.tod = (TOD_Sky) null;
      this.cameraTransform = (Transform) null;
      this.rain = (RainManager) null;
      InstanceByRequest<UpdateService>.Instance.EnvironmentUpdater.RemoveUpdatable((IUpdatable) this);
    }

    public void ComputeUpdate()
    {
      bool flag = false;
      if ((UnityEngine.Object) this.cameraTransform == (UnityEngine.Object) null)
      {
        this.cameraTransform = GameCamera.Instance.CameraTransform;
        if ((UnityEngine.Object) this.cameraTransform != (UnityEngine.Object) null)
          flag = true;
      }
      if ((UnityEngine.Object) this.rain == (UnityEngine.Object) null)
      {
        this.rain = RainManager.Instance;
        if ((UnityEngine.Object) this.rain != (UnityEngine.Object) null)
          flag = true;
      }
      if ((UnityEngine.Object) this.fallingLeaves == (UnityEngine.Object) null)
      {
        this.fallingLeaves = MonoBehaviourInstance<LeafManager>.Instance;
        if ((UnityEngine.Object) this.fallingLeaves != (UnityEngine.Object) null)
          flag = true;
      }
      if ((UnityEngine.Object) this.tod == (UnityEngine.Object) null)
      {
        this.tod = TOD_Sky.Instance;
        if ((UnityEngine.Object) this.tod != (UnityEngine.Object) null)
          flag = true;
      }
      if ((UnityEngine.Object) this.rain != (UnityEngine.Object) null && (UnityEngine.Object) this.cameraTransform != (UnityEngine.Object) null)
        this.rain.playerPosition = this.cameraTransform.position;
      if ((UnityEngine.Object) this.fallingLeaves != (UnityEngine.Object) null && (UnityEngine.Object) this.cameraTransform != (UnityEngine.Object) null)
        this.fallingLeaves.playerPosition = this.cameraTransform.position;
      if ((UnityEngine.Object) this.tod != (UnityEngine.Object) null)
        this.tod.Cycle.DateTime = ScriptableObjectInstance<GameSettingsData>.Instance.OffsetTime.Value + this.timeService.SolarTime;
      if (!flag)
        return;
      Action onInvalidate = this.OnInvalidate;
      if (onInvalidate == null)
        return;
      onInvalidate();
    }
  }
}
