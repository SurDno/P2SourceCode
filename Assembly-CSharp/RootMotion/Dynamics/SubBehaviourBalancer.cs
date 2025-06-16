using System;
using UnityEngine;

namespace RootMotion.Dynamics
{
  [Serializable]
  public class SubBehaviourBalancer : SubBehaviourBase
  {
    private Settings settings;
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
      Settings settings,
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
      toJointSpace = PhysXTools.ToJointSpace(joint);
      behaviour.OnPreFixedUpdate += Solve;
    }

    private void Solve()
    {
      if (copPoints.Length == 0)
      {
        cop = joint.transform.TransformPoint(joint.anchor);
      }
      else
      {
        cop = Vector3.zero;
        foreach (Transform copPoint in copPoints)
          cop += copPoint.position;
        cop /= copPoints.Length;
      }
      cop += settings.copOffset;
      com = PhysXTools.GetCenterOfMass(rigidbodies);
      comV = PhysXTools.GetCenterOfMassVelocity(rigidbodies);
      dir = com - cop;
      dirVel = com + comV * settings.velocityF - cop;
      Vector3 v = (PhysXTools.GetFromToAcceleration(dirVel, -Physics.gravity) - Ibody.angularVelocity) / Time.fixedDeltaTime;
      PhysXTools.ScaleByInertia(ref v, Ibody.rotation, Ibody.inertiaTensor * settings.IMlp);
      Vector3 vector3 = Vector3.ClampMagnitude(v, settings.maxTorqueMag);
      if (pressureSensor == null || !pressureSensor.enabled || pressureSensor.inContact)
      {
        Ibody.AddTorque(vector3 * settings.torqueMlp, ForceMode.Force);
        joint.targetAngularVelocity = Quaternion.Inverse(toJointSpace) * Quaternion.Inverse(joint.transform.rotation) * vector3;
      }
      else
        joint.targetAngularVelocity = Vector3.zero;
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
      public float torqueMlp;
      [Tooltip("Maximum magnitude of the torque applied to the lower legs if 'Torque Mlp' > 0.")]
      public float maxTorqueMag = 45f;
    }
  }
}
