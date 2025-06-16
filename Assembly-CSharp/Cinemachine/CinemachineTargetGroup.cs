using System;

namespace Cinemachine
{
  [DocumentationSorting(19f, DocumentationSortingAttribute.Level.UserRef)]
  [AddComponentMenu("Cinemachine/CinemachineTargetGroup")]
  [SaveDuringPlay]
  [ExecuteInEditMode]
  public class CinemachineTargetGroup : MonoBehaviour
  {
    [Tooltip("How the group's position is calculated.  Select GroupCenter for the center of the bounding box, and GroupAverage for a weighted average of the positions of the members.")]
    public PositionMode m_PositionMode = PositionMode.GroupCenter;
    [Tooltip("How the group's rotation is calculated.  Select Manual to use the value in the group's transform, and GroupAverage for a weighted average of the orientations of the members.")]
    public RotationMode m_RotationMode = RotationMode.Manual;
    [Tooltip("When to update the group's transform based on the position of the group members")]
    public UpdateMethod m_UpdateMethod = UpdateMethod.LateUpdate;
    [NoSaveDuringPlay]
    [Tooltip("The target objects, together with their weights and radii, that will contribute to the group's average position, orientation, and size.")]
    public Target[] m_Targets = new Target[0];
    private float m_lastRadius;

    public Bounds BoundingBox
    {
      get
      {
        float averageWeight;
        Vector3 averagePosition = CalculateAveragePosition(out averageWeight);
        bool flag = false;
        Bounds boundingBox = new Bounds(averagePosition, new Vector3(m_lastRadius * 2f, m_lastRadius * 2f, m_lastRadius * 2f));
        if (averageWeight > 9.9999997473787516E-05)
        {
          for (int index = 0; index < m_Targets.Length; ++index)
          {
            if ((UnityEngine.Object) m_Targets[index].target != (UnityEngine.Object) null)
            {
              float weight = m_Targets[index].weight;
              float t = weight >= averageWeight - 9.9999997473787516E-05 ? 1f : weight / averageWeight;
              float num = m_Targets[index].radius * 2f * t;
              Bounds bounds = new Bounds(Vector3.Lerp(averagePosition, m_Targets[index].target.position, t), new Vector3(num, num, num));
              if (!flag)
                boundingBox = bounds;
              else
                boundingBox.Encapsulate(bounds);
              flag = true;
            }
          }
        }
        Vector3 extents = boundingBox.extents;
        m_lastRadius = Mathf.Max(extents.x, Mathf.Max(extents.y, extents.z));
        return boundingBox;
      }
    }

    public bool IsEmpty
    {
      get
      {
        for (int index = 0; index < m_Targets.Length; ++index)
        {
          if ((UnityEngine.Object) m_Targets[index].target != (UnityEngine.Object) null && m_Targets[index].weight > 9.9999997473787516E-05)
            return false;
        }
        return true;
      }
    }

    public Bounds GetViewSpaceBoundingBox(Matrix4x4 mView)
    {
      Matrix4x4 inverse = mView.inverse;
      float averageWeight;
      Vector3 vector3 = inverse.MultiplyPoint3x4(CalculateAveragePosition(out averageWeight));
      bool flag = false;
      Bounds spaceBoundingBox = new Bounds(vector3, new Vector3(m_lastRadius * 2f, m_lastRadius * 2f, m_lastRadius * 2f));
      if (averageWeight > 9.9999997473787516E-05)
      {
        for (int index = 0; index < m_Targets.Length; ++index)
        {
          if ((UnityEngine.Object) m_Targets[index].target != (UnityEngine.Object) null)
          {
            float weight = m_Targets[index].weight;
            float t = weight >= averageWeight - 9.9999997473787516E-05 ? 1f : weight / averageWeight;
            float num = m_Targets[index].radius * 2f;
            Vector4 b = (Vector4) inverse.MultiplyPoint3x4(m_Targets[index].target.position);
            Bounds bounds = new Bounds((Vector3) (Vector4) Vector3.Lerp(vector3, (Vector3) b, t), new Vector3(num, num, num));
            if (!flag)
              spaceBoundingBox = bounds;
            else
              spaceBoundingBox.Encapsulate(bounds);
            flag = true;
          }
        }
      }
      Vector3 extents = spaceBoundingBox.extents;
      m_lastRadius = Mathf.Max(extents.x, Mathf.Max(extents.y, extents.z));
      return spaceBoundingBox;
    }

