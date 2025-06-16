using System.Collections.Generic;

namespace ClipperLib;

public class PolyNode {
	internal List<PolyNode> m_Childs = new();
	internal EndType m_endtype;
	internal int m_Index;
	internal JoinType m_jointype;
	internal PolyNode m_Parent;
	internal List<IntPoint> m_polygon = new();

	public int ChildCount => m_Childs.Count;

	public List<IntPoint> Contour => m_polygon;

	public List<PolyNode> Childs => m_Childs;

	public PolyNode Parent => m_Parent;

	public bool IsHole => IsHoleNode();

	public bool IsOpen { get; set; }

	private bool IsHoleNode() {
		var flag = true;
		for (var parent = m_Parent; parent != null; parent = parent.m_Parent)
			flag = !flag;
		return flag;
	}

	internal void AddChild(PolyNode Child) {
		var count = m_Childs.Count;
		m_Childs.Add(Child);
		Child.m_Parent = this;
		Child.m_Index = count;
	}

	public PolyNode GetNext() {
		return m_Childs.Count > 0 ? m_Childs[0] : GetNextSiblingUp();
	}

	internal PolyNode GetNextSiblingUp() {
		if (m_Parent == null)
			return null;
		return m_Index == m_Parent.m_Childs.Count - 1 ? m_Parent.GetNextSiblingUp() : m_Parent.m_Childs[m_Index + 1];
	}
}