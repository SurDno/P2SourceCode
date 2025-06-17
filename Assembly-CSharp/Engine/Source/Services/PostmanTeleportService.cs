using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Settings;
using Inspectors;
using UnityEngine;
using UnityEngine.AI;

namespace Engine.Source.Services
{
  [GameService(typeof (PostmanTeleportService))]
  public class PostmanTeleportService : IInitialisable, IUpdatable
  {
    private const float hibernateTrialTime = 0.5f;
    private const float playerSeesMeUpdatedTrialTime = 10f;
    private const float minSearchDistance = 10f;
    private const float maxSearchDistance = 20f;
    private const float trialRepeatTime = 30f;
    [Inspected]
    private List<Slot> postmans = [];
    [Inspected]
    private List<Slot> teleports = [];

    public void Initialise()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
    }

    public void Terminate()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
    }

    private bool IsRegistered(IEntity owner)
    {
      return postmans.FindIndex(s => s.Owner == owner) != -1;
    }

    public void RegisterPostman(IEntity owner, int areaMask)
    {
      Debug.Log(ObjectInfoUtility.GetStream().Append("Register postman, mask : ").Append(areaMask).Append(" , owner : ").GetInfo(owner));
      if (IsRegistered(owner))
        Debug.LogError("Postman already register, owner : " + owner.GetInfo());
      else
        postmans.Add(new Slot {
          Owner = owner,
          TimeLeft = 0.0f,
          AreaMask = areaMask
        });
    }

    public void UnregisterPostman(IEntity owner)
    {
      Debug.Log(ObjectInfoUtility.GetStream().Append("Unregister postman, owner : ").GetInfo(owner));
      if (!IsRegistered(owner))
        Debug.LogError("Postman already unregister, owner : " + owner.GetInfo());
      else
        postmans.RemoveAt(postmans.FindIndex(s => s.Owner == owner));
    }

    public void ComputeUpdate()
    {
      if (InstanceByRequest<EngineApplication>.Instance.IsPaused || postmans.Count == 0)
        return;
      IEntity player = ServiceLocator.GetService<ISimulation>().Player;
      if (player == null)
        return;
      GameObject gameObject = ((IEntityView) player).GameObject;
      if (gameObject == null)
        return;
      LocationItemComponent component1 = player.GetComponent<LocationItemComponent>();
      if (component1 == null || component1.IsIndoor)
        return;
      PlayerControllerComponent component2 = player.GetComponent<PlayerControllerComponent>();
      if (component2 != null && !component2.CanReceiveMail.Value)
        return;
      UpdatePostmans(Time.deltaTime, gameObject, component1);
    }

    private void UpdatePostmans(
      float deltaTime,
      GameObject playerGameObject,
      LocationItemComponent playerLocation)
    {
      teleports.Clear();
      foreach (Slot postman in postmans)
      {
        ILocationComponent location = postman.Owner.GetComponent<LocationItemComponent>()?.Location;
        if (location != null)
        {
          postman.TimeLeft -= Time.deltaTime;
          if (location.IsHibernation)
          {
            postman.TimeLeft = Mathf.Min(postman.TimeLeft, 0.5f);
          }
          else
          {
            IEntityView owner = (IEntityView) postman.Owner;
            if (owner.GameObject != null && PostmanTeleportUtility.IsPointVisibleByPlayer(playerGameObject, owner.GameObject.transform.position))
            {
              postman.TimeLeft = Mathf.Max(postman.TimeLeft, 10f);
              continue;
            }
          }
          if (postman.TimeLeft <= 0.0)
            teleports.Add(postman);
        }
      }
      foreach (Slot teleport in teleports)
      {
        if (TryPoint(RandomPoint(playerGameObject), playerGameObject.transform.position, teleport.AreaMask, out Vector3 realPoint))
        {
          teleport.Owner.GetComponent<NavigationComponent>().TeleportTo(playerLocation.Location, realPoint, Quaternion.LookRotation(playerGameObject.transform.position - realPoint));
          teleport.TimeLeft = 30f;
        }
        else
          teleport.TimeLeft = 0.5f;
      }
    }

    public void ReportPostmanIsOK(IEntity owner, bool ok)
    {
      int index = postmans.FindIndex(s => s.Owner == owner);
      if (index == -1)
        return;
      postmans[index].TimeLeft = ok ? 30f : 0.5f;
    }

    private Vector3 RandomPoint(GameObject player)
    {
      float num1 = InstanceByRequest<GraphicsGameSettings>.Instance.FieldOfView.Value * GameCamera.Instance.Camera.aspect;
      float num2 = Random.Range((float) (num1 / 2.0 - 180.0), (float) (180.0 - num1 / 2.0));
      float y = player.transform.rotation.eulerAngles.y + num2;
      float num3 = Random.Range(10f, 20f);
      return player.transform.position + Quaternion.Euler(0.0f, y, 0.0f) * Vector3.back * num3;
    }

    private bool TryPoint(
      Vector3 point,
      Vector3 playerPosition,
      int areaMask,
      out Vector3 realPoint)
    {
      if (!NavMesh.SamplePosition(point, out NavMeshHit hit, 5f, -1))
      {
        realPoint = point;
        return false;
      }
      realPoint = hit.position;
      return true;
    }

    public class Slot
    {
      [Inspected]
      public IEntity Owner;
      [Inspected]
      public float TimeLeft;
      [Inspected]
      public int AreaMask;

      [Inspected(Header = true)]
      private string Header => Owner != null ? Owner.Name : null;
    }
  }
}
