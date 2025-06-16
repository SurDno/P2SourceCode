using System.Collections.Generic;
using Engine.Common.MindMap;
using Engine.Common.Types;
using Engine.Impl.Services.Factories;
using Inspectors;

namespace Engine.Source.UI.Menu.Protagonist.MindMap;

[Factory(typeof(IMMPage))]
public class MMPage : IMMPage {
	private List<MMLink> links = new();
	private List<MMNode> nodes = new();

	[Inspected] public bool Global { get; set; }

	[Inspected] public LocalizedText Title { get; set; }

	[Inspected] public IEnumerable<IMMNode> Nodes => nodes;

	public int NodesCount => nodes.Count;

	[Inspected] public IEnumerable<IMMLink> Links => links;

	public int LinksCount => links.Count;

	public MMWindow MindMap { get; set; }

	public MMLink GetLink(int index) {
		return links[index];
	}

	public MMNode GetNode(int index) {
		return nodes[index];
	}

	public void AddNode(IMMNode node) {
		((MMNode)node).MindMap = MindMap;
		nodes.Add((MMNode)node);
	}

	public void RemoveNode(IMMNode node) {
		((MMNode)node).MindMap = null;
		nodes.Remove((MMNode)node);
	}

	public void AddLink(IMMLink link) {
		links.Add((MMLink)link);
	}

	public void RemoveLink(IMMLink link) {
		links.Remove((MMLink)link);
	}

	public bool HasUndiscovered() {
		foreach (var node in nodes)
			if (node != null && node.Content != null && node.Undiscovered)
				return true;
		return false;
	}
}