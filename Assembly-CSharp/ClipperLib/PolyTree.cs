using System.Collections.Generic;

namespace ClipperLib
{
  public class PolyTree : PolyNode
  {
    internal List<PolyNode> m_AllPolys = new List<PolyNode>();

    public int Total
    {
      get
      {
        int count = m_AllPolys.Count;
        if (count > 0 && m_Childs[0] != m_AllPolys[0])
          --count;
        return count;
      }
    }

    ~PolyTree() => Clear();

    public void Clear()
    {
      for (int index = 0; index < m_AllPolys.Count; ++index)
        m_AllPolys[index] = null;
      m_AllPolys.Clear();
      m_Childs.Clear();
    }

    public PolyNode GetFirst() => m_Childs.Count > 0 ? m_Childs[0] : null;
  }
}