    private Vector3 CalculateAveragePosition(out float averageWeight)
    {
      Vector3 zero = Vector3.zero;
      float num1 = 0.0f;
      int num2 = 0;
      for (int index = 0; index < m_Targets.Length; ++index)
      {
        if ((UnityEngine.Object) m_Targets[index].target != (UnityEngine.Object) null && m_Targets[index].weight > 9.9999997473787516E-05)
        {
          ++num2;
          num1 += m_Targets[index].weight;
          zero += m_Targets[index].target.position * m_Targets[index].weight;
        }
      }
      if (num1 > 9.9999997473787516E-05)
        zero /= num1;
      if (num2 == 0)
      {
        averageWeight = 0.0f;
        return this.transform.position;
      }
      averageWeight = num1 / num2;
      return zero;
    }

    private Quaternion CalculateAverageOrientation()
    {
      Quaternion q = Quaternion.identity;
      for (int index = 0; index < m_Targets.Length; ++index)
      {
        if ((UnityEngine.Object) m_Targets[index].target != (UnityEngine.Object) null)
        {
          float weight = m_Targets[index].weight;
          Quaternion rotation = m_Targets[index].target.rotation;
          q = new Quaternion(q.x + rotation.x * weight, q.y + rotation.y * weight, q.z + rotation.z * weight, q.w + rotation.w * weight);
        }
      }
      return q.Normalized();
    }

    private void OnValidate()
    {
      for (int index = 0; index < m_Targets.Length; ++index)
      {
        if (m_Targets[index].weight < 0.0)
          m_Targets[index].weight = 0.0f;
        if (m_Targets[index].radius < 0.0)
          m_Targets[index].radius = 0.0f;
      }
    }

    private void FixedUpdate()
    {
      if (m_UpdateMethod != UpdateMethod.FixedUpdate)
        return;
      UpdateTransform();
    }

    private void Update()
    {
      if (Application.isPlaying && m_UpdateMethod != UpdateMethod.Update)
        return;
      UpdateTransform();
    }

    private void LateUpdate()
    {
      if (m_UpdateMethod != UpdateMethod.LateUpdate)
        return;
      UpdateTransform();
    }

    private void UpdateTransform()
    {
      if (IsEmpty)
        return;
      switch (m_PositionMode)
      {
        case PositionMode.GroupCenter:
          this.transform.position = BoundingBox.center;
          break;
        case PositionMode.GroupAverage:
          this.transform.position = CalculateAveragePosition(out float _);
          break;
      }
      switch (m_RotationMode)
      {
        case RotationMode.GroupAverage:
          this.transform.rotation = CalculateAverageOrientation();
          break;
      }
    }

    [DocumentationSorting(19.1f, DocumentationSortingAttribute.Level.UserRef)]
    [Serializable]
    public struct Target
    {
      [Tooltip("The target objects.  This object's position and orientation will contribute to the group's average position and orientation, in accordance with its weight")]
      public Transform target;
      [Tooltip("How much weight to give the target when averaging.  Cannot be negative")]
      public float weight;
      [Tooltip("The radius of the target, used for calculating the bounding box.  Cannot be negative")]
      public float radius;
    }

    [DocumentationSorting(19.2f, DocumentationSortingAttribute.Level.UserRef)]
    public enum PositionMode
    {
      GroupCenter,
      GroupAverage,
    }

    [DocumentationSorting(19.3f, DocumentationSortingAttribute.Level.UserRef)]
    public enum RotationMode
    {
      Manual,
      GroupAverage,
    }

    public enum UpdateMethod
    {
      Update,
      FixedUpdate,
      LateUpdate,
    }
  }
}
