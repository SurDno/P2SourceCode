using System;

namespace RootMotion.Dynamics
{
  public abstract class RagdollCreator : MonoBehaviour
  {
    public static void ClearAll(Transform root)
    {
      if ((UnityEngine.Object) root == (UnityEngine.Object) null)
        return;
      Transform transform = root;
      Animator componentInChildren = root.GetComponentInChildren<Animator>();
      if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null && componentInChildren.isHuman)
      {
        Transform boneTransform = componentInChildren.GetBoneTransform(HumanBodyBones.Hips);
        if ((UnityEngine.Object) boneTransform != (UnityEngine.Object) null && boneTransform.GetComponentsInChildren<Transform>().Length > 2)
          transform = boneTransform;
      }
      Transform[] componentsInChildren = transform.GetComponentsInChildren<Transform>();
      if (componentsInChildren.Length < 2)
        return;
      for (int index = !((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null) || !componentInChildren.isHuman ? 1 : 0; index < componentsInChildren.Length; ++index)
        ClearTransform(componentsInChildren[index]);
    }

    protected static void ClearTransform(Transform transform)
    {
      if ((UnityEngine.Object) transform == (UnityEngine.Object) null)
        return;
      foreach (Collider component in transform.GetComponents<Collider>())
      {
        if ((UnityEngine.Object) component != (UnityEngine.Object) null && !component.isTrigger)
          UnityEngine.Object.DestroyImmediate((UnityEngine.Object) component);
      }
      Joint component1 = transform.GetComponent<Joint>();
      if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) component1);
      Rigidbody component2 = transform.GetComponent<Rigidbody>();
      if (!((UnityEngine.Object) component2 != (UnityEngine.Object) null))
        return;
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) component2);
    }

    protected static void CreateCollider(
      Transform t,
      Vector3 startPoint,
      Vector3 endPoint,
      ColliderType colliderType,
      float lengthOverlap,
      float width)
    {
      Vector3 direction = endPoint - startPoint;
      float num = direction.magnitude * (1f + lengthOverlap);
      Vector3 vectorToDirection = AxisTools.GetAxisVectorToDirection(t, direction);
      t.gameObject.AddComponent<Rigidbody>();
      float scaleF = GetScaleF(t);
      switch (colliderType)
      {
        case ColliderType.Box:
          Vector3 vector3 = Vector3.Scale(vectorToDirection, new Vector3(num, num, num));
          if ((double) vector3.x == 0.0)
            vector3.x = width;
          if ((double) vector3.y == 0.0)
            vector3.y = width;
          if ((double) vector3.z == 0.0)
            vector3.z = width;
          BoxCollider boxCollider = t.gameObject.AddComponent<BoxCollider>();
          boxCollider.size = vector3 / scaleF;
          boxCollider.size = new Vector3(Mathf.Abs(boxCollider.size.x), Mathf.Abs(boxCollider.size.y), Mathf.Abs(boxCollider.size.z));
          boxCollider.center = t.InverseTransformPoint(Vector3.Lerp(startPoint, endPoint, 0.5f));
          break;
        case ColliderType.Capsule:
          CapsuleCollider capsuleCollider = t.gameObject.AddComponent<CapsuleCollider>();
          capsuleCollider.height = Mathf.Abs(num / scaleF);
          capsuleCollider.radius = Mathf.Abs(width * 0.75f / scaleF);
          capsuleCollider.direction = DirectionVector3ToInt(vectorToDirection);
          capsuleCollider.center = t.InverseTransformPoint(Vector3.Lerp(startPoint, endPoint, 0.5f));
          break;
      }
    }

    protected static void CreateCollider(
      Transform t,
      Vector3 startPoint,
      Vector3 endPoint,
      ColliderType colliderType,
      float lengthOverlap,
      float width,
      float proportionAspect,
      Vector3 widthDirection)
    {
      if (colliderType == ColliderType.Capsule)
      {
        CreateCollider(t, startPoint, endPoint, colliderType, lengthOverlap, width * proportionAspect);
      }
      else
      {
        Vector3 direction = endPoint - startPoint;
        float num = direction.magnitude * (1f + lengthOverlap);
        Vector3 vectorToDirection = AxisTools.GetAxisVectorToDirection(t, direction);
        Vector3 a = AxisTools.GetAxisVectorToDirection(t, widthDirection);
        if (a == vectorToDirection)
        {
          Debug.LogWarning((object) ("Width axis = height axis on " + t.name), (UnityEngine.Object) t);
          a = new Vector3(vectorToDirection.y, vectorToDirection.z, vectorToDirection.x);
        }
        t.gameObject.AddComponent<Rigidbody>();
        Vector3 vector3 = Vector3.Scale(vectorToDirection, new Vector3(num, num, num)) + Vector3.Scale(a, new Vector3(width, width, width));
        if ((double) vector3.x == 0.0)
          vector3.x = width * proportionAspect;
        if ((double) vector3.y == 0.0)
          vector3.y = width * proportionAspect;
        if ((double) vector3.z == 0.0)
          vector3.z = width * proportionAspect;
        BoxCollider boxCollider = t.gameObject.AddComponent<BoxCollider>();
        boxCollider.size = vector3 / GetScaleF(t);
        boxCollider.center = t.InverseTransformPoint(Vector3.Lerp(startPoint, endPoint, 0.5f));
      }
    }

    protected static float GetScaleF(Transform t)
    {
      Vector3 lossyScale = t.lossyScale;
      return (float) (((double) lossyScale.x + (double) lossyScale.y + (double) lossyScale.z) / 3.0);
    }

    protected static Vector3 Abs(Vector3 v)
    {
      Vector3Abs(ref v);
      return v;
    }

    protected static void Vector3Abs(ref Vector3 v)
    {
      v.x = Mathf.Abs(v.x);
      v.y = Mathf.Abs(v.y);
      v.z = Mathf.Abs(v.z);
    }

    protected static Vector3 DirectionIntToVector3(int dir)
    {
      if (dir == 0)
        return Vector3.right;
      return dir == 1 ? Vector3.up : Vector3.forward;
    }

    protected static Vector3 DirectionToVector3(Direction dir)
    {
      if (dir == Direction.X)
        return Vector3.right;
      return dir == Direction.Y ? Vector3.up : Vector3.forward;
    }

    protected static int DirectionVector3ToInt(Vector3 dir)
    {
      float f1 = Vector3.Dot(dir, Vector3.right);
      float f2 = Vector3.Dot(dir, Vector3.up);
      float f3 = Vector3.Dot(dir, Vector3.forward);
      float num1 = Mathf.Abs(f1);
      float num2 = Mathf.Abs(f2);
      float num3 = Mathf.Abs(f3);
      int num4 = 0;
      if (num2 > (double) num1 && num2 > (double) num3)
        num4 = 1;
      if (num3 > (double) num1 && num3 > (double) num2)
        num4 = 2;
      return num4;
    }

    protected static Vector3 GetLocalOrthoDirection(Transform transform, Vector3 worldDir)
    {
      worldDir = worldDir.normalized;
      float f1 = Vector3.Dot(worldDir, transform.right);
      float f2 = Vector3.Dot(worldDir, transform.up);
      float f3 = Vector3.Dot(worldDir, transform.forward);
      float num1 = Mathf.Abs(f1);
      float num2 = Mathf.Abs(f2);
      float num3 = Mathf.Abs(f3);
      Vector3 localOrthoDirection = Vector3.right;
      if (num2 > (double) num1 && num2 > (double) num3)
        localOrthoDirection = Vector3.up;
      if (num3 > (double) num1 && num3 > (double) num2)
        localOrthoDirection = Vector3.forward;
      if ((double) Vector3.Dot(worldDir, transform.rotation * localOrthoDirection) < 0.0)
        localOrthoDirection = -localOrthoDirection;
      return localOrthoDirection;
    }

    protected static Rigidbody GetConnectedBody(Transform bone, ref Transform[] bones)
    {
      if ((UnityEngine.Object) bone.parent == (UnityEngine.Object) null)
        return (Rigidbody) null;
      foreach (Transform transform in bones)
      {
        if ((UnityEngine.Object) bone.parent == (UnityEngine.Object) transform && (UnityEngine.Object) transform.GetComponent<Rigidbody>() != (UnityEngine.Object) null)
          return transform.GetComponent<Rigidbody>();
      }
      return GetConnectedBody(bone.parent, ref bones);
    }

    protected static void CreateJoint(CreateJointParams p)
    {
      Vector3 localOrthoDirection = GetLocalOrthoDirection(p.rigidbody.transform, p.worldSwingAxis);
      Vector3 rhs = Vector3.forward;
      if ((UnityEngine.Object) p.child != (UnityEngine.Object) null)
        rhs = GetLocalOrthoDirection(p.rigidbody.transform, p.child.position - p.rigidbody.transform.position);
      else if ((UnityEngine.Object) p.connectedBody != (UnityEngine.Object) null)
        rhs = GetLocalOrthoDirection(p.rigidbody.transform, p.rigidbody.transform.position - p.connectedBody.transform.position);
      Vector3 vector3 = Vector3.Cross(localOrthoDirection, rhs);
      if (p.type == JointType.Configurable)
      {
        ConfigurableJoint configurableJoint = p.rigidbody.gameObject.AddComponent<ConfigurableJoint>();
        configurableJoint.connectedBody = p.connectedBody;
        ConfigurableJointMotion configurableJointMotion1 = (UnityEngine.Object) p.connectedBody != (UnityEngine.Object) null ? ConfigurableJointMotion.Locked : ConfigurableJointMotion.Free;
        ConfigurableJointMotion configurableJointMotion2 = (UnityEngine.Object) p.connectedBody != (UnityEngine.Object) null ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Free;
        configurableJoint.xMotion = configurableJointMotion1;
        configurableJoint.yMotion = configurableJointMotion1;
        configurableJoint.zMotion = configurableJointMotion1;
        configurableJoint.angularXMotion = configurableJointMotion2;
        configurableJoint.angularYMotion = configurableJointMotion2;
        configurableJoint.angularZMotion = configurableJointMotion2;
        if ((UnityEngine.Object) p.connectedBody != (UnityEngine.Object) null)
        {
          configurableJoint.axis = localOrthoDirection;
          configurableJoint.secondaryAxis = vector3;
          configurableJoint.lowAngularXLimit = ToSoftJointLimit(p.limits.minSwing);
          configurableJoint.highAngularXLimit = ToSoftJointLimit(p.limits.maxSwing);
          configurableJoint.angularYLimit = ToSoftJointLimit(p.limits.swing2);
          configurableJoint.angularZLimit = ToSoftJointLimit(p.limits.twist);
        }
        configurableJoint.anchor = Vector3.zero;
      }
      else
      {
        if ((UnityEngine.Object) p.connectedBody == (UnityEngine.Object) null)
          return;
        CharacterJoint characterJoint = p.rigidbody.gameObject.AddComponent<CharacterJoint>();
        characterJoint.connectedBody = p.connectedBody;
        characterJoint.axis = localOrthoDirection;
        characterJoint.swingAxis = vector3;
        characterJoint.lowTwistLimit = ToSoftJointLimit(p.limits.minSwing);
        characterJoint.highTwistLimit = ToSoftJointLimit(p.limits.maxSwing);
        characterJoint.swing1Limit = ToSoftJointLimit(p.limits.swing2);
        characterJoint.swing2Limit = ToSoftJointLimit(p.limits.twist);
        characterJoint.anchor = Vector3.zero;
      }
    }

    private static SoftJointLimit ToSoftJointLimit(float limit)
    {
      return new SoftJointLimit { limit = limit };
    }

    [Serializable]
    public enum ColliderType
    {
      Box,
      Capsule,
    }

    [Serializable]
    public enum JointType
    {
      Configurable,
      Character,
    }

    [Serializable]
    public enum Direction
    {
      X,
      Y,
      Z,
    }

    public struct CreateJointParams
    {
      public Rigidbody rigidbody;
      public Rigidbody connectedBody;
      public Transform child;
      public Vector3 worldSwingAxis;
      public Limits limits;
      public JointType type;

      public CreateJointParams(
        Rigidbody rigidbody,
        Rigidbody connectedBody,
        Transform child,
        Vector3 worldSwingAxis,
        Limits limits,
        JointType type)
      {
        this.rigidbody = rigidbody;
        this.connectedBody = connectedBody;
        this.child = child;
        this.worldSwingAxis = worldSwingAxis;
        this.limits = limits;
        this.type = type;
      }

      public struct Limits
      {
        public float minSwing;
        public float maxSwing;
        public float swing2;
        public float twist;

        public Limits(float minSwing, float maxSwing, float swing2, float twist)
        {
          this.minSwing = minSwing;
          this.maxSwing = maxSwing;
          this.swing2 = swing2;
          this.twist = twist;
        }
      }
    }
  }
}
