using System;

namespace RootMotion.FinalIK
{
  public class Amplifier : OffsetModifier
  {
    [Tooltip("The amplified bodies.")]
    public Body[] bodies;

    protected override void OnModifyOffset()
    {
      if (!ik.fixTransforms)
      {
        if (Warning.logged)
          return;
        Warning.Log("Amplifier needs the Fix Transforms option of the FBBIK to be set to true. Otherwise it might amplify to infinity, should the animator of the character stop because of culling.", this.transform);
      }
      else
      {
        foreach (Body body in bodies)
          body.Update(ik.solver, weight, deltaTime);
      }
    }

    [Serializable]
    public class Body
    {
      [Tooltip("The Transform that's motion we are reading.")]
      public Transform transform;
      [Tooltip("Amplify the 'transform's' position relative to this Transform.")]
      public Transform relativeTo;
      [Tooltip("Linking the body to effectors. One Body can be used to offset more than one effector.")]
      public EffectorLink[] effectorLinks;
      [Tooltip("Amplification magnitude along the up axis of the character.")]
      public float verticalWeight = 1f;
      [Tooltip("Amplification magnitude along the horizontal axes of the character.")]
      public float horizontalWeight = 1f;
      [Tooltip("Speed of the amplifier. 0 means instant.")]
      public float speed = 3f;
      private Vector3 lastRelativePos;
      private Vector3 smoothDelta;
      private bool firstUpdate;

      public void Update(IKSolverFullBodyBiped solver, float w, float deltaTime)
      {
        if ((UnityEngine.Object) transform == (UnityEngine.Object) null || (UnityEngine.Object) relativeTo == (UnityEngine.Object) null)
          return;
        Vector3 vector3_1 = relativeTo.InverseTransformDirection(transform.position - relativeTo.position);
        if (firstUpdate)
        {
          lastRelativePos = vector3_1;
          firstUpdate = false;
        }
        Vector3 b = (vector3_1 - lastRelativePos) / deltaTime;
        smoothDelta = speed <= 0.0 ? b : Vector3.Lerp(smoothDelta, b, deltaTime * speed);
        Vector3 v = relativeTo.TransformDirection(smoothDelta);
        Vector3 vector3_2 = V3Tools.ExtractVertical(v, solver.GetRoot().up, verticalWeight) + V3Tools.ExtractHorizontal(v, solver.GetRoot().up, horizontalWeight);
        for (int index = 0; index < effectorLinks.Length; ++index)
          solver.GetEffector(effectorLinks[index].effector).positionOffset += vector3_2 * w * effectorLinks[index].weight;
        lastRelativePos = vector3_1;
      }

      private static Vector3 Multiply(Vector3 v1, Vector3 v2)
      {
        v1.x *= v2.x;
        v1.y *= v2.y;
        v1.z *= v2.z;
        return v1;
      }

      [Serializable]
      public class EffectorLink
      {
        [Tooltip("Type of the FBBIK effector to use")]
        public FullBodyBipedEffector effector;
        [Tooltip("Weight of using this effector")]
        public float weight;
      }
    }
  }
}
