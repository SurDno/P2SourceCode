using System.Collections.Generic;

namespace UnityEngine.AI;

[ExecuteInEditMode]
[AddComponentMenu("Navigation/NavMeshModifier", 32)]
[HelpURL("https://github.com/Unity-Technologies/NavMeshComponents#documentation-draft")]
public class NavMeshModifier : MonoBehaviour {
	[SerializeField] private bool m_OverrideArea;
	[SerializeField] private int m_Area;
	[SerializeField] private bool m_IgnoreFromBuild;

	[SerializeField] private List<int> m_AffectedAgents = new(new int[1] {
		-1
	});

	private static readonly List<NavMeshModifier> s_NavMeshModifiers = new();

	public bool overrideArea {
		get => m_OverrideArea;
		set => m_OverrideArea = value;
	}

	public int area {
		get => m_Area;
		set => m_Area = value;
	}

	public bool ignoreFromBuild {
		get => m_IgnoreFromBuild;
		set => m_IgnoreFromBuild = value;
	}

	public static List<NavMeshModifier> activeModifiers => s_NavMeshModifiers;

	private void OnEnable() {
		if (s_NavMeshModifiers.Contains(this))
			return;
		s_NavMeshModifiers.Add(this);
	}

	private void OnDisable() {
		s_NavMeshModifiers.Remove(this);
	}

	public bool AffectsAgentType(int agentTypeID) {
		if (m_AffectedAgents.Count == 0)
			return false;
		return m_AffectedAgents[0] == -1 || m_AffectedAgents.IndexOf(agentTypeID) != -1;
	}
}