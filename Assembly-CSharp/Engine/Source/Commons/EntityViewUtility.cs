using Engine.Common;
using UnityEngine;
using UnityEngine.AI;

namespace Engine.Source.Commons
{
  public static class EntityViewUtility
  {
    public static void SetTransformAndData(
      IEntity entity,
      Vector3 position,
      Quaternion rotation,
      bool player)
    {
      ((IEntityView) entity).Position = position;
      ((IEntityView) entity).Rotation = rotation;
      if (!((IEntityView) entity).IsAttached)
        return;
      Transform transform = ((IEntityView) entity).GameObject.transform;
      Rigidbody componentNonAlloc1 = transform.GetComponentNonAlloc<Rigidbody>();
      bool flag = false;
      if ((Object) componentNonAlloc1 != (Object) null && !componentNonAlloc1.isKinematic)
      {
        flag = true;
        componentNonAlloc1.isKinematic = true;
      }
      if (player)
        rotation = Quaternion.Euler(0.0f, rotation.eulerAngles.y, 0.0f);
      transform.SetPositionAndRotation(position, rotation);
      if ((Object) componentNonAlloc1 != (Object) null)
      {
        if (flag)
          componentNonAlloc1.isKinematic = false;
        componentNonAlloc1.velocity = Vector3.zero;
      }
      Physics.SyncTransforms();
      NavMeshAgent componentNonAlloc2 = transform.GetComponentNonAlloc<NavMeshAgent>();
      if (!((Object) componentNonAlloc2 != (Object) null))
        return;
      componentNonAlloc2.Warp(position);
    }

    public static void FromTransformToData(IEntity entity)
    {
      if (!((IEntityView) entity).IsAttached)
        return;
      GameObject gameObject = ((IEntityView) entity).GameObject;
      if ((Object) gameObject == (Object) null)
      {
        Debug.LogError((object) ("GameObject is destroed, owner : " + entity.GetInfo()));
      }
      else
      {
        Transform transform = gameObject.transform;
        ((IEntityView) entity).Position = transform.position;
        ((IEntityView) entity).Rotation = transform.rotation;
      }
    }

    public static void ConvertMatrix(
      Matrix4x4 matrix,
      out Vector3 position,
      out Quaternion rotation)
    {
      position = new Vector3(matrix.m03, matrix.m13, matrix.m23);
      Vector3 forward;
      forward.x = matrix.m02;
      forward.y = matrix.m12;
      forward.z = matrix.m22;
      Vector3 upwards;
      upwards.x = matrix.m01;
      upwards.y = matrix.m11;
      upwards.z = matrix.m21;
      int num = Mathf.Approximately(forward.sqrMagnitude, 0.0f) || Mathf.Approximately(upwards.sqrMagnitude, 0.0f) ? 1 : (forward == upwards ? 1 : 0);
      rotation = num == 0 ? Quaternion.LookRotation(forward, upwards) : Quaternion.Euler(0.0f, 0.0f, 0.0f);
      Vector3 vector3;
      vector3.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
      ref Vector3 local1 = ref vector3;
      Vector4 vector4 = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31);
      double magnitude1 = (double) vector4.magnitude;
      local1.y = (float) magnitude1;
      ref Vector3 local2 = ref vector3;
      vector4 = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32);
      double magnitude2 = (double) vector4.magnitude;
      local2.z = (float) magnitude2;
    }
  }
}
