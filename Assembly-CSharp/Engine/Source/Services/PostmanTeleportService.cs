// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.PostmanTeleportService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Settings;
using Inspectors;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#nullable disable
namespace Engine.Source.Services
{
  [GameService(new System.Type[] {typeof (PostmanTeleportService)})]
  public class PostmanTeleportService : IInitialisable, IUpdatable
  {
    private const float hibernateTrialTime = 0.5f;
    private const float playerSeesMeUpdatedTrialTime = 10f;
    private const float minSearchDistance = 10f;
    private const float maxSearchDistance = 20f;
    private const float trialRepeatTime = 30f;
    [Inspected]
    private List<PostmanTeleportService.Slot> postmans = new List<PostmanTeleportService.Slot>();
    [Inspected]
    private List<PostmanTeleportService.Slot> teleports = new List<PostmanTeleportService.Slot>();

    public void Initialise()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
    }

    public void Terminate()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable((IUpdatable) this);
    }

    private bool IsRegistered(IEntity owner)
    {
      return this.postmans.FindIndex((Predicate<PostmanTeleportService.Slot>) (s => s.Owner == owner)) != -1;
    }

    public void RegisterPostman(IEntity owner, int areaMask)
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append("Register postman, mask : ").Append(areaMask).Append(" , owner : ").GetInfo((object) owner));
      if (this.IsRegistered(owner))
        Debug.LogError((object) ("Postman already register, owner : " + owner.GetInfo()));
      else
        this.postmans.Add(new PostmanTeleportService.Slot()
        {
          Owner = owner,
          TimeLeft = 0.0f,
          AreaMask = areaMask
        });
    }

    public void UnregisterPostman(IEntity owner)
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append("Unregister postman, owner : ").GetInfo((object) owner));
      if (!this.IsRegistered(owner))
        Debug.LogError((object) ("Postman already unregister, owner : " + owner.GetInfo()));
      else
        this.postmans.RemoveAt(this.postmans.FindIndex((Predicate<PostmanTeleportService.Slot>) (s => s.Owner == owner)));
    }

    public void ComputeUpdate()
    {
      if (InstanceByRequest<EngineApplication>.Instance.IsPaused || this.postmans.Count == 0)
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
      this.UpdatePostmans(Time.deltaTime, gameObject, component1);
    }

    private void UpdatePostmans(
      float deltaTime,
      GameObject playerGameObject,
      LocationItemComponent playerLocation)
    {
      this.teleports.Clear();
      foreach (PostmanTeleportService.Slot postman in this.postmans)
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
          if ((double) postman.TimeLeft <= 0.0)
            this.teleports.Add(postman);
        }
      }
      foreach (PostmanTeleportService.Slot teleport in this.teleports)
      {
        Vector3 realPoint;
        if (this.TryPoint(this.RandomPoint(playerGameObject), playerGameObject.transform.position, teleport.AreaMask, out realPoint))
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
      int index = this.postmans.FindIndex((Predicate<PostmanTeleportService.Slot>) (s => s.Owner == owner));
      if (index == -1)
        return;
      this.postmans[index].TimeLeft = ok ? 30f : 0.5f;
    }

    private Vector3 RandomPoint(GameObject player)
    {
      float num1 = InstanceByRequest<GraphicsGameSettings>.Instance.FieldOfView.Value * GameCamera.Instance.Camera.aspect;
      float num2 = UnityEngine.Random.Range((float) ((double) num1 / 2.0 - 180.0), (float) (180.0 - (double) num1 / 2.0));
      float y = player.transform.rotation.eulerAngles.y + num2;
      float num3 = UnityEngine.Random.Range(10f, 20f);
      return player.transform.position + Quaternion.Euler(0.0f, y, 0.0f) * Vector3.back * num3;
    }

    private bool TryPoint(
      Vector3 point,
      Vector3 playerPosition,
      int areaMask,
      out Vector3 realPoint)
    {
      NavMeshHit hit;
      if (!NavMesh.SamplePosition(point, out hit, 5f, -1))
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
      private string Header => this.Owner != null ? this.Owner.Name : (string) null;
    }
  }
}
