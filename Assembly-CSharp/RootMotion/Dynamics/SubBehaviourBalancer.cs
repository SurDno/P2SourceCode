// Decompiled with JetBrains decompiler
// Type: RootMotion.Dynamics.SubBehaviourBalancer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace RootMotion.Dynamics
{
  [Serializable]
  public class SubBehaviourBalancer : SubBehaviourBase
  {
    private SubBehaviourBalancer.Settings settings;
    private Rigidbody[] rigidbodies = new Rigidbody[0];
    private Transform[] copPoints = new Transform[0];
    private PressureSensor pressureSensor;
    private Rigidbody Ibody;
    private Vector3 I;
    private Quaternion toJointSpace = Quaternion.identity;

    public ConfigurableJoint joint { get; private set; }

    public Vector3 dir { get; private set; }

    public Vector3 dirVel { get; private set; }

    public Vector3 cop { get; private set; }

    public Vector3 com { get; private set; }

    public Vector3 comV { get; private set; }

    public void Initiate(
      BehaviourBase behaviour,
      SubBehaviourBalancer.Settings settings,
      Rigidbody Ibody,
      Rigidbody[] rigidbodies,
      ConfigurableJoint joint,
      Transform[] copPoints,
      PressureSensor pressureSensor)
    {
      this.behaviour = behaviour;
      this.settings = settings;
      this.Ibody = Ibody;
      this.rigidbodies = rigidbodies;
      this.joint = joint;
      this.copPoints = copPoints;
      this.pressureSensor = pressureSensor;
      this.toJointSpace = PhysXTools.ToJointSpace(joint);
      behaviour.OnPreFixedUpdate += new BehaviourBase.BehaviourDelegate(this.Solve);
    }

    private void Solve()
    {
      if (this.copPoints.Length == 0)
      {
        this.cop = this.joint.transform.TransformPoint(this.joint.anchor);
      }
      else
      {
        this.cop = Vector3.zero;
        foreach (Transform copPoint in this.copPoints)
          this.cop += copPoint.position;
        this.cop /= (float) this.copPoints.Length;
      }
      this.cop += this.settings.copOffset;
      this.com = PhysXTools.GetCenterOfMass(this.rigidbodies);
      this.comV = PhysXTools.GetCenterOfMassVelocity(this.rigidbodies);
      this.dir = this.com - this.cop;
      this.dirVel = this.com + this.comV * this.settings.velocityF - this.cop;
      Vector3 v = (PhysXTools.GetFromToAcceleration(this.dirVel, -Physics.gravity) - this.Ibody.angularVelocity) / Time.fixedDeltaTime;
      PhysXTools.ScaleByInertia(ref v, this.Ibody.rotation, this.Ibody.inertiaTensor * this.settings.IMlp);
      Vector3 vector3 = Vector3.ClampMagnitude(v, this.settings.maxTorqueMag);
      if ((UnityEngine.Object) this.pressureSensor == (UnityEngine.Object) null || !this.pressureSensor.enabled || this.pressureSensor.inContact)
      {
        this.Ibody.AddTorque(vector3 * this.settings.torqueMlp, ForceMode.Force);
        this.joint.targetAngularVelocity = Quaternion.Inverse(this.toJointSpace) * Quaternion.Inverse(this.joint.transform.rotation) * vector3;
      }
      else
        this.joint.targetAngularVelocity = Vector3.zero;
    }

    [Serializable]
    public class Settings
    {
      [Tooltip("Ankle joint damper / spring. Increase to make the balancing effect softer.")]
      public float damperForSpring = 1f;
      [Tooltip("Multiplier for joint max force.")]
      public float maxForceMlp = 0.05f;
      [Tooltip("Multiplier for the inertia tensor. Increasing this will increase the balancing forces.")]
      public float IMlp = 1f;
      [Tooltip("Velocity-based prediction.")]
      public float velocityF = 0.5f;
      [Tooltip("World space offset for the center of pressure. Can be used to make the characer lean in a certain direction.")]
      public Vector3 copOffset;
      [Tooltip("The amount of torque applied to the lower legs to help keep the puppet balanced. Note that this is an external force (not coming from the joints themselves) and might make the simulation seem unnatural.")]
      public float torqueMlp = 0.0f;
      [Tooltip("Maximum magnitude of the torque applied to the lower legs if 'Torque Mlp' > 0.")]
      public float maxTorqueMag = 45f;
    }
  }
}
