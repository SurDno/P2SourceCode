// Decompiled with JetBrains decompiler
// Type: Engine.Behaviours.Engines.Controllers.BirdFlockMovementController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Components;
using System;
using UnityEngine;
using UnityEngine.AI;

#nullable disable
namespace Engine.Behaviours.Engines.Controllers
{
  public class BirdFlockMovementController : IMovementController
  {
    private NavMeshAgent agent;
    private GameObject gameObject;
    private const float flockSpeed = 5f;

    public bool IsPaused { get; set; }

    public bool GeometryVisible
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }

    public void Initialize(GameObject gameObject)
    {
      this.gameObject = gameObject;
      this.agent = gameObject.GetComponent<NavMeshAgent>();
      if (!(bool) (UnityEngine.Object) this.agent)
        return;
      this.agent.enabled = false;
    }

    public void StartMovement(Vector3 direction, EngineBehavior.GaitType gait)
    {
    }

    public bool Move(Vector3 direction, float remainingDistance, EngineBehavior.GaitType gait)
    {
      float num = gait == EngineBehavior.GaitType.Walk ? 5f : 10f;
      this.gameObject.transform.position += direction.normalized * num * Time.deltaTime;
      return (double) remainingDistance < 1.0;
    }

    public bool Rotate(Vector3 direction) => true;

    public void OnAnimatorMove()
    {
    }

    public void Update()
    {
    }

    public void FixedUpdate()
    {
    }
  }
}
