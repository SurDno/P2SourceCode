using System.Collections.Generic;

namespace UnityEngine.AI
{
  [ExecuteInEditMode]
  [AddComponentMenu("Navigation/NavMeshModifier", 32)]
  [HelpURL("https://github.com/Unity-Technologies/NavMeshComponents#documentation-draft")]
  public class NavMeshModifier : MonoBehaviour
  {
    [SerializeField]
    private bool m_OverrideArea;
    [SerializeField]
    private int m_Area;
    [SerializeField]
    private bool m_IgnoreFromBuild;
    [SerializeField]
    private List<int> m_AffectedAgents = new List<int>((IEnumerable<int>) new int[1]
    {
      -1
    });
    private static readonly List<NavMeshModifier> s_NavMeshModifiers = new List<NavMeshModifier>();

    public bool overrideArea
    {
      get => this.m_OverrideArea;
      set => this.m_OverrideArea = value;
    }

    public int area
    {
      get => this.m_Area;
      set => this.m_Area = value;
    }

    public bool ignoreFromBuild
    {
      get => this.m_IgnoreFromBuild;
      set => this.m_IgnoreFromBuild = value;
    }

    public static List<NavMeshModifier> activeModifiers => NavMeshModifier.s_NavMeshModifiers;

    private void OnEnable()
    {
      if (NavMeshModifier.s_NavMeshModifiers.Contains(this))
        return;
      NavMeshModifier.s_NavMeshModifiers.Add(this);
    }

    private void OnDisable() => NavMeshModifier.s_NavMeshModifiers.Remove(this);

    public bool AffectsAgentType(int agentTypeID)
    {
      if (this.m_AffectedAgents.Count == 0)
        return false;
      return this.m_AffectedAgents[0] == -1 || this.m_AffectedAgents.IndexOf(agentTypeID) != -1;
    }
  }
}
