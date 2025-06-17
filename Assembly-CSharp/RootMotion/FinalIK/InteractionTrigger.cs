using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
  [HelpURL("https://www.youtube.com/watch?v=-TDZpNjt2mk&index=15&list=PLVxSIA1OaTOu8Nos3CalXbJ2DrKnntMv6")]
  [AddComponentMenu("Scripts/RootMotion.FinalIK/Interaction System/Interaction Trigger")]
  public class InteractionTrigger : MonoBehaviour
  {
    [Tooltip("The valid ranges of the character's and/or it's camera's position for triggering interaction when the character is in contact with the collider of this trigger.")]
    public Range[] ranges = [];

    [ContextMenu("TUTORIAL VIDEO")]
    private void OpenTutorial4()
    {
      Application.OpenURL("https://www.youtube.com/watch?v=-TDZpNjt2mk&index=15&list=PLVxSIA1OaTOu8Nos3CalXbJ2DrKnntMv6");
    }

    [ContextMenu("Support Group")]
    private void SupportGroup()
    {
      Application.OpenURL("https://groups.google.com/forum/#!forum/final-ik");
    }

    [ContextMenu("Asset Store Thread")]
    private void ASThread()
    {
      Application.OpenURL("http://forum.unity3d.com/threads/final-ik-full-body-ik-aim-look-at-fabrik-ccd-ik-1-0-released.222685/");
    }

    private void Start()
    {
    }

    public int GetBestRangeIndex(Transform character, Transform raycastFrom, RaycastHit raycastHit)
    {
      if (GetComponent<Collider>() == null)
      {
        Warning.Log("Using the InteractionTrigger requires a Collider component.", transform);
        return -1;
      }
      int bestRangeIndex = -1;
      float num = 180f;
      for (int index = 0; index < ranges.Length; ++index)
      {
        if (ranges[index].IsInRange(character, raycastFrom, raycastHit, transform, out float maxError) && maxError <= (double) num)
        {
          num = maxError;
          bestRangeIndex = index;
        }
      }
      return bestRangeIndex;
    }

    [Serializable]
    public class CharacterPosition
    {
      [Tooltip("If false, will not care where the character stands, as long as it is in contact with the trigger collider.")]
      public bool use;
      [Tooltip("The offset of the character's position relative to the trigger in XZ plane. Y position of the character is unlimited as long as it is contact with the collider.")]
      public Vector2 offset;
      [Tooltip("Angle offset from the default forward direction.")]
      [Range(-180f, 180f)]
      public float angleOffset;
      [Tooltip("Max angular offset of the character's forward from the direction of this trigger.")]
      [Range(0.0f, 180f)]
      public float maxAngle = 45f;
      [Tooltip("Max offset of the character's position from this range's center.")]
      public float radius = 0.5f;
      [Tooltip("If true, will rotate the trigger around it's Y axis relative to the position of the character, so the object can be interacted with from all sides.")]
      public bool orbit;
      [Tooltip("Fixes the Y axis of the trigger to Vector3.up. This makes the trigger symmetrical relative to the object. For example a gun will be able to be picked up from the same direction relative to the barrel no matter which side the gun is resting on.")]
      public bool fixYAxis;

      public Vector3 offset3D => new(offset.x, 0.0f, offset.y);

      public Vector3 direction3D => Quaternion.AngleAxis(angleOffset, Vector3.up) * Vector3.forward;

      public bool IsInRange(Transform character, Transform trigger, out float error)
      {
        error = 0.0f;
        if (!use)
          return true;
        error = 180f;
        if (radius <= 0.0 || maxAngle <= 0.0)
          return false;
        Vector3 forward1 = trigger.forward;
        if (fixYAxis)
          forward1.y = 0.0f;
        if (forward1 == Vector3.zero)
          return false;
        Vector3 normal = fixYAxis ? Vector3.up : trigger.up;
        Quaternion quaternion = Quaternion.LookRotation(forward1, normal);
        Vector3 vector3_1 = trigger.position + quaternion * offset3D;
        Vector3 vector3_2 = orbit ? trigger.position : vector3_1;
        Vector3 tangent1 = character.position - vector3_2;
        Vector3.OrthoNormalize(ref normal, ref tangent1);
        Vector3 vector3_3 = tangent1 * Vector3.Project(character.position - vector3_2, tangent1).magnitude;
        if (orbit)
        {
          float magnitude1 = offset.magnitude;
          float magnitude2 = vector3_3.magnitude;
          if (magnitude2 < magnitude1 - (double) radius || magnitude2 > magnitude1 + (double) radius)
            return false;
        }
        else if (vector3_3.magnitude > (double) radius)
          return false;
        Vector3 tangent2 = quaternion * direction3D;
        Vector3.OrthoNormalize(ref normal, ref tangent2);
        if (orbit)
        {
          Vector3 forward2 = vector3_1 - trigger.position;
          if (forward2 == Vector3.zero)
            forward2 = Vector3.forward;
          Vector3 vector3_4 = Quaternion.Inverse(Quaternion.LookRotation(forward2, normal)) * vector3_3;
          tangent2 = Quaternion.AngleAxis(Mathf.Atan2(vector3_4.x, vector3_4.z) * 57.29578f, normal) * tangent2;
        }
        float num = Vector3.Angle(tangent2, character.forward);
        if (num > (double) maxAngle)
          return false;
        error = (float) (num / (double) maxAngle * 180.0);
        return true;
      }
    }

    [Serializable]
    public class CameraPosition
    {
      [Tooltip("What the camera should be looking at to trigger the interaction?")]
      public Collider lookAtTarget;
      [Tooltip("The direction from the lookAtTarget towards the camera (in lookAtTarget's space).")]
      public Vector3 direction = -Vector3.forward;
      [Tooltip("Max distance from the lookAtTarget to the camera.")]
      public float maxDistance = 0.5f;
      [Tooltip("Max angle between the direction and the direction towards the camera.")]
      [Range(0.0f, 180f)]
      public float maxAngle = 45f;
      [Tooltip("Fixes the Y axis of the trigger to Vector3.up. This makes the trigger symmetrical relative to the object.")]
      public bool fixYAxis;

      public Quaternion GetRotation()
      {
        Vector3 forward = lookAtTarget.transform.forward;
        if (fixYAxis)
          forward.y = 0.0f;
        if (forward == Vector3.zero)
          return Quaternion.identity;
        Vector3 upwards = fixYAxis ? Vector3.up : lookAtTarget.transform.up;
        return Quaternion.LookRotation(forward, upwards);
      }

      public bool IsInRange(
        Transform raycastFrom,
        RaycastHit hit,
        Transform trigger,
        out float error)
      {
        error = 0.0f;
        if (lookAtTarget == null)
          return true;
        error = 180f;
        if (raycastFrom == null || hit.collider != lookAtTarget || hit.distance > (double) maxDistance || direction == Vector3.zero || maxDistance <= 0.0 || maxAngle <= 0.0)
          return false;
        Vector3 to = GetRotation() * direction;
        float num = Vector3.Angle(raycastFrom.position - hit.point, to);
        if (num > (double) maxAngle)
          return false;
        error = (float) (num / (double) maxAngle * 180.0);
        return true;
      }
    }

    [Serializable]
    public class Range
    {
      [HideInInspector]
      [SerializeField]
      public string name;
      [HideInInspector]
      [SerializeField]
      public bool show = true;
      [Tooltip("The range for the character's position and rotation.")]
      public CharacterPosition characterPosition;
      [Tooltip("The range for the character camera's position and rotation.")]
      public CameraPosition cameraPosition;
      [Tooltip("Definitions of the interactions associated with this range.")]
      public Interaction[] interactions;

      public bool IsInRange(
        Transform character,
        Transform raycastFrom,
        RaycastHit raycastHit,
        Transform trigger,
        out float maxError)
      {
        maxError = 0.0f;
        if (!characterPosition.IsInRange(character, trigger, out float error1) || !cameraPosition.IsInRange(raycastFrom, raycastHit, trigger, out float error2))
          return false;
        maxError = Mathf.Max(error1, error2);
        return true;
      }

      [Serializable]
      public class Interaction
      {
        [Tooltip("The InteractionObject to interact with.")]
        public InteractionObject interactionObject;
        [Tooltip("The effectors to interact with.")]
        public FullBodyBipedEffector[] effectors;
      }
    }
  }
}
