using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Services
{
  [GameService(typeof (PickingService))]
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
    public Ray Ray => ray;

    [Inspected]
    public IEntity TargetEntity => targetEntity;

    [Inspected]
    public float TargetEntityDistance => targetEntityDistance;

    [Inspected]
    public GameObject TargetGameObject => targetGameObject;

    [Inspected]
    public float TargetGameObjectDistance => targetGameObjectDistance;

    public void Initialise()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
    }

    public void Terminate()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
    }

    public void ComputeUpdate()
    {
      Clear();
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
      ray = new Ray(cameraTransform.position, cameraTransform.forward);
      targetEntityDistance = float.PositiveInfinity;
      PhysicsUtility.Raycast(hits, Ray, 10f, -1, QueryTriggerInteraction.Ignore);
      for (int index = 0; index < hits.Count; ++index)
      {
        RaycastHit hit = hits[index];
        if (!((UnityEngine.Object) hit.transform == (UnityEngine.Object) null))
        {
          GameObject gameObject = hit.transform.gameObject;
          if (!((UnityEngine.Object) gameObject == (UnityEngine.Object) entityView.GameObject))
          {
            targetGameObject = gameObject;
            targetGameObjectDistance = hit.distance;
            IEntity entity = EntityUtility.GetEntity(gameObject);
            if (entity == null)
              break;
            targetEntity = entity;
            targetEntityDistance = hit.distance;
            break;
          }
        }
      }
    }

    private void Clear()
    {
      ray = new Ray();
      targetEntity = null;
      targetEntityDistance = float.PositiveInfinity;
      targetGameObject = (GameObject) null;
      targetGameObjectDistance = float.PositiveInfinity;
    }
  }
}
