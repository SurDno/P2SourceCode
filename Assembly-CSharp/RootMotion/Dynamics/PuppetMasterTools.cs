// Decompiled with JetBrains decompiler
// Type: RootMotion.Dynamics.PuppetMasterTools
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace RootMotion.Dynamics
{
  public static class PuppetMasterTools
  {
    public static void PositionRagdoll(PuppetMaster puppetMaster)
    {
      Rigidbody[] componentsInChildren = puppetMaster.transform.GetComponentsInChildren<Rigidbody>();
      if (componentsInChildren.Length == 0)
        return;
      foreach (Muscle muscle in puppetMaster.muscles)
      {
        if ((Object) muscle.joint == (Object) null || (Object) muscle.target == (Object) null)
          return;
      }
      Vector3[] vector3Array = new Vector3[componentsInChildren.Length];
      for (int index = 0; index < componentsInChildren.Length; ++index)
      {
        if (componentsInChildren[index].transform.childCount == 1)
          vector3Array[index] = componentsInChildren[index].transform.InverseTransformDirection(componentsInChildren[index].transform.GetChild(0).position - componentsInChildren[index].transform.position);
      }
      foreach (Rigidbody rigidbody in componentsInChildren)
      {
        foreach (Muscle muscle in puppetMaster.muscles)
        {
          if ((Object) muscle.joint.GetComponent<Rigidbody>() == (Object) rigidbody)
            rigidbody.transform.position = muscle.target.position;
        }
      }
      for (int index = 0; index < componentsInChildren.Length; ++index)
      {
        if (componentsInChildren[index].transform.childCount == 1)
        {
          Vector3 position = componentsInChildren[index].transform.GetChild(0).position;
          componentsInChildren[index].transform.rotation = Quaternion.FromToRotation(componentsInChildren[index].transform.rotation * vector3Array[index], position - componentsInChildren[index].transform.position) * componentsInChildren[index].transform.rotation;
          componentsInChildren[index].transform.GetChild(0).position = position;
        }
      }
    }

    public static void RealignRagdoll(PuppetMaster puppetMaster)
    {
      foreach (Muscle muscle in puppetMaster.muscles)
      {
        if ((Object) muscle.joint == (Object) null || (Object) muscle.joint.transform == (Object) null || (Object) muscle.target == (Object) null)
        {
          Debug.LogWarning((object) "Muscles incomplete, can not realign ragdoll.");
          return;
        }
      }
      foreach (Muscle muscle in puppetMaster.muscles)
      {
        if ((Object) muscle.target != (Object) null)
        {
          Transform[] transformArray = new Transform[muscle.joint.transform.childCount];
          for (int index = 0; index < transformArray.Length; ++index)
            transformArray[index] = muscle.joint.transform.GetChild(index);
          foreach (Transform transform in transformArray)
            transform.parent = (Transform) null;
          BoxCollider component1 = muscle.joint.GetComponent<BoxCollider>();
          Vector3 vector1 = Vector3.zero;
          Vector3 vector2 = Vector3.zero;
          if ((Object) component1 != (Object) null)
          {
            vector1 = component1.transform.TransformVector(component1.size);
            vector2 = component1.transform.TransformVector(component1.center);
          }
          CapsuleCollider component2 = muscle.joint.GetComponent<CapsuleCollider>();
          Vector3 vector3 = Vector3.zero;
          Vector3 direction = Vector3.zero;
          if ((Object) component2 != (Object) null)
          {
            vector3 = component2.transform.TransformVector(component2.center);
            direction = component2.transform.TransformVector(PuppetMasterTools.DirectionIntToVector3(component2.direction));
          }
          SphereCollider component3 = muscle.joint.GetComponent<SphereCollider>();
          Vector3 vector4 = Vector3.zero;
          if ((Object) component3 != (Object) null)
            vector4 = component3.transform.TransformVector(component3.center);
          Vector3 vector5 = muscle.joint.transform.TransformVector(muscle.joint.axis);
          Vector3 vector6 = muscle.joint.transform.TransformVector(muscle.joint.secondaryAxis);
          muscle.joint.transform.rotation = muscle.target.rotation;
          if ((Object) component1 != (Object) null)
          {
            component1.size = component1.transform.InverseTransformVector(vector1);
            component1.center = component1.transform.InverseTransformVector(vector2);
          }
          if ((Object) component2 != (Object) null)
          {
            component2.center = component2.transform.InverseTransformVector(vector3);
            Vector3 dir = component2.transform.InverseTransformDirection(direction);
            component2.direction = PuppetMasterTools.DirectionVector3ToInt(dir);
          }
          if ((Object) component3 != (Object) null)
            component3.center = component3.transform.InverseTransformVector(vector4);
          muscle.joint.axis = muscle.joint.transform.InverseTransformVector(vector5);
          muscle.joint.secondaryAxis = muscle.joint.transform.InverseTransformVector(vector6);
          foreach (Transform transform in transformArray)
            transform.parent = muscle.joint.transform;
        }
      }
    }

    private static Vector3 DirectionIntToVector3(int dir)
    {
      if (dir == 0)
        return Vector3.right;
      return dir == 1 ? Vector3.up : Vector3.forward;
    }

    private static int DirectionVector3ToInt(Vector3 dir)
    {
      float f1 = Vector3.Dot(dir, Vector3.right);
      float f2 = Vector3.Dot(dir, Vector3.up);
      float f3 = Vector3.Dot(dir, Vector3.forward);
      float num1 = Mathf.Abs(f1);
      float num2 = Mathf.Abs(f2);
      float num3 = Mathf.Abs(f3);
      int num4 = 0;
      if ((double) num2 > (double) num1 && (double) num2 > (double) num3)
        num4 = 1;
      if ((double) num3 > (double) num1 && (double) num3 > (double) num2)
        num4 = 2;
      return num4;
    }
  }
}
