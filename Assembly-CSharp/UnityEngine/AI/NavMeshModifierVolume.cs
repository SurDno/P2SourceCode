// Decompiled with JetBrains decompiler
// Type: UnityEngine.AI.NavMeshModifierVolume
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable
namespace UnityEngine.AI
{
  [ExecuteInEditMode]
  [AddComponentMenu("Navigation/NavMeshModifierVolume", 31)]
  [HelpURL("https://github.com/Unity-Technologies/NavMeshComponents#documentation-draft")]
  public class NavMeshModifierVolume : MonoBehaviour
  {
    [SerializeField]
    private Vector3 m_Size = new Vector3(4f, 3f, 4f);
    [SerializeField]
    private Vector3 m_Center = new Vector3(0.0f, 1f, 0.0f);
    [SerializeField]
    private int m_Area;
    [SerializeField]
    private List<int> m_AffectedAgents = new List<int>((IEnumerable<int>) new int[1]
    {
      -1
    });
    private static readonly List<NavMeshModifierVolume> s_NavMeshModifiers = new List<NavMeshModifierVolume>();

    public Vector3 size
    {
      get => this.m_Size;
      set => this.m_Size = value;
    }

    public Vector3 center
    {
      get => this.m_Center;
      set => this.m_Center = value;
    }

    public int area
    {
      get => this.m_Area;
      set => this.m_Area = value;
    }

    public static List<NavMeshModifierVolume> activeModifiers
    {
      get => NavMeshModifierVolume.s_NavMeshModifiers;
    }

    private void OnEnable()
    {
      if (NavMeshModifierVolume.s_NavMeshModifiers.Contains(this))
        return;
      NavMeshModifierVolume.s_NavMeshModifiers.Add(this);
    }

    private void OnDisable() => NavMeshModifierVolume.s_NavMeshModifiers.Remove(this);

    public bool AffectsAgentType(int agentTypeID)
    {
      if (this.m_AffectedAgents.Count == 0)
        return false;
      return this.m_AffectedAgents[0] == -1 || this.m_AffectedAgents.IndexOf(agentTypeID) != -1;
    }
  }
}
