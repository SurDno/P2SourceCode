// Decompiled with JetBrains decompiler
// Type: Engine.Source.UI.Menu.Protagonist.MindMap.MMTooltip
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Commons;
using Engine.Common.MindMap;
using Engine.Common.Types;

#nullable disable
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
