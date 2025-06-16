using Engine.Common.Commons;
using Engine.Common.MindMap;
using Engine.Common.Types;

namespace Engine.Source.UI.Menu.Protagonist.MindMap
{
  public class MMTooltip
  {
    public IMMNode node;
    public IMapTooltipResource tooltipResource;
    public LocalizedText tooltipText;

    public MMTooltip(IMMNode node) => this.node = node;

    public MMTooltip(IMapTooltipResource tooltipResource, LocalizedText tooltipText)
    {
      this.tooltipResource = tooltipResource;
      this.tooltipText = tooltipText;
    }
  }
}
