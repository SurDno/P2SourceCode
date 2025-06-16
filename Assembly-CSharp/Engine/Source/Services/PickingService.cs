using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Inspectors;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Source.Services
{
  [GameService(new System.Type[] {typeof (PickingService)})]
  public class PickingService : IUpdatable, IInitialisable
  {
    private const float maxDistance = 10f;
    private const float sphereCastRadius = 0.1f;
    private Ray ray;
    private IEntity targetEntity;
    private float targetEntityDistance;
    private GameObject targetGameObject;
    private float targetGameObjectDistance;
    private static List<RaycastHit> hits = new List<RaycastHit>();

    [Inspected]
    public Ray Ray => this.ray;

    [Inspected]
    public IEntity TargetEntity => this.targetEntity;

    [Inspected]
    public float TargetEntityDistance => this.targetEntityDistance;

    [Inspected]
    public GameObject TargetGameObject => this.targetGameObject;

    [Inspected]
    public float TargetGameObjectDistance => this.targetGameObjectDistance;

    public void Initialise()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
    }

    public void Terminate()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable((IUpdatable) this);
    }

    public void ComputeUpdate()
    {
      this.Clear();
      if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
        return;
      IEntity player = ServiceLocator.GetService<ISimulation>().Player;
      if (player == null)
        return;
      IEntityView entityView = (IEntityView) player;
      if ((UnityEngine.Object) entityView.GameObject == (UnityEngine.Object) null)
        return;
      Transform cameraTransform = GameCamera.Instance.CameraTransform;
      if ((UnityEngine.Object) cameraTransform == (UnityEngine.Object) null)
        return;
      this.ray = new Ray(cameraTransform.position, cameraTransform.forward);
      this.targetEntityDistance = float.PositiveInfinity;
      PhysicsUtility.Raycast(PickingService.hits, this.Ray, 10f, -1, QueryTriggerInteraction.Ignore);
      for (int index = 0; index < PickingService.hits.Count; ++index)
      {
        RaycastHit hit = PickingService.hits[index];
        if (!((UnityEngine.Object) hit.transform == (UnityEngine.Object) null))
        {
          GameObject gameObject = hit.transform.gameObject;
          if (!((UnityEngine.Object) gameObject == (UnityEngine.Object) entityView.GameObject))
          {
            this.targetGameObject = gameObject;
            this.targetGameObjectDistance = hit.distance;
            IEntity entity = EntityUtility.GetEntity(gameObject);
            if (entity == null)
              break;
            this.targetEntity = entity;
            this.targetEntityDistance = hit.distance;
            break;
          }
        }
      }
    }

    private void Clear()
    {
      this.ray = new Ray();
      this.targetEntity = (IEntity) null;
      this.targetEntityDistance = float.PositiveInfinity;
      this.targetGameObject = (GameObject) null;
      this.targetGameObjectDistance = float.PositiveInfinity;
    }
  }
}
