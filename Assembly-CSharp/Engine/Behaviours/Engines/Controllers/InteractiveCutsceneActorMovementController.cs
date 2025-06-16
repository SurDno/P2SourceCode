// Decompiled with JetBrains decompiler
// Type: Engine.Behaviours.Engines.Controllers.InteractiveCutsceneActorMovementController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Components;
using System;
using UnityEngine;

#nullable disable
namespace Engine.Behaviours.Engines.Controllers
{
  public class InteractiveCutsceneActorMovementController : IMovementController
  {
    private Animator animator;
    private Rigidbody rigidbody;
    private CapsuleCollider collider;
    private Pivot pivot;
    private GameObject gameObject;

    public bool IsPaused { get; set; }

    public bool GeometryVisible
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }

    public void Initialize(GameObject gameObject)
    {
      this.gameObject = gameObject;
      this.collider = gameObject.GetComponent<CapsuleCollider>();
      this.rigidbody = gameObject.GetComponent<Rigidbody>();
      if ((UnityEngine.Object) this.rigidbody != (UnityEngine.Object) null)
        this.rigidbody.isKinematic = true;
      Pivot component = gameObject.GetComponent<Pivot>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        component.RagdollWeight = 0.0f;
      this.animator = component.GetAnimator();
      this.animator.updateMode = AnimatorUpdateMode.Normal;
    }

    public void StartMovement(Vector3 direction, EngineBehavior.GaitType gait)
    {
    }

    public bool Move(Vector3 direction, float remainingDistance, EngineBehavior.GaitType gait)
    {
      return false;
    }

    public void OnAnimatorMove() => this.animator.ApplyBuiltinRootMotion();

    public bool Rotate(Vector3 direction) => throw new NotImplementedException();

    public void Update()
    {
    }

    public void FixedUpdate()
    {
    }
  }
}
