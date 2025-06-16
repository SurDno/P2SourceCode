// Decompiled with JetBrains decompiler
// Type: UnityEngine.AI.NavMeshLink
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable
namespace UnityEngine.AI
{
  [ExecuteInEditMode]
  [DefaultExecutionOrder(-101)]
  [AddComponentMenu("Navigation/NavMeshLink", 33)]
  [HelpURL("https://github.com/Unity-Technologies/NavMeshComponents#documentation-draft")]
  public class NavMeshLink : MonoBehaviour
  {
    [SerializeField]
    private int m_AgentTypeID;
    [SerializeField]
    private Vector3 m_StartPoint = new Vector3(0.0f, 0.0f, -2.5f);
    [SerializeField]
    private Vector3 m_EndPoint = new Vector3(0.0f, 0.0f, 2.5f);
    [SerializeField]
    private float m_Width;
    [SerializeField]
    private int m_CostModifier = -1;
    [SerializeField]
    private bool m_Bidirectional = true;
    [SerializeField]
    private bool m_AutoUpdatePosition;
    [SerializeField]
    private int m_Area;
    private NavMeshLinkInstance m_LinkInstance = new NavMeshLinkInstance();
    private Vector3 m_LastPosition = Vector3.zero;
    private Quaternion m_LastRotation = Quaternion.identity;
    private static readonly List<NavMeshLink> s_Tracked = new List<NavMeshLink>();

    public int agentTypeID
    {
      get => this.m_AgentTypeID;
      set
      {
        this.m_AgentTypeID = value;
        this.UpdateLink();
      }
    }

    public Vector3 startPoint
    {
      get => this.m_StartPoint;
      set
      {
        this.m_StartPoint = value;
        this.UpdateLink();
      }
    }

    public Vector3 endPoint
    {
      get => this.m_EndPoint;
      set
      {
        this.m_EndPoint = value;
        this.UpdateLink();
      }
    }

    public float width
    {
      get => this.m_Width;
      set
      {
        this.m_Width = value;
        this.UpdateLink();
      }
    }

    public int costModifier
    {
      get => this.m_CostModifier;
      set
      {
        this.m_CostModifier = value;
        this.UpdateLink();
      }
    }

    public bool bidirectional
    {
      get => this.m_Bidirectional;
      set
      {
        this.m_Bidirectional = value;
        this.UpdateLink();
      }
    }

    public bool autoUpdate
    {
      get => this.m_AutoUpdatePosition;
      set => this.SetAutoUpdate(value);
    }

    public int area
    {
      get => this.m_Area;
      set
      {
        this.m_Area = value;
        this.UpdateLink();
      }
    }

    private void OnEnable()
    {
      this.AddLink();
      if (!this.m_AutoUpdatePosition || !this.m_LinkInstance.valid)
        return;
      NavMeshLink.AddTracking(this);
    }

    private void OnDisable()
    {
      NavMeshLink.RemoveTracking(this);
      this.m_LinkInstance.Remove();
    }

    public void UpdateLink()
    {
      this.m_LinkInstance.Remove();
      this.AddLink();
    }

    private static void AddTracking(NavMeshLink link)
    {
      if (NavMeshLink.s_Tracked.Count == 0)
        NavMesh.onPreUpdate += new NavMesh.OnNavMeshPreUpdate(NavMeshLink.UpdateTrackedInstances);
      NavMeshLink.s_Tracked.Add(link);
    }

    private static void RemoveTracking(NavMeshLink link)
    {
      NavMeshLink.s_Tracked.Remove(link);
      if (NavMeshLink.s_Tracked.Count != 0)
        return;
      NavMesh.onPreUpdate -= new NavMesh.OnNavMeshPreUpdate(NavMeshLink.UpdateTrackedInstances);
    }

    private void SetAutoUpdate(bool value)
    {
      if (this.m_AutoUpdatePosition == value)
        return;
      this.m_AutoUpdatePosition = value;
      if (value)
        NavMeshLink.AddTracking(this);
      else
        NavMeshLink.RemoveTracking(this);
    }

    private void AddLink()
    {
      this.m_LinkInstance = NavMesh.AddLink(new NavMeshLinkData()
      {
        startPosition = this.m_StartPoint,
        endPosition = this.m_EndPoint,
        width = this.m_Width,
        costModifier = (float) this.m_CostModifier,
        bidirectional = this.m_Bidirectional,
        area = this.m_Area,
        agentTypeID = this.m_AgentTypeID
      }, this.transform.position, this.transform.rotation);
      if (this.m_LinkInstance.valid)
        this.m_LinkInstance.owner = (Object) this;
      this.m_LastPosition = this.transform.position;
      this.m_LastRotation = this.transform.rotation;
    }

    private bool HasTransformChanged()
    {
      return this.m_LastPosition != this.transform.position || this.m_LastRotation != this.transform.rotation;
    }

    private void OnDidApplyAnimationProperties() => this.UpdateLink();

    private static void UpdateTrackedInstances()
    {
      foreach (NavMeshLink navMeshLink in NavMeshLink.s_Tracked)
      {
        if (navMeshLink.HasTransformChanged())
          navMeshLink.UpdateLink();
      }
    }
  }
}
