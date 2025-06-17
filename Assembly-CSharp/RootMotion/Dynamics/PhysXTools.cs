using System;
using UnityEngine;

namespace RootMotion.Dynamics
{
  public static class PhysXTools
  {
    public static Vector3 GetCenterOfMass(Rigidbody[] rigidbodies)
    {
      Vector3 zero = Vector3.zero;
      float num = 0.0f;
      for (int index = 0; index < rigidbodies.Length; ++index)
      {
        if (rigidbodies[index].gameObject.activeInHierarchy)
        {
          zero += rigidbodies[index].worldCenterOfMass * rigidbodies[index].mass;
          num += rigidbodies[index].mass;
        }
      }
      return zero / num;
    }

    public static Vector3 GetCenterOfMassVelocity(Rigidbody[] rigidbodies)
    {
      Vector3 zero = Vector3.zero;
      float num = 0.0f;
      for (int index = 0; index < rigidbodies.Length; ++index)
      {
        if (rigidbodies[index].gameObject.activeInHierarchy)
        {
          zero += rigidbodies[index].velocity * rigidbodies[index].mass;
          num += rigidbodies[index].mass;
        }
      }
      return zero / num;
    }

    public static void DivByInertia(ref Vector3 v, Quaternion rotation, Vector3 inertiaTensor)
    {
      v = rotation * Div(Quaternion.Inverse(rotation) * v, inertiaTensor);
    }

    public static void ScaleByInertia(ref Vector3 v, Quaternion rotation, Vector3 inertiaTensor)
    {
      v = rotation * Vector3.Scale(Quaternion.Inverse(rotation) * v, inertiaTensor);
    }

    public static Vector3 GetFromToAcceleration(Vector3 fromV, Vector3 toV)
    {
      Quaternion rotation = Quaternion.FromToRotation(fromV, toV);
      rotation.ToAngleAxis(out float angle, out Vector3 axis);
      return angle * axis * ((float) Math.PI / 180f) / Time.fixedDeltaTime;
    }

    public static Vector3 GetAngularAcceleration(Quaternion fromR, Quaternion toR)
    {
      Vector3 vector3_1 = Vector3.Cross(fromR * Vector3.forward, toR * Vector3.forward);
      Vector3 vector3_2 = Vector3.Cross(fromR * Vector3.up, toR * Vector3.up);
      float num = Quaternion.Angle(fromR, toR);
      return Vector3.Normalize(vector3_1 + vector3_2) * num * ((float) Math.PI / 180f) / Time.fixedDeltaTime;
    }

    public static void AddFromToTorque(Rigidbody r, Quaternion toR, ForceMode forceMode)
    {
      Vector3 torque = GetAngularAcceleration(r.rotation, toR) - r.angularVelocity;
      switch (forceMode)
      {
        case ForceMode.Force:
          Vector3 v1 = torque / Time.fixedDeltaTime;
          ScaleByInertia(ref v1, r.rotation, r.inertiaTensor);
          r.AddTorque(v1, forceMode);
          break;
        case ForceMode.Impulse:
          Vector3 v2 = torque;
          ScaleByInertia(ref v2, r.rotation, r.inertiaTensor);
          r.AddTorque(v2, forceMode);
          break;
        case ForceMode.VelocityChange:
          r.AddTorque(torque, forceMode);
          break;
        case ForceMode.Acceleration:
          r.AddTorque(torque / Time.fixedDeltaTime, forceMode);
          break;
      }
    }

    public static void AddFromToTorque(
      Rigidbody r,
      Vector3 fromV,
      Vector3 toV,
      ForceMode forceMode)
    {
      Vector3 torque = GetFromToAcceleration(fromV, toV) - r.angularVelocity;
      switch (forceMode)
      {
        case ForceMode.Force:
          Vector3 v1 = torque / Time.fixedDeltaTime;
          ScaleByInertia(ref v1, r.rotation, r.inertiaTensor);
          r.AddTorque(v1, forceMode);
          break;
        case ForceMode.Impulse:
          Vector3 v2 = torque;
          ScaleByInertia(ref v2, r.rotation, r.inertiaTensor);
          r.AddTorque(v2, forceMode);
          break;
        case ForceMode.VelocityChange:
          r.AddTorque(torque, forceMode);
          break;
        case ForceMode.Acceleration:
          r.AddTorque(torque / Time.fixedDeltaTime, forceMode);
          break;
      }
    }

    public static void AddFromToForce(
      Rigidbody r,
      Vector3 fromV,
      Vector3 toV,
      ForceMode forceMode)
    {
      Vector3 force1 = GetLinearAcceleration(fromV, toV) - r.velocity;
      switch (forceMode)
      {
        case ForceMode.Force:
          Vector3 force2 = force1 / Time.fixedDeltaTime * r.mass;
          r.AddForce(force2, forceMode);
          break;
        case ForceMode.Impulse:
          Vector3 force3 = force1 * r.mass;
          r.AddForce(force3, forceMode);
          break;
        case ForceMode.VelocityChange:
          r.AddForce(force1, forceMode);
          break;
        case ForceMode.Acceleration:
          r.AddForce(force1 / Time.fixedDeltaTime, forceMode);
          break;
      }
    }

    public static Vector3 GetLinearAcceleration(Vector3 fromPoint, Vector3 toPoint)
    {
      return (toPoint - fromPoint) / Time.fixedDeltaTime;
    }

    public static Quaternion ToJointSpace(ConfigurableJoint joint)
    {
      Vector3 vector3 = Vector3.Cross(joint.axis, joint.secondaryAxis);
      Vector3 upwards = Vector3.Cross(vector3, joint.axis);
      return Quaternion.LookRotation(vector3, upwards);
    }

    public static Vector3 CalculateInertiaTensorCuboid(Vector3 size, float mass)
    {
      float num1 = size.x * size.x;
      float num2 = size.y * size.y;
      float num3 = size.z * size.z;
      float num4 = 0.0833333358f * mass;
      return new Vector3(num4 * (num2 + num3), num4 * (num1 + num3), num4 * (num1 + num2));
    }

    public static Vector3 Div(Vector3 v, Vector3 v2)
    {
      return new Vector3(v.x / v2.x, v.y / v2.y, v.z / v2.z);
    }
  }
}
