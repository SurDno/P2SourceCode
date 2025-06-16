// Decompiled with JetBrains decompiler
// Type: RootMotion.Dynamics.JointConverter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace RootMotion.Dynamics
{
  public static class JointConverter
  {
    public static void ToConfigurable(GameObject root)
    {
      int num = 0;
      foreach (CharacterJoint componentsInChild in root.GetComponentsInChildren<CharacterJoint>())
      {
        JointConverter.CharacterToConfigurable(componentsInChild);
        ++num;
      }
      foreach (HingeJoint componentsInChild in root.GetComponentsInChildren<HingeJoint>())
      {
        JointConverter.HingeToConfigurable(componentsInChild);
        ++num;
      }
      foreach (FixedJoint componentsInChild in root.GetComponentsInChildren<FixedJoint>())
      {
        JointConverter.FixedToConfigurable(componentsInChild);
        ++num;
      }
      foreach (SpringJoint componentsInChild in root.GetComponentsInChildren<SpringJoint>())
      {
        JointConverter.SpringToConfigurable(componentsInChild);
        ++num;
      }
      if (num > 0)
        Debug.Log((object) (num.ToString() + " joints were successfully converted to ConfigurableJoints."));
      else
        Debug.Log((object) ("No joints found in the children of " + root.name + " to convert to ConfigurableJoints."));
    }

    public static void HingeToConfigurable(HingeJoint src)
    {
      ConfigurableJoint conf = src.gameObject.AddComponent<ConfigurableJoint>();
      JointConverter.ConvertJoint(ref conf, (Joint) src);
      conf.secondaryAxis = Vector3.zero;
      conf.xMotion = ConfigurableJointMotion.Locked;
      conf.yMotion = ConfigurableJointMotion.Locked;
      conf.zMotion = ConfigurableJointMotion.Locked;
      conf.angularXMotion = src.useLimits ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Free;
      conf.angularYMotion = ConfigurableJointMotion.Locked;
      conf.angularZMotion = ConfigurableJointMotion.Locked;
      conf.highAngularXLimit = JointConverter.ConvertToHighSoftJointLimit(src.limits, src.spring, src.useSpring);
      conf.angularXLimitSpring = JointConverter.ConvertToSoftJointLimitSpring(src.limits, src.spring, src.useSpring);
      conf.lowAngularXLimit = JointConverter.ConvertToLowSoftJointLimit(src.limits, src.spring, src.useSpring);
      if (src.useMotor)
        Debug.LogWarning((object) "Can not convert HingeJoint Motor to ConfigurableJoint.");
      Object.DestroyImmediate((Object) src);
    }

    public static void FixedToConfigurable(FixedJoint src)
    {
      ConfigurableJoint conf = src.gameObject.AddComponent<ConfigurableJoint>();
      JointConverter.ConvertJoint(ref conf, (Joint) src);
      conf.secondaryAxis = Vector3.zero;
      conf.xMotion = ConfigurableJointMotion.Locked;
      conf.yMotion = ConfigurableJointMotion.Locked;
      conf.zMotion = ConfigurableJointMotion.Locked;
      conf.angularXMotion = ConfigurableJointMotion.Locked;
      conf.angularYMotion = ConfigurableJointMotion.Locked;
      conf.angularZMotion = ConfigurableJointMotion.Locked;
      Object.DestroyImmediate((Object) src);
    }

    public static void SpringToConfigurable(SpringJoint src)
    {
      ConfigurableJoint conf = src.gameObject.AddComponent<ConfigurableJoint>();
      JointConverter.ConvertJoint(ref conf, (Joint) src);
      conf.xMotion = ConfigurableJointMotion.Limited;
      conf.yMotion = ConfigurableJointMotion.Limited;
      conf.zMotion = ConfigurableJointMotion.Limited;
      conf.angularXMotion = ConfigurableJointMotion.Free;
      conf.angularYMotion = ConfigurableJointMotion.Free;
      conf.angularZMotion = ConfigurableJointMotion.Free;
      conf.linearLimit = new SoftJointLimit()
      {
        bounciness = 0.0f,
        limit = src.maxDistance
      };
      conf.linearLimitSpring = new SoftJointLimitSpring()
      {
        damper = src.damper,
        spring = src.spring
      };
      Object.DestroyImmediate((Object) src);
    }

    public static void CharacterToConfigurable(CharacterJoint src)
    {
      ConfigurableJoint conf = src.gameObject.AddComponent<ConfigurableJoint>();
      JointConverter.ConvertJoint(ref conf, (Joint) src);
      conf.secondaryAxis = src.swingAxis;
      conf.xMotion = ConfigurableJointMotion.Locked;
      conf.yMotion = ConfigurableJointMotion.Locked;
      conf.zMotion = ConfigurableJointMotion.Locked;
      conf.angularXMotion = ConfigurableJointMotion.Limited;
      conf.angularYMotion = ConfigurableJointMotion.Limited;
      conf.angularZMotion = ConfigurableJointMotion.Limited;
      conf.highAngularXLimit = JointConverter.CopyLimit(src.highTwistLimit);
      conf.lowAngularXLimit = JointConverter.CopyLimit(src.lowTwistLimit);
      conf.angularYLimit = JointConverter.CopyLimit(src.swing1Limit);
      conf.angularZLimit = JointConverter.CopyLimit(src.swing2Limit);
      conf.angularXLimitSpring = JointConverter.CopyLimitSpring(src.twistLimitSpring);
      conf.angularYZLimitSpring = JointConverter.CopyLimitSpring(src.swingLimitSpring);
      conf.enableCollision = src.enableCollision;
      conf.projectionMode = src.enableProjection ? JointProjectionMode.PositionAndRotation : JointProjectionMode.None;
      conf.projectionAngle = src.projectionAngle;
      conf.projectionDistance = src.projectionDistance;
      Object.DestroyImmediate((Object) src);
    }

    private static void ConvertJoint(ref ConfigurableJoint conf, Joint src)
    {
      conf.anchor = src.anchor;
      conf.autoConfigureConnectedAnchor = src.autoConfigureConnectedAnchor;
      conf.axis = src.axis;
      conf.breakForce = src.breakForce;
      conf.breakTorque = src.breakTorque;
      conf.connectedAnchor = src.connectedAnchor;
      conf.connectedBody = src.connectedBody;
      conf.enableCollision = src.enableCollision;
    }

    private static SoftJointLimit ConvertToHighSoftJointLimit(
      JointLimits src,
      JointSpring spring,
      bool useSpring)
    {
      return new SoftJointLimit()
      {
        limit = -src.max,
        bounciness = src.bounciness
      };
    }

    private static SoftJointLimit ConvertToLowSoftJointLimit(
      JointLimits src,
      JointSpring spring,
      bool useSpring)
    {
      return new SoftJointLimit()
      {
        limit = -src.min,
        bounciness = src.bounciness
      };
    }

    private static SoftJointLimitSpring ConvertToSoftJointLimitSpring(
      JointLimits src,
      JointSpring spring,
      bool useSpring)
    {
      return new SoftJointLimitSpring()
      {
        damper = useSpring ? spring.damper : 0.0f,
        spring = useSpring ? spring.spring : 0.0f
      };
    }

    private static SoftJointLimit CopyLimit(SoftJointLimit src)
    {
      return new SoftJointLimit()
      {
        limit = src.limit,
        bounciness = src.bounciness
      };
    }

    private static SoftJointLimitSpring CopyLimitSpring(SoftJointLimitSpring src)
    {
      return new SoftJointLimitSpring()
      {
        damper = src.damper,
        spring = src.spring
      };
    }
  }
}
