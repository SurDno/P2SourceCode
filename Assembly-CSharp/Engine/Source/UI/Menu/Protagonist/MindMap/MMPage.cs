using Engine.Common.MindMap;
using Engine.Common.Types;
using Engine.Impl.Services.Factories;
using Inspectors;
using System.Collections.Generic;

namespace Engine.Source.UI.Menu.Protagonist.MindMap
{
  [Factory(typeof (IMMPage))]
  public class MMPage : IMMPage
  {
    private List<MMLink> links = new List<MMLink>();
    private List<MMNode> nodes = new List<MMNode>();

    [Inspected]
    public bool Global { get; set; }

    [Inspected]
    public LocalizedText Title { get; set; }

    [Inspected]
    public IEnumerable<IMMNode> Nodes => (IEnumerable<IMMNode>) this.nodes;

    public int NodesCount => this.nodes.Count;

    [Inspected]
    public IEnumerable<IMMLink> Links => (IEnumerable<IMMLink>) this.links;

    public int LinksCount => this.links.Count;

    public MMWindow MindMap { get; set; }

    public MMLink GetLink(int index) => this.links[index];

    public MMNode GetNode(int index) => this.nodes[index];

    public void AddNode(IMMNode node)
    {
      ((MMNode) node).MindMap = this.MindMap;
      this.nodes.Add((MMNode) node);
    }

    public void RemoveNode(IMMNode node)
    {
      ((MMNode) node).MindMap = (MMWindow) null;
      this.nodes.Remove((MMNode) node);
    }

    public void AddLink(IMMLink link) => this.links.Add((MMLink) link);

    public void RemoveLink(IMMLink link) => this.links.Remove((MMLink) link);

    public bool HasUndiscovered()
    {
      foreach (MMNode node in this.nodes)
      {
        if (node != null && node.Content != null && node.Undiscovered)
          return true;
      }
      return false;
    }
  }
}
