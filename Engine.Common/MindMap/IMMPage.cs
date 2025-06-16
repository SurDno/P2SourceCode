using Engine.Common.Types;
using System.Collections.Generic;

namespace Engine.Common.MindMap
{
  public interface IMMPage
  {
    bool Global { get; set; }

    LocalizedText Title { get; set; }

    IEnumerable<IMMNode> Nodes { get; }

    void AddNode(IMMNode node);

    void RemoveNode(IMMNode node);

    IEnumerable<IMMLink> Links { get; }

    void AddLink(IMMLink link);

    void RemoveLink(IMMLink link);
  }
}
