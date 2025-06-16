using UnityEngine;
using UnityEngine.Serialization;

namespace Cinemachine
{
  [DocumentationSorting(21f, DocumentationSortingAttribute.Level.UserRef)]
  [ExecuteInEditMode]
  public class CinemachineDollyCart : MonoBehaviour
  {
    [Tooltip("The path to follow")]
    public CinemachinePathBase m_Path;
    [Tooltip("When to move the cart, if Velocity is non-zero")]
    public UpdateMethod m_UpdateMethod = UpdateMethod.Update;
    [Tooltip("How to interpret the Path Position.  If set to Path Units, values are as follows: 0 represents the first waypoint on the path, 1 is the second, and so on.  Values in-between are points on the path in between the waypoints.  If set to Distance, then Path Position represents distance along the path.")]
    public CinemachinePathBase.PositionUnits m_PositionUnits = CinemachinePathBase.PositionUnits.Distance;
    [Tooltip("Move the cart with this speed along the path.  The value is interpreted according to the Position Units setting.")]
    [FormerlySerializedAs("m_Velocity")]
    public float m_Speed;
    [Tooltip("The position along the path at which the cart will be placed.  This can be animated directly or, if the velocity is non-zero, will be updated automatically.  The value is interpreted according to the Position Units setting.")]
    [FormerlySerializedAs("m_CurrentDistance")]
    public float m_Position;

    private void FixedUpdate()
    {
      if (m_UpdateMethod != UpdateMethod.FixedUpdate)
        return;
      SetCartPosition(m_Position += m_Speed * Time.deltaTime);
    }

    private void Update()
    {
      if (!Application.isPlaying)
      {
        SetCartPosition(m_Position);
      }
      else
      {
        if (m_UpdateMethod != UpdateMethod.Update)
          return;
        SetCartPosition(m_Position += m_Speed * Time.deltaTime);
      }
    }

    private void SetCartPosition(float distanceAlongPath)
    {
      if (!(m_Path != null))
        return;
      m_Position = m_Path.NormalizeUnit(distanceAlongPath, m_PositionUnits);
      transform.position = m_Path.EvaluatePositionAtUnit(m_Position, m_PositionUnits);
      transform.rotation = m_Path.EvaluateOrientationAtUnit(m_Position, m_PositionUnits);
    }

    public enum UpdateMethod
    {
      Update,
      FixedUpdate,
    }
  }
}
