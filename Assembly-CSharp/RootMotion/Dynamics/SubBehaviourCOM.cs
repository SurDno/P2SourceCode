// Decompiled with JetBrains decompiler
// Type: RootMotion.Dynamics.SubBehaviourCOM
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace RootMotion.Dynamics
{
  [Serializable]
  public class SubBehaviourCOM : SubBehaviourBase
  {
    public SubBehaviourCOM.Mode mode;
    public float velocityDamper = 1f;
    public float velocityLerpSpeed = 5f;
    public float velocityMax = 1f;
    public float centerOfPressureSpeed = 5f;
    public Vector3 offset;
    [HideInInspector]
    public bool[] groundContacts;
    [HideInInspector]
    public Vector3[] groundContactPoints;
    private LayerMask groundLayers;

    public Vector3 position { get; private set; }

    public Vector3 direction { get; private set; }

    public float angle { get; private set; }

    public Vector3 velocity { get; private set; }

    public Vector3 centerOfPressure { get; private set; }

    public Quaternion rotation { get; private set; }

    public Quaternion inverseRotation { get; private set; }

    public bool isGrounded { get; private set; }

    public float lastGroundedTime { get; private set; }

    public void Initiate(BehaviourBase behaviour, LayerMask groundLayers)
    {
      this.behaviour = behaviour;
      this.groundLayers = groundLayers;
      this.rotation = Quaternion.identity;
      this.groundContacts = new bool[behaviour.puppetMaster.muscles.Length];
      this.groundContactPoints = new Vector3[this.groundContacts.Length];
      behaviour.OnPreActivate += new BehaviourBase.BehaviourDelegate(this.OnPreActivate);
      behaviour.OnPreLateUpdate += new BehaviourBase.BehaviourDelegate(this.OnPreLateUpdate);
      behaviour.OnPreDeactivate += new BehaviourBase.BehaviourDelegate(this.OnPreDeactivate);
      behaviour.OnPreMuscleCollision += new BehaviourBase.CollisionDelegate(this.OnPreMuscleCollision);
      behaviour.OnPreMuscleCollisionExit += new BehaviourBase.CollisionDelegate(this.OnPreMuscleCollisionExit);
      behaviour.OnHierarchyChanged += new BehaviourBase.BehaviourDelegate(this.OnHierarchyChanged);
    }

    private void OnHierarchyChanged()
    {
      Array.Resize<bool>(ref this.groundContacts, this.behaviour.puppetMaster.muscles.Length);
      Array.Resize<Vector3>(ref this.groundContactPoints, this.behaviour.puppetMaster.muscles.Length);
    }

    private void OnPreMuscleCollision(MuscleCollision c)
    {
      if (!LayerMaskExtensions.Contains(this.groundLayers, c.collision.gameObject.layer) || c.collision.contacts.Length == 0)
        return;
      this.lastGroundedTime = Time.time;
      this.groundContacts[c.muscleIndex] = true;
      if (this.mode != SubBehaviourCOM.Mode.CenterOfPressure)
        return;
      this.groundContactPoints[c.muscleIndex] = this.GetCollisionCOP(c.collision);
    }

    private void OnPreMuscleCollisionExit(MuscleCollision c)
    {
      if (!LayerMaskExtensions.Contains(this.groundLayers, c.collision.gameObject.layer))
        return;
      this.groundContacts[c.muscleIndex] = false;
      this.groundContactPoints[c.muscleIndex] = Vector3.zero;
    }

    private void OnPreActivate()
    {
      this.position = this.GetCenterOfMass();
      this.centerOfPressure = this.GetFeetCentroid();
      this.direction = this.position - this.centerOfPressure;
      this.angle = Vector3.Angle(this.direction, Vector3.up);
      this.velocity = Vector3.zero;
    }

    private void OnPreLateUpdate()
    {
      this.isGrounded = this.IsGrounded();
      if (this.mode == SubBehaviourCOM.Mode.FeetCentroid || !this.isGrounded)
      {
        this.centerOfPressure = this.GetFeetCentroid();
      }
      else
      {
        Vector3 b = this.isGrounded ? this.GetCenterOfPressure() : this.GetFeetCentroid();
        this.centerOfPressure = (double) this.centerOfPressureSpeed <= 2.0 ? b : Vector3.Lerp(this.centerOfPressure, b, Time.deltaTime * this.centerOfPressureSpeed);
      }
      this.position = this.GetCenterOfMass();
      Vector3 vector3 = (this.GetCenterOfMassVelocity() - this.position) with
      {
        y = 0.0f
      };
      vector3 = Vector3.ClampMagnitude(vector3, this.velocityMax);
      this.velocity = (double) this.velocityLerpSpeed <= 0.0 ? vector3 : Vector3.Lerp(this.velocity, vector3, Time.deltaTime * this.velocityLerpSpeed);
      this.position += this.velocity * this.velocityDamper;
      this.position += this.behaviour.puppetMaster.targetRoot.rotation * this.offset;
      this.direction = this.position - this.centerOfPressure;
      this.rotation = Quaternion.FromToRotation(Vector3.up, this.direction);
      this.inverseRotation = Quaternion.Inverse(this.rotation);
      this.angle = Quaternion.Angle(Quaternion.identity, this.rotation);
    }

    private void OnPreDeactivate() => this.velocity = Vector3.zero;

    private Vector3 GetCollisionCOP(Collision collision)
    {
      Vector3 zero = Vector3.zero;
      for (int index = 0; index < collision.contacts.Length; ++index)
        zero += collision.contacts[index].point;
      return zero / (float) collision.contacts.Length;
    }

    private bool IsGrounded()
    {
      for (int index = 0; index < this.groundContacts.Length; ++index)
      {
        if (this.groundContacts[index])
          return true;
      }
      return false;
    }

    private Vector3 GetCenterOfMass()
    {
      Vector3 zero = Vector3.zero;
      float num = 0.0f;
      foreach (Muscle muscle in this.behaviour.puppetMaster.muscles)
      {
        zero += muscle.rigidbody.worldCenterOfMass * muscle.rigidbody.mass;
        num += muscle.rigidbody.mass;
      }
      Vector3 vector3;
      return vector3 = zero / num;
    }

    private Vector3 GetCenterOfMassVelocity()
    {
      Vector3 vector3_1 = Vector3.zero;
      float num = 0.0f;
      foreach (Muscle muscle in this.behaviour.puppetMaster.muscles)
      {
        vector3_1 = vector3_1 + muscle.rigidbody.worldCenterOfMass * muscle.rigidbody.mass + muscle.rigidbody.velocity * muscle.rigidbody.mass;
        num += muscle.rigidbody.mass;
      }
      Vector3 vector3_2;
      return vector3_2 = vector3_1 / num;
    }

    private Vector3 GetMomentum()
    {
      Vector3 zero = Vector3.zero;
      for (int index = 0; index < this.behaviour.puppetMaster.muscles.Length; ++index)
        zero += this.behaviour.puppetMaster.muscles[index].rigidbody.velocity * this.behaviour.puppetMaster.muscles[index].rigidbody.mass;
      return zero;
    }

    private Vector3 GetCenterOfPressure()
    {
      Vector3 zero = Vector3.zero;
      int num = 0;
      for (int index = 0; index < this.groundContacts.Length; ++index)
      {
        if (this.groundContacts[index])
        {
          zero += this.groundContactPoints[index];
          ++num;
        }
      }
      return zero / (float) num;
    }

    private Vector3 GetFeetCentroid()
    {
      Vector3 zero = Vector3.zero;
      int num = 0;
      for (int index = 0; index < this.behaviour.puppetMaster.muscles.Length; ++index)
      {
        if (this.behaviour.puppetMaster.muscles[index].props.group == Muscle.Group.Foot)
        {
          zero += this.behaviour.puppetMaster.muscles[index].rigidbody.worldCenterOfMass;
          ++num;
        }
      }
      return zero / (float) num;
    }

    [Serializable]
    public enum Mode
    {
      FeetCentroid,
      CenterOfPressure,
    }
  }
}
