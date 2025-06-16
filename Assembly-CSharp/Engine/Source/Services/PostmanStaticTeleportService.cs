using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.MessangerStationary;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Settings;
using Inspectors;

namespace Engine.Source.Services
{
  [GameService(typeof (PostmanStaticTeleportService))]
  public class PostmanStaticTeleportService : IInitialisable, IUpdatable
  {
    private const float hibernateTrialTime = 0.5f;
    private const float playerSeesMeUpdatedTrialTime = 10f;
    private const float trialRepeatTime = 30f;
    [Inspected]
    private HashSet<Spawnpoint> spawnpoints = new HashSet<Spawnpoint>();
    [Inspected]
    private List<Slot> postmans = new List<Slot>();
    [Inspected]
    private List<Slot> teleports = new List<Slot>();

    public IEnumerable<Spawnpoint> Spawnpoints => spawnpoints;

    public void AddSpawnpoints(Spawnpoint point) => spawnpoints.Add(point);

    public void RemoveSpawnpoints(Spawnpoint point) => spawnpoints.Remove(point);

    public void Initialise()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
    }

    public void Terminate()
    {
      spawnpoints.Clear();
      postmans.Clear();
      teleports.Clear();
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
    }

    private bool IsRegistered(IEntity owner)
    {
      return postmans.FindIndex(s => s.Owner == owner) != -1;
    }

    public void RegisterPostman(IEntity owner, SpawnpointKindEnum spawnpointKind)
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append("Register static postman, kind : ").Append(spawnpointKind).Append(" , owner : ").GetInfo(owner));
      if (spawnpointKind == SpawnpointKindEnum.None)
        Debug.LogError((object) string.Format("Adding static postman with {0} {1}", SpawnpointKindEnum.None, owner.GetInfo()));
      if (IsRegistered(owner))
        Debug.LogError((object) ("Postman already register, owner : " + owner.GetInfo()));
      else
        postmans.Add(new Slot {
          Owner = owner,
          TimeLeft = 0.0f,
          SpawnpointKind = spawnpointKind
        });
    }

    public void UnregisterPostman(IEntity owner)
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append("Unregister static postman, owner : ").GetInfo(owner));
      if (!IsRegistered(owner))
      {
        Debug.LogError((object) ("Postman already unregister, owner : " + owner.GetInfo()));
      }
      else
      {
        int index = postmans.FindIndex(s => s.Owner == owner);
        if ((UnityEngine.Object) postmans[index].Spawnpoint != (UnityEngine.Object) null)
          postmans[index].Spawnpoint.Locked = false;
        postmans.RemoveAt(index);
      }
    }

    public void ComputeUpdate()
    {
      if (InstanceByRequest<EngineApplication>.Instance.IsPaused || postmans.Count == 0)
        return;
      IEntity player = ServiceLocator.GetService<ISimulation>().Player;
      if (player == null)
        return;
      GameObject gameObject = ((IEntityView) player).GameObject;
      if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
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
            if ((UnityEngine.Object) owner.GameObject != (UnityEngine.Object) null && PostmanTeleportUtility.IsPointVisibleByPlayer(playerGameObject, owner.GameObject.transform.position))
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
        Spawnpoint spawnpoint;
        if (GetRandomPoint(false, playerGameObject, out spawnpoint, teleport.SpawnpointKind))
        {
          teleport.Owner.GetComponent<NavigationComponent>().TeleportTo(playerLocation.Location, spawnpoint.transform.position, spawnpoint.transform.rotation);
          teleport.TimeLeft = 30f;
          if ((UnityEngine.Object) teleport.Spawnpoint != (UnityEngine.Object) null)
            teleport.Spawnpoint.Locked = false;
          teleport.Spawnpoint = spawnpoint;
          teleport.Spawnpoint.Locked = true;
        }
      }
    }

    private bool GetRandomPoint(
      bool playerIndoor,
      GameObject playerGameObject,
      out Spawnpoint spawnpoint,
      SpawnpointKindEnum kind)
    {
      if (spawnpoints.Count == 0)
      {
        spawnpoint = null;
        return false;
      }
      float num = InstanceByRequest<GraphicsGameSettings>.Instance.FieldOfView.Value * GameCamera.Instance.Camera.aspect;
      foreach (Spawnpoint spawnpoint1 in spawnpoints.OrderBy((Func<Spawnpoint, float>) (p => Vector3.Distance(playerGameObject.transform.position, p.transform.position))))
      {
        if (spawnpoint1.SpawnpointKind == kind && !spawnpoint1.Locked)
        {
          if (!playerIndoor)
          {
            Vector3 to = (spawnpoint1.transform.position - playerGameObject.transform.position) with
            {
              y = 0.0f
            };
            if ((double) to.magnitude < 30.0 && (double) Vector3.Angle(playerGameObject.transform.forward, to) < num / 2.0)
              continue;
          }
          spawnpoint = spawnpoint1;
          return true;
        }
      }
      spawnpoint = null;
      return false;
    }

    public class Slot
    {
      [Inspected]
      public IEntity Owner;
      [Inspected]
      public float TimeLeft;
      [Inspected]
      public SpawnpointKindEnum SpawnpointKind;
      [Inspected]
      public Spawnpoint Spawnpoint;

      [Inspected(Header = true)]
      private string Header => Owner != null ? Owner.Name : null;
    }
  }
}
