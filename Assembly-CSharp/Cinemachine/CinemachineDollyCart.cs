// Decompiled with JetBrains decompiler
// Type: Cinemachine.CinemachineDollyCart
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
namespace Cinemachine
{
  [DocumentationSorting(21f, DocumentationSortingAttribute.Level.UserRef)]
  [ExecuteInEditMode]
  public class CinemachineDollyCart : MonoBehaviour
  {
    [Tooltip("The path to follow")]
    public CinemachinePathBase m_Path;
    [Tooltip("When to move the cart, if Velocity is non-zero")]
    public CinemachineDollyCart.UpdateMethod m_UpdateMethod = CinemachineDollyCart.UpdateMethod.Update;
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
      if (this.m_UpdateMethod != CinemachineDollyCart.UpdateMethod.FixedUpdate)
        return;
      this.SetCartPosition(this.m_Position += this.m_Speed * Time.deltaTime);
    }

    private void Update()
    {
      if (!Application.isPlaying)
      {
        this.SetCartPosition(this.m_Position);
      }
      else
      {
        if (this.m_UpdateMethod != CinemachineDollyCart.UpdateMethod.Update)
          return;
        this.SetCartPosition(this.m_Position += this.m_Speed * Time.deltaTime);
      }
    }

    private void SetCartPosition(float distanceAlongPath)
    {
      if (!((Object) this.m_Path != (Object) null))
        return;
      this.m_Position = this.m_Path.NormalizeUnit(distanceAlongPath, this.m_PositionUnits);
      this.transform.position = this.m_Path.EvaluatePositionAtUnit(this.m_Position, this.m_PositionUnits);
      this.transform.rotation = this.m_Path.EvaluateOrientationAtUnit(this.m_Position, this.m_PositionUnits);
    }

    public enum UpdateMethod
    {
      Update,
      FixedUpdate,
    }
  }
}
